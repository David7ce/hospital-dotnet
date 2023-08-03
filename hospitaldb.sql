CREATE DATABASE HospitalDB;
GO

-- Hacemos uso de la base de datos
USE HospitalDB;
GO

-- Creamos las tablas de la base de datos: Paciente, Doctor, Consulta, Tratamiento
CREATE TABLE Paciente (
    ID INT IDENTITY(1,1) PRIMARY KEY,
	Nombre VARCHAR(50) NOT NULL,
    Apellido1 VARCHAR(50) NOT NULL,
    Apellido2 VARCHAR(50),
    Fecha_Nacimiento DATE NOT NULL,
    Direccion VARCHAR(100),
    Telefono VARCHAR(15) NOT NULL,
    Email VARCHAR(45)
);

CREATE TABLE Especialidad (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL
);

CREATE TABLE Doctor (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Apellido1 VARCHAR(50) NOT NULL,
    Apellido2 VARCHAR(50),
    ID_Especialidad INT,
    FOREIGN KEY (ID_Especialidad) REFERENCES Especialidad(ID)
);

CREATE TABLE Consulta (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ID_Paciente INT NOT NULL,
    ID_Doctor INT,
    Fecha_Consulta DATE NOT NULL,
    Diagnostico VARCHAR(100),
    FOREIGN KEY (ID_Paciente) REFERENCES Paciente(ID),
    FOREIGN KEY (ID_Doctor) REFERENCES Doctor(ID)
);

CREATE TABLE Tratamiento (
    ID INT IDENTITY(50,5) PRIMARY KEY,
    ID_Paciente INT NOT NULL,
    ID_Doctor INT,
    Medicamento VARCHAR(50) NOT NULL,
    Dosis VARCHAR(20),
    Duracion INT,
    FOREIGN KEY (ID_Paciente) REFERENCES Paciente(ID),
    FOREIGN KEY (ID_DOCTOR) REFERENCES Doctor(ID)
);

GO

CREATE TABLE Isla (
	ID INT IDENTITY(10,1) PRIMARY KEY,
	Nombre VARCHAR(30) NOT NULL,
	Provincia VARCHAR(30) NOT NULL
);
GO

CREATE TABLE Personal_Enfermeria (
	ID INT IDENTITY(20,2) PRIMARY KEY,
	Nombre VARCHAR(30) NOT NULL,
	Apellido1 VARCHAR(30) NOT NULL,
	Apellido2 VARCHAR(30),
	NIF_NIE VARCHAR(9) NOT NULL,
	Telefono VARCHAR(9) NOT NULL,
	Isla_Residencia INT,
	Fecha_Alta DATE NOT NULL,
	ID_Supervisor INT,
	FOREIGN KEY (Isla_Residencia) REFERENCES Isla(ID),
	FOREIGN KEY (ID_Supervisor) REFERENCES Doctor(ID)
);
GO

-- DML

-- Hacemos uso de la base de datos HospitalDB
USE HospitalDB

INSERT INTO Isla (Nombre, Provincia) VALUES
    ('Tenerife', 'Santa Cruz de Tenerife'),
	('Gran Canaria', 'Las Palmas'),
	('Lanzarote', 'Las Palmas'),
	('Fuerteventura', 'Las Palmas'),
	('La Palma', 'Santa Cruz de Tenerife'),
	('La Gomera', 'Santa Cruz de Tenerife'),
	('El Hierro', 'Santa Cruz de Tenerife'),
	('La Graciosa', 'Las Palmas');
GO

-- 1. Insertar 3 pacientes en sentencias diferentes, con el nombre y apellidos
--	  de tres cantantes (el resto de datos inventados).
SET IDENTITY_INSERT Paciente ON;

INSERT INTO Paciente (ID, Nombre, Apellido1, Apellido2, Fecha_Nacimiento, Direccion, Telefono, Email)
VALUES (1, 'Michael', 'Jackson', NULL, '1980-01-01', 'Calle Dancing Street', '555-111-1111', 'michael@example.com');

INSERT INTO Paciente (ID, Nombre, Apellido1, Apellido2, Fecha_Nacimiento, Direccion, Telefono, Email)
VALUES (2, 'Freddie', 'Mercury', NULL, '1970-02-02', '456 Calle Champions', '555-222-2222', 'freddie@example.com');

