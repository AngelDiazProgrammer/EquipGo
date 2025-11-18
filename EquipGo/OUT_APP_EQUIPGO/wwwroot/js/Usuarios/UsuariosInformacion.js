// Variables globales
window.usuariosParaCargar = [];
window.erroresCargaUsuarios = [];

// ================================
// FUNCIONES PARA CARGA MASIVA
// ================================

// Función para inicializar eventos del modal de carga masiva
window.inicializarEventosCargaMasiva = function () {
    console.log("🔄 Inicializando eventos de carga masiva...");

    // Asignar evento al botón procesar
    const procesarBtn = document.getElementById('procesarCargaBtnUsuarios');
    if (procesarBtn) {
        // Remover evento anterior para evitar duplicados
        procesarBtn.removeEventListener('click', window.manejarProcesarCarga);

        // Crear nuevo event listener
        window.manejarProcesarCarga = function () {
            console.log("🎯 Botón procesar clickeado");
            window.procesarCargaMasivaUsuarios();
        };

        // Agregar event listener
        procesarBtn.addEventListener('click', window.manejarProcesarCarga);
        console.log("✅ Evento asignado al botón procesar");
    } else {
        console.error("❌ No se encontró el botón procesarCargaBtnUsuarios");
    }
};

// Función simplificada para abrir modal carga masiva
window.abrirModalCargaMasiva = function () {
    console.log("🔄 Abriendo modal para carga masiva");

    // Inicializar el modal primero
    window.inicializarModalCargaMasivaUsuarios();

    // Abrir el modal manualmente
    const modalElement = document.getElementById('modalCargaMasivaUsuarios');
    if (modalElement) {
        const modal = new bootstrap.Modal(modalElement);
        modal.show();
    } else {
        console.error("❌ No se pudo encontrar el modal carga masiva");
    }
};

// Inicialización mejorada para modal carga masiva
window.inicializarModalCargaMasivaUsuarios = function () {
    console.log("🔄 Inicializando modal carga masiva usuarios...");

    const modalElement = document.getElementById('modalCargaMasivaUsuarios');
    if (!modalElement) {
        console.error("❌ No se encontró el modal modalCargaMasivaUsuarios");
        return;
    }

    // Remover event listeners anteriores
    modalElement.removeEventListener('show.bs.modal', window.manejarShowModalCarga);
    modalElement.removeEventListener('hidden.bs.modal', window.manejarHideModalCarga);

    // Crear nuevos event listeners
    window.manejarShowModalCarga = function () {
        console.log("🎯 Modal de carga masiva de usuarios abierto");
        window.inicializarCargaMasivaUsuarios();
    };

    window.manejarHideModalCarga = function () {
        console.log("🎯 Modal de carga masiva de usuarios cerrado - limpiando todo");
        window.limpiarTodoUsuarios();
    };

    // Agregar event listeners
    modalElement.addEventListener('show.bs.modal', window.manejarShowModalCarga);
    modalElement.addEventListener('hidden.bs.modal', window.manejarHideModalCarga);

    console.log("✅ Modal carga masiva usuarios inicializado correctamente");
};

//Inicializar carga masiva
window.inicializarCargaMasivaUsuarios = function () {
    console.log("🔄 Inicializando modal de carga masiva de usuarios...");

    // Limpiar estado anterior
    window.usuariosParaCargar = [];

    // Configurar evento del file input
    const fileInput = document.getElementById('fileCargaMasivaUsuarios');
    if (fileInput) {
        fileInput.addEventListener('change', window.manejarSeleccionArchivoUsuarios);
    }

    // Inicializar eventos del botón procesar
    window.inicializarEventosCargaMasiva();

    // Limpiar info del archivo
    const infoArchivo = document.getElementById('infoArchivoUsuarios');
    if (infoArchivo) {
        infoArchivo.style.display = 'none';
    }

    // Deshabilitar botón procesar
    const procesarBtn = document.getElementById('procesarCargaBtnUsuarios');
    if (procesarBtn) {
        procesarBtn.disabled = true;
    }

    console.log("✅ Modal de carga masiva de usuarios inicializado");
};

