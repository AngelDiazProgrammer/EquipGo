window.mostrarModalDetallesEquipo = function (dotNetRef) {
    const modalEl = document.getElementById('modalDetallesEquipo');
    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    modal.show();

    modalEl.addEventListener('shown.bs.modal', function handler() {
        modalEl.removeEventListener('shown.bs.modal', handler);
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync("MostrarMapa");
        }
    });
};

window.initializeLeafletMap = function (lat, lon) {
    if (window.mapInstance) {
        window.mapInstance.setView([lat, lon], 16);
        window.mapInstance.eachLayer(function (layer) {
            if (layer instanceof L.Marker) {
                window.mapInstance.removeLayer(layer);
            }
        });
        L.marker([lat, lon]).addTo(window.mapInstance);
    } else {
        window.mapInstance = L.map('leaflet-map').setView([lat, lon], 16);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(window.mapInstance);
        L.marker([lat, lon]).addTo(window.mapInstance);
    }
};


