// Funciones para mostrar notificaciones
window.mostrarExito = function (mensaje) {
    if (typeof Swal !== 'undefined') {
        Swal.fire({
            icon: 'success',
            title: 'Éxito',
            text: mensaje,
            timer: 3000,
            showConfirmButton: false
        });
    } else {
        alert('✅ ' + mensaje);
    }
};

window.mostrarError = function (mensaje) {
    if (typeof Swal !== 'undefined') {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: mensaje,
            timer: 5000,
            showConfirmButton: true
        });
    } else {
        alert('❌ ' + mensaje);
    }
};

// Funciones para confirmaciones
window.mostrarConfirmacion = function (mensaje) {
    if (typeof Swal !== 'undefined') {
        return Swal.fire({
            title: '¿Está seguro?',
            text: mensaje,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            return result.isConfirmed;
        });
    } else {
        return confirm(mensaje);
    }
};

// Funciones para cerrar modales
window.cerrarModalUsuarioSession = function () {
    const modal = bootstrap.Modal.getInstance(document.getElementById('modalCrearUsuarioSession'));
    if (modal) {
        modal.hide();
    }
    // Limpiar el backdrop si es necesario
    const backdrops = document.querySelectorAll('.modal-backdrop');
    backdrops.forEach(backdrop => {
        backdrop.remove();
    });
    document.body.classList.remove('modal-open');
    document.body.style.overflow = '';
    document.body.style.paddingRight = '';
};

window.cerrarModalCambioContraseña = function () {
    const modal = bootstrap.Modal.getInstance(document.getElementById('modalCambiarContraseña'));
    if (modal) {
        modal.hide();
    }
    // Limpiar el backdrop si es necesario
    const backdrops = document.querySelectorAll('.modal-backdrop');
    backdrops.forEach(backdrop => {
        backdrop.remove();
    });
    document.body.classList.remove('modal-open');
    document.body.style.overflow = '';
    document.body.style.paddingRight = '';
};

// Función para inicializar componentes JavaScript
window.inicializarUsuariosSession = function () {
    console.log("🔄 Inicializando componentes JavaScript para Usuarios Session...");

    // Inicializar tooltips de Bootstrap
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    console.log("✅ Componentes JavaScript para Usuarios Session inicializados");
};

// Función para validar formularios
window.validarFormularioUsuarioSession = function () {
    const formulario = document.getElementById('formUsuarioSession');
    if (!formulario) return true;

    const inputsRequeridos = formulario.querySelectorAll('[required]');
    let valido = true;

    inputsRequeridos.forEach(input => {
        if (!input.value.trim()) {
            input.classList.add('is-invalid');
            valido = false;
        } else {
            input.classList.remove('is-invalid');
        }
    });

    return valido;
};

// Función para limpiar formularios
window.limpiarFormularioUsuarioSession = function () {
    const formulario = document.getElementById('formUsuarioSession');
    if (formulario) {
        formulario.reset();
        const inputs = formulario.querySelectorAll('input, select');
        inputs.forEach(input => {
            input.classList.remove('is-invalid');
        });
    }
};

// Función para formatear números de documento
window.formatearNumeroDocumento = function (numeroDocumento) {
    if (!numeroDocumento) return '';

    // Eliminar cualquier caracter que no sea número
    const soloNumeros = numeroDocumento.replace(/\D/g, '');

    // Formatear según la longitud
    if (soloNumeros.length <= 8) {
        return soloNumeros;
    } else if (soloNumeros.length <= 10) {
        return soloNumeros.replace(/(\d{1,3})(\d{1,3})(\d{1,4})/, '$1.$2.$3');
    } else {
        return soloNumeros.replace(/(\d{1,3})(\d{1,3})(\d{1,3})(\d{1,3})/, '$1.$2.$3.$4');
    }
};

// Función para mostrar loading en botones
window.mostrarLoadingBoton = function (botonId, texto = 'Procesando...') {
    const boton = document.getElementById(botonId);
    if (boton) {
        const textoOriginal = boton.innerHTML;
        boton.innerHTML = `<i class="bi bi-arrow-repeat spinner"></i> ${texto}`;
        boton.disabled = true;
        boton.setAttribute('data-texto-original', textoOriginal);
    }
};

window.ocultarLoadingBoton = function (botonId) {
    const boton = document.getElementById(botonId);
    if (boton) {
        const textoOriginal = boton.getAttribute('data-texto-original');
        if (textoOriginal) {
            boton.innerHTML = textoOriginal;
        }
        boton.disabled = false;
    }
};

// Función para copiar al portapapeles
window.copiarAlPortapapeles = function (texto) {
    if (navigator.clipboard && window.isSecureContext) {
        return navigator.clipboard.writeText(texto).then(() => {
            window.mostrarExito('Copiado al portapapeles');
            return true;
        }).catch(err => {
            console.error('Error al copiar: ', err);
            return false;
        });
    } else {
        // Fallback para navegadores más antiguos
        const textArea = document.createElement("textarea");
        textArea.value = texto;
        textArea.style.position = "fixed";
        textArea.style.left = "-999999px";
        textArea.style.top = "-999999px";
        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();
        try {
            document.execCommand('copy');
            window.mostrarExito('Copiado al portapapeles');
            return true;
        } catch (err) {
            console.error('Error al copiar: ', err);
            return false;
        } finally {
            textArea.remove();
        }
    }
};

// Inicializar cuando el documento esté listo
document.addEventListener('DOMContentLoaded', function () {
    console.log("📄 DOM cargado - Inicializando Usuarios Session");
    window.inicializarUsuariosSession();
});

// Manejar eventos de teclado para mejor UX
document.addEventListener('keydown', function (e) {
    // Cerrar modal con ESC
    if (e.key === 'Escape') {
        const modalesAbiertos = document.querySelectorAll('.modal.show');
        modalesAbiertos.forEach(modal => {
            const modalInstance = bootstrap.Modal.getInstance(modal);
            if (modalInstance) {
                modalInstance.hide();
            }
        });
    }

    // Enviar formulario con Ctrl+Enter
    if (e.ctrlKey && e.key === 'Enter') {
        const modalActivo = document.querySelector('.modal.show');
        if (modalActivo) {
            const botonGuardar = modalActivo.querySelector('.boton-modal-equipgo:not(.boton-modal-secundario)');
            if (botonGuardar && !botonGuardar.disabled) {
                botonGuardar.click();
            }
        }
    }
});

// Exportar funciones para uso global
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        mostrarExito: window.mostrarExito,
        mostrarError: window.mostrarError,
        mostrarConfirmacion: window.mostrarConfirmacion,
        cerrarModalUsuarioSession: window.cerrarModalUsuarioSession,
        cerrarModalCambioContraseña: window.cerrarModalCambioContraseña,
        inicializarUsuariosSession: window.inicializarUsuariosSession,
        validarFormularioUsuarioSession: window.validarFormularioUsuarioSession,
        limpiarFormularioUsuarioSession: window.limpiarFormularioUsuarioSession,
        formatearNumeroDocumento: window.formatearNumeroDocumento,
        mostrarLoadingBoton: window.mostrarLoadingBoton,
        ocultarLoadingBoton: window.ocultarLoadingBoton,
        copiarAlPortapapeles: window.copiarAlPortapapeles
    };
}