// Manejar selección de archivo para usuarios
window.manejarSeleccionArchivoUsuarios = async function (event) {
    console.log("📁 ARCHIVO USUARIOS SELECCIONADO");

    const file = event.target.files[0];
    if (!file) return;

    console.log("📦 Archivo:", file.name, file.size, "bytes");

    const btnProcesar = document.getElementById('procesarCargaBtnUsuarios');

    try {
        // Mostrar estado de carga
        if (btnProcesar) {
            btnProcesar.disabled = true;
            btnProcesar.innerHTML = '<i class="bi bi-arrow-repeat spinner"></i> Leyendo...';
        }

        // Leer archivo
        window.usuariosParaCargar = await leerArchivoExcelUsuarios(file);
        console.log("✅ Usuarios leídos:", window.usuariosParaCargar.length);

        if (window.usuariosParaCargar.length === 0) {
            alert('❌ El archivo no contiene usuarios válidos.');
            if (btnProcesar) {
                btnProcesar.disabled = true;
                btnProcesar.innerHTML = '<i class="bi bi-upload"></i> Procesar Carga Masiva';
            }
            return;
        }

        // Mostrar info del archivo
        document.getElementById('nombreArchivoUsuarios').textContent = file.name;
        document.getElementById('cantidadUsuarios').textContent = `${window.usuariosParaCargar.length} usuarios`;
        document.getElementById('infoArchivoUsuarios').style.display = 'block';

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

// Leer archivo Excel para usuarios
function leerArchivoExcelUsuarios(file) {
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

                const usuarios = jsonData
                    .map((row, index) => {
                        const nombres = row.Nombres?.toString().trim() || '';
                        const apellidos = row.Apellidos?.toString().trim() || '';
                        const numeroDocumento = row.NumeroDocumento?.toString().trim() || '';
                        const nombreCampaña = row.NombreCampaña?.toString().trim() || '';

                        // 🔥 CONVERTIR CAMPOS NUMÉRICOS CON MANEJO DE NULLS
                        const idTipoDocumento = convertirANullableInt(row.IdTipoDocumento);
                        const idArea = convertirANullableInt(row.IdArea);

                        // 🔥 SOLO validar campaña como obligatoria
                        if (!nombreCampaña) {
                            console.warn(`Fila ${index + 2} ignorada - Campaña es obligatoria`);
                            return null;
                        }

                        return {
                            idTipoDocumento: idTipoDocumento, // ✅ Puede ser número o null
                            numeroDocumento: numeroDocumento || null,
                            nombres: nombres || null,
                            apellidos: apellidos || null,
                            idArea: idArea, // ✅ Puede ser número o null
                            nombreCampaña: nombreCampaña, // ✅ Obligatorio
                            idEstado: 1 // ✅ Siempre activo
                        };
                    })
                    .filter(u => u !== null);

                console.log(`✅ Usuarios leídos: ${usuarios.length}`);
                console.log("📋 Estructura del primer usuario:", usuarios[0]);
                resolve(usuarios);
            } catch (error) {
                reject(new Error('Formato inválido: ' + error.message));
            }
        };

        reader.onerror = () => reject(new Error('Error al leer archivo'));
        reader.readAsArrayBuffer(file);
    });
}

// 🔥 FUNCIÓN AUXILIAR PARA CONVERTIR A NÚMERO O NULL
function convertirANullableInt(valor) {
    if (valor === null || valor === undefined || valor === '') {
        return null;
    }

    const numero = parseInt(valor);
    return isNaN(numero) ? null : numero;
}

// Procesar carga masiva de usuarios
// Procesar carga masiva de usuarios - CON PROTECCIÓN CONTRA DOBLE EJECUCIÓN
window.procesarCargaMasivaUsuarios = async function () {
    // 🔥 PROTECCIÓN CONTRA DOBLE EJECUCIÓN
    if (window.procesandoCargaMasiva) {
        console.log("⏳ Ya se está procesando una carga masiva, ignorando...");
        return;
    }

    window.procesandoCargaMasiva = true;
    console.log("🎯 PROCESANDO CARGA MASIVA DE USUARIOS");
    console.log("📊 Usuarios:", window.usuariosParaCargar.length);

    if (!window.usuariosParaCargar || window.usuariosParaCargar.length === 0) {
        alert('❌ No hay usuarios para procesar');
        window.procesandoCargaMasiva = false;
        return;
    }

    const btnProcesar = document.getElementById('procesarCargaBtnUsuarios');
    if (!btnProcesar) {
        window.procesandoCargaMasiva = false;
        return;
    }

    btnProcesar.disabled = true;
    btnProcesar.innerHTML = '<i class="bi bi-arrow-repeat spinner"></i> Procesando...';

    try {
        console.log("📤 Enviando solicitud al servidor...");

        const response = await fetch('/api/usuarios/carga-masiva', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(window.usuariosParaCargar)
        });

        console.log("📥 Respuesta recibida, status:", response.status);

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || 'Error del servidor');
        }

        const resultado = await response.json();
        console.log("✅ Resultado:", resultado);

        mostrarResultadosCargaUsuarios(resultado);

    } catch (error) {
        console.error("❌ Error:", error);
        alert('Error en la carga: ' + error.message);
    } finally {
        window.procesandoCargaMasiva = false;
        if (btnProcesar) {
            btnProcesar.disabled = false;
            btnProcesar.innerHTML = '<i class="bi bi-upload"></i> Procesar Carga Masiva';
        }
    }
};

