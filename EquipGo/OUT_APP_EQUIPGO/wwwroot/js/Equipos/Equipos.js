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
        const selectsIds = ['usuarioInfo', 'estado', 'sede', 'tipoDispositivo', 'proveedor'];
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

// 🧩 Guardar o actualizar equipo
window.guardarEquipo = async function () {
    const form = document.getElementById('formCrearEquipo');
    const equipoId = form.getAttribute('data-id'); // puede ser null o string

    const equipoDto = {
        marca: document.getElementById('marca').value,
        modelo: document.getElementById('modelo').value,
        serial: document.getElementById('serial').value,
        codigoBarras: document.getElementById('codigoBarras').value,
/*        ubicacion: document.getElementById('ubicacion').value,*/
        idUsuarioInfo: parseInt(document.getElementById('usuarioInfo').value) || null,
        idEstado: parseInt(document.getElementById('estado').value) || null,
/*        idEquipoPersonal: parseInt(document.getElementById('equipoPersonal').value) || null,*/
        idSede: parseInt(document.getElementById('sede').value) || null,
        idTipoDispositivo: parseInt(document.getElementById('tipoDispositivo').value) || null,
        idProveedor: parseInt(document.getElementById('proveedor').value) || null,
        latitud: parseFloat(document.getElementById('latitud').value) || null,
        longitud: parseFloat(document.getElementById('longitud').value) || null,
        sistemaOperativo: document.getElementById('sistemaOperativo').value,
        macEquipo: document.getElementById('macEquipo').value,
/*        versionSoftware: document.getElementById('versionSoftware').value*/
    };

    const url = equipoId ? `/api/equipos/admin/${equipoId}` : `/api/equipos/admin`;
    const method = equipoId ? 'PUT' : 'POST';

    try {
        const response = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(equipoDto)
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
            alert('❌ Error: ' + (error.error || 'No se pudo guardar el equipo.'));
        }
    } catch (error) {
        console.error(error);
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

    const equipoDto = {
        marca: document.getElementById('editarMarca').value,
        modelo: document.getElementById('editarModelo').value,
        serial: document.getElementById('editarSerial').value,
        codigoBarras: document.getElementById('editarCodigoBarras').value,
/*        ubicacion: document.getElementById('editarUbicacion').value,*/
        idUsuarioInfo: parseInt(document.getElementById('editarUsuarioInfo').value) || null,
        idEstado: parseInt(document.getElementById('editarEstado').value) || null,
/*        idEquipoPersonal: parseInt(document.getElementById('editarEquipoPersonal').value) || null,*/
        idSede: parseInt(document.getElementById('editarSede').value) || null,
        idTipoDispositivo: parseInt(document.getElementById('editarTipoDispositivo').value) || null,
        idProveedor: parseInt(document.getElementById('editarProveedor').value) || null,
        latitud: parseFloat(document.getElementById('editarLatitud').value) || null,
        longitud: parseFloat(document.getElementById('editarLongitud').value) || null,
        sistemaOperativo: document.getElementById('editarSistemaOperativo').value,
        macEquipo: document.getElementById('editarMacEquipo').value,
/*        versionSoftware: document.getElementById('editarVersionSoftware').value*/
    };

    try {
        const response = await fetch(`/api/equipos/admin/${equipoId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(equipoDto)
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
            alert('❌ Error: ' + (error.error || 'No se pudo actualizar el equipo.'));
        }
    } catch (error) {
        console.error(error);
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
        await cargarSelects();

        // Obtener datos del equipo
        const response = await fetch(`/api/equipos/${id}`);
        if (!response.ok) throw new Error("No se pudo obtener el equipo");
        const equipo = await response.json();

        console.log("📦 Datos del equipo recibidos:", equipo);

        // 🔍 VERIFICAR QUE LOS ELEMENTOS EXISTEN ANTES DE ASIGNAR VALORES
        const campos = [
            { id: 'marca', value: equipo.marca || "" },
            { id: 'modelo', value: equipo.modelo || "" },
            { id: 'serial', value: equipo.serial || "" },
            { id: 'codigoBarras', value: equipo.codigoBarras || "" },
            { id: 'ubicacion', value: equipo.ubicacion || "" },
            { id: 'latitud', value: equipo.latitud || "" },
            { id: 'longitud', value: equipo.longitud || "" },
            { id: 'sistemaOperativo', value: equipo.sistemaOperativo || "" },
            { id: 'macEquipo', value: equipo.macEquipo || "" }
            // 'versionSoftware' está comentado en el HTML, por eso lo omitimos
        ];

        // Asignar valores solo si los elementos existen
        campos.forEach(campo => {
            const elemento = document.getElementById(campo.id);
            if (elemento) {
                elemento.value = campo.value;
                console.log(`✅ Campo ${campo.id} asignado:`, campo.value);
            } else {
                console.warn(`⚠️ Campo no encontrado: ${campo.id}`);
            }
        });

        // Establecer el ID del equipo en el formulario
        const form = document.getElementById('formCrearEquipo');
        if (form) {
            form.setAttribute('data-id', id);
            console.log("✅ ID del equipo establecido en el formulario:", id);
        }

        // Configurar selects con TomSelect después de un breve delay
        setTimeout(() => {
            const selectConfigs = [
                { id: 'usuarioInfo', value: equipo.idUsuarioInfo },
                { id: 'estado', value: equipo.idEstado },
                { id: 'sede', value: equipo.idSede },
                { id: 'tipoDispositivo', value: equipo.idTipoDispositivo },
                { id: 'proveedor', value: equipo.idProveedor }
            ];

            selectConfigs.forEach(({ id, value }) => {
                const selectElement = document.getElementById(id);
                if (selectElement && selectElement.tomselect && value) {
                    try {
                        selectElement.tomselect.setValue(value);
                        console.log(`✅ Select ${id} configurado:`, value);
                    } catch (error) {
                        console.error(`❌ Error configurando select ${id}:`, error);
                    }
                } else if (!selectElement) {
                    console.warn(`⚠️ Select no encontrado: ${id}`);
                } else if (!selectElement.tomselect) {
                    console.warn(`⚠️ TomSelect no inicializado en: ${id}`);
                }
            });
        }, 500); // Aumentamos el delay para asegurar que TomSelect esté listo

        // Mostrar el modal
        const modalElement = document.getElementById('modalCrearEquipo');
        if (modalElement) {
            const modal = new bootstrap.Modal(modalElement);
            modal.show();
            console.log("✅ Modal de edición abierto");
        } else {
            console.error("❌ No se encontró el modal modalCrearEquipo");
        }

    } catch (error) {
        console.error("❌ Error completo en editarEquipo:", error);
        alert("❌ Error al cargar el equipo para edición.");
    }
};

// 🆕 Función para manejar el cambio de usuario y mostrar/ocultar formulario adicional
window.manejarCambioUsuario = function (selectElement, esEditar = false) {
    const formId = esEditar ? 'formUsuarioInfoEditar' : 'formUsuarioInfo';
    const formUsuario = document.getElementById(formId);
    const usuarioSeleccionado = selectElement.value;

    if (usuarioSeleccionado) {
        // Buscar información del usuario en el caché
        const usuario = window._usuariosCache?.find(u => u.usuario === usuarioSeleccionado);

        if (usuario) {
            // Llenar los campos de nombres y apellidos
            const nombresId = esEditar ? 'editarNombres' : 'nombres';
            const apellidosId = esEditar ? 'editarApellidos' : 'apellidos';

            document.getElementById(nombresId).value = usuario.nombre || '';
            document.getElementById(apellidosId).value = usuario.apellidos || '';

            console.log('✅ Usuario seleccionado:', {
                usuario: usuario.usuario,
                nombre: usuario.nombre,
                apellidos: usuario.apellidos,
                correo: usuario.correo
            });
        }

        // Mostrar el formulario con animación
        formUsuario.style.display = 'block';
    } else {
        // Ocultar el formulario si no hay usuario seleccionado
        formUsuario.style.display = 'none';

        // Limpiar campos
        const nombresId = esEditar ? 'editarNombres' : 'nombres';
        const apellidosId = esEditar ? 'editarApellidos' : 'apellidos';
        const tipoDocId = esEditar ? 'editarTipoDocumento' : 'tipoDocumento';
        const numDocId = esEditar ? 'editarNumeroDocumento' : 'numeroDocumento';
        const areaId = esEditar ? 'editarArea' : 'area';
        const campanaId = esEditar ? 'editarCampana' : 'campana';

        document.getElementById(nombresId).value = '';
        document.getElementById(apellidosId).value = '';
        document.getElementById(tipoDocId).value = '';
        document.getElementById(numDocId).value = '';
        document.getElementById(areaId).value = '';
        document.getElementById(campanaId).value = '';
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
            { id: 'usuarioInfo', list: window._usuariosCache || data.usuarios, value: 'usuario', text: item => `${item.nombreCompleto} (${item.correo || item.usuario})` },
            { id: 'estado', list: data.estados, value: 'id', text: item => item.nombreEstado },
            { id: 'equipoPersonal', list: data.equiposPersonales, value: 'id', text: item => item.nombrePersonal },
            { id: 'sede', list: data.sedes, value: 'id', text: item => item.nombreSede },
            { id: 'tipoDispositivo', list: data.tiposDispositivo, value: 'id', text: item => item.nombreTipo },
            { id: 'proveedor', list: data.proveedores, value: 'id', text: item => item.nombreProveedor }
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

                const tomSelectInstance = new TomSelect(select, {
                    placeholder: 'Buscar...',
                    maxOptions: 500,
                    allowEmptyOption: true,
                    sortField: { field: "text", direction: "asc" },
                    searchField: ['text', 'value'],
                    score: function (search) {
                        const normalize = str => str
                            .toLowerCase()
                            .normalize("NFD")
                            .replace(/[\u0300-\u036f]/g, "");
                        const words = normalize(search).split(/\s+/).filter(Boolean);
                        return function (item) {
                            const text = normalize(item.text);
                            return words.every(word => text.includes(word)) ? 1 : 0;
                        };
                    },
                    onChange: function (value) {
                        // 🆕 Detectar cambio en el select de usuario
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

        // Guardar usuarios en caché si no existe
        if (!window._usuariosCache && data.usuarios?.length) {
            window._usuariosCache = data.usuarios;
        }

        const selects = [
            { id: 'editarUsuarioInfo', list: window._usuariosCache || data.usuarios, value: 'usuario', text: item => `${item.nombreCompleto} (${item.correo || item.usuario})` },
            { id: 'editarEstado', list: data.estados, value: 'id', text: item => item.nombreEstado },
            { id: 'editarEquipoPersonal', list: data.equiposPersonales, value: 'id', text: item => item.nombrePersonal },
            { id: 'editarSede', list: data.sedes, value: 'id', text: item => item.nombreSede },
            { id: 'editarTipoDispositivo', list: data.tiposDispositivo, value: 'id', text: item => item.nombreTipo },
            { id: 'editarProveedor', list: data.proveedores, value: 'id', text: item => item.nombreProveedor }
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

                const tomSelectInstance = new TomSelect(select, {
                    placeholder: 'Buscar...',
                    maxOptions: 500,
                    allowEmptyOption: true,
                    sortField: { field: "text", direction: "asc" },
                    searchField: ['text', 'value'],
                    score: function (search) {
                        const normalize = str => str
                            .toLowerCase()
                            .normalize("NFD")
                            .replace(/[\u0300-\u036f]/g, "");
                        const words = normalize(search).split(/\s+/).filter(Boolean);
                        return function (item) {
                            const text = normalize(item.text);
                            return words.every(word => text.includes(word)) ? 1 : 0;
                        };
                    },
                    onChange: function (value) {
                        // 🆕 Detectar cambio en el select de usuario (modal editar)
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

// 🧩 Precargar selects (solo se ejecuta una vez al inicio)
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