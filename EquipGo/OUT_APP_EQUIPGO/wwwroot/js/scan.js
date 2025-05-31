console.log("✅ scan.js cargado correctamente");
document.addEventListener("DOMContentLoaded", () => {
    console.log("✅ DOM listo (scan.js)");
});

/**
 * Mostrar el modal de información del equipo.
 * @param {object} equipoDto - Objeto recibido desde Blazor con la información real del equipo.
 */
function mostrarModalResultado(equipoDto) {
    console.log("📋 Mostrando modal para el DTO:", equipoDto);

    const equipoInfo = document.getElementById('equipoInfo');
    if (equipoInfo) {
        equipoInfo.innerText =
            `Marca: ${equipoDto.marca}, Modelo: ${equipoDto.modelo}, Serial: ${equipoDto.serial}, ` +
            `Ubicación: ${equipoDto.ubicacion}, Usuario: ${equipoDto.nombreUsuario} (${equipoDto.documentoUsuario}), ` +
            `Área: ${equipoDto.area}, Campaña: ${equipoDto.campaña}`;
    }

    const historial = document.getElementById('historial');
    if (historial) {
        historial.innerHTML = ''; // Limpiar historial anterior
        if (equipoDto.historialTransacciones && equipoDto.historialTransacciones.length > 0) {
            equipoDto.historialTransacciones.forEach(item => {
                const li = document.createElement('li');
                li.textContent = item;
                li.classList.add('list-group-item');
                historial.appendChild(li);
            });
        } else {
            const li = document.createElement('li');
            li.textContent = 'No hay historial de transacciones.';
            li.classList.add('list-group-item', 'text-muted');
            historial.appendChild(li);
        }
    }

    const modalElement = document.getElementById('resultadoModal');
    if (modalElement) {
        const modal = new bootstrap.Modal(modalElement);
        modal.show();
    } else {
        console.warn("⚠️ Modal de resultado no encontrado en el DOM.");
    }
}
