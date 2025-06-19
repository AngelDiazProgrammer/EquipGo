window.registroVisitante = {
    cargarProveedores: async function (intentos = 0) {
        console.log("✅ visitante.js cargado");

        if (intentos > 10) {
            console.error("❌ No se pudo encontrar el select después de varios intentos.");
            return;
        }

        const select = document.querySelector("#selectProveedores");

        if (!select) {
            console.warn("⚠️ No se encontró el select. Reintentando...");
            setTimeout(() => window.registroVisitante.cargarProveedores(intentos + 1), 300);
            return;
        }

        try {
            const response = await fetch("/api/proveedores");
            if (!response.ok) throw new Error("Respuesta inválida");

            const data = await response.json();

            data.forEach(prov => {
                const option = document.createElement("option");
                option.value = prov.id;
                option.text = prov.nombreProveedor;
                select.appendChild(option);
            });

            console.log("✅ Proveedores cargados correctamente");
        } catch (error) {
            console.error("❌ Error cargando proveedores:", error);
        }
    }
};
