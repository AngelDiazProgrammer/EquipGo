window.guardarEquipo = async function () {
    const form = document.getElementById('formCrearEquipo');
    const equipoId = form.getAttribute('data-id'); // puede ser null o string

    const equipoDto = {
        marca: document.getElementById('marca').value,
        modelo: document.getElementById('modelo').value,
        serial: document.getElementById('serial').value,
        codigoBarras: document.getElementById('codigoBarras').value,
        ubicacion: document.getElementById('ubicacion').value,
        idUsuarioInfo: parseInt(document.getElementById('usuarioInfo').value),
        idEstado: parseInt(document.getElementById('estado').value) || null,
        idEquipoPersonal: parseInt(document.getElementById('equipoPersonal').value) || null,
        idSede: parseInt(document.getElementById('sede').value) || null,
        idTipoDispositivo: parseInt(document.getElementById('tipoDispositivo').value) || null,
        latitud: parseFloat(document.getElementById('latitud').value) || null,
        longitud: parseFloat(document.getElementById('longitud').value) || null,
        sistemaOperativo: document.getElementById('sistemaOperativo').value,
        macEquipo: document.getElementById('macEquipo').value,
        versionSoftware: document.getElementById('versionSoftware').value
    };

    const url = equipoId
        ? `/api/equipos/admin/${equipoId}`
        : `/api/equipos/admin`;
    const method = equipoId ? 'PUT' : 'POST';

    try {
        const response = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(equipoDto)
        });

        if (response.ok) {
            alert(`✅ Equipo ${equipoId ? 'actualizado' : 'registrado'} correctamente.`);
            form.reset();
            form.removeAttribute('data-id');

            const modalElement = document.getElementById('modalCrearEquipo');
            const modalInstance = bootstrap.Modal.getInstance(modalElement);
            modalInstance.hide();

            modalElement.addEventListener('hidden.bs.modal', async () => {
                try {
                    await DotNet.invokeMethodAsync('OUT_APP_EQUIPGO', 'RefrescarListaEquipos');
                } catch (ex) {
                    console.error("❌ Excepción capturada:", ex);
                    window.location.reload();
                }
            }, { once: true });
        } else {
            const error = await response.json();
            alert('❌ Error: ' + (error.error || 'No se pudo guardar el equipo.'));
        }
    } catch (error) {
        console.error(error);
        alert('❌ Error de red o servidor.');
    }
};

window.abrirModalEditar = async function (idEquipo) {
    try {
        const response = await fetch(`/api/equipos/${idEquipo}`);
        if (!response.ok) throw new Error("No se pudo obtener el equipo");

        const equipo = await response.json();

        // Llenar los campos del modal
        document.getElementById('editarId').value = equipo.id;
        document.getElementById('editarMarca').value = equipo.marca || "";
        document.getElementById('editarModelo').value = equipo.modelo || "";
        document.getElementById('editarSerial').value = equipo.serial || "";
        document.getElementById('editarCodigoBarras').value = equipo.codigoBarras || "";
        document.getElementById('editarUbicacion').value = equipo.ubicacion || "";
        document.getElementById('editarLatitud').value = equipo.latitud || "";
        document.getElementById('editarLongitud').value = equipo.longitud || "";
        document.getElementById('editarSistemaOperativo').value = equipo.sistemaOperativo || "";
        document.getElementById('editarMacEquipo').value = equipo.macEquipo || "";
        document.getElementById('editarVersionSoftware').value = equipo.versionSoftware || "";

        document.getElementById('editarUsuarioInfo').value = equipo.idUsuarioInfo;
        document.getElementById('editarEstado').value = equipo.idEstado || "";
        document.getElementById('editarEquipoPersonal').value = equipo.idEquipoPersonal || "";
        document.getElementById('editarSede').value = equipo.idSede || "";
        document.getElementById('editarTipoDispositivo').value = equipo.idTipoDispositivo || "";

        // Mostrar modal
        const modal = new bootstrap.Modal(document.getElementById('modalEditarEquipo'));
        modal.show();
    } catch (error) {
        console.error("Error al abrir modal de edición:", error);
        alert("❌ No se pudo cargar la información del equipo.");
    }
};

