// ================================
// FUNCIONES PARA TRANSACCIONES
// ================================

// Variables globales para transacciones
window.transaccionesFiltradas = [];
window.paginaActualTransacciones = 1;
window.registrosPorPaginaTransacciones = 10;

// ================================
// FUNCIONES PARA DESCARGAR EXCEL CON FILTROS
// ================================

// Descargar Excel de transacciones normales con filtros aplicados
window.descargarExcelTransaccionesConFiltros = async function (filtrosDesdeBlazor) {
    try {
        console.log("📊 Descargando Excel de transacciones con filtros aplicados...");

        // Mostrar loading
        let boton = event?.target;
        let textoOriginal = '';

        if (boton) {
            textoOriginal = boton.innerHTML;
            boton.innerHTML = '<i class="bi bi-arrow-repeat spinner"></i> Generando Excel...';
            boton.disabled = true;
        }

        // Obtener filtros actuales - usar los que vienen de Blazor
        let filtros = filtrosDesdeBlazor;

        if (!filtros) {
            // Fallback: intentar obtener desde los inputs
            filtros = window.obtenerFiltrosTransaccionesActuales();
        }

        console.log("🔍 Filtros aplicados para Excel:", filtros);

        // Validar que tengamos filtros
        if (!filtros) {
            throw new Error('No se pudieron obtener los filtros aplicados');
        }

        // CORREGIDO: Crear objeto de filtros con estructura explícita
        const filtrosParaEnviar = {
            codigoBarras: filtros.codigoBarras || '',
            usuario: filtros.usuario || '',
            tipoTransaccion: filtros.tipoTransaccion || '',
            sede: filtros.sede || '',
            fecha: filtros.fecha || '' // Asegurar que se incluya la fecha
        };

        console.log("📤 Filtros que se enviarán al servidor:", filtrosParaEnviar);

        // Llamar al endpoint para obtener datos filtrados
        const response = await fetch('/api/transacciones/descargar-excel-transacciones', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(filtrosParaEnviar)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || 'Error al generar el reporte');
        }

        const result = await response.json();

        if (!result.success) {
            throw new Error(result.message || 'Error en la respuesta del servidor');
        }

        const transacciones = result.data;

        console.log(`✅ Obtenidas ${transacciones.length} transacciones para Excel`);

        // Generar Excel con los datos
        await window.generarExcelTransacciones(transacciones, filtrosParaEnviar);

        // Mostrar mensaje de éxito
        if (typeof bootstrap !== 'undefined') {
            window.mostrarToast('Éxito', `Excel generado correctamente (${transacciones.length} registros)`, 'success');
        } else {
            alert(`✅ Excel generado correctamente (${transacciones.length} registros)`);
        }

    } catch (error) {
        console.error("❌ Error descargando Excel con filtros:", error);

        // Mostrar error
        if (typeof bootstrap !== 'undefined') {
            window.mostrarToast('Error', 'Error al generar el Excel: ' + error.message, 'danger');
        } else {
            alert('❌ Error al generar el Excel: ' + error.message);
        }
    } finally {
        // Restaurar botón
        if (event?.target) {
            event.target.innerHTML = '<i class="bi bi-file-earmark-excel"></i> Descargar Excel';
            event.target.disabled = false;
        }
    }
};

