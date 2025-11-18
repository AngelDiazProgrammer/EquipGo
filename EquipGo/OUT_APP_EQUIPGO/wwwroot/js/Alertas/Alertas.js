function mostrarModal(modalId) {
    var modalElement = document.getElementById(modalId);
    var modal = new bootstrap.Modal(modalElement);
    modal.show();
}

function mostrarMapaUbicacion(latitud, longitud, serial, apiKey) {
    // Crear modal simple solo para el mapa
    const modalHtml = `
        <div class="modal fade" id="mapModal" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Ubicación de alerta: ${serial}</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body p-0">
                        <iframe 
                            src="https://www.google.com/maps?q=${latitud},${longitud}&z=15&output=embed" 
                            width="100%" 
                            height="400" 
                            style="border:0;" 
                            allowfullscreen>
                        </iframe>
                    </div>
                </div>
            </div>
        </div>
    `;

    // Remover modal anterior si existe
    const existingModal = document.getElementById('mapModal');
    if (existingModal) existingModal.remove();

    // Agregar nuevo modal y mostrarlo
    document.body.insertAdjacentHTML('beforeend', modalHtml);
    const modal = new bootstrap.Modal(document.getElementById('mapModal'));
    modal.show();
}