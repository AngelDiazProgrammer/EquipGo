// Guardar nuevo estado o actualizar existente
window.guardarEstado = async function () {
    const form = document.getElementById('formCrearEstado');
    const estadoId = form.getAttribute('data-id');

    const estadoDto = {
        NombreEstado: document.getElementById('nombre').value
    };

    const url = estadoId
        ? `/api/estados/admin/${estadoId}`
        : `/api/estados/admin`;

    const method = estadoId ? 'PUT' : 'POST';

    try {
        const response = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(estadoDto)
        });

        if (response.ok) {
            alert(`✅ Estado ${estadoId ? 'actualizado' : 'registrado'} correctamente.`);
            form.reset();
            form.removeAttribute('data-id');

            const modalElement = document.getElementById('modalCrearEstado');
            const modalInstance = bootstrap.Modal.getInstance(modalElement);
            modalInstance.hide();

            modalElement.addEventListener('hidden.bs.modal', async () => {
                try {
                    await DotNet.invokeMethodAsync('OUT_APP_EQUIPGO', 'RefrescarListaEstados');
                } catch (ex) {
                    console.error("❌ Excepción capturada:", ex);
                    window.location.reload();
                }
            }, { once: true });
        } else {
            const error = await response.json();
            alert('❌ Error: ' + (error.error || 'No se pudo guardar el estado.'));
        }
    } catch (error) {
        console.error(error);
        alert('❌ Error de red o servidor.');
    }
};

// Abrir modal para editar estado
window.editarEstado = async function (id) {
    try {
        const response = await fetch(`/api/estados/${id}`);
        if (!response.ok) throw new Error("No se pudo obtener el estado");

        const estado = await response.json();

        document.getElementById('nombre').value = estado.NombreEstado || "";
        document.getElementById('formCrearEstado').setAttribute('data-id', estado.id);

        const modal = new bootstrap.Modal(document.getElementById('modalCrearEstado'));
        modal.show();
    } catch (error) {
        console.error("Error al cargar el estado para editar:", error);
        alert("❌ No se pudo cargar la información del estado.");
    }
};

// Abrir modal eliminar
let estadoIdAEliminar = null;
window.abrirModalEliminarEstado = function (id) {
    estadoIdAEliminar = id;

    const modalElement = document.getElementById('modalEliminarEstado');
    if (!modalElement) {
        console.error("❌ No se encontró el modal con ID 'modalEliminarEstado'");
        return;
    }

    try {
        const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
        modal.show();
    } catch (error) {
        console.error("❌ Error al abrir el modal de eliminación:", error);
    }
};

window.confirmarEliminarEstado = async function () {
    if (!estadoIdAEliminar) return;

    try {
        const response = await fetch(`/api/estados/admin/${estadoIdAEliminar}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            alert("✅ Estado eliminado correctamente.");
            estadoIdAEliminar = null;

            const modal = bootstrap.Modal.getInstance(document.getElementById('modalEliminarEstado'));
            modal.hide();

            setTimeout(() => {
                DotNet.invokeMethodAsync('OUT_APP_EQUIPGO', 'RefrescarListaEstados');
            }, 300);
        } else {
            alert("❌ Error al eliminar el estado.");
        }
    } catch (error) {
        console.error(error);
        alert("❌ Error de red o servidor.");
    }
};