// Descargar Excel de transacciones visitantes con filtros aplicados
window.descargarExcelTransaccionesVisitantesConFiltros = async function (filtrosDesdeBlazor) {
    try {
        console.log("📊 Descargando Excel de transacciones visitantes con filtros aplicados...");

        // Mostrar loading
        let boton = event?.target;
        let textoOriginal = '';

        if (boton) {
            textoOriginal = boton.innerHTML;
            boton.innerHTML = '<i class="bi bi-arrow-repeat spinner"></i> Generando Excel...';
            boton.disabled = true;
        }

        // Obtener filtros actuales - usar los que vienen de Blazor
        let filtros = filtrosDesdeBlazor;

        if (!filtros) {
            // Fallback: intentar obtener desde los inputs
            filtros = window.obtenerFiltrosTransaccionesVisitantesActuales();
        }

        console.log("🔍 Filtros aplicados para Excel visitantes:", filtros);

        // Validar que tengamos filtros
        if (!filtros) {
            throw new Error('No se pudieron obtener los filtros aplicados');
        }

        // CORREGIDO: Crear objeto de filtros con estructura explícita
        const filtrosParaEnviar = {
            visitante: filtros.visitante || '',
            aprobador: filtros.aprobador || '',
            tipoTransaccion: filtros.tipoTransaccion || '',
            sede: filtros.sede || '',
            fecha: filtros.fecha || '' // Asegurar que se incluya la fecha
        };

        console.log("📤 Filtros que se enviarán al servidor (visitantes):", filtrosParaEnviar);

        // Llamar al endpoint para obtener datos filtrados
        const response = await fetch('/api/transacciones/descargar-excel-transacciones-visitantes', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(filtrosParaEnviar)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || 'Error al generar el reporte');
        }

        const result = await response.json();

        if (!result.success) {
            throw new Error(result.message || 'Error en la respuesta del servidor');
        }

        const transacciones = result.data;

        console.log(`✅ Obtenidas ${transacciones.length} transacciones visitantes para Excel`);

        // Generar Excel con los datos
        await window.generarExcelTransaccionesVisitantes(transacciones, filtrosParaEnviar);

        // Mostrar mensaje de éxito
        if (typeof bootstrap !== 'undefined') {
            window.mostrarToast('Éxito', `Excel generado correctamente (${transacciones.length} registros)`, 'success');
        } else {
            alert(`✅ Excel generado correctamente (${transacciones.length} registros)`);
        }

    } catch (error) {
        console.error("❌ Error descargando Excel visitantes con filtros:", error);

        if (typeof bootstrap !== 'undefined') {
            window.mostrarToast('Error', 'Error al generar el Excel: ' + error.message, 'danger');
        } else {
            alert('❌ Error al generar el Excel: ' + error.message);
        }
    } finally {
        // Restaurar botón
        if (event?.target) {
            event.target.innerHTML = '<i class="bi bi-file-earmark-excel"></i> Descargar Excel';
            event.target.disabled = false;
        }
    }
};

// Generar archivo Excel para transacciones normales
window.generarExcelTransacciones = function (transacciones, filtros) {
    try {
        // Crear workbook
        const wb = XLSX.utils.book_new();

        // Preparar datos para Excel
        const datosExcel = transacciones.map(t => ({
            'Código Barras': t.codigoBarras || '',
            'Usuario': t.nombreUsuarioInfo || '',
            'Tipo Transacción': t.nombreTipoTransaccion || '',
            'Equipo Personal': t.nombreEquipoPersonal || '',
            'Usuario Sesión': t.nombreUsuarioSession || '',
            'Sede': t.nombreSedeOs || '',
            'Fecha/Hora': t.fechaHora ? new Date(t.fechaHora).toLocaleString() : ''
        }));

        // Crear worksheet
        const ws = XLSX.utils.json_to_sheet(datosExcel);

        // Agregar worksheet al workbook
        XLSX.utils.book_append_sheet(wb, ws, 'Transacciones');

        // CORREGIDO: Solo un campo de fecha en los metadatos
        const metadatos = [
            ['Reporte de Transacciones'],
            ['Generado el', new Date().toLocaleString()],
            ['Total de registros', transacciones.length],
            [''],
            ['Filtros Aplicados:'],
            ['Código Barras', filtros.codigoBarras || 'Todos'],
            ['Usuario', filtros.usuario || 'Todos'],
            ['Tipo Transacción', filtros.tipoTransaccion || 'Todos'],
            ['Sede', filtros.sede || 'Todas'],
            ['Fecha', filtros.fecha || 'Todas'] // CORREGIDO: solo un campo
        ];

        const wsMetadatos = XLSX.utils.aoa_to_sheet(metadatos);
        XLSX.utils.book_append_sheet(wb, wsMetadatos, 'Metadatos');

        // Generar nombre del archivo
        const fecha = new Date().toISOString().slice(0, 16).replace(/[-:T]/g, '');
        const nombreArchivo = `Transacciones_Filtradas_${fecha}.xlsx`;

        // Descargar archivo
        XLSX.writeFile(wb, nombreArchivo);

        console.log("✅ Excel de transacciones con filtros generado correctamente");

    } catch (error) {
        console.error("❌ Error generando Excel transacciones:", error);
        throw error;
    }
};