// Mostrar resultados de la carga de usuarios - VERSIÓN ULTRA SEGURA
window.mostrarResultadosCargaUsuarios = function (resultado) {
    console.log("📊 Mostrando resultados usuarios:", resultado);

    // 🔥 BLOQUEO DE SEGURIDAD - Verificar que no se ejecute código automático
    console.log("🔒 BLOQUEO ACTIVADO - No se ejecutará descarga automática");

    // Mostrar sección de resultados
    document.getElementById('resultadosCargaUsuarios').style.display = 'block';

    // Actualizar resumen
    document.getElementById('totalRegistrosUsuarios').textContent = resultado.totalRegistros;
    document.getElementById('registrosExitososUsuarios').textContent = resultado.registrosExitosos;
    document.getElementById('registrosFallidosUsuarios').textContent = resultado.registrosFallidos;

    // Guardar errores para posible exportación
    window.erroresCargaUsuarios = resultado.errores || [];

    // Mostrar mensaje principal
    const mensajeElement = document.getElementById('mensajeResultadoUsuarios');

    // 🔥 NUEVA LÓGICA: Mostrar opciones en lugar de acciones automáticas
    if (resultado.registrosExitosos > 0 && resultado.registrosFallidos === 0) {
        // ✅ ÉXITO COMPLETO - Mostrar opción de recargar
        mensajeElement.className = 'alert alert-success';
        mensajeElement.innerHTML = `
            <strong>${resultado.mensaje}</strong>
            <div class="mt-2">
                <span class="badge bg-primary">Total: ${resultado.totalRegistros}</span>
                <span class="badge bg-success">Éxitos: ${resultado.registrosExitosos}</span>
                <span class="badge bg-danger">Fallidos: ${resultado.registrosFallidos}</span>
            </div>
            <div class="mt-3">
                <button class="btn btn-success btn-sm" onclick="window.recargarPaginaUsuarios()">
                    <i class="bi bi-arrow-clockwise"></i> Recargar página para ver los cambios
                </button>
                <button class="btn btn-outline-secondary btn-sm ms-2" onclick="window.continuarSinDescargarUsuarios()">
                    <i class="bi bi-skip-forward"></i> Continuar sin recargar
                </button>
            </div>
        `;

    } else if (resultado.registrosExitosos === 0 && resultado.registrosFallidos > 0) {
        // 🔥 SOLO ERRORES - Mostrar opciones de descarga
        mensajeElement.className = 'alert alert-danger';
        mensajeElement.innerHTML = `
            <strong>${resultado.mensaje}</strong>
            <div class="mt-2">
                <span class="badge bg-primary">Total: ${resultado.totalRegistros}</span>
                <span class="badge bg-success">Éxitos: ${resultado.registrosExitosos}</span>
                <span class="badge bg-danger">Fallidos: ${resultado.registrosFallidos}</span>
            </div>
            <div class="mt-3">
                <div class="btn-group" role="group">
                    <button class="btn btn-outline-danger btn-sm" onclick="window.descargarErroresUsuariosOpcional()">
                        <i class="bi bi-download"></i> Descargar reporte de errores
                    </button>
                    <button class="btn btn-outline-secondary btn-sm" onclick="window.continuarSinDescargarUsuarios()">
                        <i class="bi bi-skip-forward"></i> Continuar sin descargar
                    </button>
                </div>
            </div>
        `;

        // Mostrar errores en la tabla
        if (window.erroresCargaUsuarios.length > 0) {
            window.mostrarErroresUsuarios(window.erroresCargaUsuarios);
            document.getElementById('panelErroresUsuarios').style.display = 'block';
        }

    } else if (resultado.registrosExitosos > 0 && resultado.registrosFallidos > 0) {
        // ⚠️ CASO MIXTO - Mostrar ambas opciones
        mensajeElement.className = 'alert alert-warning';
        mensajeElement.innerHTML = `
            <strong>${resultado.mensaje}</strong>
            <div class="mt-2">
                <span class="badge bg-primary">Total: ${resultado.totalRegistros}</span>
                <span class="badge bg-success">Éxitos: ${resultado.registrosExitosos}</span>
                <span class="badge bg-danger">Fallidos: ${resultado.registrosFallidos}</span>
            </div>
            <div class="mt-3">
                <div class="btn-group" role="group">
                    <button class="btn btn-success btn-sm" onclick="window.recargarPaginaUsuarios()">
                        <i class="bi bi-arrow-clockwise"></i> Recargar para ver éxitos
                    </button>
                    <button class="btn btn-outline-danger btn-sm" onclick="window.descargarErroresUsuariosOpcional()">
                        <i class="bi bi-download"></i> Descargar errores
                    </button>
                    <button class="btn btn-outline-secondary btn-sm" onclick="window.continuarSinDescargarUsuarios()">
                        <i class="bi bi-skip-forward"></i> Continuar
                    </button>
                </div>
            </div>
        `;

        // Mostrar errores en la tabla si los hay
        if (window.erroresCargaUsuarios.length > 0) {
            window.mostrarErroresUsuarios(window.erroresCargaUsuarios);
            document.getElementById('panelErroresUsuarios').style.display = 'block';
        }
    }

    // Scroll a resultados
    document.getElementById('resultadosCargaUsuarios').scrollIntoView({ behavior: 'smooth' });

    // 🔥 CONFIRMACIÓN FINAL
    console.log("✅ mostrarResultadosCargaUsuarios completado SIN descarga automática");
};

