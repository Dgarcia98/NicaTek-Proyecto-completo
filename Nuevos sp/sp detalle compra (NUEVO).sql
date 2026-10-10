USE NicaTek;
GO

CREATE OR ALTER PROC sp_Tbl_Detalle_Compra
    @Mode                   VARCHAR(3)    = NULL,
    @O_Msg                  VARCHAR(255)  = NULL OUTPUT,
    @O_Num                  INT           = NULL OUTPUT,
    @Id_Detalle_Compra      INT           = NULL,
    @Id_Compra              INT           = NULL,
    @Id_Bodega              INT           = NULL,
    @Id_Producto            INT           = NULL,
    @Cantidad               INT           = NULL,
    @Precio_Unitario_Compra DECIMAL(12,2) = NULL,
    @Subtotal_Compra        DECIMAL(12,2) = NULL,
    @Id_Creador             INT           = NULL,
    @Id_Modificador         INT           = NULL,
    @Filtro                 VARCHAR(100)  = NULL
AS
BEGIN
    SET NOCOUNT ON;


    IF @Mode = 'INS'
    BEGIN
        IF @Id_Compra IS NULL OR @Id_Producto IS NULL OR @Id_Bodega IS NULL
           OR @Cantidad IS NULL OR @Precio_Unitario_Compra IS NULL
           OR @Id_Creador IS NULL OR @Id_Modificador IS NULL
        BEGIN
            SET @O_Msg = 'Faltan datos obligatorios.';
            SET @O_Num = -1; RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM Tbl_Compra WHERE Id_Compra = @Id_Compra)
        BEGIN
            SET @O_Msg = 'La compra no existe.';
            SET @O_Num = -1; RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM Tbl_Producto 
                       WHERE Id_Producto = @Id_Producto AND Id_Estado = 1)
        BEGIN
            SET @O_Msg = 'El producto no existe o está inactivo.';
            SET @O_Num = -1; RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM Tbl_Bodega 
                       WHERE Id_Bodega = @Id_Bodega AND Id_Estado = 1)
        BEGIN
            SET @O_Msg = 'La bodega no existe o está inactiva.';
            SET @O_Num = -1; RETURN;
        END

        IF @Subtotal_Compra IS NULL
            SET @Subtotal_Compra = @Cantidad * @Precio_Unitario_Compra;

        -- Tipo movimiento: Compra a Proveedor
        DECLARE @Id_Tipo_Movimiento INT;
        SELECT TOP 1 @Id_Tipo_Movimiento = Id_Tipo_Movimiento
        FROM Cls_Tipo_Movimiento
        WHERE Descripcion = 'Compra a Proveedor';

        -- Buscar si ya existe el producto en esa bodega
        DECLARE @Id_Bodega_Producto INT;
        DECLARE @Stock_Anterior     INT;

        SELECT @Id_Bodega_Producto = Id_Bodega_Producto,
               @Stock_Anterior     = Cantidad
        FROM Tbl_Bodega_Producto
        WHERE Id_Bodega = @Id_Bodega AND Id_Producto = @Id_Producto;

        BEGIN TRY
            BEGIN TRAN;

            -- Si no existe la combinación bodega+producto, crearla con cantidad 0
            IF @Id_Bodega_Producto IS NULL
            BEGIN
                INSERT INTO Tbl_Bodega_Producto (
                    Id_Bodega, Id_Producto, Cantidad,
                    Fecha_Creacion, Fecha_Modificacion,
                    Id_Creador, Id_Modificador
                )
                VALUES (
                    @Id_Bodega, @Id_Producto, 0,
                    GETDATE(), GETDATE(),
                    @Id_Creador, @Id_Modificador
                );
                SET @Id_Bodega_Producto = SCOPE_IDENTITY();
                SET @Stock_Anterior     = 0;
            END

            DECLARE @Stock_Actual INT = @Stock_Anterior + @Cantidad;

            -- Insertar detalle compra
            INSERT INTO Tbl_Detalle_Compra (
                Id_Compra, Id_Producto, Id_Bodega, Cantidad,
                Precio_Unitario_Compra, Subtotal_Compra,
                Fecha_Creacion, Fecha_Modificacion,
                Id_Creador, Id_Modificador
            )
            VALUES (
                @Id_Compra, @Id_Producto, @Id_Bodega, @Cantidad,
                @Precio_Unitario_Compra, @Subtotal_Compra,
                GETDATE(), GETDATE(),
                @Id_Creador, @Id_Modificador
            );
            SET @O_Num = SCOPE_IDENTITY();

            -- Registrar movimiento — el trigger actualiza Tbl_Bodega_Producto
            INSERT INTO Tbl_Movimientos_Inventario (
                Id_Producto, Id_Bodega, Id_Bodega_Producto,
                Id_Tipo_Movimiento, Cantidad,
                Stock_Anterior, Stock_Actual,
                Costo_Unitario, Referencia, Observaciones,
                Fecha_Movimiento, Id_Usuario_Responsable
            )
            VALUES (
                @Id_Producto, @Id_Bodega, @Id_Bodega_Producto,
                @Id_Tipo_Movimiento, @Cantidad,
                @Stock_Anterior, @Stock_Actual,
                @Precio_Unitario_Compra,
                CAST(@Id_Compra AS VARCHAR),
                'Entrada por compra',
                GETDATE(), @Id_Creador
            );

            SET @O_Msg = 'Detalle registrado y stock actualizado.';
            COMMIT;
        END TRY
        BEGIN CATCH
            ROLLBACK;
            SET @O_Msg = ERROR_MESSAGE();
            SET @O_Num = ERROR_NUMBER();
            THROW;
        END CATCH
    END

    ELSE IF @Mode = 'UPD'
    BEGIN
        IF @Id_Detalle_Compra IS NULL
        BEGIN
            SET @O_Msg = 'Id_Detalle_Compra requerido.';
            SET @O_Num = -1; RETURN;
        END

        DECLARE @NuevaCantidad INT;
        DECLARE @NuevoPrecio   DECIMAL(12,2);

        SELECT @NuevaCantidad = COALESCE(@Cantidad,               Cantidad),
               @NuevoPrecio   = COALESCE(@Precio_Unitario_Compra, Precio_Unitario_Compra)
        FROM Tbl_Detalle_Compra
        WHERE Id_Detalle_Compra = @Id_Detalle_Compra;

        IF @Subtotal_Compra IS NULL
            SET @Subtotal_Compra = @NuevaCantidad * @NuevoPrecio;

        BEGIN TRY
            BEGIN TRAN;
            UPDATE Tbl_Detalle_Compra
            SET Cantidad               = COALESCE(@Cantidad,               Cantidad),
                Precio_Unitario_Compra = COALESCE(@Precio_Unitario_Compra, Precio_Unitario_Compra),
                Subtotal_Compra        = @Subtotal_Compra,
                Fecha_Modificacion     = GETDATE(),
                Id_Modificador         = COALESCE(@Id_Modificador,         Id_Modificador)
            WHERE Id_Detalle_Compra = @Id_Detalle_Compra;
            SET @O_Num = @Id_Detalle_Compra;
            SET @O_Msg = 'Detalle actualizado.';
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
        IF @Id_Detalle_Compra IS NULL
        BEGIN
            SET @O_Msg = 'Id_Detalle_Compra requerido.';
            SET @O_Num = -1; RETURN;
        END

        BEGIN TRY
            BEGIN TRAN;
            DELETE FROM Tbl_Detalle_Compra
            WHERE Id_Detalle_Compra = @Id_Detalle_Compra;
            SET @O_Num = @Id_Detalle_Compra;
            SET @O_Msg = 'Detalle eliminado.';
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
            dc.Id_Detalle_Compra,
            dc.Id_Compra,
            c.Total_Compra,
            dc.Id_Producto,
            p.Nombre        AS Producto,
            dc.Id_Bodega,
            b.Nombre_Bodega,
            dc.Cantidad,
            dc.Precio_Unitario_Compra,
            dc.Subtotal_Compra,
            dc.Fecha_Creacion,
            dc.Fecha_Modificacion,
            dc.Id_Creador,
            dc.Id_Modificador
        FROM Tbl_Detalle_Compra dc
        JOIN Tbl_Compra  c ON dc.Id_Compra  = c.Id_Compra
        JOIN Tbl_Producto p ON dc.Id_Producto = p.Id_Producto
        JOIN Tbl_Bodega  b ON dc.Id_Bodega  = b.Id_Bodega;

        SET @O_Msg = 'Listado de detalle de compras.';
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
            dc.Id_Detalle_Compra,
            dc.Id_Compra,
            c.Total_Compra,
            dc.Id_Producto,
            p.Nombre        AS Producto,
            dc.Id_Bodega,
            b.Nombre_Bodega,
            dc.Cantidad,
            dc.Precio_Unitario_Compra,
            dc.Subtotal_Compra,
            dc.Fecha_Creacion,
            dc.Fecha_Modificacion,
            dc.Id_Creador,
            dc.Id_Modificador
        FROM Tbl_Detalle_Compra dc
        JOIN Tbl_Compra  c ON dc.Id_Compra   = c.Id_Compra
        JOIN Tbl_Producto p ON dc.Id_Producto = p.Id_Producto
        JOIN Tbl_Bodega  b ON dc.Id_Bodega   = b.Id_Bodega
        WHERE CAST(dc.Id_Detalle_Compra AS VARCHAR) = @Filtro
           OR CAST(dc.Id_Compra         AS VARCHAR) = @Filtro
           OR p.Nombre LIKE '%' + @Filtro + '%'
           OR b.Nombre_Bodega LIKE '%' + @Filtro + '%';

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