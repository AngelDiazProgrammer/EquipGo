// sistema-paginacion-automatica-v2.js
class PaginacionAutomatica {
    constructor() {
        this.tablas = new Map();
        this.mutationObserver = null;
        this.resizeObservers = new Map();
        this.isInitialized = false;
        this.storageKey = 'equipgo-paginacion-config';
        this.init();
    }

    init() {
        this.inicializarTablasExistentes();
        this.configurarMutationObserver();
        this.configurarFiltros();
        this.isInitialized = true;

        console.log('✅ Sistema de paginación automática v2 inicializado');
    }

    configurarFiltros() {
        const inputsFiltro = document.querySelectorAll('.dashboard-filtros input[type="text"]');
        inputsFiltro.forEach(input => {
            input.addEventListener('input', () => this.aplicarFiltros());
        });
    }

    aplicarFiltros() {
        const filtroArea = document.querySelector('input[placeholder*="área"]')?.value.toLowerCase() || '';
        const filtroCampania = document.querySelector('input[placeholder*="campaña"]')?.value.toLowerCase() || '';
        const filtroEstado = document.querySelector('input[placeholder*="estado"]')?.value.toLowerCase() || '';

        this.tablas.forEach((config, wrapper) => {
            this.aplicarFiltrosATabla(wrapper, filtroArea, filtroCampania, filtroEstado);
        });
    }

    aplicarFiltrosATabla(wrapper, filtroArea, filtroCampania, filtroEstado) {
        const config = this.tablas.get(wrapper);
        if (!config || !config.datosOriginales) return;

        const datosFiltrados = config.datosOriginales.filter(dato => {
            const area = this.extraerTextoDeCelda(dato, 3).toLowerCase();
            const campania = this.extraerTextoDeCelda(dato, 4).toLowerCase();
            const estado = this.extraerTextoDeCelda(dato, 5).toLowerCase();

            const coincideArea = !filtroArea || area.includes(filtroArea);
            const coincideCampania = !filtroCampania || campania.includes(filtroCampania);
            const coincideEstado = !filtroEstado || estado.includes(filtroEstado);

            return coincideArea && coincideCampania && coincideEstado;
        });

        config.datos = datosFiltrados;
        config.paginaActual = 1;
        this.calcularYActualizarPaginacion(wrapper);
    }

    extraerTextoDeCelda(htmlFila, indiceCelda) {
        const tempDiv = document.createElement('div');
        tempDiv.innerHTML = htmlFila;
        const celdas = tempDiv.querySelectorAll('td');
        return celdas[indiceCelda]?.textContent || '';
    }

    // NUEVO MÉTODO: Obtener configuración guardada
    obtenerConfiguracionGuardada() {
        try {
            const guardado = localStorage.getItem(this.storageKey);
            if (guardado) {
                const config = JSON.parse(guardado);
                console.log('💾 Configuración cargada:', config);
                return config;
            }
        } catch (error) {
            console.error('❌ Error cargando configuración:', error);
        }
        return null;
    }

    // NUEVO MÉTODO: Guardar configuración
    guardarConfiguracion() {
        try {
            const configGuardar = {
                registrosPorPagina: this.registrosPorPaginaGlobal,
                modoAuto: this.modoAutoGlobal,
                ultimaActualizacion: new Date().toISOString()
            };
            localStorage.setItem(this.storageKey, JSON.stringify(configGuardar));
            console.log('💾 Configuración guardada:', configGuardar);
        } catch (error) {
            console.error('❌ Error guardando configuración:', error);
        }
    }

    inicializarTablasExistentes() {
        document.querySelectorAll('.table-equipgo').forEach(tabla => {
            this.agregarTabla(tabla);
        });
    }