// Generar archivo Excel para transacciones visitantes
window.generarExcelTransaccionesVisitantes = function (transacciones, filtros) {
    try {
        // Crear workbook
        const wb = XLSX.utils.book_new();

        // Preparar datos para Excel
        const datosExcel = transacciones.map(t => ({
            'Visitante': t.nombresVisitante || '',
            'Marca Equipo': t.marcaEquipo || '',
            'Aprobador': t.nombreAprobador || '',
            'Sede': t.nombreSede || '',
            'Tipo Transacción': t.tipoTransaccion || '',
            'Fecha Transacción': t.fechaTransaccion ? new Date(t.fechaTransaccion).toLocaleString() : ''
        }));

        // Crear worksheet
        const ws = XLSX.utils.json_to_sheet(datosExcel);

        // Agregar worksheet al workbook
        XLSX.utils.book_append_sheet(wb, ws, 'TransaccionesVisitantes');

        // CORREGIDO: Solo un campo de fecha en los metadatos
        const metadatos = [
            ['Reporte de Transacciones de Visitantes'],
            ['Generado el', new Date().toLocaleString()],
            ['Total de registros', transacciones.length],
            [''],
            ['Filtros Aplicados:'],
            ['Visitante', filtros.visitante || 'Todos'],
            ['Aprobador', filtros.aprobador || 'Todos'],
            ['Tipo Transacción', filtros.tipoTransaccion || 'Todos'],
            ['Sede', filtros.sede || 'Todas'],
            ['Fecha', filtros.fecha || 'Todas'] // CORREGIDO: solo un campo
        ];

        const wsMetadatos = XLSX.utils.aoa_to_sheet(metadatos);
        XLSX.utils.book_append_sheet(wb, wsMetadatos, 'Metadatos');

        // Generar nombre del archivo
        const fecha = new Date().toISOString().slice(0, 16).replace(/[-:T]/g, '');
        const nombreArchivo = `Transacciones_Visitantes_Filtradas_${fecha}.xlsx`;

        // Descargar archivo
        XLSX.writeFile(wb, nombreArchivo);

        console.log("✅ Excel de transacciones visitantes con filtros generado correctamente");

    } catch (error) {
        console.error("❌ Error generando Excel transacciones visitantes:", error);
        throw error;
    }
};

// ================================
// FUNCIONES PARA FILTRADO
// ================================

// Obtener filtros actuales de transacciones normales
window.obtenerFiltrosTransaccionesActuales = function () {
    try {
        const filtros = {};

        // Obtener valores de los inputs de filtro
        const inputs = document.querySelectorAll('#transaccionesFiltros input[type="text"], #transaccionesFiltros input[type="date"]');
        inputs.forEach(input => {
            const filtroId = input.getAttribute('data-filtro');
            if (filtroId) {
                filtros[filtroId] = input.value || '';
            }
        });

        // CORREGIDO: Asegurar que el objeto tenga la estructura correcta
        return {
            codigoBarras: filtros.codigoBarras || '',
            usuario: filtros.usuario || '',
            tipoTransaccion: filtros.tipoTransaccion || '',
            sede: filtros.sede || '',
            fecha: filtros.fecha || ''
        };

    } catch (error) {
        console.error("❌ Error obteniendo filtros transacciones:", error);
        return {
            codigoBarras: '',
            usuario: '',
            tipoTransaccion: '',
            sede: '',
            fecha: ''
        };
    }
};

// Obtener filtros actuales de transacciones visitantes
window.obtenerFiltrosTransaccionesVisitantesActuales = function () {
    try {
        const filtros = {};

        // Obtener valores de los inputs de filtro
        const inputs = document.querySelectorAll('#transaccionesVisitantesFiltros input[type="text"], #transaccionesVisitantesFiltros input[type="date"]');
        inputs.forEach(input => {
            const filtroId = input.getAttribute('data-filtro');
            if (filtroId) {
                filtros[filtroId] = input.value || '';
            }
        });

        // CORREGIDO: Asegurar que el objeto tenga la estructura correcta
        return {
            visitante: filtros.visitante || '',
            aprobador: filtros.aprobador || '',
            tipoTransaccion: filtros.tipoTransaccion || '',
            sede: filtros.sede || '',
            fecha: filtros.fecha || ''
        };

    } catch (error) {
        console.error("❌ Error obteniendo filtros transacciones visitantes:", error);
        return {
            visitante: '',
            aprobador: '',
            tipoTransaccion: '',
            sede: '',
            fecha: ''
        };
    }
};

