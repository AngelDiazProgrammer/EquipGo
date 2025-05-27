CREATE DATABASE EquipGo;
GO
USE EquipGo;
GO

-- Crear esquemas
CREATE SCHEMA seguridad;
CREATE SCHEMA configuracion;
CREATE SCHEMA procesos;
CREATE SCHEMA smart;
GO

-- ===============================
-- ESQUEMA: smart
-- ===============================
CREATE TABLE smart.tipoDocumento (
    id INT PRIMARY KEY,
    nombre_documento VARCHAR(100)
);

CREATE TABLE smart.Estado (
    id INT PRIMARY KEY,
    nombre_estado VARCHAR(100)
);

CREATE TABLE smart.Sedes (
    id INT PRIMARY KEY,
    nombre_sede VARCHAR(100)
);

CREATE TABLE smart.tiposDispositivos (
    id INT PRIMARY KEY,
    nombre_tipo VARCHAR(100)
);
GO

-- ===============================
-- ESQUEMA: seguridad
-- ===============================
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
    URL VARCHAR(255),
    Icon VARCHAR(100),
    Estado BIT,
    FechaRegistro DATETIME,
    UsuarioRegistroId INT,
    FechaActualizacion DATETIME,
    UsuarioActualizacionId INT
);

CREATE TABLE seguridad.PermisosRolPaginas (
    id INT PRIMARY KEY,
    id_rol INT,
    pagina_id INT,
    Estado BIT,
    FechaRegistro DATETIME,
    UsuarioRegistroId INT,
    FechaActualizacion DATETIME,
    UsuarioActualizacionId INT,
    FOREIGN KEY (id_rol) REFERENCES seguridad.roles(id),
    FOREIGN KEY (pagina_id) REFERENCES seguridad.paginas(id)
);

CREATE TABLE seguridad.usuariosSession (
    id INT PRIMARY KEY,
    nombre VARCHAR(100),
    apellido VARCHAR(100),
    id_Tipodocumento INT,
    numeroDocumento VARCHAR(100),
    id_rol INT,
    estado BIT,
    fecha_creacion DATETIME,
    ultima_modificacion DATETIME,
    FOREIGN KEY (id_Tipodocumento) REFERENCES smart.tipoDocumento(id),
    FOREIGN KEY (id_rol) REFERENCES seguridad.roles(id)
);
GO

-- ===============================
-- ESQUEMA: configuracion
-- ===============================
CREATE TABLE configuracion.zonasSedes (
    id INT PRIMARY KEY,
    nombre VARCHAR(100),
    latitude DECIMAL(9,6),
    longitud DECIMAL(9,6),
    readioMetros INT,
	sedeOS int,
	FOREIGN KEY (sedeOs) REFERENCES smart.Sedes(id)
);

CREATE TABLE configuracion.equipos (
    id INT PRIMARY KEY,
    marca VARCHAR(100),
    modelo VARCHAR(100),
    serial VARCHAR(100),
    codigodebarras VARCHAR(100) UNIQUE,
    propietario INT,
    ubicación VARCHAR(255),
    id_estado INT,
    equipo_personal BIT,
    latitud DECIMAL(9,6),
    longitud DECIMAL(9,6),
    tipo_dispositivo int,
    sistema_operativo VARCHAR(100),
    mac_equipo VARCHAR(100),
    fecha_creacion DATETIME,
    ultima_modificacion DATETIME,
    version_software VARCHAR(100),
    sedeOs INT,
    FOREIGN KEY (propietario) REFERENCES procesos.usuariosInformacion(id),
    FOREIGN KEY (id_estado) REFERENCES smart.Estado(id),
    FOREIGN KEY (sedeOs) REFERENCES smart.Sedes(id),
	FOREIGN KEY (tipo_dispositivo) REFERENCES smart.tiposDispositivos(id)
);
GO

-- ===============================
-- ESQUEMA: procesos
-- ===============================
CREATE TABLE procesos.usuariosInformacion (
    id INT PRIMARY KEY,
    nombres VARCHAR(100),
    apellidos VARCHAR(100),
    id_Tipodocumento INT,
    numeroDocumento VARCHAR(100),
    area VARCHAR(100),
    campaña VARCHAR(100),
    estado BIT,
    fecha_creacion DATETIME,
    ultima_modificacion DATETIME,
    FOREIGN KEY (id_Tipodocumento) REFERENCES smart.tipoDocumento(id)
);

CREATE TABLE procesos.Transacciones (
    id INT PRIMARY KEY,
    codigoBarra VARCHAR(100),
    fechaHora DATETIME,
    tipo VARCHAR(50), -- entrada / salida
    usuario VARCHAR(100),
    equipo_personal BIT,
    propietario INT,
    rolAprobador INT,
    sedeOs INT,
    FOREIGN KEY (codigoBarra) REFERENCES configuracion.equipos(codigodebarras),
    FOREIGN KEY (propietario) REFERENCES procesos.usuariosInformacion(id),
    FOREIGN KEY (rolAprobador) REFERENCES seguridad.usuariosSession(id),
    FOREIGN KEY (sedeOs) REFERENCES smart.Sedes(id)
);

CREATE TABLE procesos.BitacoraEventos (
    id INT PRIMARY KEY,
    usuarios INT,
    accion VARCHAR(255),
    descripcion TEXT,
    fecha DATETIME,
    tipo_evento VARCHAR(100),
    FOREIGN KEY (usuarios) REFERENCES seguridad.usuariosSession(id)
);

CREATE TABLE procesos.historialUbicaciones (
    id INT PRIMARY KEY,
    id_equipoOS INT,
    latitud DECIMAL(9,6),
    longitud DECIMAL(9,6),
    fecha DATETIME,
    FOREIGN KEY (id_equipoOS) REFERENCES configuracion.equipos(id)
);

CREATE TABLE procesos.AlertasGeofencing (
    id INT PRIMARY KEY,
    id_equipo INT,
    tipo_alerta VARCHAR(100),
    descripcion TEXT,
    fecha DATETIME,
    id_equipoOS INT,
    FOREIGN KEY (id_equipo) REFERENCES configuracion.equipos(id),
    FOREIGN KEY (id_equipoOS) REFERENCES configuracion.equipos(id)
);

CREATE TABLE procesos.EquiposPorPersona (
    id INT PRIMARY KEY,
    idUsuario INT,
    id_equipoOS INT,
    id_equipoPersonal INT,
    fecha_creacion DATETIME,
    fecha_desasignacion DATETIME,
    FOREIGN KEY (idUsuario) REFERENCES procesos.usuariosInformacion(id),
    FOREIGN KEY (id_equipoOS) REFERENCES configuracion.equipos(id),
    FOREIGN KEY (id_equipoPersonal) REFERENCES configuracion.equipos(id)
);

CREATE TABLE procesos.LogEventos (
    id INT PRIMARY KEY,
    usuario_id INT,
    Accion VARCHAR(100),
    Descripcion TEXT,
    FechaEvento DATETIME,
    TipoEvento VARCHAR(100),
    Origen VARCHAR(100),
    IPUsuario VARCHAR(100),
    FOREIGN KEY (usuario_id) REFERENCES seguridad.usuariosSession(id)
);
GO
