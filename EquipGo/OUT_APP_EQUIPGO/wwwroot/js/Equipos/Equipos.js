// Cache global para usuarios del Active Directory
window._usuariosCache = null;
// Variable global para almacenar información temporal
window.equipoAsignacionActual = null;

// Función para abrir el modal de asignación
window.abrirModalAsignarUsuario = async function (equipoId) {
    try {
        console.log("🔄 Abriendo modal de asignación para equipo:", equipoId);

        // Guardar información del equipo
        window.equipoAsignacionActual = { equipoId };

        // Limpiar formulario
        limpiarFormularioAsignacion();

        // Cargar selects necesarios
        await cargarSelectsAsignacion();

        // Cargar información del usuario actual (si existe)
        await cargarEstadoActualAsignacion(equipoId);

        // Mostrar el modal
        const modal = new bootstrap.Modal(document.getElementById('modalAsignarUsuario'));
        modal.show();

        console.log("✅ Modal de asignación abierto correctamente");

    } catch (error) {
        console.error("❌ Error al abrir modal de asignación:", error);
        alert("Error al cargar el modal de asignación");
    }
};

// Función para cargar el estado actual de asignación
window.cargarEstadoActualAsignacion = async function (equipoId) {
    try {
        const response = await fetch(`/api/equipos/${equipoId}`);
        if (!response.ok) return;

        const equipo = await response.json();
        const estadoDiv = document.getElementById('estadoActualAsignacion');
        const btnDesasignar = document.getElementById('btnDesasignar');
        const tituloAsignacion = document.getElementById('tituloAsignacion');

        if (equipo.idUsuarioInfo && equipo.usuarioNombreCompleto) {
            // Hay un usuario asignado
            estadoDiv.className = 'alert alert-warning';
            estadoDiv.innerHTML = `
                <div class="d-flex align-items-center">
                    <i class="bi bi-person-check me-2" style="font-size: 1.2rem;"></i>
                    <div>
                        <strong>Usuario actualmente asignado:</strong><br>
                        <span class="fw-bold">${equipo.usuarioNombreCompleto}</span><br>
                        ${equipo.usuarioDocumento ? `<small>Documento: ${equipo.usuarioDocumento}</small><br>` : ''}
                        ${equipo.usuarioArea ? `<small>Área: ${equipo.usuarioArea}</small>` : ''}
                    </div>
                </div>
            `;
            btnDesasignar.style.display = 'block';
            tituloAsignacion.textContent = 'Gestionar Usuario Asignado';
        } else {
            // No hay usuario asignado
            estadoDiv.className = 'alert alert-secondary';
            estadoDiv.innerHTML = `
                <div class="d-flex align-items-center">
                    <i class="bi bi-person-x me-2" style="font-size: 1.2rem;"></i>
                    <div>
                        <strong>Estado:</strong> Sin usuario asignado<br>
                        <small>Selecciona o crea un usuario para asignar a este equipo</small>
                    </div>
                </div>
            `;
            btnDesasignar.style.display = 'none';
            tituloAsignacion.textContent = 'Asignar Usuario al Equipo';
        }

    } catch (error) {
        console.error("❌ Error cargando estado actual:", error);
        document.getElementById('estadoActualAsignacion').innerHTML = `
            <div class="alert alert-danger">
                <i class="bi bi-exclamation-triangle"></i> Error al cargar información del usuario actual
            </div>
        `;
    }
};

// Función para cargar los selects del modal de asignación
window.cargarSelectsAsignacion = async function () {
    try {
        console.log("🔄 Cargando selects para asignación...");

        const response = await fetch('/api/equipos/admin/form-data');
        if (!response.ok) throw new Error('Error al cargar datos del formulario');
        const data = await response.json();

        // Cargar select de usuarios
        await cargarSelectUsuarios(data.usuarios || []);

        // Cargar selects para nuevo usuario
        await cargarSelectsNuevoUsuario(data);

        console.log("✅ Selects de asignación cargados correctamente");

    } catch (error) {
        console.error("❌ Error cargando selects de asignación:", error);
    }
};

// Función para cargar el select de usuarios
window.cargarSelectUsuarios = async function (usuarios) {
    const select = document.getElementById('asignarUsuarioSelect');
    if (!select) return;

    // Limpiar select
    select.innerHTML = '<option value="">-- Buscar usuario existente --</option>';

    if (usuarios && usuarios.length > 0) {
        usuarios.forEach(usuario => {
            // Solo agregar usuarios que no sean la opción "nuevo"
            if (usuario.usuario !== 'nuevo') {
                const option = document.createElement('option');
                option.value = usuario.usuario || usuario.id;

                let texto = usuario.nombreCompleto || `${usuario.nombres} ${usuario.apellidos}`;
                if (usuario.correo) {
                    texto += ` (${usuario.correo})`;
                }
                if (usuario.numeroDocumento) {
                    texto += ` - ${usuario.numeroDocumento}`;
                }

                option.textContent = texto;
                select.appendChild(option);
            }
        });
    }

    // Inicializar TomSelect si está disponible
    if (typeof TomSelect !== 'undefined' && !select.tomselect) {
        new TomSelect(select, {
            placeholder: 'Buscar usuario por nombre, email o documento...',
            maxOptions: 500,
            allowEmptyOption: true,
            sortField: { field: "text", direction: "asc" },
            searchField: ['text']
        });
    }
};

// Función para cargar selects del formulario de nuevo usuario
window.cargarSelectsNuevoUsuario = async function (data) {
    const selects = [
        { id: 'asignarTipoDocumento', list: data.tiposDocumento, value: 'id', text: 'nombreDocumento' },
        { id: 'asignarArea', list: data.areas, value: 'id', text: 'nombreArea' },
        { id: 'asignarCampana', list: data.campanas, value: 'id', text: 'nombreCampaña' }
    ];

    selects.forEach(({ id, list, value, text }) => {
        const select = document.getElementById(id);
        if (select && list) {
            select.innerHTML = '<option value="">Seleccionar...</option>';
            list.forEach(item => {
                const option = document.createElement('option');
                option.value = item[value];
                option.textContent = item[text];
                select.appendChild(option);
            });

            // Inicializar TomSelect para estos también
            if (typeof TomSelect !== 'undefined' && !select.tomselect) {
                new TomSelect(select, {
                    placeholder: 'Seleccionar...',
                    allowEmptyOption: true
                });
            }
        }
    });
};

// Función para mostrar/ocultar formulario de nuevo usuario
window.toggleFormularioNuevoUsuario = function () {
    const checkbox = document.getElementById('crearNuevoUsuarioCheckbox');
    const formulario = document.getElementById('formularioNuevoUsuario');
    const selectUsuario = document.getElementById('asignarUsuarioSelect');

    if (checkbox.checked) {
        formulario.style.display = 'block';
        if (selectUsuario.tomselect) {
            selectUsuario.tomselect.disable();
        } else {
            selectUsuario.disabled = true;
        }
        // Limpiar selección de usuario existente
        if (selectUsuario.tomselect) {
            selectUsuario.tomselect.setValue('', true);
        } else {
            selectUsuario.value = '';
        }
    } else {
        formulario.style.display = 'none';
        if (selectUsuario.tomselect) {
            selectUsuario.tomselect.enable();
        } else {
            selectUsuario.disabled = false;
        }
    }
};

// Función para limpiar el formulario de asignación
window.limpiarFormularioAsignacion = function () {
    // Limpiar checkbox
    document.getElementById('crearNuevoUsuarioCheckbox').checked = false;

    // Ocultar formulario nuevo usuario
    document.getElementById('formularioNuevoUsuario').style.display = 'none';

    // Limpiar select de usuarios
    const selectUsuario = document.getElementById('asignarUsuarioSelect');
    if (selectUsuario.tomselect) {
        selectUsuario.tomselect.setValue('', true);
        selectUsuario.tomselect.enable();
    } else {
        selectUsuario.value = '';
        selectUsuario.disabled = false;
    }

    // Limpiar formulario nuevo usuario
    document.getElementById('asignarNumeroDocumento').value = '';
    document.getElementById('asignarNombres').value = '';
    document.getElementById('asignarApellidos').value = '';

    const selectsNuevo = ['asignarTipoDocumento', 'asignarArea', 'asignarCampana'];
    selectsNuevo.forEach(id => {
        const select = document.getElementById(id);
        if (select && select.tomselect) {
            select.tomselect.setValue('', true);
        }
    });
};

// Funciones placeholder (las implementaremos después)
window.guardarAsignacionUsuario = async function () {
    alert("🔄 Función de guardar asignación - En desarrollo");
    // Aquí implementaremos la lógica completa después
};