// DESCARGAR ERRORES DE USUARIOS DE FORMA OPCIONAL (sin cerrar modal)
window.descargarErroresUsuariosOpcional = function () {
    console.log("📊 Descargando errores de usuarios de forma opcional...");

    if (window.erroresCargaUsuarios.length === 0) {
        alert('❌ No hay errores para descargar');
        return;
    }

    try {
        // Crear datos para el Excel
        const datosErrores = window.erroresCargaUsuarios.map(error => ({
            'Fila': error.indiceFila,
            'Nombres': error.nombres || '',
            'Apellidos': error.apellidos || '',
            'Documento': error.numeroDocumento || '',
            'Error': error.error || 'Error desconocido'
        }));

        // Crear workbook
        const wb = XLSX.utils.book_new();
        const ws = XLSX.utils.json_to_sheet(datosErrores);

        // Agregar hoja al workbook
        XLSX.utils.book_append_sheet(wb, ws, 'ErroresCargaMasivaUsuarios');

        // Generar nombre del archivo con timestamp
        const nombreArchivo = `Errores_Carga_Masiva_Usuarios_${new Date().toISOString().slice(0, 16).replace(/[-:T]/g, '')}.xlsx`;

        // Descargar archivo
        XLSX.writeFile(wb, nombreArchivo);

        console.log("✅ Excel de errores de usuarios descargado opcionalmente:", nombreArchivo);

        // Mostrar mensaje de confirmación (sin cerrar el modal)
        const mensajeElement = document.getElementById('mensajeResultadoUsuarios');
        if (mensajeElement) {
            const originalHTML = mensajeElement.innerHTML;
            mensajeElement.innerHTML = `
                <div class="alert alert-info">
                    <i class="bi bi-check-circle"></i> Reporte de errores descargado exitosamente.
                    <br><small>Archivo: ${nombreArchivo}</small>
                </div>
                ${originalHTML}
            `;
        }

    } catch (error) {
        console.error('❌ Error al descargar errores opcionalmente:', error);
        alert('❌ Error al descargar el reporte de errores');
    }
};

// CONTINUAR SIN DESCARGAR ERRORES DE USUARIOS (solo limpiar para nueva carga)
window.continuarSinDescargarUsuarios = function () {
    console.log("🚀 Continuando sin descargar errores de usuarios...");

    // Mantener el modal abierto pero limpiar para nueva carga
    window.limpiarParaNuevaCargaUsuarios();
};

// LIMPIAR PARA NUEVA CARGA DE USUARIOS (sin cerrar el modal)
window.limpiarParaNuevaCargaUsuarios = function () {
    console.log("🔄 Preparando para nueva carga de usuarios...");

    // Limpiar file input
    const fileInput = document.getElementById('fileCargaMasivaUsuarios');
    if (fileInput) fileInput.value = '';

    // Limpiar info del archivo
    const infoArchivo = document.getElementById('infoArchivoUsuarios');
    if (infoArchivo) infoArchivo.style.display = 'none';

    // Limpiar resultados
    const resultadosCarga = document.getElementById('resultadosCargaUsuarios');
    if (resultadosCarga) resultadosCarga.style.display = 'none';

    // Limpiar mensajes
    const mensajeResultado = document.getElementById('mensajeResultadoUsuarios');
    if (mensajeResultado) {
        mensajeResultado.className = 'alert';
        mensajeResultado.innerHTML = '';
    }

    // Limpiar tabla de errores
    const tbodyErrores = document.getElementById('tbodyErroresUsuarios');
    if (tbodyErrores) tbodyErrores.innerHTML = '';

    const panelErrores = document.getElementById('panelErroresUsuarios');
    if (panelErrores) panelErrores.style.display = 'none';

    // Resetear botón procesar
    const procesarBtn = document.getElementById('procesarCargaBtnUsuarios');
    if (procesarBtn) {
        procesarBtn.disabled = true;
        procesarBtn.innerHTML = '<i class="bi bi-upload"></i> Procesar Carga Masiva';
    }

    // Limpiar variables globales (excepto errores por si quieren descargar después)
    window.usuariosParaCargar = [];
    // Mantenemos window.erroresCargaUsuarios por si quieren descargar más tarde

    console.log("✅ Listo para nueva carga de usuarios");
};

