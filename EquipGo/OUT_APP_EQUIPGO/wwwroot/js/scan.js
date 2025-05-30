console.log("✅ scan.js cargado correctamente");
document.addEventListener("DOMContentLoaded", () => {
    console.log("✅ DOM listo (scan.js)");
});

function mostrarModalResultado(codigoBarras) {
    console.log("📋 Mostrando modal para el código:", codigoBarras);

    // Simular llamada a backend o generación de datos
    const datosSimulados = {
        marca: "MarcaX",
        modelo: "ModeloY",
        serial: codigoBarras,
        ubicacion: "Almacén Principal",
        nombreUsuario: "Juan Pérez",
        documentoUsuario: "123456789",
        historialTransacciones: [
            "2024-05-29 09:00 - Usuario1",
            "2024-05-28 17:30 - Usuario2"
        ]
    };

    // Llenar el modal con la información simulada
    const equipoInfo = document.getElementById('equipoInfo');
    if (equipoInfo) {
        equipoInfo.innerText = `Marca: ${datosSimulados.marca}, Modelo: ${datosSimulados.modelo}, Serial: ${datosSimulados.serial}, Ubicación: ${datosSimulados.ubicacion}, Usuario: ${datosSimulados.nombreUsuario} (${datosSimulados.documentoUsuario})`;
    }

    const historial = document.getElementById('historial');
    if (historial) {
        historial.innerHTML = '';
        datosSimulados.historialTransacciones.forEach(item => {
            const li = document.createElement('li');
            li.textContent = item;
            li.classList.add('list-group-item');
            historial.appendChild(li);
        });
    }

    // Mostrar el modal
    const modalElement = document.getElementById('resultadoModal');
    if (modalElement) {
        console.log("🔍 modalElement:", modalElement);
        const modal = new bootstrap.Modal(modalElement);
        console.log("🚀 Modal creado, intentando mostrar...");
        modal.show();
    }
}