window.desasignarUsuario = async function () {
    alert("🔄 Función de desasignar usuario - En desarrollo");
    // Aquí implementaremos la lógica completa después
};


// Función para limpiar el formulario
window.limpiarFormularioCrear = function () {
    console.log("Limpiando formulario de crear equipo...");

    try {
        const form = document.getElementById('formCrearEquipo');
        if (!form) {
            console.error("No se encontró el formulario formCrearEquipo");
            return;
        }

        // 1. Remover data-id para evitar modo edición
        form.removeAttribute('data-id');

        // 2. Limpiar campos de texto y número
        const camposTextoLimpios = [
            'marca', 'modelo', 'serial', 'codigoBarras', 'sistemaOperativo',
            'macEquipo', 'latitud', 'longitud'
        ];

        camposTextoLimpios.forEach(id => {
            const elemento = document.getElementById(id);
            if (elemento) {
                elemento.value = '';
                console.log(`✅ Campo ${id} limpiado`);
            }
        });

        // 3. Limpiar selects con TomSelect
        const selectsIds = ['usuarioInfo', 'estado', 'subEstado', 'sede', 'tipoDispositivo', 'proveedor', 'tipoDocumento', 'area', 'campana'];
        selectsIds.forEach(id => {
            const select = document.getElementById(id);
            if (select) {
                if (select.tomselect) {
                    select.tomselect.setValue('', true); // true para silenciar eventos
                } else {
                    select.value = '';
                }
                console.log(`✅ Select ${id} limpiado`);
            }
        });

        // 4. Ocultar contenedor de motivo/subestado
        const motivoContainer = document.getElementById('motivoContainer');
        if (motivoContainer) {
            motivoContainer.style.display = 'none';
            console.log("Contenedor de motivo ocultado");
        }

        // 5. Ocultar y limpiar formulario de usuario
        const formUsuarioInfo = document.getElementById('formUsuarioInfo');
        if (formUsuarioInfo) {
            formUsuarioInfo.style.display = 'none';

            // Limpiar campos del formulario de usuario
            const camposUsuario = ['tipoDocumento', 'numeroDocumento', 'nombres', 'apellidos', 'area', 'campana'];
            camposUsuario.forEach(campoId => {
                const campo = document.getElementById(campoId);
                if (campo) campo.value = '';
            });

            console.log("Formulario de usuario limpiado y ocultado");
        }

        console.log("Formulario completamente limpiado");

    } catch (error) {
        console.error("Error al limpiar el formulario:", error);
    }
};

// Guardar o actualizar equipo con usuario (versión tolerante a null)
window.guardarEquipo = async function () {
    const form = document.getElementById('formCrearEquipo');
    const equipoId = form?.getAttribute('data-id') || null;

    // Manejo seguro de selección de usuario
    const usuarioSelect = document.getElementById('usuarioInfo');
    const usuarioSeleccionado = usuarioSelect ? usuarioSelect.value : null;
    const esNuevoUsuario = usuarioSeleccionado === 'nuevo';

    // Formulario visible para nuevo usuario
    const formUsuarioInfo = document.getElementById('formUsuarioInfo');
    const formUsuarioVisible = formUsuarioInfo && formUsuarioInfo.style.display !== 'none';

    let url, method, requestBody;

    // Función auxiliar para obtener valor seguro
    const getInt = id => {
        const el = document.getElementById(id);
        if (!el || el.value.trim() === '') return null;
        const num = parseInt(el.value);
        return isNaN(num) ? null : num;
    };

    const getFloat = id => {
        const el = document.getElementById(id);
        if (!el || el.value.trim() === '') return null;
        const num = parseFloat(el.value);
        return isNaN(num) ? null : num;
    };

    if (equipoId) {
        // 🔁 Editar equipo existente
        const equipoDto = {
            marca: document.getElementById('marca').value,
            modelo: document.getElementById('modelo').value,
            serial: document.getElementById('serial').value,
            codigoBarras: document.getElementById('codigoBarras').value,
            idUsuarioInfo: getInt('usuarioInfo'),
            idEstado: getInt('estado'),
            idSubEstado: getInt('subEstado'), // ✅ Agregado subestado
            idSede: getInt('sede'),
            idTipoDispositivo: getInt('tipoDispositivo'),
            idProveedor: getInt('proveedor'),
            latitud: getFloat('latitud'),
            longitud: getFloat('longitud'),
            sistemaOperativo: document.getElementById('sistemaOperativo').value,
            macEquipo: document.getElementById('macEquipo').value,
        };

        url = `/api/equipos/admin/${equipoId}`;
        method = 'PUT';
        requestBody = JSON.stringify(equipoDto);
    } else {
        // 🆕 Crear nuevo equipo
        if (formUsuarioVisible || esNuevoUsuario) {
            console.log("🆕 Creando equipo con nuevo usuario");

            const equipoUsuarioDto = {
                // Datos del equipo
                marca: document.getElementById('marca').value,
                modelo: document.getElementById('modelo').value,
                serial: document.getElementById('serial').value,
                codigoBarras: document.getElementById('codigoBarras').value,
                idEstado: getInt('estado'),
                idSubEstado: getInt('subEstado'), // ✅ Agregado subestado
                idSede: getInt('sede'),
                idTipoDispositivo: getInt('tipoDispositivo'),
                idProveedor: getInt('proveedor'),
                latitud: getFloat('latitud'),
                longitud: getFloat('longitud'),
                sistemaOperativo: document.getElementById('sistemaOperativo').value,
                macEquipo: document.getElementById('macEquipo').value,

                // Datos del usuario
                idTipoDocumento: getInt('tipoDocumento'),
                numeroDocumento: document.getElementById('numeroDocumento').value,
                nombres: document.getElementById('nombres').value,
                apellidos: document.getElementById('apellidos').value,
                idArea: getInt('area'),
                idCampaña: getInt('campana'),
            };

            url = '/api/equipos/admin/conusuario';
            method = 'POST';
            requestBody = JSON.stringify(equipoUsuarioDto);
        } else {
            console.log("📦 Creando equipo con usuario existente o sin usuario");

            const equipoDto = {
                marca: document.getElementById('marca').value,
                modelo: document.getElementById('modelo').value,
                serial: document.getElementById('serial').value,
                codigoBarras: document.getElementById('codigoBarras').value,
                idUsuarioInfo: getInt('usuarioInfo'),
                idEstado: getInt('estado'),
                idSubEstado: getInt('subEstado'), // ✅ Agregado subestado
                idSede: getInt('sede'),
                idTipoDispositivo: getInt('tipoDispositivo'),
                idProveedor: getInt('proveedor'),
                latitud: getFloat('latitud'),
                longitud: getFloat('longitud'),
                sistemaOperativo: document.getElementById('sistemaOperativo').value,
                macEquipo: document.getElementById('macEquipo').value,
            };

            url = '/api/equipos/admin';
            method = 'POST';
            requestBody = JSON.stringify(equipoDto);
        }
    }

    try {
        console.log(`🚀 Enviando solicitud a ${url} con método ${method}`);

        const response = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: requestBody
        });

        if (response.ok) {
            alert(`✅ Equipo ${equipoId ? 'actualizado' : 'registrado'} correctamente.`);

            window.limpiarFormularioCrear();

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
            console.error("❌ Error del servidor:", error);
            alert('❌ Error: ' + (error.error || 'No se pudo guardar el equipo.'));
        }
    } catch (error) {
        console.error("❌ Error en la solicitud:", error);
        alert('❌ Error de red o servidor.');
    }
};

