using System;
using System.Device.Location;
using System.Net;

namespace EquipGo.Agent
{
    public class GeoHelper
    {
        private readonly FileLogger _logger;

        public GeoHelper(FileLogger logger)
        {
            _logger = logger;
        }

        public (double lat, double lon) GetLocation()
        {
            try
            {
                var watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                bool started = watcher.TryStart(false, TimeSpan.FromSeconds(30));

                _logger.Log($"📡 GeoWatcher iniciado: {started}, Status={watcher.Status}");

                var coord = watcher.Position.Location;
                if (!coord.IsUnknown)
                {
                    _logger.Log($"✅ GPS encontró coordenadas: Lat={coord.Latitude}, Lon={coord.Longitude}");
                    return (coord.Latitude, coord.Longitude);
                }
                else
                {
                    _logger.Log("⚠️ GPS no encontró coordenadas, Location.IsUnknown=true");
                }
            }
            catch (Exception ex)
            {
                _logger.Log("❌ Error con GPS: " + ex.Message);
            }

            // 🔄 Fallback: IP
            try
            {
                using (var client = new WebClient())
                {
                    string json = client.DownloadString("http://ip-api.com/json/");
                    dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                    double lat = obj.lat;
                    double lon = obj.lon;

                    _logger.Log($"🌐 Ubicación obtenida por IP: Lat={lat}, Lon={lon}");
                    return (lat, lon);
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
