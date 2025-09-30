using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ManagedNativeWifi;

namespace EquipGo.Agent
{
    /// <summary>
    /// Esta clase es nuestra herramienta principal para obtener la ubicación geográfica de un equipo.
    /// Intenta varios métodos, desde el más preciso (Wi-Fi) hasta el menos preciso (IP), para asegurarse
    /// de siempre poder devolver una coordenada.
    /// </summary>
    public class GeoHelper
    {
        // Una instancia de nuestro logger para poder escribir lo que va pasando en un archivo.
        private readonly FileLogger _logger;
        // Un cliente HTTP que usaremos para comunicarnos con las APIs de ubicación (como HERE).
        private readonly HttpClient _httpClient;
        public GeoHelper(FileLogger logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        #region DTOs (Data Transfer Objects) para la API de HERE
        // Estas clases son como "moldes" para crear los objetos JSON que la API de HERE entiende.
        // Usamos [JsonProperty] para asegurarnos de que el nombre de la propiedad en el JSON
        // sea exactamente el que HERE espera, aunque en nuestro código la llamemos diferente.

        /// <summary>
        /// Este es el molde para la solicitud principal que le enviaremos a HERE.
        /// La propiedad más importante es "wlan", que es una lista de puntos de acceso Wi-Fi.
        /// </summary>
        private class HereRequest
        {
            [JsonProperty("wlan")]
            public List<WlanAccessPoint> wlan { get; set; } = new List<WlanAccessPoint>();
        }

        /// <summary>
        /// Este es el molde para cada punto de acceso Wi-Fi que encontremos.
        /// HERE necesita la dirección MAC (obligatoria) y la fuerza de la señal (opcional, pero recomendada).
        /// </summary>
        private class WlanAccessPoint
        {
            [JsonProperty("mac")]
            public string mac { get; set; }

            [JsonProperty("rss")]
            public int rss { get; set; }
        }

        /// <summary>
        /// Este es el molde para la respuesta que esperamos recibir de HERE.
        /// La respuesta viene dentro de un objeto "location".
        /// </summary>
        private class HereResponse
        {
            public HereLocation location { get; set; }
        }

        /// <summary>
        /// Este es el molde para los datos de la ubicación que nos devuelve HERE.
        /// </summary>
        private class HereLocation
        {
            public double lat { get; set; }
            public double lng { get; set; }
            public double accuracy { get; set; }
        }
        #endregion

        /// <summary>
        /// Este es el método público principal. Es el que el resto del código llamará para obtener la ubicación.
        /// Funciona como un "director", intentando los métodos de ubicación en orden de precisión.
        /// </summary>
        /// <returns>Una tupla con la latitud y longitud. Si todo falla, devuelve (0, 0).</returns>
        public (double lat, double lon) GetLocation()
        {
            // 1. INTENTO PRINCIPAL: Wi-Fi con HERE (el más preciso para desktops).
            _logger.Log("📡 Intentando obtener ubicación por Wi-Fi con HERE (método principal)...");
            var wifiLocation = TryGetLocationByWifi();
            if (wifiLocation.lat != 0 && wifiLocation.lon != 0)
            {
                return wifiLocation; // Si funcionó, devolvemos el resultado y terminamos.
            }

            // 2. INTENTO SECUNDARIO: Usar el servicio de localización de Windows.
            _logger.Log("🔍 Falló el método de Wi-Fi. Intentando obtener ubicación con GeoCoordinateWatcher (servicios de Windows)...");
            var windowsLocation = TryGetLocationByWindowsServices();
            if (windowsLocation.lat != 0 && windowsLocation.lon != 0)
            {
                return windowsLocation; // Si funcionó, devolvemos el resultado.
            }

            // 3. ÚLTIMO RECURSO (Fallback): Ubicación por IP (muy impreciso, pero mejor que nada).
            _logger.Log("🌐 Fallaron los métodos anteriores. Usando fallback por IP (muy impreciso)...");
            return TryGetLocationByIp();
        }

        /// <summary>
        /// Intenta obtener la ubicación usando el servicio de localización de Windows.
        /// A veces funciona si el equipo tiene GPS o si Windows está configurado para usar servicios de ubicación.
        /// </summary>
        /// <returns>Una tupla con la latitud y longitud.</returns>
        private (double lat, double lon) TryGetLocationByWindowsServices()
        {
            try
            {
                // Creamos un "vigilante" de la ubicación. Le pedimos alta precisión.
                var watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);

                // Iniciamos el vigilante, pero no queremos que la aplicación se quede congelada esperando,
                // así que lo ejecutamos en una tarea en segundo plano con un tiempo límite de 10 segundos.
                var started = Task.Run(() => watcher.TryStart(false, TimeSpan.FromSeconds(10))).Result;

                _logger.Log($"📡 GeoWatcher iniciado: {started}, Status={watcher.Status}");

                // Si se inició y tiene datos...
                if (started && watcher.Status != GeoPositionStatus.NoData)
                {
                    var coord = watcher.Position.Location;
                    // ...y la coordenada no es desconocida, ¡perfecto!
                    if (!coord.IsUnknown)
                    {
                        _logger.Log($"✅ GPS/Windows encontró coordenadas: Lat={coord.Latitude}, Lon={coord.Longitude}");
                        watcher.Stop(); // Es importante parar el vigilante para que no consuma recursos.
                        return (coord.Latitude, coord.Longitude);
                    }
                }
                watcher.Stop(); // Nos aseguramos de pararlo siempre.
            }
            catch (Exception ex)
            {
                _logger.Log("❌ Error con GPS/Windows: " + ex.Message);
            }
            return (0, 0); // Si algo falló, devolvemos (0, 0).
        }