// Guardar cambios del equipo editado (modal de edición)
window.guardarCambiosEquipo = async function () {
    const equipoId = document.getElementById('editarId').value;

    if (!equipoId) {
        alert('❌ No se encontró el ID del equipo a editar.');
        return;
    }

    // 🔍 DEPURACIÓN: Verificar elementos del subestado
    console.log("🔍 DEPURACIÓN SubEstado en guardarCambiosEquipo:");

    const subEstadoSelect = document.getElementById('editarSubEstado');
    console.log("   - Elemento editarSubEstado:", !!subEstadoSelect);
    console.log("   - Valor de editarSubEstado:", subEstadoSelect?.value);
    console.log("   - Valor parseado:", parseInt(subEstadoSelect?.value));

    const motivoContainer = document.getElementById('motivoContainerEditar');
    console.log("   - Contenedor motivoContainerEditar:", !!motivoContainer);
    console.log("   - Display del contenedor:", motivoContainer?.style.display);

    const estadoSelect = document.getElementById('editarEstado');
    console.log("   - Estado seleccionado:", estadoSelect?.value);

    // Obtener el valor del subestado
    const idSubEstado = subEstadoSelect ? parseInt(subEstadoSelect.value) || null : null;
    console.log("   - idSubEstado final:", idSubEstado);

    // Verificar si se está creando un nuevo usuario
    const formUsuarioInfoEditar = document.getElementById('formUsuarioInfoEditar');
    const formUsuarioVisible = formUsuarioInfoEditar && formUsuarioInfoEditar.style.display !== 'none';

    // También verificamos si se seleccionó "Crear nuevo usuario"
    const usuarioSeleccionado = document.getElementById('editarUsuarioInfo').value;
    const esNuevoUsuario = usuarioSeleccionado === 'nuevo';

    let url, method, requestBody;

    if (formUsuarioVisible || esNuevoUsuario) {
        // Si el formulario de usuario está visible o se seleccionó "Crear nuevo usuario", usamos el nuevo endpoint
        console.log("🆕 Actualizando equipo con nuevo usuario");

        const equipoUsuarioDto = {
            // Datos del equipo
            marca: document.getElementById('editarMarca').value,
            modelo: document.getElementById('editarModelo').value,
            serial: document.getElementById('editarSerial').value,
            codigoBarras: document.getElementById('editarCodigoBarras').value,
            idEstado: parseInt(document.getElementById('editarEstado').value) || null,
            idSubEstado: idSubEstado, // ✅ Usar la variable ya verificada
            idSede: parseInt(document.getElementById('editarSede').value) || null,
            idTipoDispositivo: parseInt(document.getElementById('editarTipoDispositivo').value) || null,
            idProveedor: parseInt(document.getElementById('editarProveedor').value) || null,
            latitud: parseFloat(document.getElementById('editarLatitud').value) || null,
            longitud: parseFloat(document.getElementById('editarLongitud').value) || null,
            sistemaOperativo: document.getElementById('editarSistemaOperativo').value,
            macEquipo: document.getElementById('editarMacEquipo').value,

            // Datos del usuario
            idTipoDocumento: parseInt(document.getElementById('editarTipoDocumento').value),
            numeroDocumento: document.getElementById('editarNumeroDocumento').value,
            nombres: document.getElementById('editarNombres').value,
            apellidos: document.getElementById('editarApellidos').value,
            idArea: parseInt(document.getElementById('editarArea').value),
            idCampaña: parseInt(document.getElementById('editarCampana').value)
        };

        console.log("📦 Enviando datos (con usuario):", equipoUsuarioDto);

        url = '/api/equipos/admin/conusuario';
        method = 'POST';
        requestBody = JSON.stringify(equipoUsuarioDto);
    } else {
        // Si el formulario de usuario no está visible, usamos el método original
        console.log("📦 Actualizando equipo con usuario existente");

        const equipoDto = {
            marca: document.getElementById('editarMarca').value,
            modelo: document.getElementById('editarModelo').value,
            serial: document.getElementById('editarSerial').value,
            codigoBarras: document.getElementById('editarCodigoBarras').value,
            idUsuarioInfo: parseInt(document.getElementById('editarUsuarioInfo').value) || null,
            idEstado: parseInt(document.getElementById('editarEstado').value) || null,
            idSubEstado: idSubEstado, // ✅ Usar la variable ya verificada
            idSede: parseInt(document.getElementById('editarSede').value) || null,
            idTipoDispositivo: parseInt(document.getElementById('editarTipoDispositivo').value) || null,
            idProveedor: parseInt(document.getElementById('editarProveedor').value) || null,
            latitud: parseFloat(document.getElementById('editarLatitud').value) || null,
            longitud: parseFloat(document.getElementById('editarLongitud').value) || null,
            sistemaOperativo: document.getElementById('editarSistemaOperativo').value,
            macEquipo: document.getElementById('editarMacEquipo').value,
        };

        console.log("📦 Enviando datos (sin usuario):", equipoDto);

        url = `/api/equipos/admin/${equipoId}`;
        method = 'PUT';
        requestBody = JSON.stringify(equipoDto);
    }

    try {
        console.log(`🚀 Enviando solicitud a ${url} con método ${method}`);

        const response = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: requestBody
        });

        if (response.ok) {
            alert('✅ Equipo actualizado correctamente.');

            const modalElement = document.getElementById('modalEditarEquipo');
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
            console.error("❌ Error del servidor:", error);
            alert('❌ Error: ' + (error.error || 'No se pudo actualizar el equipo.'));
        }
    } catch (error) {
        console.error("❌ Error en la solicitud:", error);
        alert('❌ Error de red o servidor.');
    }
};

// Abrir modal de edición con carga de selects
window.abrirModalEditar = async function (idEquipo) {
    try {
        // Cargar los selects primero
        await cargarSelectsEditar();

        // Obtener los datos del equipo
        const response = await fetch(`/api/equipos/${idEquipo}`);
        if (!response.ok) throw new Error("No se pudo obtener el equipo");
        const equipo = await response.json();

        // Llenar los campos del formulario
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

        // Esperar un momento para que TomSelect se inicialice
        setTimeout(() => {
            // Establecer valores en los selects con TomSelect
            const selectIds = [
                { id: 'editarUsuarioInfo', value: equipo.idUsuarioInfo },
                { id: 'editarEstado', value: equipo.idEstado },
                { id: 'editarSubEstado', value: equipo.idSubEstado }, // ✅ Agregado subestado
                { id: 'editarEquipoPersonal', value: equipo.idEquipoPersonal },
                { id: 'editarSede', value: equipo.idSede },
                { id: 'editarTipoDispositivo', value: equipo.idTipoDispositivo },
                { id: 'editarProveedor', value: equipo.idProveedor }
            ];

            selectIds.forEach(({ id, value }) => {
                const element = document.getElementById(id);
                if (element && element.tomselect && value) {
                    element.tomselect.setValue(value);
                }
            });

            // Verificar y mostrar/ocultar el contenedor de motivo según el estado
            window.manejarCambioEstado(true);
        }, 200);

        const modal = new bootstrap.Modal(document.getElementById('modalEditarEquipo'));
        modal.show();
    } catch (error) {
        console.error("Error al abrir modal de edición:", error);
        alert("❌ No se pudo cargar la información del equipo.");
    }
};

// Editar equipo (reutiliza el modal de crear)
window.editarEquipo = async function (id) {
    try {
        console.log("🔄 Iniciando edición del equipo:", id);

        // Cargar selects primero
        await cargarSelectsEditar();

        // Obtener datos del equipo
        const response = await fetch(`/api/equipos/${id}`);
        if (!response.ok) throw new Error("No se pudo obtener el equipo");
        const equipo = await response.json();

        console.log("📦 Datos del equipo recibidos:", equipo);

        // Llenar los campos del formulario de edición
        document.getElementById('editarId').value = equipo.id;
        document.getElementById('editarMarca').value = equipo.marca || "";
        document.getElementById('editarModelo').value = equipo.modelo || "";
        document.getElementById('editarSerial').value = equipo.serial || "";
        document.getElementById('editarCodigoBarras').value = equipo.codigoBarras || "";
        document.getElementById('editarLatitud').value = equipo.latitud || "";
        document.getElementById('editarLongitud').value = equipo.longitud || "";
        document.getElementById('editarSistemaOperativo').value = equipo.sistemaOperativo || "";
        document.getElementById('editarMacEquipo').value = equipo.macEquipo || "";

        // Esperar un momento para que TomSelect se inicialice
        setTimeout(() => {
            // Establecer valores en los selects con TomSelect
            const selectIds = [
                { id: 'editarUsuarioInfo', value: equipo.idUsuarioInfo },
                { id: 'editarEstado', value: equipo.idEstado },
                { id: 'editarSubEstado', value: equipo.idSubEstado }, // ✅ Agregado subestado
                { id: 'editarSede', value: equipo.idSede },
                { id: 'editarTipoDispositivo', value: equipo.idTipoDispositivo },
                { id: 'editarProveedor', value: equipo.idProveedor }
            ];

            selectIds.forEach(({ id, value }) => {
                const element = document.getElementById(id);
                if (element && element.tomselect && value) {
                    try {
                        element.tomselect.setValue(value);
                        console.log(`✅ Select ${id} configurado:`, value);
                    } catch (error) {
                        console.error(`❌ Error configurando select ${id}:`, error);
                    }
                } else if (!element) {
                    console.warn(`⚠️ Select no encontrado: ${id}`);
                } else if (!element.tomselect) {
                    console.warn(`⚠️ TomSelect no inicializado en: ${id}`);
                }
            });

            // Verificar y mostrar/ocultar el contenedor de motivo según el estado
            window.manejarCambioEstado(true);
        }, 500);

        // Mostrar el modal de edición
        const modalElement = document.getElementById('modalEditarEquipo');
        if (modalElement) {
            const modal = new bootstrap.Modal(modalElement);
            modal.show();
            console.log("✅ Modal de edición abierto");
        } else {
            console.error("❌ No se encontró el modal modalEditarEquipo");
        }

    } catch (error) {
        console.error("❌ Error completo en editarEquipo:", error);
        alert("❌ Error al cargar el equipo para edición.");
    }
};

