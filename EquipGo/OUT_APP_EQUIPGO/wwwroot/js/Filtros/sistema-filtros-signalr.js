// sistema-filtros-signalr.js
class SistemaFiltrosSignalR {
    constructor() {
        this.conexionSignalR = null;
        this.filtrosActivos = new Map();
        this.tablas = new Map();
        this.init();
    }

    async init() {
        await this.inicializarSignalR();
        this.inicializarFiltrosExistentes();
        this.configurarMutationObserver();
        this.inicializarTablasExistentes();
        console.log('✅ Sistema de filtros SignalR inicializado');
    }

    async inicializarSignalR() {
        // Cargar SignalR dinámicamente si no está disponible
        if (typeof signalR === 'undefined') {
            await this.cargarSignalR();
        }

        this.conexionSignalR = new signalR.HubConnectionBuilder()
            .withUrl("/filtrosHub")
            .withAutomaticReconnect()
            .build();

        // Configurar handlers de SignalR
        this.conexionSignalR.on("FiltrosActualizados", (filtrosData) => {
            this.procesarFiltrosDesdeSignalR(filtrosData);
        });

        this.conexionSignalR.on("DatosActualizados", (tablaData) => {
            this.actualizarTablaDesdeSignalR(tablaData);
        });

        try {
            await this.conexionSignalR.start();
            console.log('🔗 Conectado a SignalR Hub');
        } catch (err) {
            console.error('❌ Error conectando a SignalR:', err);
        }
    }

    cargarSignalR() {
        return new Promise((resolve, reject) => {
            if (typeof signalR !== 'undefined') {
                resolve();
                return;
            }

            const script = document.createElement('script');
            script.src = '/lib/microsoft/signalr/dist/browser/signalr.min.js';
            script.onload = resolve;
            script.onerror = reject;
            document.head.appendChild(script);
        });
    }

    inicializarFiltrosExistentes() {
        document.querySelectorAll('.dashboard-filtros').forEach(contenedor => {
            this.configurarContenedorFiltros(contenedor);
        });
    }

    inicializarTablasExistentes() {
        document.querySelectorAll('.table-equipgo').forEach(tabla => {
            this.registrarTabla(tabla);
        });
    }