INSERT INTO Paciente (ID, Nombre, Apellido1, Apellido2, Fecha_Nacimiento, Direccion, Telefono, Email)
VALUES (3, 'Beyoncé', 'Knowles', 'Carter', '1985-03-03', '789 Calle El Rosario', '555-333-3333', 'beyonce@example.com');

SET IDENTITY_INSERT Paciente OFF;

-- 2. Insertar 3 doctores en sentencias diferentes, con el nombre y apellidos de
--    tres deportistas (el resto de datos inventados).
INSERT INTO Especialidad(Nombre)
VALUES
    ('Cardiología'),
    ('Ortopedia'), --'Oftalmología'
    ('Medicina Deportiva'); --'Traumatología'


SET IDENTITY_INSERT Doctor ON;
INSERT INTO Doctor (ID, Nombre, Apellido1, Apellido2, ID_Especialidad)
VALUES
    (1, 'Lionel', 'Messi', 'Cuccittini', 1),
    (2, 'Serena', 'Williams', NULL, 2),
    (3, 'Usain', 'Bolt', NULL, 3);
SET IDENTITY_INSERT Doctor OFF;

-- 3. Insertar 3 consultas en una misma sentencia (un doctor con dos pacientes y otro doctor con uno de esos mismos pacientes.
--    El tercer doctor y un paciente no tendrán relación con ninguna “consulta”)
INSERT INTO Consulta (ID_Paciente, ID_Doctor, Fecha_Consulta, Diagnostico)
VALUES
    (1, 1, '2023-01-01', 'Hipertensión'),
    (2, 1, '2023-02-02', 'Fractura de pierna'),
    (1, 2, '2023-03-03', 'Dolor de espalda');

-- 4. Insertar 3 tratamientos en una misma sentencia que uno (en relación a las 3 “consultas” creadas anteriormente
-- tenga “Ibuprofeno” y duración “7”, otro “Paracetamol” y duración “10” y el otro con doctor, dosis y duración desconocidos)
INSERT INTO Tratamiento (ID_Paciente, ID_Doctor, Medicamento, Dosis, Duracion)
VALUES
    (1, 1, 'Ibuprofeno', '200 mg', 7),
    (2, 1, 'Paracetamol', '500 mg', 10),
    (3, NULL, 'Desconocido', NULL, NULL);

-- 5. Obtener el total de pacientes que no se llamen "Pepe".
SELECT COUNT(*) AS 'Total de pacientes no llamados Pepe'
FROM Paciente
WHERE Nombre <> 'Pepe';

-- 6. Obtener el promedio de días de la duración de los tratamientos, redondeando por encima sin decimales.
SELECT CEILING(AVG(Duracion)) AS Promedio_Duracion_Tratamiento
FROM Tratamiento;

-- 7. Actualizar los apellidos del primer doctor insertado (mediante el ID), para que tenga
--	  tu nombre y apellidos.
UPDATE Doctor
SET Nombre = 'David', Apellido1 = 'Alonso', Apellido2 = 'de Dios'
WHERE ID = 1;

-- 8. Obtener el nombre y los apellidos de los pacientes cuyo nombre empiece con "A" y no acabe en "n".
SELECT Nombre, Apellido1, Apellido2 FROM Paciente
WHERE Nombre LIKE 'A%' AND Nombre NOT LIKE '%n';

-- 9. Obtener el nombre y los apellidos de los doctores cuya especialidad no sea desconocida ni existente,
--    ordenados de forma descendente por el nombre.
SELECT D.Nombre, D.Apellido1, D.Apellido2
FROM Doctor D
WHERE D.ID_Especialidad IS NOT NULL
ORDER BY Nombre DESC;

-- 10. Obtener el nombre, los apellidos y fecha de nacimiento de los pacientes que no hayan
--     tenido ninguna consulta.
SELECT Nombre, Apellido1, Apellido2, Fecha_Nacimiento
FROM Paciente
WHERE ID NOT IN (SELECT ID_Paciente FROM Consulta);

-- 11. Obtener el nombre y teléfono de los pacientes que hayan tenido un tratamiento
--     con el medicamento llamado "Aspirina" o "Ibuprofeno" durante más de 10 días.
SELECT p.Nombre, p.Telefono, t.*
FROM Paciente p
INNER JOIN Tratamiento t ON p.ID = t.ID_Paciente
WHERE (t.Medicamento = 'Ibuprofeno' OR  t.Medicamento = 'Aspirina') AND t.Duracion > 10;