// ================================
// FUNCIONES DE UTILIDAD
// ================================

// Función auxiliar para mostrar toasts de Bootstrap
window.mostrarToast = function (titulo, mensaje, tipo = 'info') {
    try {
        // Crear toast dinámicamente
        const toastContainer = document.getElementById('toastContainer') || crearToastContainer();

        const toastId = 'toast-' + Date.now();
        const toastHtml = `
            <div id="${toastId}" class="toast align-items-center text-bg-${tipo} border-0" role="alert">
                <div class="d-flex">
                    <div class="toast-body">
                        <strong>${titulo}:</strong> ${mensaje}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>
        `;

        toastContainer.insertAdjacentHTML('beforeend', toastHtml);

        const toastElement = document.getElementById(toastId);
        const toast = new bootstrap.Toast(toastElement);
        toast.show();

        // Remover el toast del DOM después de que se oculte
        toastElement.addEventListener('hidden.bs.toast', function () {
            toastElement.remove();
        });

    } catch (error) {
        console.error("Error mostrando toast:", error);
        // Fallback a alert normal
        alert(`${titulo}: ${mensaje}`);
    }
};

// Crear contenedor de toasts si no existe
function crearToastContainer() {
    const container = document.createElement('div');
    container.id = 'toastContainer';
    container.className = 'toast-container position-fixed top-0 end-0 p-3';
    container.style.zIndex = '9999';
    document.body.appendChild(container);
    return container;
}

// Formatear fecha para mostrar
window.formatearFecha = function (fechaString) {
    try {
        if (!fechaString) return '';

        const fecha = new Date(fechaString);
        return fecha.toLocaleDateString('es-ES', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit'
        });

    } catch (error) {
        console.error("❌ Error formateando fecha:", error);
        return fechaString;
    }
};

// Mostrar u ocultar loading
window.mostrarLoadingTransacciones = function (mostrar) {
    try {
        const loadingElement = document.getElementById('loadingTransacciones');
        if (loadingElement) {
            loadingElement.style.display = mostrar ? 'flex' : 'none';
        }
    } catch (error) {
        console.error("❌ Error mostrando loading:", error);
    }
};

// ================================
// INICIALIZACIÓN GLOBAL
// ================================

// Inicialización cuando se carga el DOM
document.addEventListener('DOMContentLoaded', function () {
    console.log("🎯 transacciones.js cargado correctamente");

    // Debug para verificar que las funciones estén disponibles
    console.log("🔍 Funciones transacciones disponibles:", {
        descargarExcelTransaccionesConFiltros: typeof window.descargarExcelTransaccionesConFiltros,
        descargarExcelTransaccionesVisitantesConFiltros: typeof window.descargarExcelTransaccionesVisitantesConFiltros,
        generarExcelTransacciones: typeof window.generarExcelTransacciones,
        generarExcelTransaccionesVisitantes: typeof window.generarExcelTransaccionesVisitantes
    });

    // Inicializar tooltips de Bootstrap si están disponibles
    if (typeof bootstrap !== 'undefined') {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
});

// Función para limpiar recursos cuando se cambia de página
window.limpiarTransacciones = function () {
    try {
        console.log("🧹 Limpiando recursos de transacciones...");

        // Limpiar timeouts
        if (window.filtroTimeoutTransacciones) {
            clearTimeout(window.filtroTimeoutTransacciones);
        }
        if (window.filtroTimeoutTransaccionesVisitantes) {
            clearTimeout(window.filtroTimeoutTransaccionesVisitantes);
        }

        // Limpiar referencias .NET
        window.transaccionesDotNetRef = null;
        window.transaccionesVisitantesDotNetRef = null;

        console.log("✅ Recursos de transacciones limpiados");

    } catch (error) {
        console.error("❌ Error limpiando recursos transacciones:", error);
    }
};