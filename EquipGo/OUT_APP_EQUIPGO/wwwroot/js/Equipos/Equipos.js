// 🧠 Cache global para usuarios del Active Directory
window._usuariosCache = null;

// 🧹 Función mejorada para limpiar el formulario
window.limpiarFormularioCrear = function () {
    console.log("🧹 Limpiando formulario de crear equipo...");

    try {
        const form = document.getElementById('formCrearEquipo');
        if (!form) {
            console.error("❌ No se encontró el formulario formCrearEquipo");
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
        const selectsIds = ['usuarioInfo', 'estado', 'sede', 'tipoDispositivo', 'proveedor', 'tipoDocumento', 'area', 'campana'];
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

        // 4. Ocultar y limpiar formulario de usuario
        const formUsuarioInfo = document.getElementById('formUsuarioInfo');
        if (formUsuarioInfo) {
            formUsuarioInfo.style.display = 'none';

            // Limpiar campos del formulario de usuario
            const camposUsuario = ['tipoDocumento', 'numeroDocumento', 'nombres', 'apellidos', 'area', 'campana'];
            camposUsuario.forEach(campoId => {
                const campo = document.getElementById(campoId);
                if (campo) campo.value = '';
            });

            console.log("✅ Formulario de usuario limpiado y ocultado");
        }

        console.log("🎉 Formulario completamente limpiado");

    } catch (error) {
        console.error("❌ Error al limpiar el formulario:", error);
    }
};

// 🧩 Guardar o actualizar equipo con usuario
window.guardarEquipo = async function () {
    const form = document.getElementById('formCrearEquipo');
    const equipoId = form.getAttribute('data-id'); // puede ser null o string

    // Verificar si se está creando un nuevo usuario
    const formUsuarioInfo = document.getElementById('formUsuarioInfo');
    const formUsuarioVisible = formUsuarioInfo && formUsuarioInfo.style.display !== 'none';

    // También verificamos si se seleccionó "Crear nuevo usuario"
    const usuarioSeleccionado = document.getElementById('usuarioInfo').value;
    const esNuevoUsuario = usuarioSeleccionado === 'nuevo';

    let url, method, requestBody;

    if (equipoId) {
        // Si estamos editando un equipo existente, usamos el método original
        const equipoDto = {
            marca: document.getElementById('marca').value,
            modelo: document.getElementById('modelo').value,
            serial: document.getElementById('serial').value,
            codigoBarras: document.getElementById('codigoBarras').value,
            idUsuarioInfo: parseInt(document.getElementById('usuarioInfo').value) || null,
            idEstado: parseInt(document.getElementById('estado').value) || null,
            idSede: parseInt(document.getElementById('sede').value) || null,
            idTipoDispositivo: parseInt(document.getElementById('tipoDispositivo').value) || null,
            idProveedor: parseInt(document.getElementById('proveedor').value) || null,
            latitud: parseFloat(document.getElementById('latitud').value) || null,
            longitud: parseFloat(document.getElementById('longitud').value) || null,
            sistemaOperativo: document.getElementById('sistemaOperativo').value,
            macEquipo: document.getElementById('macEquipo').value,
        };

        url = `/api/equipos/admin/${equipoId}`;
        method = 'PUT';
        requestBody = JSON.stringify(equipoDto);
    } else {
        // Si estamos creando un nuevo equipo
        if (formUsuarioVisible || esNuevoUsuario) {
            // Si el formulario de usuario está visible o se seleccionó "Crear nuevo usuario", usamos el nuevo endpoint
            console.log("🆕 Creando equipo con nuevo usuario");

            const equipoUsuarioDto = {
                // Datos del equipo
                marca: document.getElementById('marca').value,
                modelo: document.getElementById('modelo').value,
                serial: document.getElementById('serial').value,
                codigoBarras: document.getElementById('codigoBarras').value,
                idEstado: parseInt(document.getElementById('estado').value) || null,
                idSede: parseInt(document.getElementById('sede').value) || null,
                idTipoDispositivo: parseInt(document.getElementById('tipoDispositivo').value) || null,
                idProveedor: parseInt(document.getElementById('proveedor').value) || null,
                latitud: parseFloat(document.getElementById('latitud').value) || null,
                longitud: parseFloat(document.getElementById('longitud').value) || null,
                sistemaOperativo: document.getElementById('sistemaOperativo').value,
                macEquipo: document.getElementById('macEquipo').value,

                // Datos del usuario
                idTipoDocumento: parseInt(document.getElementById('tipoDocumento').value),
                numeroDocumento: document.getElementById('numeroDocumento').value,
                nombres: document.getElementById('nombres').value,
                apellidos: document.getElementById('apellidos').value,
                idArea: parseInt(document.getElementById('area').value),
                idCampaña: parseInt(document.getElementById('campana').value)
            };

            console.log("📦 Enviando datos:", equipoUsuarioDto);

            url = '/api/equipos/admin/conusuario';
            method = 'POST';
            requestBody = JSON.stringify(equipoUsuarioDto);
        } else {
            // Si el formulario de usuario no está visible, usamos el método original
            console.log("📦 Creando equipo con usuario existente");

            const equipoDto = {
                marca: document.getElementById('marca').value,
                modelo: document.getElementById('modelo').value,
                serial: document.getElementById('serial').value,
                codigoBarras: document.getElementById('codigoBarras').value,
                idUsuarioInfo: parseInt(document.getElementById('usuarioInfo').value) || null,
                idEstado: parseInt(document.getElementById('estado').value) || null,
                idSede: parseInt(document.getElementById('sede').value) || null,
                idTipoDispositivo: parseInt(document.getElementById('tipoDispositivo').value) || null,
                idProveedor: parseInt(document.getElementById('proveedor').value) || null,
                latitud: parseFloat(document.getElementById('latitud').value) || null,
                longitud: parseFloat(document.getElementById('longitud').value) || null,
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

            // 🧹 LIMPIAR FORMULARIO DESPUÉS DE GUARDAR
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

// 🧩 Guardar cambios del equipo editado (modal de edición)
window.guardarCambiosEquipo = async function () {
    const equipoId = document.getElementById('editarId').value;

    if (!equipoId) {
        alert('❌ No se encontró el ID del equipo a editar.');
        return;
    }

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

        console.log("📦 Enviando datos:", equipoUsuarioDto);

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
            idSede: parseInt(document.getElementById('editarSede').value) || null,
            idTipoDispositivo: parseInt(document.getElementById('editarTipoDispositivo').value) || null,
            idProveedor: parseInt(document.getElementById('editarProveedor').value) || null,
            latitud: parseFloat(document.getElementById('editarLatitud').value) || null,
            longitud: parseFloat(document.getElementById('editarLongitud').value) || null,
            sistemaOperativo: document.getElementById('editarSistemaOperativo').value,
            macEquipo: document.getElementById('editarMacEquipo').value,
        };

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

// 🧩 Abrir modal de edición con carga de selects
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
        }, 200);

        const modal = new bootstrap.Modal(document.getElementById('modalEditarEquipo'));
        modal.show();
    } catch (error) {
        console.error("Error al abrir modal de edición:", error);
        alert("❌ No se pudo cargar la información del equipo.");
    }
};

// 🧩 Editar equipo (reutiliza el modal de crear)
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

// ⚙️ Cargar selects para el modal de CREAR/EDITAR (reutilizable)
window.cargarSelects = async function () {
    try {
        const response = await fetch('/api/equipos/admin/form-data');
        if (!response.ok) throw new Error('Error al cargar datos del formulario');
        const data = await response.json();

        // Guardar usuarios en caché si no existe
        if (!window._usuariosCache && data.usuarios?.length) {
            window._usuariosCache = data.usuarios;
        }

        const selects = [
            {
                id: 'usuarioInfo',
                list: [{ usuario: 'nuevo', nombreCompleto: 'Crear nuevo usuario' }, ...(window._usuariosCache || data.usuarios)],
                value: 'usuario',
                text: item => item.usuario === 'nuevo' ? item.nombreCompleto : `${item.nombreCompleto} (${item.correo || item.usuario})`
            },
            { id: 'estado', list: data.estados, value: 'id', text: item => item.nombreEstado },
            { id: 'equipoPersonal', list: data.equiposPersonales, value: 'id', text: item => item.nombrePersonal },
            { id: 'sede', list: data.sedes, value: 'id', text: item => item.nombreSede },
            { id: 'tipoDispositivo', list: data.tiposDispositivo, value: 'id', text: item => item.nombreTipo },
            { id: 'proveedor', list: data.proveedores, value: 'id', text: item => item.nombreProveedor },
            // CORRECCIÓN: Usar 'nombreDocumento' en lugar de 'TipoDocumento'
            { id: 'tipoDocumento', list: data.tiposDocumento, value: 'id', text: item => item.nombreDocumento },
            { id: 'area', list: data.areas, value: 'id', text: item => item.nombreArea },
            { id: 'campana', list: data.campanas, value: 'id', text: item => item.nombreCampaña }
        ];

        for (const { id, list, value, text } of selects) {
            const select = document.getElementById(id);
            if (!select) continue;

            // Destruir TomSelect existente si hay uno
            if (select.tomselect) {
                select.tomselect.destroy();
            }

            select.innerHTML = '<option value="">Seleccionar...</option>';

            requestAnimationFrame(() => {
                list.forEach(item => {
                    const option = document.createElement('option');
                    option.value = item[value];
                    option.text = text(item);
                    select.appendChild(option);
                });

                new TomSelect(select, {
                    placeholder: 'Buscar...',
                    maxOptions: 500,
                    allowEmptyOption: true,
                    sortField: { field: "text", direction: "asc" },
                    searchField: ['text', 'value'],
                    onChange: function (value) {
                        // Detectar cambio en el select de usuario
                        if (id === 'usuarioInfo') {
                            window.manejarCambioUsuario(select, false);
                        }
                    }
                });
            });
        }
    } catch (error) {
        console.error('❌ Error en cargarSelects:', error);
    }
};

// ⚙️ Cargar selects para el modal de EDITAR
window.cargarSelectsEditar = async function () {
    try {
        const response = await fetch('/api/equipos/admin/form-data');
        if (!response.ok) throw new Error('Error al cargar datos del formulario');
        const data = await response.json();

        if (!window._usuariosCache && data.usuarios?.length) {
            window._usuariosCache = data.usuarios;
        }

        const selects = [
            {
                id: 'editarUsuarioInfo',
                list: [{ usuario: 'nuevo', nombreCompleto: 'Crear nuevo usuario' }, ...(window._usuariosCache || data.usuarios)],
                value: 'usuario',
                text: item => item.usuario === 'nuevo' ? item.nombreCompleto : `${item.nombreCompleto} (${item.correo || item.usuario})`
            },
            { id: 'editarEstado', list: data.estados, value: 'id', text: item => item.nombreEstado },
            { id: 'editarEquipoPersonal', list: data.equiposPersonales, value: 'id', text: item => item.nombrePersonal },
            { id: 'editarSede', list: data.sedes, value: 'id', text: item => item.nombreSede },
            { id: 'editarTipoDispositivo', list: data.tiposDispositivo, value: 'id', text: item => item.nombreTipo },
            { id: 'editarProveedor', list: data.proveedores, value: 'id', text: item => item.nombreProveedor },
            // CORRECCIÓN: Usar 'nombreDocumento'
            { id: 'editarTipoDocumento', list: data.tiposDocumento, value: 'id', text: item => item.nombreDocumento },
            { id: 'editarArea', list: data.areas, value: 'id', text: item => item.nombreArea },
            { id: 'editarCampana', list: data.campanas, value: 'id', text: item => item.nombreCampaña }
        ];

        for (const { id, list, value, text } of selects) {
            const select = document.getElementById(id);
            if (!select) continue;

            if (select.tomselect) {
                select.tomselect.destroy();
            }

            select.innerHTML = '<option value="">Seleccionar...</option>';

            requestAnimationFrame(() => {
                list.forEach(item => {
                    const option = document.createElement('option');
                    option.value = item[value];
                    option.text = text(item);
                    select.appendChild(option);
                });

                new TomSelect(select, {
                    placeholder: 'Buscar...',
                    maxOptions: 500,
                    allowEmptyOption: true,
                    sortField: { field: "text", direction: "asc" },
                    onChange: function (value) {
                        if (id === 'editarUsuarioInfo') {
                            window.manejarCambioUsuario(select, true);
                        }
                    }
                });
            });
        }
    } catch (error) {
        console.error('❌ Error en cargarSelectsEditar:', error);
    }
};


window.manejarCambioUsuario = async function (selectElement, esEditar = false) {
    const formId = esEditar ? 'formUsuarioInfoEditar' : 'formUsuarioInfo';
    const formUsuario = document.getElementById(formId);
    const usuarioSeleccionado = selectElement.value;

    console.log(`🔄 Cambio de usuario detectado: ${usuarioSeleccionado}, esEditar: ${esEditar}`);

    // Si se selecciona "Crear nuevo usuario", mostrar el formulario y limpiar campos
    if (usuarioSeleccionado === 'nuevo') {
        if (formUsuario) {
            formUsuario.style.display = 'block';
            // Limpiamos los campos por si acaso
            limpiarCamposUsuarioFormulario(esEditar);
        }
        return;
    }

    if (usuarioSeleccionado) {
        // Buscar información del usuario en el caché del AD
        const usuario = window._usuariosCache?.find(u => u.usuario === usuarioSeleccionado);

        if (usuario) {
            // Llenar los campos de nombres y apellidos
            const nombresId = esEditar ? 'editarNombres' : 'nombres';
            const apellidosId = esEditar ? 'editarApellidos' : 'apellidos';

            const nombresInput = document.getElementById(nombresId);
            const apellidosInput = document.getElementById(apellidosId);

            nombresInput.value = usuario.nombre || '';
            apellidosInput.value = usuario.apellidos || '';

            console.log('✅ Usuario del AD seleccionado:', {
                usuario: usuario.usuario,
                nombre: usuario.nombre,
                apellidos: usuario.apellidos,
                correo: usuario.correo
            });

            // 🆕 NUEVA LÓGICA: Buscar en la base de datos local
            if (usuario.nombre && usuario.apellidos) {
                console.log("🔍 Buscando usuario en la base de datos local...");
                try {
                    const response = await fetch(`/api/equipos/admin/usuario-por-nombre?nombres=${encodeURIComponent(usuario.nombre)}&apellidos=${encodeURIComponent(usuario.apellidos)}`);

                    if (response.ok) {
                        const datosUsuarioLocal = await response.json();
                        console.log("✅ Usuario encontrado en BD local:", datosUsuarioLocal);

                        // Rellenar el formulario con los datos de la BD
                        rellenarFormularioUsuario(datosUsuarioLocal, esEditar);

                    } else if (response.status === 404) {
                        console.log("ℹ️ Usuario no encontrado en BD local. Se puede crear uno nuevo.");
                        // Limpiamos los campos para que se puedan ingresar nuevos datos
                        limpiarCamposUsuarioFormulario(esEditar);
                    } else {
                        console.error("❌ Error al buscar usuario en BD local:", response.statusText);
                    }
                } catch (error) {
                    console.error("❌ Error de red al buscar usuario:", error);
                }
            }
        }

        // Mostrar el formulario con animación
        if (formUsuario) {
            formUsuario.style.display = 'block';
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
// Función auxiliar para rellenar el formulario
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

// Función auxiliar para limpiar los campos del formulario de usuario
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

// 🗑️ Manejo de eliminación de equipos
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