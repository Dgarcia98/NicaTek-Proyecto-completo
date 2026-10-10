USE NicaTek;
GO

-- 3. Agregar Nombre_Bodega si no existe (ya lo tienes con ALTER previo, verificar)
-- Si ya existe salta este bloque
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('Tbl_Bodega') AND name = 'Nombre_Bodega'
)
BEGIN
    ALTER TABLE Tbl_Bodega ADD Nombre_Bodega VARCHAR(100) NOT NULL DEFAULT 'Sin nombre';
END
GO

-- 4. Crear tabla Tbl_Bodega_Producto
-- Stock por combinación bodega + producto
CREATE TABLE Tbl_Bodega_Producto (
    Id_Bodega_Producto  INT PRIMARY KEY IDENTITY(1,1),
    Id_Bodega           INT NOT NULL REFERENCES Tbl_Bodega(Id_Bodega),
    Id_Producto         INT NOT NULL REFERENCES Tbl_Producto(Id_Producto),
    Cantidad            INT NOT NULL DEFAULT 0,
    Fecha_Creacion      DATETIME NOT NULL DEFAULT GETDATE(),
    Fecha_Modificacion  DATETIME NOT NULL DEFAULT GETDATE(),
    Id_Creador          INT NOT NULL REFERENCES Tbl_Usuarios(Id_Usuario),
    Id_Modificador      INT NOT NULL REFERENCES Tbl_Usuarios(Id_Usuario),
    -- Un producto no puede repetirse en la misma bodega
    CONSTRAINT UQ_Bodega_Producto UNIQUE (Id_Bodega, Id_Producto)
);
GO

-- 5. Agregar Id_Bodega_Producto a Tbl_Movimientos_Inventario
ALTER TABLE Tbl_Movimientos_Inventario
ADD Id_Bodega_Producto INT NULL REFERENCES Tbl_Bodega_Producto(Id_Bodega_Producto);
GO

-- 6. Verificar estructura final
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Tbl_Bodega'
ORDER BY ORDINAL_POSITION;

SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Tbl_Bodega_Producto'
ORDER BY ORDINAL_POSITION;

SELECT * FROM Tbl_Bodega