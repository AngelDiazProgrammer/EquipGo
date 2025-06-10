"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/dashboardHub")
    .build();

connection.start().then(() => {
    console.log("✅ Conexión SignalR establecida");
}).catch(err => console.error("❌ Error de conexión SignalR:", err.toString()));

// Evento del servidor
connection.on("NuevaTransaccion", () => {
    console.log("🔄 Nueva transacción detectada");
    agregarFilaNueva();
});

// Función para agregar solo la nueva fila
function agregarFilaNueva() {
    fetch("/api/Transacciones/GetTransaccionesHoy")
        .then(response => response.json())
        .then(data => {
            if (data.length === 0) return;

            const tbody = document.querySelector("table tbody");

            // La transacción más reciente es la primera (ordenada descendentemente)
            const nueva = data[0];

            let rowColor = "";
            // Aquí podrías usar una regla más detallada si necesitas
            if (nueva.nombreTipoTransaccion.toLowerCase() === "entrada") {
                rowColor = "#d4edda"; // verde suave
            } else if (nueva.nombreTipoTransaccion.toLowerCase() === "salida") {
                rowColor = "#f8d7da"; // rojo suave
            }

            const row = document.createElement("tr");
            row.innerHTML = `
    <td>${nueva.nombreUsuarioInfo}</td>
    <td>${nueva.codigoBarras}</td>
    <td>${nueva.nombreTipoTransaccion}</td>
    <td>${nueva.nombreEquipoPersonal}</td>
    <td>${nueva.nombreUsuarioSession}</td>
    <td>${nueva.nombreSedeOs}</td>
    <td><button>Ver ubicación</button></td>
`;

            row.style.backgroundColor = rowColor;
            tbody.insertBefore(row, tbody.firstChild); // Insertar arriba

            // Quitar el color luego de 3 segundos
            setTimeout(() => {
                row.style.backgroundColor = "";
            }, 3000);

            // Limitar a 12 filas máximo
            while (tbody.rows.length > 12) {
                tbody.deleteRow(tbody.rows.length - 1);
            }
        })
        .catch(error => console.error("❌ Error al agregar transacción:", error));
}
