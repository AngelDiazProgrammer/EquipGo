const connection = new signalR.HubConnectionBuilder()
    .withUrl("/dashboardHub")
    .build();

// Función para actualizar contadores
function actualizarConteosDashboard() {
    fetch("/api/Transacciones/GetConteosDashboard")
        .then(response => {
            if (!response.ok) throw new Error('Error en la respuesta del servidor');
            return response.json();
        })
        .then(data => {
            console.log("📊 Datos de conteos recibidos:", data);

            // Actualizar contadores usando los IDs
            const totalHoyElement = document.getElementById("totalHoy");
            const totalNormalesElement = document.getElementById("totalNormales");
            const totalVisitantesElement = document.getElementById("totalVisitantes");

            if (totalHoyElement) {
                totalHoyElement.textContent = data.totalHoy;
                console.log("✅ Actualizado totalHoy:", data.totalHoy);
            } else {
                console.warn("⚠️ Elemento totalHoy no encontrado");
            }

            if (totalNormalesElement) {
                totalNormalesElement.textContent = data.totalNormales;
                console.log("✅ Actualizado totalNormales:", data.totalNormales);
            } else {
                console.warn("⚠️ Elemento totalNormales no encontrado");
            }

            if (totalVisitantesElement) {
                totalVisitantesElement.textContent = data.totalVisitantes;
                console.log("✅ Actualizado totalVisitantes:", data.totalVisitantes);
            } else {
                console.warn("⚠️ Elemento totalVisitantes no encontrado");
            }
        })
        .catch(error => console.error("❌ Error al obtener conteos:", error));
}

// Función para agregar transacción corporativa
async function agregarFilaNueva() {
    try {
        console.log("🔄 Buscando nueva transacción corporativa...");

        const data = await fetch("/api/Transacciones/GetTransaccionesHoy").then(response => {
            if (!response.ok) throw new Error('Error al obtener transacciones');
            return response.json();
        });

        if (data.length === 0) {
            console.log("ℹ️ No hay transacciones nuevas");
            return;
        }

        console.log("📦 Transacción encontrada:", data[0]);

        // Buscar la tabla por ID
        const tbody = document.getElementById("tbodyDashboard");

        if (!tbody) {
            console.error("❌ tbodyDashboard no encontrado");
            return;
        }

        const nueva = data[0];

        let rowColor = "";
        if (nueva.nombreTipoTransaccion?.toLowerCase() === "entrada") {
            rowColor = "#d4edda";
        } else if (nueva.nombreTipoTransaccion?.toLowerCase() === "salida") {
            rowColor = "#f8d7da";
        }

        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${nueva.nombreUsuarioInfo || 'N/A'}</td>
            <td>${nueva.codigoBarras || 'N/A'}</td>
            <td>${nueva.nombreTipoTransaccion || 'N/A'}</td>
            <td>${nueva.nombreUsuarioSession || 'N/A'}</td>
            <td>${nueva.nombreSedeOs || 'N/A'}</td>
        `;

        if (rowColor) {
            row.style.backgroundColor = rowColor;
            setTimeout(() => {
                row.style.backgroundColor = "";
            }, 3000);
        }

        // Insertar al inicio de la tabla
        if (tbody.firstChild) {
            tbody.insertBefore(row, tbody.firstChild);
        } else {
            tbody.appendChild(row);
        }

        console.log("✅ Fila corporativa agregada correctamente");

        // Mantener máximo 20 filas
        while (tbody.rows.length > 6) {
            tbody.deleteRow(tbody.rows.length - 1);
        }

    } catch (error) {
        console.error("❌ Error al agregar transacción corporativa:", error);
    }
}

// Función para agregar transacción de visitante
async function agregarFilaNuevaVisitante() {
    try {
        console.log("🔄 Buscando nueva transacción de visitante...");

        const data = await fetch("/api/Transacciones/GetTransaccionesVisitantesHoy").then(response => {
            if (!response.ok) throw new Error('Error al obtener transacciones de visitantes');
            return response.json();
        });

        if (data.length === 0) {
            console.log("ℹ️ No hay transacciones de visitantes nuevas");
            return;
        }

        console.log("📦 Transacción de visitante encontrada:", data[0]);

        // Buscar la tabla por ID
        const tbody = document.getElementById("tbodyVisitantes");

        if (!tbody) {
            console.error("❌ tbodyVisitantes no encontrado");
            return;
        }

        const t = data[0];

        let rowColor = "";
        if (t.tipoTransaccion?.toLowerCase() === "entrada") {
            rowColor = "#d4edda";
        } else if (t.tipoTransaccion?.toLowerCase() === "salida") {
            rowColor = "#f8d7da";
        }

        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${t.nombresVisitante || 'N/A'}</td>
            <td>${t.marcaEquipo || 'N/A'}</td>
            <td>${t.tipoTransaccion || 'N/A'}</td>
            <td>${t.nombreAprobador || 'N/A'}</td>
            <td>${t.nombreSede || 'N/A'}</td>
        `;

        if (rowColor) {
            row.style.backgroundColor = rowColor;
            setTimeout(() => {
                row.style.backgroundColor = "";
            }, 3000);
        }

        // Insertar al inicio de la tabla
        if (tbody.firstChild) {
            tbody.insertBefore(row, tbody.firstChild);
        } else {
            tbody.appendChild(row);
        }

        console.log("✅ Fila de visitante agregada correctamente");

        // Mantener máximo 6 filas
        while (tbody.rows.length > 6) {
            tbody.deleteRow(tbody.rows.length - 1);
        }

    } catch (error) {
        console.error("❌ Error al agregar transacción de visitante:", error);
    }
}

// Eventos de SignalR
connection.on("NuevaTransaccion", () => {
    console.log("🔔 Evento SignalR: Nueva transacción corporativa");
    agregarFilaNueva();
    actualizarConteosDashboard();
});

connection.on("NuevaTransaccionVisitante", () => {
    console.log("🔔 Evento SignalR: Nueva transacción de visitante");
    agregarFilaNuevaVisitante();
    actualizarConteosDashboard();
});

// Iniciar conexión
connection.start()
    .then(() => {
        console.log("✅ Conexión SignalR establecida");
        actualizarConteosDashboard();
    })
    .catch(err => {
        console.error("❌ Error de conexión SignalR:", err.toString());
        // Reintentar conexión después de 5 segundos
        setTimeout(() => {
            console.log("🔄 Reintentando conexión SignalR...");
            connection.start();
        }, 5000);
    });