// Función MEJORADA para detectar cuando se selecciona "Inactivo" en el estado
window.manejarCambioEstado = function (esEditar = false) {
    const estadoSelectId = esEditar ? 'editarEstado' : 'estado';
    const motivoContainerId = esEditar ? 'motivoContainerEditar' : 'motivoContainer';
    const subEstadoSelectId = esEditar ? 'editarSubEstado' : 'subEstado';

    const estadoSelect = document.getElementById(estadoSelectId);
    const motivoContainer = document.getElementById(motivoContainerId);
    const subEstadoSelect = document.getElementById(subEstadoSelectId);

    console.log(`🔍 manejarCambioEstado (editar: ${esEditar}):`);
    console.log("   - estadoSelect:", estadoSelect?.value);
    console.log("   - motivoContainer existe:", !!motivoContainer);
    console.log("   - subEstadoSelect existe:", !!subEstadoSelect);
    console.log("   - subEstadoSelect valor actual:", subEstadoSelect?.value);

    if (!estadoSelect || !motivoContainer) {
        console.error(`❌ No se encontraron elementos: estadoSelect: ${!!estadoSelect}, motivoContainer: ${!!motivoContainer}`);
        return;
    }

    // Obtener el valor seleccionado
    const valorSeleccionado = estadoSelect.value;

    if (!valorSeleccionado) {
        motivoContainer.style.display = 'none';
        console.log("✅ No hay estado seleccionado, ocultando motivo");
        return;
    }

    // Obtener el texto de la opción seleccionada - MÚLTIPLES MÉTODOS
    let textoEstado = '';

    // Método 1: Si tiene TomSelect, buscar en sus opciones
    if (estadoSelect.tomselect) {
        const options = estadoSelect.tomselect.options;
        if (options[valorSeleccionado]) {
            textoEstado = options[valorSeleccionado].text || '';
            console.log(`🔍 Texto obtenido de TomSelect.options: "${textoEstado}"`);
        }
    }

    // Método 2: Fallback al select nativo
    if (!textoEstado) {
        const option = estadoSelect.querySelector(`option[value="${valorSeleccionado}"]`);
        if (option) {
            textoEstado = option.text || option.textContent || '';
            console.log(`🔍 Texto obtenido del select nativo: "${textoEstado}"`);
        }
    }

    console.log(`🔍 Estado seleccionado: "${textoEstado}" (valor: ${valorSeleccionado})`);

    // Mostrar motivo si el estado contiene "inactivo" (case insensitive)
    const esInactivo = textoEstado.toLowerCase().includes('inactivo');

    if (esInactivo) {
        motivoContainer.style.display = 'block';
        console.log("🎯 Estado Inactivo detectado, mostrando select de Motivo");
    } else {
        motivoContainer.style.display = 'none';
        console.log("✅ Estado diferente a Inactivo, ocultando select de Motivo");

        // 🔄 NUEVO: Limpiar el subestado cuando el estado NO es inactivo
        if (subEstadoSelect) {
            if (subEstadoSelect.tomselect) {
                subEstadoSelect.tomselect.setValue('', true); // true para silenciar eventos
            } else {
                subEstadoSelect.value = '';
            }
            console.log("🧹 SubEstado limpiado porque el estado ya no es Inactivo");
        }
    }
};

