USE NicaTek;
GO

CREATE OR ALTER PROC sp_Tbl_Bodega_Producto
    @Mode               VARCHAR(3)   = NULL,
    @O_Msg              VARCHAR(255) = NULL OUTPUT,
    @O_Num              INT          = NULL OUTPUT,
    @Id_Bodega_Producto INT          = NULL,
    @Id_Bodega          INT          = NULL,
    @Id_Producto        INT          = NULL,
    @Cantidad           INT          = NULL,
    @Id_Creador         INT          = NULL,
    @Id_Modificador     INT          = NULL,
    @Filtro             VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;


    IF @Mode = 'INS'
    BEGIN
        IF @Id_Bodega IS NULL OR @Id_Producto IS NULL
           OR @Id_Creador IS NULL OR @Id_Modificador IS NULL
        BEGIN
            SET @O_Msg = 'Faltan campos obligatorios.';
            SET @O_Num = -1; RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM Tbl_Bodega 
                       WHERE Id_Bodega = @Id_Bodega AND Id_Estado = 1)
        BEGIN
            SET @O_Msg = 'La bodega no existe o está inactiva.';
            SET @O_Num = -1; RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM Tbl_Producto 
                       WHERE Id_Producto = @Id_Producto AND Id_Estado = 1)
        BEGIN
            SET @O_Msg = 'El producto no existe o está inactivo.';
            SET @O_Num = -1; RETURN;
        END

        IF EXISTS (SELECT 1 FROM Tbl_Bodega_Producto
                   WHERE Id_Bodega = @Id_Bodega AND Id_Producto = @Id_Producto)
        BEGIN
            SET @O_Msg = 'Este producto ya existe en esa bodega.';
            SET @O_Num = -1; RETURN;
        END

        BEGIN TRY
            BEGIN TRAN;
            INSERT INTO Tbl_Bodega_Producto (
                Id_Bodega, Id_Producto, Cantidad,
                Fecha_Creacion, Fecha_Modificacion,
                Id_Creador, Id_Modificador
            )
            VALUES (
                @Id_Bodega, @Id_Producto, COALESCE(@Cantidad, 0),
                GETDATE(), GETDATE(),
                @Id_Creador, @Id_Modificador
            );
            SET @O_Num = SCOPE_IDENTITY();
            SET @O_Msg = 'Producto registrado en bodega.';
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
        IF @Id_Bodega_Producto IS NULL
        BEGIN
            SET @O_Msg = 'Id_Bodega_Producto requerido.';
            SET @O_Num = -1; RETURN;
        END

        BEGIN TRY
            BEGIN TRAN;
            UPDATE Tbl_Bodega_Producto
            SET Cantidad           = COALESCE(@Cantidad,       Cantidad),
                Fecha_Modificacion = GETDATE(),
                Id_Modificador     = COALESCE(@Id_Modificador, Id_Modificador)
            WHERE Id_Bodega_Producto = @Id_Bodega_Producto;
            SET @O_Num = @Id_Bodega_Producto;
            SET @O_Msg = 'Stock actualizado correctamente.';
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
        IF @Id_Bodega_Producto IS NULL OR @Id_Modificador IS NULL
        BEGIN
            SET @O_Msg = 'Id_Bodega_Producto e Id_Modificador requeridos.';
            SET @O_Num = -1; RETURN;
        END

        -- No eliminar si tiene stock
        IF EXISTS (SELECT 1 FROM Tbl_Bodega_Producto
                   WHERE Id_Bodega_Producto = @Id_Bodega_Producto AND Cantidad > 0)
        BEGIN
            SET @O_Msg = 'No se puede eliminar: el producto tiene stock en esta bodega.';
            SET @O_Num = -1; RETURN;
        END

        BEGIN TRY
            BEGIN TRAN;
            DELETE FROM Tbl_Bodega_Producto
            WHERE Id_Bodega_Producto = @Id_Bodega_Producto;
            SET @O_Num = @Id_Bodega_Producto;
            SET @O_Msg = 'Registro eliminado correctamente.';
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
            bp.Id_Bodega_Producto,
            bp.Id_Bodega,
            b.Nombre_Bodega,
            bp.Id_Producto,
            p.Codigo        AS Codigo_Producto,
            p.Nombre        AS Nombre_Producto,
            p.Precio        AS Precio_Producto,
            mm.Id_Marca,
            m.Nombre_Marca,
            mm.Id_Modelo,
            mo.Nombre_Modelo,
            cat.Nombre      AS Nombre_Categoria,
            bp.Cantidad,
            e.Estado        AS Estado_Bodega,
            bp.Fecha_Creacion,
            bp.Fecha_Modificacion,
            bp.Id_Creador,
            bp.Id_Modificador
        FROM Tbl_Bodega_Producto bp
        JOIN Tbl_Bodega      b   ON bp.Id_Bodega   = b.Id_Bodega
        JOIN Tbl_Producto    p   ON bp.Id_Producto = p.Id_Producto
        JOIN Tbl_Marca_Modelo mm ON p.Id_Marca_Modelo = mm.Id_Marca_Modelo
        JOIN Cls_Marca       m   ON mm.Id_Marca    = m.Id_Marca
        JOIN Cls_Modelo      mo  ON mm.Id_Modelo   = mo.Id_Modelo
        JOIN Tbl_Categoria   cat ON p.Id_Categoria = cat.Id_Categoria
        JOIN Cls_Estados     e   ON b.Id_Estado    = e.Id_Estado
        WHERE b.Id_Estado = 1
        ORDER BY b.Nombre_Bodega, p.Nombre;

        SET @O_Msg = 'Listado de stock por bodega.';
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
            bp.Id_Bodega_Producto,
            bp.Id_Bodega,
            b.Nombre_Bodega,
            bp.Id_Producto,
            p.Codigo        AS Codigo_Producto,
            p.Nombre        AS Nombre_Producto,
            p.Precio        AS Precio_Producto,
            mm.Id_Marca,
            m.Nombre_Marca,
            mm.Id_Modelo,
            mo.Nombre_Modelo,
            cat.Nombre      AS Nombre_Categoria,
            bp.Cantidad,
            e.Estado        AS Estado_Bodega,
            bp.Fecha_Creacion,
            bp.Fecha_Modificacion,
            bp.Id_Creador,
            bp.Id_Modificador
        FROM Tbl_Bodega_Producto bp
        JOIN Tbl_Bodega      b   ON bp.Id_Bodega   = b.Id_Bodega
        JOIN Tbl_Producto    p   ON bp.Id_Producto = p.Id_Producto
        JOIN Tbl_Marca_Modelo mm ON p.Id_Marca_Modelo = mm.Id_Marca_Modelo
        JOIN Cls_Marca       m   ON mm.Id_Marca    = m.Id_Marca
        JOIN Cls_Modelo      mo  ON mm.Id_Modelo   = mo.Id_Modelo
        JOIN Tbl_Categoria   cat ON p.Id_Categoria = cat.Id_Categoria
        JOIN Cls_Estados     e   ON b.Id_Estado    = e.Id_Estado
        WHERE b.Id_Estado = 1
          AND (
              b.Nombre_Bodega  LIKE '%' + @Filtro + '%'
           OR p.Nombre         LIKE '%' + @Filtro + '%'
           OR p.Codigo         LIKE '%' + @Filtro + '%'
           OR m.Nombre_Marca   LIKE '%' + @Filtro + '%'
           OR cat.Nombre       LIKE '%' + @Filtro + '%'
           OR CAST(bp.Id_Bodega AS VARCHAR) = @Filtro
          )
        ORDER BY b.Nombre_Bodega, p.Nombre;

        SET @O_Msg = 'Consulta filtrada.';
        SET @O_Num = 0;
    END


    ELSE IF @Mode = 'PBD'
    BEGIN
        IF @Id_Bodega IS NULL
        BEGIN
            SET @O_Msg = 'Id_Bodega requerido.';
            SET @O_Num = -1; RETURN;
        END

        SELECT
            bp.Id_Bodega_Producto,
            bp.Id_Bodega,
            b.Nombre_Bodega,
            bp.Id_Producto,
            p.Codigo        AS Codigo_Producto,
            p.Nombre        AS Nombre_Producto,
            p.Precio        AS Precio_Producto,
            mm.Id_Marca,
            m.Nombre_Marca,
            mm.Id_Modelo,
            mo.Nombre_Modelo,
            cat.Nombre      AS Nombre_Categoria,
            bp.Cantidad,
            bp.Fecha_Creacion,
            bp.Fecha_Modificacion
        FROM Tbl_Bodega_Producto bp
        JOIN Tbl_Bodega      b   ON bp.Id_Bodega   = b.Id_Bodega
        JOIN Tbl_Producto    p   ON bp.Id_Producto = p.Id_Producto
        JOIN Tbl_Marca_Modelo mm ON p.Id_Marca_Modelo = mm.Id_Marca_Modelo
        JOIN Cls_Marca       m   ON mm.Id_Marca    = m.Id_Marca
        JOIN Cls_Modelo      mo  ON mm.Id_Modelo   = mo.Id_Modelo
        JOIN Tbl_Categoria   cat ON p.Id_Categoria = cat.Id_Categoria
        WHERE bp.Id_Bodega = @Id_Bodega
        ORDER BY p.Nombre;

        SET @O_Msg = 'Productos de la bodega.';
        SET @O_Num = 0;
    END

    ELSE
    BEGIN
        SET @O_Msg = 'Modo no reconocido.';
        SET @O_Num = -1;
    END
END
GO