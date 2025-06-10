"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/dashboardHub")
    .build();

connection.start().then(() => {
    console.log("✅ Conexión SignalR establecida");
    actualizarConteosDashboard(); // 🟢 Llamada inicial para los contadores
}).catch(err => console.error("❌ Error de conexión SignalR:", err.toString()));

connection.on("NuevaTransaccion", () => {
    console.log("🔄 Nueva transacción detectada");
    agregarFilaNueva();
    actualizarConteosDashboard();
});

// Función para actualizar las tarjetas de conteo
function actualizarConteosDashboard() {
    fetch("/api/Transacciones/GetConteosDashboard")
        .then(response => response.json())
        .then(data => {
            document.getElementById("totalHoy").innerText = data.totalHoy;
            document.getElementById("totalPersonales").innerText = data.totalPersonales;
            document.getElementById("totalCorporativos").innerText = data.totalCorporativos;
            document.getElementById("totalProveedores").innerText = data.totalProveedores;
        })
        .catch(error => console.error("❌ Error al obtener conteos:", error));
}

// Función para agregar la nueva fila (igual que antes)
function agregarFilaNueva() {
    fetch("/api/Transacciones/GetTransaccionesHoy")
        .then(response => response.json())
        .then(data => {
            if (data.length === 0) return;

            const tbody = document.querySelector("table tbody");
            const nueva = data[0];

            let rowColor = "";
            if (nueva.nombreTipoTransaccion.toLowerCase() === "entrada") {
                rowColor = "#d4edda";
            } else if (nueva.nombreTipoTransaccion.toLowerCase() === "salida") {
                rowColor = "#f8d7da";
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
            tbody.insertBefore(row, tbody.firstChild);

            setTimeout(() => {
                row.style.backgroundColor = "";
            }, 3000);

            while (tbody.rows.length > 12) {
                tbody.deleteRow(tbody.rows.length - 1);
            }
        })
        .catch(error => console.error("❌ Error al agregar transacción:", error));
}
