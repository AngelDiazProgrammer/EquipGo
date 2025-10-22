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

// 🧩 Guardar o actualizar equipo con usuario (versión tolerante a null)
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

// 🎯 Función CORREGIDA para detectar cuando se selecciona "Inactivo" en el estado
window.manejarCambioEstado = function (esEditar = false) {
    const estadoSelectId = esEditar ? 'editarEstado' : 'estado';
    const motivoContainerId = esEditar ? 'motivoContainerEditar' : 'motivoContainer';

    const estadoSelect = document.getElementById(estadoSelectId);
    const motivoContainer = document.getElementById(motivoContainerId);

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
    }
};

// 🎯 Función MEJORADA para cargar los subestados en el select
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

    // IMPORTANTE: Usar requestAnimationFrame para asegurar que el DOM se actualice
    requestAnimationFrame(() => {
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
                },
                onLoad: function (data) {
                    console.log(`📥 TomSelect onLoad ejecutado para ${selectId}:`, data);
                }
            });

            console.log(`✅ SubEstados cargados exitosamente en ${selectId}`);
            console.log(`   - ${subEstados.length} subestados disponibles`);

            // Verificar instancia de TomSelect
            if (select.tomselect) {
                console.log(`✅ Instancia TomSelect creada correctamente`);
                console.log(`   - Opciones en TomSelect.options:`, Object.keys(select.tomselect.options).length);
            } else {
                console.error(`❌ No se pudo crear la instancia de TomSelect`);
            }

        } catch (error) {
            console.error(`❌ Error al inicializar TomSelect para ${selectId}:`, error);
        }
    });

    console.log(`🏁 cargarSubEstados finalizado para ${selectId}`);
};

// ⚙️ ACTUALIZACIÓN para cargarSelects - Modal CREAR
// Reemplaza tu función cargarSelects con esta versión mejorada
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

// ⚙️ ACTUALIZACIÓN para cargarSelectsEditar - Modal EDITAR
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