// Función MEJORADA para cargar los subestados en el select
window.cargarSubEstados = function (subEstados, esEditar = false) {
    const selectId = esEditar ? 'editarSubEstado' : 'subEstado';
    const select = document.getElementById(selectId);

    console.log(`🎬 INICIANDO cargarSubEstados para ${selectId}`);
    console.log(`📦 SubEstados recibidos:`, subEstados);

    if (!select) {
        console.error(`❌ No se encontró el select: ${selectId}`);
        console.error(`❌ ¿Existe en el DOM? ${document.querySelector(`#${selectId}`) !== null}`);
        return;
    }

    if (!subEstados || subEstados.length === 0) {
        console.warn(`⚠️ No hay subestados para cargar en: ${selectId}`);
        return;
    }

    console.log(`🔄 Cargando ${subEstados.length} subestados en ${selectId}`);

    // Destruir TomSelect existente si hay uno
    if (select.tomselect) {
        console.log(`🗑️ Destruyendo TomSelect existente en ${selectId}`);
        select.tomselect.destroy();
        select.tomselect = null;
    }

    // Limpiar opciones existentes
    select.innerHTML = '<option value="">Seleccionar motivo...</option>';
    console.log(`🧹 Select limpiado, agregando ${subEstados.length} opciones...`);

    // Agregar opciones de subestados
    subEstados.forEach((subEstado, index) => {
        const option = document.createElement('option');
        option.value = subEstado.id;
        option.textContent = subEstado.nombreSubEstado;
        select.appendChild(option);
        console.log(`  ${index + 1}. Opción agregada: [${subEstado.id}] ${subEstado.nombreSubEstado}`);
    });

    // Verificar que las opciones se agregaron
    console.log(`📊 Total opciones en el select: ${select.options.length} (incluyendo opción vacía)`);

    // 🔍 VERIFICAR VISUALMENTE EL SELECT ANTES DE TOMSELECT
    console.log("🔍 HTML del select antes de TomSelect:", select.innerHTML);

    // IMPORTANTE: Usar setTimeout para asegurar que el DOM se actualice
    setTimeout(() => {
        try {
            console.log(`🚀 Inicializando TomSelect para ${selectId}...`);

            // Inicializar TomSelect para el select de subestados
            const tomselect = new TomSelect(select, {
                placeholder: 'Seleccionar motivo...',
                allowEmptyOption: true,
                sortField: { field: "text", direction: "asc" },
                onInitialize: function () {
                    const optCount = this.options ? Object.keys(this.options).length : 0;
                    console.log(`✅ TomSelect inicializado para ${selectId}`);
                    console.log(`   - Opciones disponibles en TomSelect: ${optCount}`);
                    console.log(`   - Opciones en TomSelect:`, this.options);

                    // 🔍 VERIFICAR QUE LAS OPCIONES SE VEN EN EL DOM
                    const optionsVisible = document.querySelectorAll(`#${selectId} option`);
                    console.log(`   - Opciones visibles en DOM: ${optionsVisible.length}`);
                },
                onLoad: function (data) {
                    console.log(`📥 TomSelect onLoad ejecutado para ${selectId}:`, data);
                },
                onFocus: function () {
                    console.log(`🎯 TomSelect ${selectId} enfocado`);
                }
            });

            console.log(`✅ SubEstados cargados exitosamente en ${selectId}`);
            console.log(`   - ${subEstados.length} subestados disponibles`);

            // Verificar instancia de TomSelect
            if (select.tomselect) {
                console.log(`✅ Instancia TomSelect creada correctamente`);
                console.log(`   - Opciones en TomSelect.options:`, Object.keys(select.tomselect.options).length);

                // 🔍 PROBAR SI SE PUEDE SELECCIONAR UN VALOR
                setTimeout(() => {
                    if (select.tomselect.options && Object.keys(select.tomselect.options).length > 1) {
                        // Seleccionar el primer subestado disponible para prueba
                        const firstOptionId = Object.keys(select.tomselect.options).find(key => key !== '');
                        if (firstOptionId) {
                            select.tomselect.setValue(firstOptionId);
                            console.log(`🔧 Valor forzado en ${selectId}: ${firstOptionId}`);
                        }
                    }
                }, 500);
            } else {
                console.error(`❌ No se pudo crear la instancia de TomSelect`);
            }

        } catch (error) {
            console.error(`❌ Error al inicializar TomSelect para ${selectId}:`, error);
            console.error(`   - Error details:`, error.message);
            console.error(`   - Stack:`, error.stack);
        }
    }, 100);

    console.log(`🏁 cargarSubEstados finalizado para ${selectId}`);
};

// ACTUALIZACIÓN para cargarSelects - Modal CREAR
window.cargarSelects = async function () {
    try {
        const response = await fetch('/api/equipos/admin/form-data');
        if (!response.ok) throw new Error('Error al cargar datos del formulario');
        const data = await response.json();

        console.log("📊 Datos recibidos:", {
            usuarios: data.usuarios?.length || 0,
            estados: data.estados?.length || 0,
            subEstados: data.subEstados?.length || 0
        });

        // Guardar usuarios en caché
        if (data.usuarios?.length) {
            window._usuariosCache = data.usuarios;
        }

        const selects = [
            {
                id: 'usuarioInfo',
                list: data.usuarios || [],
                value: 'usuario',
                text: item => {
                    if (item.usuario === 'nuevo') return item.nombreCompleto;
                    let texto = item.nombreCompleto;
                    if (item.correo) texto += ` (${item.correo})`;
                    return texto;
                }
            },
            { id: 'estado', list: data.estados, value: 'id', text: item => item.nombreEstado },
            { id: 'subEstado', list: data.subEstados, value: 'id', text: item => item.nombreSubEstado }, // ✅ Agregado subestado
            { id: 'equipoPersonal', list: data.equiposPersonales, value: 'id', text: item => item.nombrePersonal },
            { id: 'sede', list: data.sedes, value: 'id', text: item => item.nombreSede },
            { id: 'tipoDispositivo', list: data.tiposDispositivo, value: 'id', text: item => item.nombreTipo },
            { id: 'proveedor', list: data.proveedores, value: 'id', text: item => item.nombreProveedor },
            { id: 'tipoDocumento', list: data.tiposDocumento, value: 'id', text: item => item.nombreDocumento },
            { id: 'area', list: data.areas, value: 'id', text: item => item.nombreArea },
            { id: 'campana', list: data.campanas, value: 'id', text: item => item.nombreCampaña }
        ];

        // 🎯 CARGAR SUBESTADOS PRIMERO
        if (data.subEstados) {
            window.cargarSubEstados(data.subEstados, false);
        }

        for (const { id, list, value, text } of selects) {
            const select = document.getElementById(id);
            if (!select) continue;

            // Destruir TomSelect existente
            if (select.tomselect) {
                select.tomselect.destroy();
            }

            select.innerHTML = '<option value="">Seleccionar...</option>';

            requestAnimationFrame(() => {
                list.forEach(item => {
                    const option = document.createElement('option');
                    option.value = item[value];
                    option.text = text(item);
                    option.dataset.origen = item.origen || '';
                    option.dataset.tieneDatosCompletos = item.tieneDatosCompletos || false;
                    select.appendChild(option);
                });

                const tomselect = new TomSelect(select, {
                    placeholder: id === 'usuarioInfo' ? 'Buscar usuario...' : 'Seleccionar...',
                    maxOptions: 500,
                    allowEmptyOption: true,
                    sortField: { field: "text", direction: "asc" },
                    searchField: ['text', 'value'],
                    onChange: function (value) {
                        console.log(`🔄 Cambio en ${id}: ${value}`);

                        if (id === 'usuarioInfo') {
                            window.manejarCambioUsuario(select, false);
                        }

                        if (id === 'estado') {
                            // Usar setTimeout para asegurar que el cambio se procese
                            setTimeout(() => window.manejarCambioEstado(false), 50);
                        }
                    },
                    onInitialize: function () {
                        console.log(`✅ TomSelect inicializado: ${id}`);
                    }
                });
            });
        }

        // Verificar estado inicial después de cargar todo
        setTimeout(() => {
            console.log("🔍 Verificando estado inicial...");
            window.manejarCambioEstado(false);
        }, 300);

    } catch (error) {
        console.error('❌ Error en cargarSelects:', error);
    }
};

// ACTUALIZACIÓN para cargarSelectsEditar - Modal EDITAR
window.cargarSelectsEditar = async function () {
    try {
        const response = await fetch('/api/equipos/admin/form-data');
        if (!response.ok) throw new Error('Error al cargar datos del formulario');
        const data = await response.json();

        console.log("📊 Datos para editar:", {
            usuarios: data.usuarios?.length || 0,
            estados: data.estados?.length || 0,
            subEstados: data.subEstados?.length || 0
        });

        // Guardar usuarios en caché
        if (data.usuarios?.length) {
            window._usuariosCache = data.usuarios;
        }

        const selects = [
            {
                id: 'editarUsuarioInfo',
                list: data.usuarios || [],
                value: 'usuario',
                text: item => {
                    if (item.usuario === 'nuevo') return item.nombreCompleto;
                    let texto = item.nombreCompleto;
                    if (item.correo) texto += ` (${item.correo})`;
                    return texto;
                }
            },
            { id: 'editarEstado', list: data.estados, value: 'id', text: item => item.nombreEstado },
            { id: 'editarSubEstado', list: data.subEstados, value: 'id', text: item => item.nombreSubEstado }, // ✅ Agregado subestado
            { id: 'editarEquipoPersonal', list: data.equiposPersonales, value: 'id', text: item => item.nombrePersonal },
            { id: 'editarSede', list: data.sedes, value: 'id', text: item => item.nombreSede },
            { id: 'editarTipoDispositivo', list: data.tiposDispositivo, value: 'id', text: item => item.nombreTipo },
            { id: 'editarProveedor', list: data.proveedores, value: 'id', text: item => item.nombreProveedor },
            { id: 'editarTipoDocumento', list: data.tiposDocumento, value: 'id', text: item => item.nombreDocumento },
            { id: 'editarArea', list: data.areas, value: 'id', text: item => item.nombreArea },
            { id: 'editarCampana', list: data.campanas, value: 'id', text: item => item.nombreCampaña }
        ];

        // 🎯 CARGAR SUBESTADOS PRIMERO
        if (data.subEstados) {
            window.cargarSubEstados(data.subEstados, true);
        }

        for (const { id, list, value, text } of selects) {
            const select = document.getElementById(id);
            if (!select) {
                console.warn(`⚠️ Select no encontrado: ${id}`);
                continue;
            }

            if (select.tomselect) {
                select.tomselect.destroy();
            }

            select.innerHTML = '<option value="">Seleccionar...</option>';

            requestAnimationFrame(() => {
                list.forEach(item => {
                    const option = document.createElement('option');
                    option.value = item[value];
                    option.text = text(item);
                    option.dataset.origen = item.origen || '';
                    option.dataset.tieneDatosCompletos = item.tieneDatosCompletos || false;
                    option.dataset.nombres = item.nombres || item.nombre || '';
                    option.dataset.apellidos = item.apellidos || '';
                    option.dataset.numeroDocumento = item.numeroDocumento || '';
                    option.dataset.idTipoDocumento = item.idTipoDocumento || '';
                    option.dataset.idArea = item.idArea || '';
                    option.dataset.idCampaña = item.idCampaña || '';
                    select.appendChild(option);
                });

                const tomselect = new TomSelect(select, {
                    placeholder: 'Buscar...',
                    maxOptions: 500,
                    allowEmptyOption: true,
                    sortField: { field: "text", direction: "asc" },
                    searchField: ['text', 'value'],
                    onChange: function (value) {
                        console.log(`🔄 Cambio en ${id}: ${value}`);

                        if (id === 'editarUsuarioInfo') {
                            window.manejarCambioUsuario(select, true);
                        }

                        if (id === 'editarEstado') {
                            setTimeout(() => window.manejarCambioEstado(true), 50);
                        }
                    },
                    onInitialize: function () {
                        console.log(`✅ TomSelect inicializado: ${id}`);
                    }
                });
            });
        }

        // Verificar estado inicial después de cargar todo
        setTimeout(() => {
            console.log("🔍 Verificando estado inicial en edición...");
            window.manejarCambioEstado(true);
        }, 300);

        console.log("✅ Todos los selects de edición cargados");

    } catch (error) {
        console.error('❌ Error en cargarSelectsEditar:', error);
        alert('Error al cargar los datos del formulario. Por favor, recarga la página.');
    }
};

//Manejo de cambio de usuario entre AD y Local
window.manejarCambioUsuario = async function (selectElement, esEditar = false) {
    const formId = esEditar ? 'formUsuarioInfoEditar' : 'formUsuarioInfo';
    const formUsuario = document.getElementById(formId);
    const usuarioSeleccionado = selectElement.value;

    console.log(`🔄 Cambio de usuario detectado: ${usuarioSeleccionado}, esEditar: ${esEditar}`);

    // Si se selecciona "Crear nuevo usuario", mostrar el formulario y limpiar campos
    if (usuarioSeleccionado === 'nuevo') {
        if (formUsuario) {
            formUsuario.style.display = 'block';
            limpiarCamposUsuarioFormulario(esEditar);
        }
        return;
    }

    if (usuarioSeleccionado) {
        // Buscar información del usuario en el caché
        const usuario = window._usuariosCache?.find(u =>
            u.usuario === usuarioSeleccionado || u.numeroDocumento === usuarioSeleccionado
        );

        if (usuario) {
            console.log('✅ Usuario seleccionado:', usuario);

            // Llenar los campos de nombres y apellidos
            const nombresId = esEditar ? 'editarNombres' : 'nombres';
            const apellidosId = esEditar ? 'editarApellidos' : 'apellidos';

            const nombresInput = document.getElementById(nombresId);
            const apellidosInput = document.getElementById(apellidosId);

            nombresInput.value = usuario.nombres || usuario.nombre || '';
            apellidosInput.value = usuario.apellidos || '';

            // Si el usuario tiene datos completos (de BD local), rellenar automáticamente
            if (usuario.tieneDatosCompletos) {
                console.log("✅ Usuario con datos completos, rellenando formulario...");
                rellenarFormularioUsuario({
                    idTipodocumento: usuario.idTipoDocumento,
                    idArea: usuario.idArea,
                    idCampaña: usuario.idCampaña,
                    numeroDocumento: usuario.numeroDocumento
                }, esEditar);
            } else {
                console.log("ℹ️ Usuario sin datos completos, limpiando formulario...");
                limpiarCamposUsuarioFormulario(esEditar);

                // Si es usuario de AD, pre-llenar número de documento si está disponible
                if (usuario.origen === 'ad' && usuario.numeroDocumento) {
                    const numDocId = esEditar ? 'editarNumeroDocumento' : 'numeroDocumento';
                    const numDocElement = document.getElementById(numDocId);
                    if (numDocElement) {
                        numDocElement.value = usuario.numeroDocumento;
                    }
                }
            }

            // Mostrar el formulario
            if (formUsuario) {
                formUsuario.style.display = 'block';
            }
        }
    } else {
        // Ocultar el formulario si no hay usuario seleccionado
        if (formUsuario) {
            formUsuario.style.display = 'none';
        }
        limpiarCamposUsuarioFormulario(esEditar);
    }
};

// Función auxiliar para rellenar el formulario de usuario
function rellenarFormularioUsuario(datos, esEditar) {
    const prefijo = esEditar ? 'editar' : '';

    console.log(`🔧 Rellenando formulario con datos:`, datos);

    // Pequeño retraso para asegurar que TomSelect esté completamente inicializado
    setTimeout(() => {
        // Lista de campos a rellenar
        const campos = [
            { nombre: 'TipoDocumento', valor: datos.idTipodocumento },
            { nombre: 'Area', valor: datos.idArea },
            { nombre: 'Campana', valor: datos.idCampaña }
        ];

        campos.forEach(campo => {
            const selectId = `${prefijo}${campo.nombre}`;
            const selectElement = document.getElementById(selectId);

            if (selectElement) {
                console.log(`📝 Intentando rellenar select #${selectId} con valor: ${campo.valor}`);

                // 1. Establecer el valor en el <select> original
                selectElement.value = campo.valor;

                // 2. Usar la API de TomSelect para actualizar la vista
                if (selectElement.tomselect) {
                    selectElement.tomselect.setValue(campo.valor);
                    console.log(`✅ TomSelect para #${selectId} actualizado.`);
                } else {
                    console.warn(`⚠️ La instancia de TomSelect para #${selectId} no se encontró.`);
                }
            } else {
                console.error(`❌ No se encontró el elemento con ID: ${selectId}`);
            }
        });

        // El campo de número de documento es un input normal, lo rellenamos directamente
        const numDocId = `${prefijo}NumeroDocumento`;
        const numDocElement = document.getElementById(numDocId);
        if (numDocElement) {
            numDocElement.value = datos.numeroDocumento || '';
            console.log(`📝 Input #${numDocId} rellenado con: ${datos.numeroDocumento}`);
        }

    }, 150); // 150ms de retraso debería ser suficiente
}

function limpiarCamposUsuarioFormulario(esEditar) {
    const prefijo = esEditar ? 'editar' : '';

    const campos = ['TipoDocumento', 'Area', 'Campana'];
    campos.forEach(campo => {
        const selectId = `${prefijo}${campo}`;
        const selectElement = document.getElementById(selectId);
        if (selectElement) {
            selectElement.value = '';
            if (selectElement.tomselect) {
                selectElement.tomselect.setValue('');
            }
        }
    });

    const numDocId = `${prefijo}NumeroDocumento`;
    const numDocElement = document.getElementById(numDocId);
    if (numDocElement) {
        numDocElement.value = '';
    }
}

document.addEventListener('DOMContentLoaded', async () => {
    try {
        // Precargar solo los usuarios en caché al inicio
        const response = await fetch('/api/equipos/admin/form-data');
        if (response.ok) {
            const data = await response.json();
            if (data.usuarios?.length) {
                window._usuariosCache = data.usuarios;
                console.log("✅ Usuarios del AD precargados en caché");
            }
        }
    } catch (e) {
        console.warn("⚠️ No se pudo precargar usuarios:", e);
    }
});

// Manejo de eliminación de equipos
let equipoIdAEliminar = null;

window.abrirModalEliminar = function (id) {
    equipoIdAEliminar = id;
    const modalElement = document.getElementById('modalEliminarEquipo');
    if (!modalElement) return console.error("❌ No se encontró el modal 'modalEliminarEquipo'");
    bootstrap.Modal.getOrCreateInstance(modalElement).show();
};

window.confirmarEliminarEquipo = async function () {
    if (!equipoIdAEliminar) return;

    try {
        const response = await fetch(`/api/equipos/admin/${equipoIdAEliminar}`, { method: 'DELETE' });
        if (response.ok) {
            alert("✅ Equipo eliminado correctamente.");
            equipoIdAEliminar = null;
            bootstrap.Modal.getInstance(document.getElementById('modalEliminarEquipo')).hide();
            setTimeout(() => {
                DotNet.invokeMethodAsync('OUT_APP_EQUIPGO', 'RefrescarListaEquipos');
            }, 300);
        } else {
            alert("❌ Error al eliminar el equipo.");
        }
    } catch (error) {
        console.error(error);
        alert("❌ Error de red o servidor.");
    }
};



// CARGA MASIVA 

// Variables globales
window.equiposParaCargar = [];
window.erroresCarga = [];

// INICIALIZAR MODAL DE CARGA MASIVA
window.inicializarCargaMasiva = function () {
    console.log("🔄 Inicializando modal de carga masiva...");

    // Limpiar estado anterior
    window.equiposParaCargar = [];
    // Los errores se limpian en limpiarTodo()

    // Configurar evento del file input
    const fileInput = document.getElementById('fileCargaMasiva');
    if (fileInput) {
        fileInput.addEventListener('change', window.manejarSeleccionArchivo);
    }

    // Limpiar solo la info del archivo
    const infoArchivo = document.getElementById('infoArchivo');
    if (infoArchivo) {
        infoArchivo.style.display = 'none';
    }

    // Deshabilitar botón procesar
    const procesarBtn = document.getElementById('procesarCargaBtn');
    if (procesarBtn) {
        procesarBtn.disabled = true;
    }

    console.log("✅ Modal de carga masiva inicializado");
};

// Manejar selección de archivo
window.manejarSeleccionArchivo = async function (event) {
    console.log("📁 ARCHIVO SELECCIONADO");

    const file = event.target.files[0];
    if (!file) return;

    console.log("📦 Archivo:", file.name, file.size, "bytes");

    const btnProcesar = document.getElementById('procesarCargaBtn');

    try {
        // Mostrar estado de carga
        if (btnProcesar) {
            btnProcesar.disabled = true;
            btnProcesar.innerHTML = '<i class="bi bi-arrow-repeat spinner"></i> Leyendo...';
        }

        // Leer archivo
        window.equiposParaCargar = await leerArchivoExcel(file);
        console.log("✅ Equipos leídos:", window.equiposParaCargar.length);

        if (window.equiposParaCargar.length === 0) {
            alert('❌ El archivo no contiene equipos válidos.');
            if (btnProcesar) {
                btnProcesar.disabled = true;
                btnProcesar.innerHTML = '<i class="bi bi-upload"></i> Procesar Carga Masiva';
            }
            return;
        }

        // Mostrar info del archivo
        document.getElementById('nombreArchivo').textContent = file.name;
        document.getElementById('cantidadEquipos').textContent = `${window.equiposParaCargar.length} equipos`;
        document.getElementById('infoArchivo').style.display = 'block';

        // HABILITAR BOTÓN
        if (btnProcesar) {
            btnProcesar.disabled = false;
            btnProcesar.innerHTML = '<i class="bi bi-upload"></i> Procesar Carga Masiva';
            console.log("🎯 BOTÓN HABILITADO");
        }

    } catch (error) {
        console.error("❌ Error:", error);
        alert('Error al leer el archivo: ' + error.message);
        if (btnProcesar) {
            btnProcesar.disabled = true;
            btnProcesar.innerHTML = '<i class="bi bi-upload"></i> Procesar Carga Masiva';
        }
    }
};

// Leer archivo Excel
function leerArchivoExcel(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();

        reader.onload = function (e) {
            try {
                const data = new Uint8Array(e.target.result);
                const workbook = XLSX.read(data, { type: 'array' });
                const worksheet = workbook.Sheets[workbook.SheetNames[0]];
                const jsonData = XLSX.utils.sheet_to_json(worksheet);

                if (jsonData.length === 0) {
                    reject(new Error('El archivo está vacío'));
                    return;
                }

                const equipos = jsonData
                    .map((row, index) => {
                        const marca = row.Marca?.toString().trim() || '';
                        const modelo = row.Modelo?.toString().trim() || '';

                        if (!marca || !modelo) {
                            console.warn(`Fila ${index + 2} ignorada`);
                            return null;
                        }

                        return {
                            marca,
                            modelo,
                            serial: row.Serial?.toString().trim() || '',
                            codigoBarras: row.CodigoBarras?.toString().trim() || '',
                            sistemaOperativo: row.SistemaOperativo?.toString().trim() || '',
                            macEquipo: row.MacEquipo?.toString().trim() || '',
                            versionSoftware: row.VersionSoftware?.toString().trim() || '',
                            ubicacion: row.Ubicacion?.toString().trim() || '',
                            idEstado: 1,
                            idSubEstado: null
                        };
                    })
                    .filter(e => e !== null);

                resolve(equipos);
            } catch (error) {
                reject(new Error('Formato inválido: ' + error.message));
            }
        };

        reader.onerror = () => reject(new Error('Error al leer archivo'));
        reader.readAsArrayBuffer(file);
    });
}

