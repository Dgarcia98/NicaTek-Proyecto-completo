USE NicaTek;
GO

CREATE OR ALTER PROC sp_Tbl_Bodega
    @Mode           VARCHAR(3)   = NULL,
    @O_Msg          VARCHAR(255) = NULL OUTPUT,
    @O_Num          INT          = NULL OUTPUT,
    @Id_Bodega      INT          = NULL,
    @Nombre_Bodega  VARCHAR(100) = NULL,
    @Descripcion    VARCHAR(255) = NULL,
    @Ubicacion      VARCHAR(255) = NULL,
    @Id_Estado      INT          = NULL,
    @Id_Creador     INT          = NULL,
    @Id_Modificador INT          = NULL,
    @Filtro         VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Mode = 'INS'
    BEGIN
        IF @Nombre_Bodega IS NULL OR @Id_Estado IS NULL
           OR @Id_Creador IS NULL OR @Id_Modificador IS NULL
        BEGIN
            SET @O_Msg = 'Faltan campos obligatorios.';
            SET @O_Num = -1; RETURN;
        END

        IF EXISTS (SELECT 1 FROM Tbl_Bodega 
                   WHERE Nombre_Bodega = @Nombre_Bodega AND Id_Estado = 1)
        BEGIN
            SET @O_Msg = 'Ya existe una bodega activa con ese nombre.';
            SET @O_Num = -1; RETURN;
        END

        BEGIN TRY
            BEGIN TRAN;
            INSERT INTO Tbl_Bodega (
                Nombre_Bodega, Cantidad, Id_Estado,
                Fecha_Creacion, Fecha_Modificacion,
                Id_Creador, Id_Modificador
            )
            VALUES (
                @Nombre_Bodega, 0, @Id_Estado,
                GETDATE(), GETDATE(),
                @Id_Creador, @Id_Modificador
            );
            SET @O_Num = SCOPE_IDENTITY();
            SET @O_Msg = 'Bodega registrada correctamente.';
            COMMIT;
        END TRY
        BEGIN CATCH
            ROLLBACK;
            SET @O_Msg = ERROR_MESSAGE();
            SET @O_Num = ERROR_NUMBER();
        END CATCH
    END

    ELSE IF @Mode = 'UPD'
    BEGIN
        IF @Id_Bodega IS NULL
        BEGIN
            SET @O_Msg = 'Id_Bodega requerido.';
            SET @O_Num = -1; RETURN;
        END

        BEGIN TRY
            BEGIN TRAN;
            UPDATE Tbl_Bodega
            SET Nombre_Bodega      = COALESCE(@Nombre_Bodega,  Nombre_Bodega),
                Id_Estado          = COALESCE(@Id_Estado,       Id_Estado),
                Fecha_Modificacion = GETDATE(),
                Id_Modificador     = COALESCE(@Id_Modificador,  Id_Modificador)
            WHERE Id_Bodega = @Id_Bodega;
            SET @O_Num = @Id_Bodega;
            SET @O_Msg = 'Bodega actualizada correctamente.';
            COMMIT;
        END TRY
        BEGIN CATCH
            ROLLBACK;
            SET @O_Msg = ERROR_MESSAGE();
            SET @O_Num = ERROR_NUMBER();
        END CATCH
    END
    ELSE IF @Mode = 'DEL'
    BEGIN
        IF @Id_Bodega IS NULL OR @Id_Modificador IS NULL
        BEGIN
            SET @O_Msg = 'Id_Bodega e Id_Modificador requeridos.';
            SET @O_Num = -1; RETURN;
        END

        -- No se puede desactivar si tiene stock
        IF EXISTS (SELECT 1 FROM Tbl_Bodega_Producto 
                   WHERE Id_Bodega = @Id_Bodega AND Cantidad > 0)
        BEGIN
            SET @O_Msg = 'No se puede desactivar: la bodega tiene productos con stock.';
            SET @O_Num = -1; RETURN;
        END

        BEGIN TRY
            BEGIN TRAN;
            UPDATE Tbl_Bodega
            SET Id_Estado          = 2,
                Fecha_Modificacion = GETDATE(),
                Id_Modificador     = @Id_Modificador
            WHERE Id_Bodega = @Id_Bodega;
            SET @O_Num = @Id_Bodega;
            SET @O_Msg = 'Bodega desactivada correctamente.';
            COMMIT;
        END TRY
        BEGIN CATCH
            ROLLBACK;
            SET @O_Msg = ERROR_MESSAGE();
            SET @O_Num = ERROR_NUMBER();
        END CATCH
    END

    ELSE IF @Mode = 'LST'
    BEGIN
        SELECT
            b.Id_Bodega,
            b.Nombre_Bodega,
            b.Cantidad        AS Stock_Total,
            b.Id_Estado,
            e.Estado,
            -- Cantidad de productos distintos en esta bodega
            COUNT(bp.Id_Bodega_Producto) AS Total_Productos,
            b.Fecha_Creacion,
            b.Fecha_Modificacion,
            b.Id_Creador,
            b.Id_Modificador
        FROM Tbl_Bodega b
        JOIN Cls_Estados e ON b.Id_Estado = e.Id_Estado
        LEFT JOIN Tbl_Bodega_Producto bp ON b.Id_Bodega = bp.Id_Bodega
        WHERE b.Id_Estado = 1
        GROUP BY
            b.Id_Bodega, b.Nombre_Bodega, b.Cantidad,
            b.Id_Estado, e.Estado,
            b.Fecha_Creacion, b.Fecha_Modificacion,
            b.Id_Creador, b.Id_Modificador;

        SET @O_Msg = 'Listado de bodegas.';
        SET @O_Num = 0;
    END

    ELSE IF @Mode = 'FIL'
    BEGIN
        IF ISNULL(@Filtro, '') = ''
        BEGIN
            SET @O_Msg = 'Filtro requerido.';
            SET @O_Num = -1; RETURN;
        END

        SELECT
            b.Id_Bodega,
            b.Nombre_Bodega,
            b.Cantidad        AS Stock_Total,
            b.Id_Estado,
            e.Estado,
            COUNT(bp.Id_Bodega_Producto) AS Total_Productos,
            b.Fecha_Creacion,
            b.Fecha_Modificacion,
            b.Id_Creador,
            b.Id_Modificador
        FROM Tbl_Bodega b
        JOIN Cls_Estados e ON b.Id_Estado = e.Id_Estado
        LEFT JOIN Tbl_Bodega_Producto bp ON b.Id_Bodega = bp.Id_Bodega
        WHERE b.Id_Estado = 1
          AND (b.Nombre_Bodega LIKE '%' + @Filtro + '%'
            OR CAST(b.Id_Bodega AS VARCHAR) = @Filtro)
        GROUP BY
            b.Id_Bodega, b.Nombre_Bodega, b.Cantidad,
            b.Id_Estado, e.Estado,
            b.Fecha_Creacion, b.Fecha_Modificacion,
            b.Id_Creador, b.Id_Modificador;

        SET @O_Msg = 'Consulta filtrada.';
        SET @O_Num = 0;
    END

    ELSE
    BEGIN
        SET @O_Msg = 'Modo no reconocido.';
        SET @O_Num = -1;
    END
END
GO