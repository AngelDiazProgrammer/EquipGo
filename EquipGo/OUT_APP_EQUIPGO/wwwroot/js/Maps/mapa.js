window.mostrarMapaGoogle = async function (lat, lng, apiKey) {
    const mapDiv = document.getElementById("google-map");
    if (!mapDiv) return;

    // Limpiar mapa anterior
    if (window.currentMap) {
        window.currentMap = null;
        mapDiv.innerHTML = "";
    }

    // Cargar script de Google Maps si no está
    if (!window.google || !window.google.maps) {
        const script = document.createElement("script");
        script.src = `https://maps.googleapis.com/maps/api/js?key=${apiKey}`;
        script.defer = true;
        script.async = true;
        script.onload = () => initMap(lat, lng);
        document.head.appendChild(script);
    } else {
        initMap(lat, lng);
    }

    function initMap(lat, lng) {
        const map = new google.maps.Map(mapDiv, {
            center: { lat, lng },
            zoom: 16,
        });

        new google.maps.Marker({
            position: { lat, lng },
            map: map,
        });

        window.currentMap = map;
    }
};