// Procesar carga masiva
window.procesarCargaMasiva = async function () {
    console.log("🎯 PROCESANDO CARGA MASIVA");
    console.log("📊 Equipos:", window.equiposParaCargar.length);

    if (!window.equiposParaCargar || window.equiposParaCargar.length === 0) {
        alert('❌ No hay equipos para procesar');
        return;
    }

    const btnProcesar = document.getElementById('procesarCargaBtn');
    if (!btnProcesar) return;

    btnProcesar.disabled = true;
    btnProcesar.innerHTML = '<i class="bi bi-arrow-repeat spinner"></i> Procesando...';

    try {
        const response = await fetch('/api/equipos/admin/carga-masiva', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(window.equiposParaCargar)
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.error || 'Error del servidor');
        }

        const resultado = await response.json();
        console.log("✅ Resultado:", resultado);

        mostrarResultadosCarga(resultado);

    } catch (error) {
        console.error("❌ Error:", error);
        alert('Error en la carga: ' + error.message);
    } finally {
        btnProcesar.disabled = false;
        btnProcesar.innerHTML = '<i class="bi bi-upload"></i> Procesar Carga Masiva';
    }
};

// Mostrar resultados de la carga - VERSIÓN CON DESCARGA AUTOMÁTICA
window.mostrarResultadosCarga = function (resultado) {
    console.log("📊 Mostrando resultados:", resultado);

    // Mostrar sección de resultados
    document.getElementById('resultadosCarga').style.display = 'block';

    // Actualizar resumen
    document.getElementById('totalRegistros').textContent = resultado.totalRegistros;
    document.getElementById('registrosExitosos').textContent = resultado.registrosExitosos;
    document.getElementById('registrosFallidos').textContent = resultado.registrosFallidos;

    // Guardar errores para posible exportación
    window.erroresCarga = resultado.errores || [];

    // Mostrar mensaje principal
    const mensajeElement = document.getElementById('mensajeResultado');

    // 🔥 DETERMINAR TIPO DE MENSAJE SEGÚN EL RESULTADO
    if (resultado.registrosExitosos > 0 && resultado.registrosFallidos === 0) {
        // ✅ ÉXITO COMPLETO - Recargar página
        mensajeElement.className = 'alert alert-success';
        mensajeElement.innerHTML = `
            <strong>${resultado.mensaje}</strong>
            <div class="mt-2">
                <span class="badge bg-primary">Total: ${resultado.totalRegistros}</span>
                <span class="badge bg-success">Éxitos: ${resultado.registrosExitosos}</span>
                <span class="badge bg-danger">Fallidos: ${resultado.registrosFallidos}</span>
            </div>
            <div class="mt-2">
                <small class="text-success">🔄 La página se recargará automáticamente...</small>
            </div>
        `;

        // Recargar página después de 3 segundos
        setTimeout(() => {
            const modalElement = document.getElementById('modalCargaMasiva');
            if (modalElement) {
                const modal = bootstrap.Modal.getInstance(modalElement);
                if (modal) {
                    modal.hide();
                    modalElement.addEventListener('hidden.bs.modal', function () {
                        window.location.reload();
                    }, { once: true });
                }
            }
        }, 3000);

    } else if (resultado.registrosExitosos === 0 && resultado.registrosFallidos > 0) {
        // 🔥 ERRORES - Cerrar modal y descargar Excel automáticamente
        mensajeElement.className = 'alert alert-danger';
        mensajeElement.innerHTML = `
            <strong>${resultado.mensaje}</strong>
            <div class="mt-2">
                <span class="badge bg-primary">Total: ${resultado.totalRegistros}</span>
                <span class="badge bg-success">Éxitos: ${resultado.registrosExitosos}</span>
                <span class="badge bg-danger">Fallidos: ${resultado.registrosFallidos}</span>
            </div>
            <div class="mt-2">
                <small class="text-danger">📊 Se descargará automáticamente un archivo Excel con los errores...</small>
                <br>
                <small class="text-danger">⏳ El modal se cerrará en <span id="contadorCierre">5</span> segundos</small>
            </div>
        `;

        // Mostrar errores en la tabla
        if (window.erroresCarga.length > 0) {
            window.mostrarErrores(window.erroresCarga);
            document.getElementById('panelErrores').style.display = 'block';
        }

        // 🔥 DESCARGAR EXCEL AUTOMÁTICAMENTE Y CERRAR MODAL
        window.descargarYcerrarModal();

    } else {
        // ⚠️ CASO MIXTO (no debería ocurrir con la nueva lógica)
        mensajeElement.className = 'alert alert-warning';
        mensajeElement.innerHTML = `
            <strong>${resultado.mensaje}</strong>
            <div class="mt-2">
                <span class="badge bg-primary">Total: ${resultado.totalRegistros}</span>
                <span class="badge bg-success">Éxitos: ${resultado.registrosExitosos}</span>
                <span class="badge bg-danger">Fallidos: ${resultado.registrosFallidos}</span>
            </div>
        `;
    }

    // Scroll a resultados
    document.getElementById('resultadosCarga').scrollIntoView({ behavior: 'smooth' });
};
// DESCARGAR EXCEL Y CERRAR MODAL AUTOMÁTICAMENTE
window.descargarYcerrarModal = function () {
    console.log("🔥 Iniciando descarga automática y cierre del modal...");

    let segundos = 5;
    const contadorElement = document.getElementById('contadorCierre');

    // Contador regresivo
    const contadorInterval = setInterval(() => {
        segundos--;
        if (contadorElement) {
            contadorElement.textContent = segundos;
        }

        if (segundos <= 0) {
            clearInterval(contadorInterval);

            // 1. Primero descargar el Excel de errores
            window.descargarErroresAutomatico();

            // 2. Esperar un poco para que inicie la descarga y luego cerrar el modal
            setTimeout(() => {
                const modalElement = document.getElementById('modalCargaMasiva');
                if (modalElement) {
                    const modal = bootstrap.Modal.getInstance(modalElement);
                    if (modal) {
                        modal.hide();

                        // 🔥 LIMPIAR TODO después de cerrar el modal
                        setTimeout(() => {
                            window.limpiarTodo();
                        }, 300);

                        console.log("✅ Modal cerrado y limpiado automáticamente");
                    }
                }
            }, 1000); // 1 segundo después de iniciar la descarga
        }
    }, 1000);
};

