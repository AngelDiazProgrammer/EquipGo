// Guardar nuevo subestado o actualizar existente
window.guardarSubEstado = async function () {
    const form = document.getElementById('formCrearSubEstado');
    const subEstadoId = form.getAttribute('data-id');

    const subEstadoDto = {
        NombreSubEstado: document.getElementById('nombre').value
    };

    const url = subEstadoId
        ? `/api/subestados/admin/${subEstadoId}`
        : `/api/subestados/admin`;

    const method = subEstadoId ? 'PUT' : 'POST';

    try {
        const response = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(subEstadoDto)
        });

        if (response.ok) {
            alert(`✅ Subestado ${subEstadoId ? 'actualizado' : 'registrado'} correctamente.`);
            form.reset();
            form.removeAttribute('data-id');

            const modalElement = document.getElementById('modalCrearSubEstado');
            const modalInstance = bootstrap.Modal.getInstance(modalElement);
            modalInstance.hide();

            modalElement.addEventListener('hidden.bs.modal', async () => {
                try {
                    await DotNet.invokeMethodAsync('OUT_APP_EQUIPGO', 'RefrescarListaSubEstados');
                } catch (ex) {
                    console.error("❌ Excepción capturada:", ex);
                    window.location.reload();
                }
            }, { once: true });
        } else {
            const error = await response.json();
            alert('❌ Error: ' + (error.error || 'No se pudo guardar el subestado.'));
        }
    } catch (error) {
        console.error(error);
        alert('❌ Error de red o servidor.');
    }
};

// Abrir modal para editar subestado
window.editarSubEstado = async function (id) {
    try {
        const response = await fetch(`/api/subestados/${id}`);
        if (!response.ok) throw new Error("No se pudo obtener el subestado");

        const subEstado = await response.json();

        document.getElementById('nombre').value = subEstado.NombreSubEstado || "";
        document.getElementById('formCrearSubEstado').setAttribute('data-id', subEstado.id);

        const modal = new bootstrap.Modal(document.getElementById('modalCrearSubEstado'));
        modal.show();
    } catch (error) {
        console.error("Error al cargar el subestado para editar:", error);
        alert("❌ No se pudo cargar la información del subestado.");
    }
};

// Abrir modal eliminar
let subEstadoIdAEliminar = null;
window.abrirModalEliminarSubEstado = function (id) {
    subEstadoIdAEliminar = id;

    const modalElement = document.getElementById('modalEliminarSubEstado');
    if (!modalElement) {
        console.error("❌ No se encontró el modal con ID 'modalEliminarSubEstado'");
        return;
    }

    try {
        const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
        modal.show();
    } catch (error) {
        console.error("❌ Error al abrir el modal de eliminación:", error);
    }
};

window.confirmarEliminarSubEstado = async function () {
    if (!subEstadoIdAEliminar) return;

    try {
        const response = await fetch(`/api/subestados/admin/${subEstadoIdAEliminar}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            alert("✅ Subestado eliminado correctamente.");
            subEstadoIdAEliminar = null;

            const modal = bootstrap.Modal.getInstance(document.getElementById('modalEliminarSubEstado'));
            modal.hide();

            setTimeout(() => {
                DotNet.invokeMethodAsync('OUT_APP_EQUIPGO', 'RefrescarListaSubEstados');
            }, 300);
        } else {
            alert("❌ Error al eliminar el subestado.");
        }
    } catch (error) {
        console.error(error);
        alert("❌ Error de red o servidor.");
    }
};