// FUNCIÓN PARA RECARGAR PÁGINA DE USUARIOS
window.recargarPaginaUsuarios = function () {
    console.log("🎯 Recargando página manualmente...");

    // Cerrar el modal primero
    const modal = document.getElementById('modalCargaMasivaUsuarios');
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


// LIMPIAR TODO COMPLETAMENTE PARA USUARIOS
window.limpiarTodoUsuarios = function () {
    console.log("🧹 Limpiando todo el modal de usuarios...");

    const fileInput = document.getElementById('fileCargaMasivaUsuarios');
    const procesarBtn = document.getElementById('procesarCargaBtnUsuarios');
    const infoArchivo = document.getElementById('infoArchivoUsuarios');
    const resultadosCarga = document.getElementById('resultadosCargaUsuarios');
    const mensajeResultado = document.getElementById('mensajeResultadoUsuarios');
    const tbodyErrores = document.getElementById('tbodyErroresUsuarios');
    const panelErrores = document.getElementById('panelErroresUsuarios');

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

    // 4. Limpiar variables globales
    window.usuariosParaCargar = [];
    window.erroresCargaUsuarios = [];

    console.log("✅ Modal de usuarios completamente limpiado");
};

// Mostrar errores en tabla para usuarios
function mostrarErroresUsuarios(errores) {
    const tbody = document.getElementById('tbodyErroresUsuarios');
    tbody.innerHTML = '';

    errores.forEach(error => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td class="fw-bold">${error.indiceFila}</td>
            <td>${error.nombres || '-'}</td>
            <td>${error.apellidos || '-'}</td>
            <td>${error.numeroDocumento || '-'}</td>
            <td class="text-danger">${error.error}</td>
        `;
        tbody.appendChild(tr);
    });
}

// Exportar errores a Excel (versión manual)
window.exportarErroresUsuarios = async function () {
    if (window.erroresCargaUsuarios.length === 0) {
        alert('No hay errores para exportar');
        return;
    }

    try {
        // Crear datos para el Excel
        const datosErrores = window.erroresCargaUsuarios.map(error => ({
            'Fila': error.indiceFila,
            'Nombres': error.nombres,
            'Apellidos': error.apellidos,
            'Documento': error.numeroDocumento,
            'Error': error.error
        }));

        // Crear workbook
        const wb = XLSX.utils.book_new();
        const ws = XLSX.utils.json_to_sheet(datosErrores);

        // Agregar hoja al workbook
        XLSX.utils.book_append_sheet(wb, ws, 'ErroresCargaMasivaUsuarios');

        // Generar nombre del archivo
        const nombreArchivo = `Errores_Carga_Masiva_Usuarios_${new Date().toISOString().slice(0, 16).replace(/[-:T]/g, '')}.xlsx`;

        // Descargar archivo
        XLSX.writeFile(wb, nombreArchivo);

        console.log("✅ Excel de errores de usuarios descargado manualmente");

    } catch (error) {
        console.error('Error exportando errores:', error);
        alert('❌ Error al exportar errores');
    }
};

// Limpiar archivo usuarios
window.limpiarArchivoUsuarios = function () {
    const fileInput = document.getElementById('fileCargaMasivaUsuarios');
    if (fileInput) fileInput.value = '';

    const btnProcesar = document.getElementById('procesarCargaBtnUsuarios');
    if (btnProcesar) btnProcesar.disabled = true;

    document.getElementById('infoArchivoUsuarios').style.display = 'none';
    window.usuariosParaCargar = [];
};

window.limpiarModalCargaUsuarios = function () {
    limpiarArchivoUsuarios();
    document.getElementById('resultadosCargaUsuarios').style.display = 'none';
};

// Descargar plantilla usuarios
window.descargarPlantillaCargaMasivaUsuarios = async function () {
    try {
        const boton = event.target;
        const textoOriginal = boton.innerHTML;
        boton.innerHTML = '<i class="bi bi-arrow-repeat spinner"></i> Generando...';
        boton.disabled = true;

        const response = await fetch('/api/usuarios/descargar-plantilla'); // ✅ Cambiado a generar-plantilla

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || 'Error al generar plantilla');
        }

        const blob = await response.blob();
        if (blob.size === 0) throw new Error('Archivo vacío');

        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Plantilla_Usuarios_${new Date().toISOString().slice(0, 16).replace(/[-:T]/g, '')}.xlsx`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);

        console.log("✅ Plantilla de usuarios descargada");
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
// ================================
// FUNCIONES CRUD PARA USUARIOS
// ================================

// Función simplificada para abrir modal crear usuario
window.abrirModalCrearUsuario = function () {
    console.log("🔄 Abriendo modal para crear usuario");

    // Inicializar el modal primero
    window.inicializarModalCrearUsuario();

    // Limpiar formulario
    window.limpiarFormularioUsuario();

    // Abrir el modal manualmente
    const modalElement = document.getElementById('modalCrearUsuario');
    if (modalElement) {
        const modal = new bootstrap.Modal(modalElement);
        modal.show();
    } else {
        console.error("❌ No se pudo encontrar el modal crear usuario");
    }
};

// Inicialización mejorada para modal crear usuario
window.inicializarModalCrearUsuario = function () {
    console.log("🔄 Inicializando modal crear usuario...");

    const modalElement = document.getElementById('modalCrearUsuario');
    if (!modalElement) {
        console.error("❌ No se encontró el modal modalCrearUsuario");
        return;
    }

    // Remover event listeners anteriores para evitar duplicados
    modalElement.removeEventListener('show.bs.modal', window.manejarShowModalCrear);
    modalElement.removeEventListener('hidden.bs.modal', window.manejarHideModalCrear);

    // Crear nuevos event listeners
    window.manejarShowModalCrear = function () {
        console.log("🎯 Modal crear usuario abierto - cargando selects...");
        window.cargarSelectsUsuario();
    };

    window.manejarHideModalCrear = function () {
        console.log("🎯 Modal crear usuario cerrado - limpiando formulario...");
        window.limpiarFormularioUsuario();
    };

    // Agregar event listeners
    modalElement.addEventListener('show.bs.modal', window.manejarShowModalCrear);
    modalElement.addEventListener('hidden.bs.modal', window.manejarHideModalCrear);

    console.log("✅ Modal crear usuario inicializado correctamente");
};

// Cargar selects para formulario de usuario
window.cargarSelectsUsuario = async function () {
    try {
        console.log("🔄 Iniciando carga de selects para usuario...");

        const response = await fetch('/api/usuarios/form-data');
        if (!response.ok) {
            throw new Error(`Error HTTP: ${response.status} - ${response.statusText}`);
        }

        const data = await response.json();
        console.log("📊 Datos recibidos del form-data:", data);

        const selects = [
            { id: 'crearTipoDocumento', list: data.tiposDocumento, value: 'id', text: item => item.nombreDocumento },
            { id: 'crearArea', list: data.areas, value: 'id', text: item => item.nombreArea },
            { id: 'crearCampana', list: data.campanas, value: 'id', text: item => item.nombreCampaña },
            { id: 'crearEstado', list: data.estados, value: 'id', text: item => item.nombreEstado }
        ];

        for (const { id, list, value, text } of selects) {
            const select = document.getElementById(id);
            console.log(`🔍 Procesando select: ${id}`, {
                existe: !!select,
                cantidadOpciones: list?.length || 0
            });

            if (!select) {
                console.warn(`⚠️ Select no encontrado: ${id}`);
                continue;
            }

            // Destruir TomSelect existente
            if (select.tomselect) {
                select.tomselect.destroy();
            }

            // Limpiar opciones existentes
            select.innerHTML = '<option value="">Seleccionar...</option>';

            // Agregar nuevas opciones
            if (list && list.length > 0) {
                list.forEach(item => {
                    const option = document.createElement('option');
                    option.value = item[value];
                    option.textContent = text(item);
                    select.appendChild(option);
                });
            } else {
                console.warn(`⚠️ Lista vacía para: ${id}`);
            }

            // Inicializar TomSelect
            if (typeof TomSelect !== 'undefined') {
                const tomselect = new TomSelect(select, {
                    placeholder: 'Seleccionar...',
                    maxOptions: 500,
                    allowEmptyOption: true,
                    sortField: { field: "text", direction: "asc" }
                });
                console.log(`✅ TomSelect inicializado para: ${id}`);
            } else {
                console.warn(`⚠️ TomSelect no disponible para: ${id}`);
            }
        }

        console.log("✅ Todos los selects de usuario cargados correctamente");

    } catch (error) {
        console.error('❌ Error en cargarSelectsUsuario:', error);
        alert('Error al cargar los datos del formulario: ' + error.message);
    }
};
// Función auxiliar para obtener el nombre de la campaña
window.obtenerNombreCampaña = function (selectId) {
    const select = document.getElementById(selectId);
    if (!select) return null;

    if (select.tomselect) {
        // Para TomSelect
        const selectedValue = select.tomselect.getValue();
        return select.tomselect.options[selectedValue]?.text || null;
    } else {
        // Para select normal
        return select.options[select.selectedIndex]?.text || null;
    }
};
// Limpiar formulario de usuario
window.limpiarFormularioUsuario = function () {
    console.log("🧹 Limpiando formulario de usuario");

    const form = document.getElementById('formCrearUsuario');
    if (form) {
        form.reset();
    }

    // Limpiar selects con TomSelect si existen
    const selects = ['crearTipoDocumento', 'crearArea', 'crearCampana', 'crearEstado'];
    selects.forEach(id => {
        const select = document.getElementById(id);
        if (select && select.tomselect) {
            select.tomselect.setValue('', true);
        }
    });
};

// Guardar usuario (crear o editar)
window.guardarUsuario = async function () {
    try {
        const form = document.getElementById('formCrearUsuario');
        const usuarioId = form?.getAttribute('data-id') || null;

        // Validar campos requeridos
        const camposRequeridos = [
            { id: 'crearTipoDocumento', nombre: 'Tipo de Documento' },
            { id: 'crearNumeroDocumento', nombre: 'Número de Documento' },
            { id: 'crearNombres', nombre: 'Nombres' },
            { id: 'crearApellidos', nombre: 'Apellidos' },
            { id: 'crearArea', nombre: 'Área' },
            { id: 'crearCampana', nombre: 'Campaña' },
            { id: 'crearEstado', nombre: 'Estado' }
        ];

        for (const campo of camposRequeridos) {
            const elemento = document.getElementById(campo.id);
            const valor = elemento.tomselect ? elemento.tomselect.getValue() : elemento.value;

            if (!valor || valor.toString().trim() === '') {
                alert(`❌ El campo "${campo.nombre}" es obligatorio`);
                if (elemento.focus) elemento.focus();
                return;
            }
        }

        // 🔥 OBTENER EL NOMBRE DE LA CAMPAÑA SELECCIONADA
        const campanaSelect = document.getElementById('crearCampana');
        const idCampaña = campanaSelect.value;
        const nombreCampaña = campanaSelect.tomselect
            ? campanaSelect.tomselect.options[idCampaña]?.text
            : campanaSelect.options[campanaSelect.selectedIndex]?.text;

        if (!nombreCampaña) {
            alert('❌ No se pudo obtener el nombre de la campaña seleccionada');
            return;
        }

        // Preparar datos del usuario
        const usuarioData = {
            idTipoDocumento: parseInt(document.getElementById('crearTipoDocumento').value),
            numeroDocumento: document.getElementById('crearNumeroDocumento').value.trim(),
            nombres: document.getElementById('crearNombres').value.trim(),
            apellidos: document.getElementById('crearApellidos').value.trim(),
            idArea: parseInt(document.getElementById('crearArea').value),
            nombreCampaña: nombreCampaña, // ✅ Enviar nombre en lugar de ID
            idEstado: parseInt(document.getElementById('crearEstado').value)
        };

        console.log("📦 Datos a enviar:", usuarioData);

        let url, method;

        if (usuarioId) {
            // 🔁 Editar usuario existente
            url = `/api/usuarios/${usuarioId}`;
            method = 'PUT';
        } else {
            // 🆕 Crear nuevo usuario
            url = '/api/usuarios';
            method = 'POST';
        }

        console.log(`🚀 Enviando solicitud a ${url} con método ${method}`);

        const response = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(usuarioData)
        });

        if (response.ok) {
            alert(`✅ Usuario ${usuarioId ? 'actualizado' : 'creado'} correctamente.`);

            // Cerrar modal y limpiar formulario
            const modalElement = document.getElementById('modalCrearUsuario');
            const modalInstance = bootstrap.Modal.getInstance(modalElement);
            modalInstance.hide();

            window.limpiarFormularioUsuario();

            // Actualizar la lista de usuarios
            await actualizarListaUsuarios();

        } else {
            const error = await response.json();
            console.error("❌ Error del servidor:", error);
            alert('❌ Error: ' + (error.error || 'No se pudo guardar el usuario.'));
        }

    } catch (error) {
        console.error("❌ Error en la solicitud:", error);
        alert('❌ Error de red o servidor.');
    }
};

