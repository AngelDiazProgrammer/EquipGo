// Guardar nuevo area o actualizar existente
window.guardarArea = async function () {
    const form = document.getElementById('formCrearArea');
    const areaId = form.getAttribute('data-id');

    const areaDto = {
        NombreArea: document.getElementById('nombre').value
    };

    const url = areaId
        ? `/api/areas/admin/${areaId}`
        : `/api/areas/admin`;

    const method = areaId ? 'PUT' : 'POST';

    try {
        const response = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(areaDto)
        });

        if (response.ok) {
            alert(`✅ Area ${areaId ? 'actualizada' : 'registrada'} correctamente.`);
            form.reset();
            form.removeAttribute('data-id');

            const modalElement = document.getElementById('modalCrearArea');
            const modalInstance = bootstrap.Modal.getInstance(modalElement);
            modalInstance.hide();

            modalElement.addEventListener('hidden.bs.modal', async () => {
                try {
                    await DotNet.invokeMethodAsync('OUT_APP_EQUIPGO', 'RefrescarListaAreas');
                } catch (ex) {
                    console.error("❌ Excepción capturada:", ex);
                    window.location.reload();
                }
            }, { once: true });
        } else {
            const error = await response.json();
            alert('❌ Error: ' + (error.error || 'No se pudo guardar el area.'));
        }
    } catch (error) {
        console.error(error);
        alert('❌ Error de red o servidor.');
    }
};

// Abrir modal para editar area
window.editarArea = async function (id) {
    try {
        const response = await fetch(`/api/areas/${id}`);
        if (!response.ok) throw new Error("No se pudo obtener el area");

        const area = await response.json();

        document.getElementById('nombre').value = area.NombreArea || "";
        document.getElementById('formCrearArea').setAttribute('data-id', area.id);

        const modal = new bootstrap.Modal(document.getElementById('modalCrearArea'));
        modal.show();
    } catch (error) {
        console.error("Error al cargar el area para editar:", error);
        alert("❌ No se pudo cargar la información del area.");
    }
};

// Abrir modal eliminar
let areaIdAEliminar = null;
window.abrirModalEliminarArea = function (id) {
    areaIdAEliminar = id;

    const modalElement = document.getElementById('modalEliminarArea');
    if (!modalElement) {
        console.error("❌ No se encontró el modal con ID 'modalEliminarArea'");
        return;
    }

    try {
        const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
        modal.show();
    } catch (error) {
        console.error("❌ Error al abrir el modal de eliminación:", error);
    }
};

window.confirmarEliminarArea = async function () {
    if (!areaIdAEliminar) {
        alert("❌ No hay area seleccionada para eliminar.");
        return;
    }

    try {
        const response = await fetch(`/api/areas/admin/${areaIdAEliminar}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            alert("✅ Area eliminada correctamente.");
            areaIdAEliminar = null;

            const modalElement = document.getElementById('modalEliminarArea');
            const modal = bootstrap.Modal.getInstance(modalElement);
            if (modal) modal.hide();

            setTimeout(() => {
                DotNet.invokeMethodAsync('OUT_OS_APP.EQUIPGO', 'RefrescarListaAreas');
            }, 300);

        } else {
            const error = await response.json();
            alert("❌ Error al eliminar el area: " + (error.error || "Error desconocido."));
        }

    } catch (error) {
        console.error("❌ Error de red o servidor:", error);
        alert("❌ Error de red o servidor al intentar eliminar el area.");
    }
};
