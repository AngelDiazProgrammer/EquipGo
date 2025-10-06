    "use strict";

    const connection = new signalR.HubConnectionBuilder()
    .withUrl("/dashboardHub")
    .build();

    connection.start()
    .then(() => {
        console.log("✅ Conexión SignalR establecida");
    actualizarConteosDashboard();
    })
    .catch(err => console.error("❌ Error de conexión SignalR:", err.toString()));

connection.on("NuevaTransaccion", () => {
        console.log("🔄 Nueva transacción detectada");
    agregarFilaNueva();
    actualizarConteosDashboard();
});

//Filtros dinamicos
// Añade esto al inicio de tu archivo dashboard.js

document.addEventListener('applyFilters', (event) => {
    const { type, data } = event.detail;
    console.log(`Evento de filtros recibido para tipo: ${type}`, data);

    // Aquí puedes construir la URL de tu API con los filtros
    // Ejemplo: /api/Transacciones/GetFiltradas?usuario=...&tipoTransaccion=...

    let apiUrl = '';
    if (type === 'corporativo') {
        apiUrl = '/api/Transacciones/GetFiltradasCorporativo';
    } else {
        apiUrl = '/api/Transacciones/GetFiltradasVisitantes';
    }

    // Construir la query string a partir del objeto 'data'
    const queryString = new URLSearchParams(data).toString();
    if (queryString) {
        apiUrl += `?${queryString}`;
    }

    // Llamar a tu API y actualizar la tabla correspondiente
    fetch(apiUrl)
        .then(response => response.json())
        .then(transaccionesFiltradas => {
            // Lógica para limpiar la tabla actual y llenarla con 'transaccionesFiltradas'
            console.log("Datos filtrados recibidos:", transaccionesFiltradas);
            // Ejemplo: actualizarTabla(type, transaccionesFiltradas);
        })
        .catch(error => console.error("Error al aplicar filtros:", error));
});

// Función para actualizar las tarjetas de conteo
function actualizarConteosDashboard() {
    fetch("/api/Transacciones/GetConteosDashboard")
        .then(response => response.json())
        .then(data => {
            document.getElementById("totalHoy").innerText = data.totalHoy;
            document.getElementById("totalNormales").innerText = data.totalNormales;
            document.getElementById("totalVisitantes").innerText = data.totalVisitantes;
        })
        .catch(error => console.error("❌ Error al obtener conteos:", error));
}

// Función para agregar la nueva fila (igual que antes)
function agregarFilaNueva() {
    fetch("/api/Transacciones/GetTransaccionesHoy")
        .then(response => response.json())
        .then(data => {
            if (data.length === 0) return;

            const tbody = document.querySelector("#tablaDashboard tbody");
            const nueva = data[0];

            let rowColor = "";
            if (nueva.nombreTipoTransaccion.toLowerCase() === "entrada") {
                rowColor = "#d4edda"; // verde claro
            } else if (nueva.nombreTipoTransaccion.toLowerCase() === "salida") {
                rowColor = "#f8d7da"; // rojo claro
            }

            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${nueva.nombreUsuarioInfo}</td>
                <td>${nueva.codigoBarras}</td>
                <td>${nueva.nombreTipoTransaccion}</td>
                <td>${nueva.nombreUsuarioSession}</td>
                <td>${nueva.nombreSedeOs}</td>
            `;
            row.style.backgroundColor = rowColor;
            tbody.insertBefore(row, tbody.firstChild); // Insertar arriba

            setTimeout(() => {
                row.style.backgroundColor = "";
            }, 3000);

            // Mantener máximo 13 filas
            while (tbody.rows.length > 6) {
                tbody.deleteRow(tbody.rows.length - 1);
            }
        })
        .catch(error => console.error("❌ Error al agregar transacción:", error));
}


//Transacciones Visitantes
connection.on("NuevaTransaccionVisitante", () => {
    console.log("🔄 Nueva transacción de visitante detectada");
    agregarFilaNuevaVisitante();
    actualizarConteosDashboard();
});

function agregarFilaNuevaVisitante() {
    fetch("/api/Transacciones/GetTransaccionesVisitantesHoy")
        .then(response => response.json())
        .then(data => {
            if (data.length === 0) return;

            const tbody = document.querySelector("#tablaVisitantes tbody");
            const t = data[0];

            let rowColor = "";
            if (t.tipoTransaccion?.toLowerCase() === "entrada") {
                rowColor = "#d4edda";
            } else if (t.tipoTransaccion?.toLowerCase() === "salida") {
                rowColor = "#f8d7da";
            }

            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${t.nombresVisitante || ''}</td>
                <td>${t.marcaEquipo || ''}</td>
                <td>${t.tipoTransaccion || ''}</td>
                <td>${t.nombreAprobador || ''}</td>
                <td>${t.nombreSede || ''}</td>
            `;
            row.style.backgroundColor = rowColor;
            tbody.insertBefore(row, tbody.firstChild); // Insertar arriba

            setTimeout(() => {
                row.style.backgroundColor = "";
            }, 3000);

            // Máximo 13 filas
            while (tbody.rows.length > 6) {
                tbody.deleteRow(tbody.rows.length - 1);
            }
        })
        .catch(error => console.error("❌ Error al agregar transacción visitante:", error));
}