// Editar usuario - función principal
window.editarUsuario = async function (id) {
    try {
        console.log("🔄 Abriendo modal para editar usuario:", id);

        // Inicializar el modal primero
        window.inicializarModalEditarUsuario();

        // Obtener datos del usuario
        const response = await fetch(`/api/usuarios/${id}`);
        if (!response.ok) throw new Error("No se pudo obtener el usuario");

        const usuario = await response.json();
        console.log("📦 Datos del usuario recibidos:", usuario);

        // Llenar el formulario de edición
        document.getElementById('editarId').value = usuario.id;
        document.getElementById('editarNumeroDocumento').value = usuario.numeroDocumento || "";
        document.getElementById('editarNombres').value = usuario.nombres || "";
        document.getElementById('editarApellidos').value = usuario.apellidos || "";

        // Cargar selects para edición
        await cargarSelectsEditarUsuario();

        // Esperar un momento para que TomSelect se inicialice
        setTimeout(() => {
            // Establecer valores en los selects con TomSelect
            const selectIds = [
                { id: 'editarTipoDocumento', value: usuario.idTipoDocumento },
                { id: 'editarArea', value: usuario.idArea },
                { id: 'editarCampana', value: usuario.idCampaña },
                { id: 'editarEstado', value: usuario.idEstado }
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
                }
            });
        }, 500);

        // Mostrar el modal de edición
        const modalElement = document.getElementById('modalEditarUsuario');
        if (modalElement) {
            const modal = new bootstrap.Modal(modalElement);
            modal.show();
            console.log("✅ Modal de edición de usuario abierto");
        } else {
            console.error("❌ No se encontró el modal modalEditarUsuario");
        }

    } catch (error) {
        console.error("❌ Error completo en editarUsuario:", error);
        alert("❌ Error al cargar el usuario para edición.");
    }
};

