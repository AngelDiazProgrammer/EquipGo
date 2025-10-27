// sistema-paginacion.js - VERSIÓN COMPLETA
class PaginacionAutomatica {
    constructor() {
        this.tablas = new Map();
        this.observer = null;
        this.recalculateTimeout = null;
        this.init();
    }

    init() {
        this.inicializarTablasExistentes();
        this.configurarMutationObserver();

        window.addEventListener('resize', () => this.recalcularTodasLasTablas());
        window.addEventListener('orientationchange', () => setTimeout(() => this.recalcularTodasLasTablas(), 100));

        console.log('✅ Sistema de paginación automática inicializado');
    }

    inicializarTablasExistentes() {
        document.querySelectorAll('.table-equipgo').forEach(tabla => {
            this.agregarTabla(tabla);
        });
    }

    configurarMutationObserver() {
        this.observer = new MutationObserver((mutations) => {
            mutations.forEach((mutation) => {
                mutation.addedNodes.forEach((node) => {
                    if (node.nodeType === 1) {
                        if (node.classList && node.classList.contains('table-equipgo')) {
                            this.agregarTabla(node);
                        } else {
                            node.querySelectorAll?.('.table-equipgo').forEach(tabla => {
                                this.agregarTabla(tabla);
                            });
                        }
                    }
                });
            });
        });

        this.observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }

    agregarTabla(tabla) {
        const wrapper = tabla.closest('.table-wrapper');

        // BUSCAR CUALQUIER CONTENEDOR PADRE CON ALTURA DEFINIDA
        let frameContainer = tabla.closest('.frame-container');

        // Si no encuentra frame-container, buscar cualquier contenedor con altura
        if (!frameContainer) {
            frameContainer = this.buscarContenedorPadre(tabla);
        }

        if (!wrapper || this.tablas.has(wrapper)) return;

        const id = 'tabla-' + Date.now() + '-' + Math.random().toString(36).substr(2, 9);
        this.tablas.set(wrapper, {
            id,
            tabla,
            wrapper,
            frameContainer, // Puede ser null, pero el sistema lo manejará
            datos: [],
            paginaActual: 1,
            registrosPorPagina: 10,
            totalPaginas: 1,
            thead: null
        });

        console.log('📊 Tabla detectada:', id, 'Contenedor:', frameContainer ? 'Encontrado' : 'Usando fallback');
        this.configurarTabla(wrapper);
    }

    // NUEVO MÉTODO PARA BUSCAR CONTENEDOR PADRE
    buscarContenedorPadre(elemento) {
        let padre = elemento.parentElement;
        let profundidad = 0;

        // Buscar hasta 5 niveles hacia arriba
        while (padre && profundidad < 5) {
            const estilo = window.getComputedStyle(padre);
            const altura = parseFloat(estilo.height);
            const display = estilo.display;

            // Si el padre tiene altura definida y es un contenedor flex/grid/block
            if (altura > 0 && altura !== Infinity &&
                (display.includes('flex') || display.includes('grid') || display === 'block')) {
                console.log('🎯 Contenedor padre encontrado:', padre, 'Altura:', altura);
                return padre;
            }

            padre = padre.parentElement;
            profundidad++;
        }

        console.log('🔍 No se encontró contenedor padre con altura definida, usando body');
        return document.body; // Fallback al body
    }

    configurarTabla(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        // Agregar clases necesarias
        wrapper.classList.add('with-pagination');

        // Crear contenedor de paginación
        this.crearPaginador(wrapper);

        // Extraer datos de la tabla
        this.extraerDatos(wrapper);

        // Calcular y aplicar paginación después de un breve delay
        setTimeout(() => {
            this.calcularYActualizarPaginacion(wrapper);
        }, 150);
    }

    crearPaginador(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config || wrapper.querySelector('.pagination-container')) return;

        const paginationContainer = document.createElement('div');
        paginationContainer.className = 'pagination-container';
        paginationContainer.innerHTML = `
            <div class="pagination-info" id="${config.id}-info">
                Mostrando 0 de 0 registros
            </div>
            <div class="pagination-controls">
                <button class="pagination-btn" id="${config.id}-prev" disabled>
                    ← Anterior
                </button>
                <div class="pagination-pages" id="${config.id}-pages"></div>
                <button class="pagination-btn" id="${config.id}-next" disabled>
                    Siguiente →
                </button>
            </div>
        `;