    configurarMutationObserver() {
        const observer = new MutationObserver((mutations) => {
            mutations.forEach((mutation) => {
                mutation.addedNodes.forEach((node) => {
                    if (node.nodeType === 1) {
                        // Detectar nuevos filtros
                        if (node.classList?.contains('dashboard-filtros')) {
                            this.configurarContenedorFiltros(node);
                        }
                        node.querySelectorAll?.('.dashboard-filtros').forEach(contenedor => {
                            this.configurarContenedorFiltros(contenedor);
                        });

                        // Detectar nuevas tablas
                        if (node.classList?.contains('table-equipgo')) {
                            this.registrarTabla(node);
                        }
                        node.querySelectorAll?.('.table-equipgo').forEach(tabla => {
                            this.registrarTabla(tabla);
                        });
                    }
                });
            });
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }

    configurarContenedorFiltros(contenedor) {
        const contenedorId = this.generarIdUnico();
        const filtrosConfig = [];

        // Configurar todos los inputs del contenedor
        const inputs = contenededor.querySelectorAll('input, select');

        inputs.forEach(input => {
            const filtroNombre = input.getAttribute('name') ||
                input.placeholder?.replace(/[^a-zA-Z0-9]/g, '_').toLowerCase() ||
                `filtro_${filtrosConfig.length}`;

            // Configurar event listeners
            input.addEventListener('input', this.debounce((e) => {
                this.enviarFiltroSignalR(contenedorId, filtroNombre, e.target.value);
            }, 300));

            input.addEventListener('change', (e) => {
                this.enviarFiltroSignalR(contenedorId, filtroNombre, e.target.value);
            });

            filtrosConfig.push({
                nombre: filtroNombre,
                elemento: input,
                tipo: input.tagName.toLowerCase()
            });
        });

        // Configurar botón limpiar si existe
        const botonLimpiar = contenedor.querySelector('.botn3');
        if (botonLimpiar) {
            botonLimpiar.addEventListener('click', () => {
                this.limpiarFiltrosContenedor(contenedorId, filtrosConfig);
            });
        }

        this.filtrosActivos.set(contenedorId, {
            contenedor,
            filtros: filtrosConfig,
            valores: {}
        });

        console.log('🎛️ Filtros configurados:', contenedorId);
    }

    registrarTabla(tabla) {
        const tablaId = tabla.getAttribute('id') || this.generarIdUnico();
        const wrapper = tabla.closest('.table-wrapper') || tabla.parentElement;

        this.tablas.set(tablaId, {
            tabla,
            wrapper,
            datosOriginales: this.extraerDatosTabla(tabla),
            config: {
                paginaActual: 1,
                registrosPorPagina: 10
            }
        });

        console.log('📊 Tabla registrada:', tablaId);
    }

    extraerDatosTabla(tabla) {
        const tbody = tabla.querySelector('tbody');
        if (!tbody) return [];

        return Array.from(tbody.querySelectorAll('tr')).map(tr => {
            const celdas = Array.from(tr.querySelectorAll('td'));
            return {
                elemento: tr.outerHTML,
                datos: celdas.map(td => td.textContent.trim())
            };
        });
    }

    async enviarFiltroSignalR(contenedorId, filtroNombre, valor) {
        if (!this.conexionSignalR || this.conexionSignalR.state !== 'Connected') {
            console.warn('SignalR no conectado');
            return;
        }

        try {
            // Actualizar estado local
            const contenedor = this.filtrosActivos.get(contenedorId);
            if (contenedor) {
                contenedor.valores[filtroNombre] = valor;
            }

            // Enviar a SignalR
            await this.conexionSignalR.invoke("EnviarFiltros", contenedorId, filtroNombre, valor);
            console.log('📤 Filtro enviado:', { contenedorId, filtroNombre, valor });

        } catch (err) {
            console.error('❌ Error enviando filtro:', err);
        }
    }

    async limpiarFiltrosContenedor(contenedorId, filtrosConfig) {
        filtrosConfig.forEach(filtro => {
            filtro.elemento.value = '';
            this.enviarFiltroSignalR(contenedorId, filtro.nombre, '');
        });

        console.log('🧹 Filtros limpiados:', contenedorId);
    }

    procesarFiltrosDesdeSignalR(filtrosData) {
        const { contenedorId, filtros } = filtrosData;

        console.log('📥 Filtros recibidos:', filtrosData);

        // Aplicar filtros a todas las tablas
        this.aplicarFiltrosATodasLasTablas(filtros);
    }

    aplicarFiltrosATodasLasTablas(filtros) {
        this.tablas.forEach((tablaConfig, tablaId) => {
            this.aplicarFiltrosATabla(tablaId, filtros);
        });
    }

    aplicarFiltrosATabla(tablaId, filtros) {
        const tablaConfig = this.tablas.get(tablaId);
        if (!tablaConfig) return;

        const datosFiltrados = tablaConfig.datosOriginales.filter(fila => {
            return this.filaCumpleFiltros(fila.datos, filtros);
        });

        this.actualizarTabla(tablaConfig, datosFiltrados);
        console.log('✅ Tabla filtrada:', tablaId, datosFiltrados.length);
    }

    filaCumpleFiltros(datosFila, filtros) {
        // Lógica de filtrado basada en la estructura de tu tabla
        // Esto debe adaptarse según tus columnas específicas

        for (const [filtroNombre, valor] of Object.entries(filtros)) {
            if (!valor) continue;

            const indiceColumna = this.obtenerIndiceColumna(filtroNombre);
            if (indiceColumna === -1) continue;

            if (datosFila[indiceColumna] &&
                !datosFila[indiceColumna].toLowerCase().includes(valor.toLowerCase())) {
                return false;
            }
        }

        return true;
    }

    obtenerIndiceColumna(filtroNombre) {
        // Mapear nombres de filtro a índices de columna
        const mapeo = {
            'area': 3,
            'campania': 4,
            'estado': 5,
            'marca': 0,
            'modelo': 1,
            'serial': 2,
            'usuario': 2,
            'codigo_barras': 1,
            'tipo_transaccion': 5,
            'nombre_visitante': 0,
            'aprobado_por': 3
        };

        return mapeo[filtroNombre] !== undefined ? mapeo[filtroNombre] : -1;
    }

    actualizarTabla(tablaConfig, datosFiltrados) {
        const tbody = tablaConfig.tabla.querySelector('tbody');
        if (!tbody) return;

        tbody.innerHTML = datosFiltrados.map(fila => fila.elemento).join('');

        // Si existe el sistema de paginación, actualizarlo
        if (window.paginacionAutomatica) {
            const wrapper = tablaConfig.wrapper;
            if (window.paginacionAutomatica.tablas.has(wrapper)) {
                const datosParaPaginacion = datosFiltrados.map(f => f.elemento);
                window.paginacionAutomatica.actualizarDatos(wrapper, datosParaPaginacion);
            }
        }
    }

    actualizarTablaDesdeSignalR(tablaData) {
        const { tablaId, datos } = tablaData;
        const tablaConfig = this.tablas.get(tablaId);

        if (tablaConfig) {
            tablaConfig.datosOriginales = datos.map(d => ({
                elemento: d,
                datos: this.extraerDatosDeHTML(d)
            }));

            this.aplicarFiltrosATabla(tablaId, this.obtenerFiltrosActivos());
        }
    }

    extraerDatosDeHTML(htmlFila) {
        const temp = document.createElement('tr');
        temp.innerHTML = htmlFila;
        return Array.from(temp.querySelectorAll('td')).map(td => td.textContent.trim());
    }

    obtenerFiltrosActivos() {
        const todosFiltros = {};
        this.filtrosActivos.forEach(contenedor => {
            Object.assign(todosFiltros, contenedor.valores);
        });
        return todosFiltros;
    }

    debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    generarIdUnico() {
        return 'id_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
    }

    destruir() {
        if (this.conexionSignalR) {
            this.conexionSignalR.stop();
        }
        this.filtrosActivos.clear();
        this.tablas.clear();
    }
}