// Inicialización mejorada para modal editar usuario
window.inicializarModalEditarUsuario = function () {
    console.log("🔄 Inicializando modal editar usuario...");

    const modalElement = document.getElementById('modalEditarUsuario');
    if (!modalElement) {
        console.error("❌ No se encontró el modal modalEditarUsuario");
        return;
    }

    // Remover event listeners anteriores
    modalElement.removeEventListener('hidden.bs.modal', window.manejarHideModalEditar);

    // Crear nuevo event listener
    window.manejarHideModalEditar = function () {
        console.log("🎯 Modal editar usuario cerrado - limpiando formulario...");
        window.limpiarFormularioEditarUsuario();
    };

    // Agregar event listener
    modalElement.addEventListener('hidden.bs.modal', window.manejarHideModalEditar);

    console.log("✅ Modal editar usuario inicializado correctamente");
};

// Cargar selects para formulario de edición
window.cargarSelectsEditarUsuario = async function () {
    try {
        const response = await fetch('/api/usuarios/form-data');
        if (!response.ok) throw new Error('Error al cargar datos del formulario');
        const data = await response.json();

        console.log("📊 Datos para formulario de edición de usuario:", data);

        const selects = [
            { id: 'editarTipoDocumento', list: data.tiposDocumento, value: 'id', text: item => item.nombreDocumento },
            { id: 'editarArea', list: data.areas, value: 'id', text: item => item.nombreArea },
            { id: 'editarCampana', list: data.campanas, value: 'id', text: item => item.nombreCampaña },
            { id: 'editarEstado', list: data.estados, value: 'id', text: item => item.nombreEstado }
        ];

        for (const { id, list, value, text } of selects) {
            const select = document.getElementById(id);
            if (!select) {
                console.warn(`⚠️ Select no encontrado: ${id}`);
                continue;
            }

            // Destruir TomSelect existente
            if (select.tomselect) {
                select.tomselect.destroy();
            }

            select.innerHTML = '<option value="">Seleccionar...</option>';

            requestAnimationFrame(() => {
                list.forEach(item => {
                    const option = document.createElement('option');
                    option.value = item[value];
                    option.textContent = text(item);
                    select.appendChild(option);
                });

                const tomselect = new TomSelect(select, {
                    placeholder: 'Seleccionar...',
                    maxOptions: 500,
                    allowEmptyOption: true,
                    sortField: { field: "text", direction: "asc" }
                });
            });
        }

        console.log("✅ Todos los selects de edición de usuario cargados");

    } catch (error) {
        console.error('❌ Error en cargarSelectsEditarUsuario:', error);
    }
};