    configurarMutationObserver() {
        this.mutationObserver = new MutationObserver((mutations) => {
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

        this.mutationObserver.observe(document.body, {
            childList: true,
            subtree: true
        });
    }

    agregarTabla(tabla) {
        const wrapper = tabla.closest('.table-wrapper');
        const frameContainer = tabla.closest('.frame-container') || document.body;

        if (!wrapper || this.tablas.has(wrapper)) return;

        // Obtener configuración guardada
        const configGuardada = this.obtenerConfiguracionGuardada();
        const registrosInicial = configGuardada?.registrosPorPagina || 5;
        const modoAutoInicial = configGuardada?.modoAuto !== false; // Por defecto true si no está guardado

        const id = 'tabla-' + Date.now() + '-' + Math.random().toString(36).substr(2, 9);
        this.tablas.set(wrapper, {
            id,
            tabla,
            wrapper,
            frameContainer,
            datos: [],
            datosOriginales: [],
            paginaActual: 1,
            registrosPorPagina: registrosInicial,
            totalPaginas: 1,
            thead: null,
            modoAuto: modoAutoInicial,
            alturaThead: 0,
            alturaFila: 0
        });

        // Guardar variables globales para persistencia
        this.registrosPorPaginaGlobal = registrosInicial;
        this.modoAutoGlobal = modoAutoInicial;

        console.log('📊 Tabla detectada:', id, {
            registrosInicial,
            modoAutoInicial,
            configGuardada: !!configGuardada
        });

        this.configurarTabla(wrapper);
    }

    configurarTabla(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        wrapper.classList.add('with-pagination');
        this.crearPaginador(wrapper);
        this.extraerDatos(wrapper);
        this.configurarResizeObserver(wrapper);

        requestAnimationFrame(() => {
            this.calcularYActualizarPaginacion(wrapper);
        });
    }

    configurarResizeObserver(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        const resizeObserver = new ResizeObserver(() => {
            if (config.modoAuto) {
                this.calcularYActualizarPaginacion(wrapper);
            }
        });

        resizeObserver.observe(config.frameContainer);
        this.resizeObservers.set(wrapper, resizeObserver);
        resizeObserver.observe(wrapper);
    }

    crearPaginador(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config || wrapper.querySelector('.pagination-container')) return;

        // Obtener configuración guardada para seleccionar la opción correcta
        const configGuardada = this.obtenerConfiguracionGuardada();
        const valorSeleccionado = configGuardada?.registrosPorPagina || 5;

        // Generar opciones del 3 al 10 con la selección guardada
        const opcionesRegistros = Array.from({ length: 8 }, (_, i) => i + 3)
            .map(num => `<option value="${num}" ${num === valorSeleccionado ? 'selected' : ''}>${num}</option>`)
            .join('');

        const paginationContainer = document.createElement('div');
        paginationContainer.className = 'pagination-container';
        paginationContainer.innerHTML = `
            <div class="pagination-controls-left">
                <div class="modo-selector" style="display: flex; align-items: center; gap: 8px;">
                    <label style="display: flex; align-items: center; gap: 6px; cursor: pointer; font-size: 0.85rem; color: #393e42;">
                        <input type="checkbox" id="${config.id}-modo-auto" ${config.modoAuto ? 'checked' : ''} style="cursor: pointer; width: 16px; height: 16px;">
                        <span>Auto</span>
                    </label>
                </div>
                <div class="registros-selector" id="${config.id}-registros-container">
                    <label for="${config.id}-registros">Registros:</label>
                    <select id="${config.id}-registros" class="registros-select" ${config.modoAuto ? 'disabled' : ''}>
                        ${opcionesRegistros}
                    </select>
                </div>
                <div class="pagination-info" id="${config.id}-info">
                    Calculando...
                </div>
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

        // Toggle modo automático/manual
        const checkboxAuto = document.getElementById(`${config.id}-modo-auto`);
        checkboxAuto.addEventListener('change', (e) => {
            this.cambiarModoAuto(wrapper, e.target.checked);
        });

        // Selector de registros (solo funciona en modo manual)
        const selectRegistros = document.getElementById(`${config.id}-registros`);
        selectRegistros.value = config.registrosPorPagina;

        // Aplicar estilos iniciales según el modo
        if (config.modoAuto) {
            selectRegistros.disabled = true;
            selectRegistros.style.opacity = '0.5';
            selectRegistros.style.cursor = 'not-allowed';
        } else {
            selectRegistros.disabled = false;
            selectRegistros.style.opacity = '1';
            selectRegistros.style.cursor = 'pointer';
        }

        selectRegistros.addEventListener('change', (e) => {
            this.cambiarRegistrosPorPagina(wrapper, parseInt(e.target.value));
        });
    }

    cambiarModoAuto(wrapper, esAuto) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        config.modoAuto = esAuto;
        this.modoAutoGlobal = esAuto; // Actualizar global

        const selectRegistros = document.getElementById(`${config.id}-registros`);

        if (esAuto) {
            selectRegistros.disabled = true;
            selectRegistros.style.opacity = '0.5';
            selectRegistros.style.cursor = 'not-allowed';
            console.log('🔄 Modo automático activado');
        } else {
            selectRegistros.disabled = false;
            selectRegistros.style.opacity = '1';
            selectRegistros.style.cursor = 'pointer';
            console.log('✋ Modo manual activado');
        }

        // Guardar configuración
        this.guardarConfiguracion();

        config.paginaActual = 1;
        this.calcularYActualizarPaginacion(wrapper);
    }

    cambiarRegistrosPorPagina(wrapper, nuevosRegistros) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        // Validar que esté en el rango 3-10
        nuevosRegistros = Math.max(3, Math.min(10, nuevosRegistros));

        config.registrosPorPagina = nuevosRegistros;
        this.registrosPorPaginaGlobal = nuevosRegistros; // Actualizar global

        // Guardar configuración inmediatamente
        this.guardarConfiguracion();

        console.log('💾 Registros por página cambiados y guardados:', nuevosRegistros);

        config.paginaActual = 1;
        this.calcularYActualizarPaginacion(wrapper);
    }

    extraerDatos(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        const tbody = config.tabla.querySelector('tbody');
        const thead = config.tabla.querySelector('thead');

        if (!tbody || !thead) return;

        config.datos = Array.from(tbody.querySelectorAll('tr')).map(tr => tr.outerHTML);
        config.datosOriginales = [...config.datos];
        config.thead = thead.outerHTML;

        this.medirAlturas(wrapper);
        console.log('📋 Datos extraídos:', config.datos.length, 'registros');
    }

    medirAlturas(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config || config.datos.length === 0) return;

        try {
            const tempTable = document.createElement('table');
            tempTable.className = 'table-equipgo';
            tempTable.style.position = 'absolute';
            tempTable.style.left = '-9999px';
            tempTable.style.visibility = 'hidden';
            tempTable.style.width = config.tabla.offsetWidth + 'px';

            // Medir thead
            tempTable.innerHTML = config.thead;
            document.body.appendChild(tempTable);
            config.alturaThead = tempTable.offsetHeight;
            tempTable.innerHTML = '';

            // Medir fila
            const tbody = document.createElement('tbody');
            tbody.innerHTML = config.datos[0];
            tempTable.appendChild(tbody);
            config.alturaFila = tbody.firstElementChild.offsetHeight;

            document.body.removeChild(tempTable);

            console.log('📏 Alturas medidas:', {
                thead: config.alturaThead,
                fila: config.alturaFila
            });

        } catch (error) {
            console.error('❌ Error midiendo alturas:', error);
            config.alturaThead = 50;
            config.alturaFila = 45;
        }
    }

    calcularAlturaDisponible(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config) return 600;

        try {
            const frameContainer = config.frameContainer;
            let alturaFrame;

            if (frameContainer === document.body) {
                alturaFrame = window.innerHeight * 0.9;
            } else {
                alturaFrame = frameContainer.clientHeight;
            }

            const paginador = wrapper.querySelector('.pagination-container');
            const alturaPaginador = paginador ? paginador.offsetHeight : 100;

            let espacioUsadoArriba = 0;
            let elemento = wrapper.previousElementSibling;

            while (elemento && frameContainer.contains(elemento)) {
                if (elemento.offsetHeight > 0) {
                    espacioUsadoArriba += elemento.offsetHeight;
                }
                elemento = elemento.previousElementSibling;
            }

            const estilosFrame = window.getComputedStyle(frameContainer);
            const paddingTop = parseInt(estilosFrame.paddingTop) || 0;
            const paddingBottom = parseInt(estilosFrame.paddingBottom) || 0;
            const gap = parseInt(estilosFrame.gap) || 16;

            const alturaDisponible = alturaFrame - espacioUsadoArriba - alturaPaginador - paddingTop - paddingBottom - gap - 30;

            console.log('📐 Cálculo de altura:', {
                alturaFrame: Math.round(alturaFrame),
                espacioUsadoArriba: Math.round(espacioUsadoArriba),
                alturaPaginador: Math.round(alturaPaginador),
                alturaDisponible: Math.round(alturaDisponible)
            });

            return Math.max(200, alturaDisponible);

        } catch (error) {
            console.error('❌ Error calculando altura:', error);
            return 600;
        }
    }

    calcularRegistrosPorPagina(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config || config.datos.length === 0) return 5;

        // Si está en modo manual, usar el valor del selector
        if (!config.modoAuto) {
            return config.registrosPorPagina;
        }

        try {
            const alturaDisponible = this.calcularAlturaDisponible(wrapper);
            const alturaThead = config.alturaThead || 50;
            const alturaFila = config.alturaFila || 45;

            const alturaUtil = alturaDisponible - alturaThead - 30;
            let registrosPorPagina = Math.floor(alturaUtil / alturaFila);

            // Aplicar límites: mínimo 3, máximo 10
            registrosPorPagina = Math.max(3, Math.min(registrosPorPagina, 10));

            console.log('🔢 Registros calculados:', {
                alturaDisponible: Math.round(alturaDisponible),
                alturaThead: Math.round(alturaThead),
                alturaFila: Math.round(alturaFila),
                registrosPorPagina
            });

            return registrosPorPagina;

        } catch (error) {
            console.error('❌ Error calculando registros:', error);
            return 5;
        }
    }

    calcularYActualizarPaginacion(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config || config.datos.length === 0) return;

        try {
            const nuevosRegistros = this.calcularRegistrosPorPagina(wrapper);

            if (config.registrosPorPagina !== nuevosRegistros) {
                config.registrosPorPagina = nuevosRegistros;
            }

            config.totalPaginas = Math.ceil(config.datos.length / config.registrosPorPagina);
            config.paginaActual = Math.min(config.paginaActual, config.totalPaginas);
            config.paginaActual = Math.max(1, config.paginaActual);

            console.log('✅ Paginación actualizada:', {
                modo: config.modoAuto ? 'AUTO' : 'MANUAL',
                registrosPorPagina: config.registrosPorPagina,
                totalPaginas: config.totalPaginas,
                paginaActual: config.paginaActual
            });

            this.aplicarPaginacion(wrapper);

        } catch (error) {
            console.error('❌ Error en paginación:', error);
        }
    }

    aplicarPaginacion(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        const inicio = (config.paginaActual - 1) * config.registrosPorPagina;
        const fin = inicio + config.registrosPorPagina;
        const datosPagina = config.datos.slice(inicio, fin);

        const tbody = config.tabla.querySelector('tbody');
        if (tbody) {
            tbody.innerHTML = datosPagina.join('');
        }

        this.actualizarControlesPaginacion(wrapper);
        this.actualizarInfoPaginacion(wrapper, inicio, fin);
        this.actualizarSelectorRegistros(wrapper);
    }

    actualizarSelectorRegistros(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        const select = document.getElementById(`${config.id}-registros`);
        if (select) {
            select.value = config.registrosPorPagina;
        }
    }

    actualizarInfoPaginacion(wrapper, inicio, fin) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        const infoElement = document.getElementById(`${config.id}-info`);
        if (infoElement) {
            const totalRegistros = config.datos.length;
            const mostrandoInicio = totalRegistros > 0 ? inicio + 1 : 0;
            const mostrandoFin = Math.min(fin, totalRegistros);
            const modo = config.modoAuto ? '🔄' : '✋';
            infoElement.textContent = `${modo} Pág ${config.paginaActual}/${config.totalPaginas} • ${mostrandoInicio}-${mostrandoFin} de ${totalRegistros}`;
        }
    }

    actualizarControlesPaginacion(wrapper) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        const prevBtn = document.getElementById(`${config.id}-prev`);
        const nextBtn = document.getElementById(`${config.id}-next`);
        const pagesContainer = document.getElementById(`${config.id}-pages`);

        if (!prevBtn || !nextBtn || !pagesContainer) return;

        prevBtn.disabled = config.paginaActual === 1;
        nextBtn.disabled = config.paginaActual === config.totalPaginas;

        pagesContainer.innerHTML = '';
        if (config.totalPaginas > 1) {
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
    }

    generarNumerosPagina(paginaActual, totalPaginas) {
        const paginas = [];
        const maxPaginasVisibles = 5;

        if (totalPaginas <= maxPaginasVisibles) {
            for (let i = 1; i <= totalPaginas; i++) {
                paginas.push(i);
            }
        } else {
            if (paginaActual <= 3) {
                for (let i = 1; i <= 4; i++) paginas.push(i);
                paginas.push('...');
                paginas.push(totalPaginas);
            } else if (paginaActual >= totalPaginas - 2) {
                paginas.push(1);
                paginas.push('...');
                for (let i = totalPaginas - 3; i <= totalPaginas; i++) paginas.push(i);
            } else {
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

        config.paginaActual = nuevaPagina;
        this.aplicarPaginacion(wrapper);
    }

    actualizarDatos(wrapper, nuevosDatos) {
        const config = this.tablas.get(wrapper);
        if (!config) return;

        config.datos = nuevosDatos;
        config.datosOriginales = [...nuevosDatos];
        config.paginaActual = 1;
        this.medirAlturas(wrapper);
        this.calcularYActualizarPaginacion(wrapper);
    }

    // NUEVO MÉTODO: Limpiar configuración guardada
    limpiarConfiguracion() {
        try {
            localStorage.removeItem(this.storageKey);
            console.log('🗑️ Configuración limpiada');
        } catch (error) {
            console.error('❌ Error limpiando configuración:', error);
        }
    }

    destruir() {
        this.resizeObservers.forEach(observer => observer.disconnect());
        this.resizeObservers.clear();

        if (this.mutationObserver) {
            this.mutationObserver.disconnect();
        }

        this.tablas.clear();
        console.log('🗑️ Sistema de paginación destruido');
    }
}

// Inicialización automática
document.addEventListener('DOMContentLoaded', function () {
    console.log('🚀 Iniciando sistema de paginación automática v2...');

    setTimeout(() => {
        window.paginacionAutomatica = new PaginacionAutomatica();

        setTimeout(() => {
            if (window.paginacionAutomatica && window.paginacionAutomatica.isInitialized) {
                window.paginacionAutomatica.tablas.forEach((config, wrapper) => {
                    window.paginacionAutomatica.calcularYActualizarPaginacion(wrapper);
                });
                console.log('✅ Recálculo inicial completado');
            }
        }, 500);
    }, 100);
});