        wrapper.appendChild(paginationContainer);

        // Event listeners
        document.getElementById(`${config.id}-prev`).addEventListener('click', () => {
            this.cambiarPagina(wrapper, config.paginaActual - 1);
        });

        document.getElementById(`${config.id}-next`).addEventListener('click', () => {
            this.cambiarPagina(wrapper, config.paginaActual + 1);
        });

        console.log('🔧 Paginador creado para:', config.id);
    }

    extraerDatos(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        const tbody = config.tabla.querySelector('tbody');
        const thead = config.tabla.querySelector('thead');

        if (!tbody || !thead) return;

        // Guardar HTML original de las filas
        config.datos = Array.from(tbody.querySelectorAll('tr')).map(tr => tr.outerHTML);
        config.thead = thead.outerHTML;

        console.log('📋 Datos extraídos:', config.datos.length, 'registros');
    }

    calcularAlturaDisponible(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config) {
            console.warn('⚠️ No se pudo encontrar configuración de tabla');
            return 400;
        }

        try {
            // USAR EL CONTENEDOR ENCONTRADO O EL BODY COMO FALLBACK
            const contenedor = config.frameContainer || document.body;
            const esBody = contenedor === document.body;

            // 1. Altura total del contenedor
            const alturaContenedor = esBody ? window.innerHeight : contenedor.offsetHeight;
            const estiloContenedor = window.getComputedStyle(contenedor);
            const paddingTop = parseFloat(estiloContenedor.paddingTop) || 0;
            const paddingBottom = parseFloat(estiloContenedor.paddingBottom) || 0;

            const alturaNetaContenedor = alturaContenedor - paddingTop - paddingBottom;

            // 2. Calcular espacio ocupado por elementos anteriores
            let alturaPrevia = 0;

            if (!esBody) {
                const elementosContenedor = Array.from(contenedor.children);
                const indexWrapper = elementosContenedor.indexOf(config.wrapper);

                if (indexWrapper > 0) {
                    for (let i = 0; i < indexWrapper; i++) {
                        const elemento = elementosContenedor[i];
                        const estilo = window.getComputedStyle(elemento);
                        alturaPrevia += elemento.offsetHeight +
                            (parseFloat(estilo.marginTop) || 0) +
                            (parseFloat(estilo.marginBottom) || 0);
                    }
                }
            } else {
                // Si usamos body, calcular altura de elementos antes del wrapper
                let elementoAnterior = config.wrapper.previousElementSibling;
                while (elementoAnterior) {
                    const estilo = window.getComputedStyle(elementoAnterior);
                    alturaPrevia += elementoAnterior.offsetHeight +
                        (parseFloat(estilo.marginTop) || 0) +
                        (parseFloat(estilo.marginBottom) || 0);
                    elementoAnterior = elementoAnterior.previousElementSibling;
                }
            }

            // 3. Altura del paginador (medida real)
            const paginador = wrapper.querySelector('.pagination-container');
            const alturaPaginador = paginador ? paginador.offsetHeight : 60;

            // 4. Margen solicitado (10-15px)
            const margen = 15;

            // 5. Cálculo final
            const alturaDisponible = alturaNetaContenedor - alturaPrevia - alturaPaginador - margen;

            console.log('📐 Cálculo de altura para:', config.id, {
                contenedor: esBody ? 'body' : 'personalizado',
                alturaContenedor,
                alturaNetaContenedor,
                alturaPrevia,
                alturaPaginador,
                margen,
                alturaDisponible
            });

            return Math.max(150, alturaDisponible); // Mínimo 150px

        } catch (error) {
            console.error('❌ Error en cálculo de altura:', error);
            return 400;
        }
    }

    calcularRegistrosPorPagina(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config || config.datos.length === 0) {
            return 5; // Default conservador
        }

        try {
            const alturaDisponible = this.calcularAlturaDisponible(wrapper);

            // Crear elemento temporal para medir alturas
            const tempContainer = document.createElement('div');
            tempContainer.style.position = 'absolute';
            tempContainer.style.left = '-9999px';
            tempContainer.style.visibility = 'hidden';
            tempContainer.innerHTML = config.thead + '<tbody>' + config.datos[0] + '</tbody>';

            document.body.appendChild(tempContainer);

            const tempTable = tempContainer.querySelector('table');
            tempTable.classList.add('table-equipgo');
            tempTable.style.width = '100%';

            // Medir altura del thead
            const thead = tempTable.querySelector('thead');
            const alturaThead = thead ? thead.offsetHeight : 50;

            // Medir altura de una fila
            const fila = tempTable.querySelector('tbody tr');
            const alturaFila = fila ? fila.offsetHeight : 40;

            document.body.removeChild(tempContainer);

            // Calcular registros que caben
            const alturaUtil = alturaDisponible - alturaThead - 10; // 10px buffer interno
            const registrosPorPagina = Math.max(1, Math.floor(alturaUtil / alturaFila));

            console.log('🔢 Cálculo de registros por página:', {
                alturaDisponible,
                alturaThead,
                alturaFila,
                alturaUtil,
                registrosPorPagina
            });

            return registrosPorPagina;

        } catch (error) {
            console.error('❌ Error en cálculo de registros:', error);
            return 5;
        }
    }

    calcularYActualizarPaginacion(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config || config.datos.length === 0) return;

        try {
            config.registrosPorPagina = this.calcularRegistrosPorPagina(wrapper);
            config.totalPaginas = Math.ceil(config.datos.length / config.registrosPorPagina);

            // Asegurar que la página actual sea válida
            config.paginaActual = Math.min(config.paginaActual, config.totalPaginas);
            config.paginaActual = Math.max(1, config.paginaActual);

            console.log('🔄 Aplicando paginación:', {
                registrosPorPagina: config.registrosPorPagina,
                totalPaginas: config.totalPaginas,
                paginaActual: config.paginaActual,
                totalRegistros: config.datos.length
            });

            this.aplicarPaginacion(wrapper);

        } catch (error) {
            console.error('❌ Error en actualización de paginación:', error);
        }
    }

    aplicarPaginacion(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        const inicio = (config.paginaActual - 1) * config.registrosPorPagina;
        const fin = inicio + config.registrosPorPagina;
        const datosPagina = config.datos.slice(inicio, fin);

        // Reconstruir tabla
        const tbody = config.tabla.querySelector('tbody');
        if (tbody) {
            tbody.innerHTML = datosPagina.join('');
        }

        // Actualizar controles de paginación
        this.actualizarControlesPaginacion(wrapper);

        // Actualizar información
        this.actualizarInfoPaginacion(wrapper, inicio, fin);
    }

    actualizarInfoPaginacion(wrapper, inicio, fin) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        const infoElement = document.getElementById(`${config.id}-info`);
        if (infoElement) {
            const totalRegistros = config.datos.length;
            const mostrandoInicio = totalRegistros > 0 ? inicio + 1 : 0;
            const mostrandoFin = Math.min(fin, totalRegistros);
            infoElement.textContent = `Mostrando ${mostrandoInicio}-${mostrandoFin} de ${totalRegistros} registros`;
        }
    }

    actualizarControlesPaginacion(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        const prevBtn = document.getElementById(`${config.id}-prev`);
        const nextBtn = document.getElementById(`${config.id}-next`);
        const pagesContainer = document.getElementById(`${config.id}-pages`);

        if (!prevBtn || !nextBtn || !pagesContainer) return;

        // Botones anterior/siguiente
        prevBtn.disabled = config.paginaActual === 1;
        nextBtn.disabled = config.paginaActual === config.totalPaginas;

        // Números de página
        pagesContainer.innerHTML = '';
        const paginas = this.generarNumerosPagina(config.paginaActual, config.totalPaginas);

        paginas.forEach(pagina => {
            if (pagina === '...') {
                const ellipsis = document.createElement('span');
                ellipsis.className = 'pagination-ellipsis';
                ellipsis.textContent = '...';
                pagesContainer.appendChild(ellipsis);
            } else {
                const pageBtn = document.createElement('button');
                pageBtn.className = `page-number ${pagina === config.paginaActual ? 'active' : ''}`;
                pageBtn.textContent = pagina;
                pageBtn.addEventListener('click', () => {
                    this.cambiarPagina(wrapper, pagina);
                });
                pagesContainer.appendChild(pageBtn);
            }
        });
    }

    generarNumerosPagina(paginaActual, totalPaginas) {
        const paginas = [];
        const maxPaginasVisibles = 5;

        if (totalPaginas <= maxPaginasVisibles) {
            // Mostrar todas las páginas
            for (let i = 1; i <= totalPaginas; i++) {
                paginas.push(i);
            }
        } else {
            if (paginaActual <= 3) {
                // Al inicio: 1, 2, 3, 4, ..., última
                for (let i = 1; i <= 4; i++) paginas.push(i);
                paginas.push('...');
                paginas.push(totalPaginas);
            } else if (paginaActual >= totalPaginas - 2) {
                // Al final: 1, ..., últimas 4 páginas
                paginas.push(1);
                paginas.push('...');
                for (let i = totalPaginas - 3; i <= totalPaginas; i++) paginas.push(i);
            } else {
                // En medio: 1, ..., actual-1, actual, actual+1, ..., última
                paginas.push(1);
                paginas.push('...');
                for (let i = paginaActual - 1; i <= paginaActual + 1; i++) paginas.push(i);
                paginas.push('...');
                paginas.push(totalPaginas);
            }
        }

        return paginas;
    }

    cambiarPagina(wrapper, nuevaPagina) {
        const config = this.tablas.get(wrapper);
        if (!config || nuevaPagina < 1 || nuevaPagina > config.totalPaginas) return;

        console.log('📄 Cambiando a página:', nuevaPagina);
        config.paginaActual = nuevaPagina;
        this.aplicarPaginacion(wrapper);
    }

    recalcularTodasLasTablas() {
        clearTimeout(this.recalculateTimeout);
        this.recalculateTimeout = setTimeout(() => {
            console.log('🔄 Recalculando todas las tablas por resize/orientation');
            this.tablas.forEach((config, wrapper) => {
                this.calcularYActualizarPaginacion(wrapper);
            });
        }, 200);
    }

    // Método público para actualizar datos dinámicamente
    actualizarDatos(wrapper, nuevosDatos) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        console.log('🔄 Actualizando datos de tabla:', nuevosDatos.length, 'registros');
        config.datos = nuevosDatos;
        config.paginaActual = 1;
        this.calcularYActualizarPaginacion(wrapper);
    }

    // Método público para forzar recálculo
    forzarRecalculo(wrapper = null) {
        if (wrapper) {
            this.calcularYActualizarPaginacion(wrapper);
        } else {
            this.recalcularTodasLasTablas();
        }
    }

    // Método para debug
    obtenerEstado() {
        const estado = {};
        this.tablas.forEach((config, wrapper) => {
            estado[config.id] = {
                registrosTotales: config.datos.length,
                registrosPorPagina: config.registrosPorPagina,
                paginaActual: config.paginaActual,
                totalPaginas: config.totalPaginas
            };
        });
        return estado;
    }
}

// Inicialización automática
document.addEventListener('DOMContentLoaded', function () {
    console.log('🚀 Iniciando sistema de paginación automática...');

    // Pequeño delay para asegurar que todo esté renderizado
    setTimeout(() => {
        window.sistemaPaginacion = new PaginacionAutomatica();

        // Recalcular después de que todo esté completamente cargado
        setTimeout(() => {
            if (window.sistemaPaginacion) {
                console.log('🎯 Recalculo inicial de tablas...');
                window.sistemaPaginacion.recalcularTodasLasTablas();
            }
        }, 300);
    }, 100);
});

// También inicializar si el DOM ya está listo (por si el script se carga después)
if (document.readyState === 'interactive' || document.readyState === 'complete') {
    setTimeout(() => {
        window.sistemaPaginacion = new PaginacionAutomatica();
        setTimeout(() => {
            if (window.sistemaPaginacion) {
                window.sistemaPaginacion.recalcularTodasLasTablas();
            }
        }, 300);
    }, 100);
}