// Guardar cambios de usuario editado
window.guardarCambiosUsuario = async function () {
    try {
        const usuarioId = document.getElementById('editarId').value;

        if (!usuarioId) {
            alert('❌ No se encontró el ID del usuario a editar.');
            return;
        }

        // Validar campos requeridos
        const camposRequeridos = [
            { id: 'editarTipoDocumento', nombre: 'Tipo de Documento' },
            { id: 'editarNumeroDocumento', nombre: 'Número de Documento' },
            { id: 'editarNombres', nombre: 'Nombres' },
            { id: 'editarApellidos', nombre: 'Apellidos' },
            { id: 'editarArea', nombre: 'Área' },
            { id: 'editarCampana', nombre: 'Campaña' },
            { id: 'editarEstado', nombre: 'Estado' }
        ];

        for (const campo of camposRequeridos) {
            const elemento = document.getElementById(campo.id);
            const valor = elemento.tomselect ? elemento.tomselect.getValue() : elemento.value;

            if (!valor || valor.toString().trim() === '') {
                alert(`❌ El campo "${campo.nombre}" es obligatorio`);
                if (elemento.focus) elemento.focus();
                return;
            }
        }

        // 🔥 OBTENER EL NOMBRE DE LA CAMPAÑA SELECCIONADA
        const campanaSelect = document.getElementById('editarCampana');
        const idCampaña = campanaSelect.value;
        const nombreCampaña = campanaSelect.tomselect
            ? campanaSelect.tomselect.options[idCampaña]?.text
            : campanaSelect.options[campanaSelect.selectedIndex]?.text;

        if (!nombreCampaña) {
            alert('❌ No se pudo obtener el nombre de la campaña seleccionada');
            return;
        }

        // Preparar datos del usuario
        const usuarioData = {
            idTipoDocumento: parseInt(document.getElementById('editarTipoDocumento').value),
            numeroDocumento: document.getElementById('editarNumeroDocumento').value.trim(),
            nombres: document.getElementById('editarNombres').value.trim(),
            apellidos: document.getElementById('editarApellidos').value.trim(),
            idArea: parseInt(document.getElementById('editarArea').value),
            nombreCampaña: nombreCampaña, // ✅ Enviar nombre en lugar de ID
            idEstado: parseInt(document.getElementById('editarEstado').value)
        };

        console.log("📦 Enviando datos de actualización:", usuarioData);

        const url = `/api/usuarios/${usuarioId}`;

        // Mostrar loading en el botón
        const btnGuardar = document.getElementById('btnGuardarEditarUsuario');
        const textoOriginal = btnGuardar.innerHTML;
        btnGuardar.innerHTML = '<i class="bi bi-arrow-repeat spinner"></i> Guardando...';
        btnGuardar.disabled = true;

        const response = await fetch(url, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(usuarioData)
        });

        if (response.ok) {
            alert('✅ Usuario actualizado correctamente.');

            // Cerrar el modal
            const modalElement = document.getElementById('modalEditarUsuario');
            const modalInstance = bootstrap.Modal.getInstance(modalElement);
            modalInstance.hide();

            // Actualizar la lista de usuarios
            await actualizarListaUsuarios();

        } else {
            const error = await response.json();
            console.error("❌ Error del servidor:", error);
            alert('❌ Error: ' + (error.error || 'No se pudo actualizar el usuario.'));
        }

    } catch (error) {
        console.error("❌ Error en la solicitud:", error);
        alert('❌ Error de red o servidor.');
    } finally {
        // Restaurar botón
        const btnGuardar = document.getElementById('btnGuardarEditarUsuario');
        if (btnGuardar) {
            btnGuardar.innerHTML = '💾 Guardar Cambios';
            btnGuardar.disabled = false;
        }
    }
};

// Limpiar formulario de edición
window.limpiarFormularioEditarUsuario = function () {
    console.log("🧹 Limpiando formulario de edición de usuario");

    document.getElementById('editarId').value = '';
    document.getElementById('editarNumeroDocumento').value = '';
    document.getElementById('editarNombres').value = '';
    document.getElementById('editarApellidos').value = '';

    // Limpiar selects con TomSelect
    const selects = ['editarTipoDocumento', 'editarArea', 'editarCampana', 'editarEstado'];
    selects.forEach(id => {
        const select = document.getElementById(id);
        if (select && select.tomselect) {
            select.tomselect.setValue('', true);
        }
    });
};

// Función para actualizar la lista de usuarios después de cambios
window.actualizarListaUsuarios = async function () {
    try {
        console.log("🔄 Actualizando lista de usuarios...");

        // Llamar al método .NET para refrescar la lista
        await DotNet.invokeMethodAsync('OUT_APP_EQUIPGO', 'RefrescarListaUsuarios');

        console.log("✅ Lista de usuarios actualizada");
    } catch (error) {
        console.error("❌ Error actualizando lista de usuarios:", error);
        // Fallback: recargar la página
        setTimeout(() => {
            window.location.reload();
        }, 1000);
    }
};

// ================================
// INICIALIZACIÓN GLOBAL
// ================================

// Solo inicializar elementos básicos al cargar la página
document.addEventListener('DOMContentLoaded', function () {
    console.log("🎯 usuarios.js cargado correctamente");

    // Debug para verificar que las funciones estén disponibles
    console.log("🔍 Funciones disponibles:", {
        abrirModalCrearUsuario: typeof window.abrirModalCrearUsuario,
        abrirModalCargaMasiva: typeof window.abrirModalCargaMasiva,
        editarUsuario: typeof window.editarUsuario,
        guardarUsuario: typeof window.guardarUsuario
    });
});