window.editarEquipo = async function (id) {
    try {
        await cargarSelects(); 
        const response = await fetch(`/api/equipos/${id}`);
        if (!response.ok) throw new Error("No se pudo obtener el equipo");

        const equipo = await response.json();

        // Llenar los campos del modal
        document.getElementById('marca').value = equipo.marca || "";
        document.getElementById('modelo').value = equipo.modelo || "";
        document.getElementById('serial').value = equipo.serial || "";
        document.getElementById('codigoBarras').value = equipo.codigoBarras || "";
        document.getElementById('ubicacion').value = equipo.ubicacion || "";
        document.getElementById('usuarioInfo').value = equipo.idUsuarioInfo || "";
        document.getElementById('estado').value = equipo.idEstado || "";
        document.getElementById('equipoPersonal').value = equipo.idEquipoPersonal || "";
        document.getElementById('sede').value = equipo.idSede || "";
        document.getElementById('tipoDispositivo').value = equipo.idTipoDispositivo || "";
        document.getElementById('latitud').value = equipo.latitud || "";
        document.getElementById('longitud').value = equipo.longitud || "";
        document.getElementById('sistemaOperativo').value = equipo.sistemaOperativo || "";
        document.getElementById('macEquipo').value = equipo.macEquipo || "";
        document.getElementById('versionSoftware').value = equipo.versionSoftware || "";

        // Guardar el ID en un atributo
        document.getElementById('formCrearEquipo').setAttribute('data-id', id);

        setTimeout(() => {
            document.getElementById('usuarioInfo').tomselect.setValue(equipo.idUsuarioInfo);
            document.getElementById('estado').tomselect.setValue(equipo.idEstado);
            document.getElementById('equipoPersonal').tomselect.setValue(equipo.idEquipoPersonal);
            document.getElementById('sede').tomselect.setValue(equipo.idSede);
            document.getElementById('tipoDispositivo').tomselect.setValue(equipo.idTipoDispositivo);
        }, 100);

        // Mostrar modal
        const modal = new bootstrap.Modal(document.getElementById('modalCrearEquipo'));
        modal.show();
    } catch (error) {
        alert("❌ Error al cargar el equipo.");
        console.error(error);
    }
};

window.cargarSelects = async function () {
    try {
        const response = await fetch('/api/equipos/admin/form-data');
        if (!response.ok) throw new Error('Error al cargar datos del formulario');
        const data = await response.json();

        const selects = [
            { id: 'usuarioInfo', list: data.usuarios, value: 'id', text: item => `${item.nombres} ${item.apellidos}` },
            { id: 'estado', list: data.estados, value: 'id', text: item => item.nombreEstado },
            { id: 'equipoPersonal', list: data.equiposPersonales, value: 'id', text: item => item.nombrePersonal },
            { id: 'sede', list: data.sedes, value: 'id', text: item => item.nombreSede },
            { id: 'tipoDispositivo', list: data.tiposDispositivo, value: 'id', text: item => item.nombreTipo }
        ];

        for (const { id, list, value, text } of selects) {
            const select = document.getElementById(id);
            if (!select) continue;
            select.innerHTML = '';
            list.forEach(item => {
                const option = document.createElement('option');
                option.value = item[value];
                option.text = text(item);
                select.appendChild(option);
            });

            if (!select.tomselect) {
                new TomSelect(select, {
                    placeholder: 'Buscar...',
                    maxOptions: 500,
                    allowEmptyOption: true,
                    sortField: { field: "text", direction: "asc" }
                });
            }
        }
    } catch (error) {
        console.error('❌ Error en cargarSelects:', error);
    }
};

window.llenarSelect = function (selectId, lista, valueField, textField) {
    const select = document.getElementById(selectId);
    if (!select) {
        console.error(`Select no encontrado: ${selectId}`);
        return;
    }
    select.innerHTML = '';
    lista.forEach(item => {
        const option = document.createElement('option');
        option.value = item[valueField];
        if (selectId === 'usuarioInfo') {
            option.text = `${item.nombres} ${item.apellidos}`;
        } else {
            option.text = item[textField];
        }
        select.appendChild(option);
    });
};

let equipoIdAEliminar = null;

window.abrirModalEliminar = function (id) {
    equipoIdAEliminar = id; // se guarda el ID aquí correctamente

    const modalElement = document.getElementById('modalEliminarEquipo');
    if (!modalElement) {
        console.error("❌ No se encontró el modal con ID 'modalEliminarEquipo'");
        return;
    }

    const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
    modal.show();
};

window.confirmarEliminarEquipo = async function () {
    if (!equipoIdAEliminar) return;

    try {
        const response = await fetch(`/api/equipos/admin/${equipoIdAEliminar}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            alert("✅ Equipo eliminado correctamente.");
            equipoIdAEliminar = null;

            const modal = bootstrap.Modal.getInstance(document.getElementById('modalEliminarEquipo'));
            modal.hide();

            setTimeout(() => {
                DotNet.invokeMethodAsync('OUT_OS_APP.EQUIPGO', 'RefrescarListaEquipos');
            }, 300);
        } else {
            alert("❌ Error al eliminar el equipo.");
        }
    } catch (error) {
        console.error(error);
        alert("❌ Error de red o servidor.");
    }
};