// DESCARGAR ERRORES AUTOMÁTICAMENTE
window.descargarErroresAutomatico = function () {
    if (window.erroresCarga.length === 0) {
        console.log("❌ No hay errores para descargar");
        return;
    }

    try {
        console.log("📊 Generando Excel de errores automáticamente...");

        // Crear datos para el Excel
        const datosErrores = window.erroresCarga.map(error => ({
            'Fila': error.indiceFila,
            'Marca': error.marca || '',
            'Modelo': error.modelo || '',
            'Serial': error.serial || '',
            'Error': error.error || 'Error desconocido'
        }));

        // Crear workbook
        const wb = XLSX.utils.book_new();
        const ws = XLSX.utils.json_to_sheet(datosErrores);

        // Agregar hoja al workbook
        XLSX.utils.book_append_sheet(wb, ws, 'ErroresCargaMasiva');

        // Generar nombre del archivo con timestamp
        const nombreArchivo = `Errores_Carga_Masiva_${new Date().toISOString().slice(0, 16).replace(/[-:T]/g, '')}.xlsx`;

        // Descargar archivo
        XLSX.writeFile(wb, nombreArchivo);

        console.log("✅ Excel de errores descargado automáticamente:", nombreArchivo);

    } catch (error) {
        console.error('❌ Error al descargar errores automáticamente:', error);
    }
};

