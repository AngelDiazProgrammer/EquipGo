window.guardarEquipo = async function () {
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

    try {
        const response = await fetch('/api/equipos/admin', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(equipoDto)
        });

        if (response.ok) {
            alert('✅ Equipo registrado correctamente.');
            document.getElementById('formCrearEquipo').reset();

            const modalElement = document.getElementById('modalCrearEquipo');
            const modalInstance = bootstrap.Modal.getInstance(modalElement);
            modalInstance.hide();

            modalElement.addEventListener('hidden.bs.modal', async () => {
                try {
                    await DotNet.invokeMethodAsync('OUT_OS_APP.EQUIPGO', 'RefrescarListaEquipos');
                } catch (ex) {
                    console.error("❌ Excepción capturada:", ex);
                    window.location.reload();
                }
            }, { once: true });

        } else {
            const error = await response.json();
            alert('❌ Error: ' + (error.error || 'No se pudo registrar el equipo.'));
        }
    } catch (error) {
        console.error(error);
        alert('❌ Error de red o servidor.');
    }
};




window.cargarSelects = async function () {
    try {
        const response = await fetch('/api/equipos/admin/form-data');
        if (!response.ok) throw new Error('Error al cargar datos del formulario');
        const data = await response.json();

        console.log('Datos cargados:', data);

        window.llenarSelect('usuarioInfo', data.usuarios, 'id', null);
        window.llenarSelect('estado', data.estados, 'id', 'nombreEstado');
        window.llenarSelect('equipoPersonal', data.equiposPersonales, 'id', 'nombrePersonal');
        window.llenarSelect('sede', data.sedes, 'id', 'nombreSede');
        window.llenarSelect('tipoDispositivo', data.tiposDispositivo, 'id', 'nombreTipo');
    } catch (error) {
        console.error(error);
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