        /// <summary>
        /// Este es el método más importante. Intenta obtener la ubicación escaneando las redes Wi-Fi
        /// y enviándolas a la API de HERE. Es asíncrono porque implica una llamada a internet.
        /// </summary>
        /// <returns>Una tupla con la latitud y longitud.</returns>
        private async Task<(double lat, double lon)> TryGetLocationByWifiAsync()
        {
            try
            {
                // PASO 1: Escanear todas las redes Wi-Fi que el equipo pueda ver.
                _logger.Log("🔍 Escaneando redes Wi-Fi disponibles...");
                await NativeWifi.ScanNetworksAsync(timeout: TimeSpan.FromSeconds(10));

                // Obtenemos la lista de redes encontradas.
                var bssNetworks = NativeWifi.EnumerateBssNetworks()
                    .Where(n => n.LinkQuality > 0) // Nos quedamos solo con las que tienen señal.
                    .ToList();

                if (!bssNetworks.Any())
                {
                    _logger.Log("⚠️ No se encontraron redes Wi-Fi con señal para geolocalizar.");
                    return (0, 0);
                }

                _logger.Log($"✅ Se encontraron {bssNetworks.Count} puntos de acceso Wi-Fi.");

                // PASO 2: Preparar los datos de las redes para enviarlos a HERE.
                var wlanNetworks = new List<WlanAccessPoint>();
                foreach (var network in bssNetworks)
                {
                    // Obtenemos la dirección MAC del punto de acceso y la formateamos con ":" en lugar de "-".
                    string macAddress = network.Bssid?.ToString().Replace('-', ':') ?? string.Empty;

                    // La librería nos da la calidad de la señal en un porcentaje (0-100).
                    // HERE necesita la fuerza de la señal en dBm (un valor negativo).
                    // Hacemos una conversión aproximada para simular dBm.
                    int rssi = -100 + (int)((network.LinkQuality / 100.0) * 50);

                    if (!string.IsNullOrEmpty(macAddress))
                    {
                        // Creamos un objeto WlanAccessPoint con los datos y lo añadimos a nuestra lista.
                        wlanNetworks.Add(new WlanAccessPoint
                        {
                            mac = macAddress,
                            rss = rssi
                        });
                    }
                }

                if (!wlanNetworks.Any())
                {
                    _logger.Log("⚠️ No se pudieron extraer direcciones MAC válidas.");
                    return (0, 0);
                }

                // PASO 3: Construir y enviar la petición a la API de HERE.
                var requestBody = new HereRequest { wlan = wlanNetworks };
                string json = JsonConvert.SerializeObject(requestBody); // Convertimos nuestro objeto a una cadena de texto JSON.
                _logger.Log($"📤 Enviando JSON a HERE: {json}");

                string apiKey = "E0ycnmgMFsdKZXWJpU-B1qgtbrQNPMMKUOHumebpB7g";
                string requestUrl = $"https://positioning.hereapi.com/v2/locate?apiKey={apiKey}";

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Hacemos la petición POST a HERE y esperamos la respuesta.
                var response = await _httpClient.PostAsync(requestUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Si la respuesta es exitosa (código 200 OK), leemos el contenido.
                    string responseJson = await response.Content.ReadAsStringAsync();
                    var locationData = JsonConvert.DeserializeObject<HereResponse>(responseJson); // Convertimos el JSON de vuelta a un objeto.

                    if (locationData?.location != null)
                    {
                        // Si todo salió bien, extraemos las coordenadas y las devolvemos.
                        _logger.Log($"✅ HERE encontró ubicación por Wi-Fi: Lat={locationData.location.lat}, Lon={locationData.location.lng}, Precisión={locationData.location.accuracy}m");
                        return (locationData.location.lat, locationData.location.lng);
                    }
                    else
                    {
                        _logger.Log("❌ La API de HERE devolvió una respuesta vacía o inválida.");
                    }
                }
                else
                {
                    // Si HERE devolvió un error (como 400 Bad Request), lo registramos.
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.Log($"❌ Error en la API de HERE: {(int)response.StatusCode} ({response.ReasonPhrase})");
                    _logger.Log($"   Detalles del error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"❌ Error inesperado en el método de Wi-Fi: {ex.Message}");
                _logger.Log($"   StackTrace: {ex.StackTrace}");
            }

            return (0, 0); // Si algo falló, devolvemos (0, 0).
        }

        /// <summary>
        /// Este es un método "envoltorio" (wrapper). Nuestro método de Wi-Fi es asíncrono (async Task),
        /// pero el método GetLocation es síncrono. Este método ejecuta la tarea asíncrona y espera
        /// a que termine para poder devolver el resultado de forma síncrona.
        /// </summary>
        /// <returns>Una tupla con la latitud y longitud.</returns>
        private (double lat, double lon) TryGetLocationByWifi()
        {
            return TryGetLocationByWifiAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Este es el último recurso. Obtiene la ubicación basándose en la dirección IP pública del equipo.
        /// Es muy impreciso (suele dar la ubicación de la ciudad o del proveedor de internet).
        /// </summary>
        /// <returns>Una tupla con la latitud y longitud.</returns>
        private (double lat, double lon) TryGetLocationByIp()
        {
            try
            {
                // Usamos un servicio gratuito (ip-api.com) que nos da la ubicación de una IP.
                string json = _httpClient.GetStringAsync("http://ip-api.com/json/").Result;
                dynamic obj = JsonConvert.DeserializeObject(json);

                if (obj.status == "success")
                {
                    double lat = obj.lat;
                    double lon = obj.lon;
                    _logger.Log($"🌐 Ubicación obtenida por IP: Lat={lat}, Lon={lon}");
                    return (lat, lon);
                }
                else
                {
                    _logger.Log($"⚠️ ip-api.com devolvió un error: {obj.message}");
                }
            }
            catch (Exception ex)
            {
                _logger.Log("❌ Error en fallback por IP: " + ex.Message);
            }

            return (0, 0);
        }
    }
}