-- 12. Obtener el nombre, email y fecha de nacimiento de los pacientes que hayan tenido tratamientos
--     con una duración superior a 7 días e inferior a 11 días, con una dosis de "2 pastillas al día".
SELECT p.Nombre, p.Email, p.Fecha_Nacimiento
FROM Paciente p
INNER JOIN Tratamiento t ON p.ID = t.ID_Paciente
WHERE t.Duracion > 7 AND t.Duracion < 11 AND t.Dosis = '2 pastillas al día';

-- 13. Obtener todos los datos de los pacientes y de sus respectivas consultas (únicamente fecha y diagnóstico),
--     incluyendo aquellos pacientes que no hayan tenido ninguna consulta.
SELECT p.*, c.Fecha_Consulta, c.Diagnostico
FROM Paciente p
LEFT JOIN Consulta c ON p.ID = c.ID_Paciente;

-- 14. Obtener el nombre y especialidad (en mayúsculas) de los doctores que no hayan realizado
--     ninguna consulta y cuyo primer apellido no tenga como tercer carácter una "o".
SELECT D.Nombre, UPPER(E.Nombre) AS 'Especialidad'
FROM Especialidad E
INNER JOIN Doctor D
ON D.ID_Especialidad = E.ID
WHERE D.ID NOT IN (SELECT ID_Doctor FROM Consulta)
AND SUBSTRING(Apellido1, 3, 1) <> 'o';

-- 15. Borrar los tratamientos cuya dosis sea desconocida o inexistente, con duración de 7 días
--     y el medicamento se llame "Ibuprofeno" o "Paracetamol".
DELETE FROM Tratamiento
WHERE (Dosis IS NULL) AND Duracion = 7 AND Medicamento IN ('Ibuprofeno', 'Paracetamol');

-- 16. Obtener el nombre (con alias "Nombre del doctor en mayúsculas") y primer apellido
--    (con alias "Primer apellido del doctor en minúsculas") de los doctores y todos los datos de
--    sus respectivas consultas, incluyendo aquellos doctores que no hayan realizado ninguna consulta.
SELECT UPPER(d.Nombre) AS "Nombre del doctor en mayúsculas", LOWER(d.Apellido1) AS "Primer apellido del doctor en minúsculas", c.*
FROM Doctor d
LEFT JOIN Consulta c ON d.ID = c.ID_Doctor;

-- 17. Obtener el Id y nombre de los doctores y sus respectivos tratamientos asociados, excluyendo aquellos
-- doctores que no hayan realizado ningún tratamiento y aquellos tratamientos que no estén asociados a ningún doctor.
SELECT d.ID, d.Nombre, t.*
FROM Doctor d
INNER JOIN Tratamiento t ON d.ID = t.ID_Doctor;

-- 18. Nombre de los doctores y de sus pacientes, junto a los medicamentos, dosis y duración de sus tratamientos,
-- cuyo medicamento no termine en "no" y la dosis contenga la palabra "semanal".
SELECT d.Nombre AS "Nombre Doctor", p.Nombre AS "Nombre Paciente", t.Medicamento, t.Dosis, t.Duracion
FROM Doctor d
INNER JOIN Tratamiento t ON d.ID = t.ID_Doctor
INNER JOIN Paciente p ON t.ID_Paciente = p.ID
WHERE t.Medicamento NOT LIKE '%no' AND t.Dosis LIKE '%semanal%';

-- 19. Obtener el nombre y apellidos de los pacientes, fecha de sus respectivas consultas y los tratamientos
--     correspondientes, excluyendo aquellos pacientes que no hayan tenido ninguna consulta.
SELECT p.Nombre, p.Apellido1, p.Apellido2, c.Fecha_Consulta, t.Medicamento, t.Dosis, t.Duracion
FROM Consulta c
INNER JOIN Paciente p ON p.ID = c.ID_Paciente
INNER JOIN Tratamiento t ON t.ID_Paciente = p.ID;

-- 20. Obtener todos los datos de los pacientes y de los doctores que los han tratado, en alguna consulta
--     anterior al 10/03/2023 y que le han asignado algún tratamiento con duración mayor a 5 días.
SELECT p.*, d.*
FROM Consulta c
INNER JOIN Paciente p ON p.ID = c.ID_Paciente
INNER JOIN Tratamiento t ON t.ID_Paciente = p.ID
INNER JOIN Doctor d ON d.ID = t.ID_Doctor
WHERE c.Fecha_Consulta < '2023-03-10' AND t.Duracion > 5;