// Función para recargar la página
window.recargarPagina = function () {
    console.log("🎯 Recargando página manualmente...");

    // Cerrar el modal primero
    const modal = document.getElementById('modalCargaMasiva');
    if (modal) {
        const modalInstance = bootstrap.Modal.getInstance(modal);
        if (modalInstance) {
            modalInstance.hide();
        }
    }

    // Recargar después de que el modal se cierre
    setTimeout(() => {
        window.location.reload();
    }, 300);
};


// LIMPIAR TODO COMPLETAMENTE (para cuando se cierra el modal)
window.limpiarTodo = function () {
    console.log("🧹 Limpiando todo el modal...");

    const fileInput = document.getElementById('fileCargaMasiva');
    const procesarBtn = document.getElementById('procesarCargaBtn');
    const infoArchivo = document.getElementById('infoArchivo');
    const resultadosCarga = document.getElementById('resultadosCarga');
    const mensajeResultado = document.getElementById('mensajeResultado');
    const tbodyErrores = document.getElementById('tbodyErrores');
    const panelErrores = document.getElementById('panelErrores');
    const panelExitosos = document.getElementById('panelExitosos');

    // 1. Limpiar inputs y botones
    if (fileInput) fileInput.value = '';
    if (procesarBtn) procesarBtn.disabled = true;
    if (infoArchivo) infoArchivo.style.display = 'none';

    // 2. Limpiar sección de resultados
    if (resultadosCarga) resultadosCarga.style.display = 'none';
    if (mensajeResultado) {
        mensajeResultado.className = 'alert';
        mensajeResultado.innerHTML = '';
    }

    // 3. Limpiar tabla de errores
    if (tbodyErrores) tbodyErrores.innerHTML = '';
    if (panelErrores) panelErrores.style.display = 'none';
    if (panelExitosos) panelExitosos.style.display = 'none';

    // 4. Limpiar variables globales
    window.equiposParaCargar = [];
    window.erroresCarga = [];

    console.log("✅ Modal completamente limpiado");
};

// Contador para recarga automática
function iniciarContadorRecarga() {
    let segundos = 5;
    const contadorElement = document.getElementById('contadorRecarga');
    const contadorInterval = setInterval(() => {
        segundos--;
        if (contadorElement) {
            contadorElement.textContent = segundos;
        }

        if (segundos <= 0) {
            clearInterval(contadorInterval);
            window.recargarPagina();
        }
    }, 1000);
}

// Mostrar errores
function mostrarErrores(errores) {
    const tbody = document.getElementById('tbodyErrores');
    tbody.innerHTML = '';

    errores.forEach(error => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td class="fw-bold">${error.indiceFila}</td>
            <td>${error.marca || '-'}</td>
            <td>${error.modelo || '-'}</td>
            <td>${error.serial || '-'}</td>
            <td class="text-danger">${error.error}</td>
        `;
        tbody.appendChild(tr);
    });
}

// Exportar errores a Excel (versión manual)
window.exportarErrores = async function () {
    if (window.erroresCarga.length === 0) {
        alert('No hay errores para exportar');
        return;
    }

    try {
        // Crear datos para el Excel
        const datosErrores = window.erroresCarga.map(error => ({
            'Fila': error.indiceFila,
            'Marca': error.marca,
            'Modelo': error.modelo,
            'Serial': error.serial,
            'Error': error.error
        }));

        // Crear workbook
        const wb = XLSX.utils.book_new();
        const ws = XLSX.utils.json_to_sheet(datosErrores);

        // Agregar hoja al workbook
        XLSX.utils.book_append_sheet(wb, ws, 'ErroresCargaMasiva');

        // Generar nombre del archivo
        const nombreArchivo = `Errores_Carga_Masiva_${new Date().toISOString().slice(0, 16).replace(/[-:T]/g, '')}.xlsx`;

        // Descargar archivo
        XLSX.writeFile(wb, nombreArchivo);

        console.log("✅ Excel de errores descargado manualmente");

    } catch (error) {
        console.error('Error exportando errores:', error);
        alert('❌ Error al exportar errores');
    }
};

// Limpiar
window.limpiarArchivo = function () {
    const fileInput = document.getElementById('fileCargaMasiva');
    if (fileInput) fileInput.value = '';

    const btnProcesar = document.getElementById('procesarCargaBtn');
    if (btnProcesar) btnProcesar.disabled = true;

    document.getElementById('infoArchivo').style.display = 'none';
    window.equiposParaCargar = [];
};

window.limpiarModalCarga = function () {
    limpiarArchivo();
    document.getElementById('resultadosCarga').style.display = 'none';
};

// Descargar plantilla
window.descargarPlantillaCargaMasiva = async function () {
    try {
        const boton = event.target;
        const textoOriginal = boton.innerHTML;
        boton.innerHTML = '<i class="bi bi-arrow-repeat spinner"></i> Generando...';
        boton.disabled = true;

        const response = await fetch('/api/equipos/admin/descargar-plantilla');

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.error || 'Error al generar plantilla');
        }

        const blob = await response.blob();
        if (blob.size === 0) throw new Error('Archivo vacío');

        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Plantilla_Equipos_${new Date().toISOString().slice(0, 16).replace(/[-:T]/g, '')}.xlsx`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);

        console.log("✅ Plantilla descargada");
    } catch (error) {
        console.error('Error:', error);
        alert('Error al descargar plantilla: ' + error.message);
    } finally {
        if (event?.target) {
            event.target.innerHTML = '<i class="bi bi-download"></i> Descargar Plantilla Excel';
            event.target.disabled = false;
        }
    }
};

// Inicialización (SOLO UNA VEZ)
document.addEventListener('DOMContentLoaded', function () {
    const modal = document.getElementById('modalCargaMasiva');
    if (modal) {
        // Cuando se ABRE el modal
        modal.addEventListener('show.bs.modal', function () {
            console.log("🎯 Modal de carga masiva abierto");
            window.inicializarCargaMasiva();
        });

        // Cuando se CIERRA el modal (por cualquier razón)
        modal.addEventListener('hidden.bs.modal', function () {
            console.log("🎯 Modal de carga masiva cerrado - limpiando todo");
            window.limpiarTodo();
        });
    }
});


// SOLUCIÓN DIRECTA: Forzar inicialización cuando el modal ya está visible
setInterval(function () {
    const modal = document.getElementById('modalCargaMasiva');
    const fileInput = document.getElementById('fileCargaMasiva');

    // Si el modal está visible Y el input existe pero no tiene listener
    if (modal && modal.classList.contains('show') && fileInput) {
        // Verificar si el input ya tiene el listener (evitar duplicados)
        if (!fileInput.dataset.listenerAdded) {
            console.log("🔧 Agregando listener al input (método directo)");

            fileInput.dataset.listenerAdded = 'true';
            fileInput.addEventListener('change', window.manejarSeleccionArchivo);

            // Asegurar que el botón también tenga su listener
            const btnProcesar = document.getElementById('procesarCargaBtn');
            if (btnProcesar && !btnProcesar.dataset.listenerAdded) {
                console.log("🔧 Agregando listener al botón (método directo)");
                btnProcesar.dataset.listenerAdded = 'true';
                btnProcesar.addEventListener('click', function (e) {
                    e.preventDefault();
                    console.log("🎯 CLICK EN PROCESAR");
                    window.procesarCargaMasiva();
                });
            }
        }
    }
}, 500); // Revisar cada 500ms