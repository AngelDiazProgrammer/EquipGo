
-- CREACIÓN DE BASE DE DATOS
CREATE DATABASE EquipGo;
GO

USE EquipGo;
GO

-- =======================
-- ESQUEMA DE SEGURIDAD
-- =======================
CREATE SCHEMA seguridad;
GO

CREATE TABLE seguridad.roles (
    id INT PRIMARY KEY,
    nombre_rol VARCHAR(100),
    estado BIT
);

CREATE TABLE seguridad.paginas (
    id INT PRIMARY KEY,
    Nombre VARCHAR(100),
    Descripcion TEXT,
    Orden INT,
    URL VARCHAR(200),
    Icon VARCHAR(100),
    FechaRegistro DATETIME,
    UsuarioRegistroId INT,
    FechaActualizacion DATETIME,
    UsuarioActualizacionId INT
);

CREATE TABLE seguridad.usuariosSession (
    id INT PRIMARY KEY,
    nombre VARCHAR(100),
    apellido VARCHAR(100),
    id_Tipodocumento INT,
    numeroDocumento VARCHAR(50),
    id_rol INT FOREIGN KEY REFERENCES seguridad.roles(id),
    estado BIT,
    fecha_creacion DATETIME,
    ultima_modificacion DATETIME
);

CREATE TABLE seguridad.PermisosRolPaginas (
    id INT PRIMARY KEY,
    id_rol INT FOREIGN KEY REFERENCES seguridad.roles(id),
    pagina_id INT FOREIGN KEY REFERENCES seguridad.paginas(id),
    Estado BIT,
    FechaRegistro DATETIME,
    UsuarioRegistroId INT,
    FechaActualizacion DATETIME,
    UsuarioActualizacionId INT
);

-- ===========================
-- ESQUEMA DE CONFIGURACIÓN
-- ===========================
CREATE SCHEMA config;
GO

CREATE TABLE config.zonasSedes (
    id INT PRIMARY KEY,
    nombre VARCHAR(100),
    latitude DECIMAL(9,6),
    longitud DECIMAL(9,6),
    readioMetros DECIMAL(10,2)
);

CREATE TABLE config.equipos (
    id INT PRIMARY KEY,
    marca VARCHAR(100),
    modelo VARCHAR(100),
    serial VARCHAR(100),
    codigodebarras VARCHAR(100),
    propietario INT,
    ubicación VARCHAR(200),
    id_estado INT,
    equipo_personal BIT,
    latitud DECIMAL(9,6),
    longitud DECIMAL(9,6),
    tipo_dispositivo VARCHAR(100),
    sistema_operativo VARCHAR(100),
    mac_equipo VARCHAR(100),
    fecha_creacion DATETIME,
    ultima_modificacion DATETIME,
    version_software VARCHAR(100),
    sedeOs INT
);

-- ===================
-- ESQUEMA DE SMART
-- ===================
CREATE SCHEMA smart;
GO

CREATE TABLE smart.tiposDispositivos (
    id INT PRIMARY KEY,
    nombre_tipo VARCHAR(100)
);

CREATE TABLE smart.tipoDocumento (
    id INT PRIMARY KEY,
    nombre_documento VARCHAR(100)
);

CREATE TABLE smart.Sedes (
    id INT PRIMARY KEY,
    nombre_sede VARCHAR(100)
);

CREATE TABLE smart.Estado (
    id INT PRIMARY KEY,
    nombre_estado VARCHAR(100)
);

-- =======================
-- ESQUEMA DE PROCESOS
-- =======================
CREATE SCHEMA procesos;
GO

CREATE TABLE procesos.usuariosInformacion (
    id INT PRIMARY KEY,
    nombres VARCHAR(100),
    apellidos VARCHAR(100),
    id_Tipodocumento INT,
    numeroDocumento VARCHAR(50),
    area VARCHAR(100),
    campaña VARCHAR(100),
    estado BIT,
    fecha_creacion DATETIME,
    ultima_modificacion DATETIME
);

CREATE TABLE procesos.Transacciones (
    id INT PRIMARY KEY,
    codigoBarra VARCHAR(100),
    fechaHora DATETIME,
    tipo VARCHAR(50),
    usuario VARCHAR(100),
    equipo_personal BIT,
    propietario INT,
    rolAprobador INT,
    sedeOs INT
);

CREATE TABLE procesos.BitacoraEventos (
    id INT PRIMARY KEY,
    usuarios INT,
    accion VARCHAR(100),
    descripcion TEXT,
    fecha DATETIME,
    tipo_evento VARCHAR(50)
);

CREATE TABLE procesos.historialUbicaciones (
    id INT PRIMARY KEY,
    id_equipoOS INT,
    latitud DECIMAL(9,6),
    longitud DECIMAL(9,6),
    fecha DATETIME
);

CREATE TABLE procesos.LogEventos (
    id INT PRIMARY KEY,
    usuario_id INT,
    Accion VARCHAR(100),
    Descripcion TEXT,
    FechaEvento DATETIME,
    TipoEvento VARCHAR(50),
    Origen VARCHAR(100),
    IPUsuario VARCHAR(100)
);

CREATE TABLE procesos.AlertasGeofencing (
    id INT PRIMARY KEY,
    id_equipo INT,
    tipo_alerta VARCHAR(100),
    descripcion TEXT,
    fecha DATETIME,
    id_equipoOS INT
);

CREATE TABLE procesos.EquiposPorPersona (
    id INT PRIMARY KEY,
    idUsuario INT,
    id_equipoOS INT,
    id_equipoPersonal INT,
    fecha_creacion DATETIME,
    fecha_desasignacion DATETIME
);
