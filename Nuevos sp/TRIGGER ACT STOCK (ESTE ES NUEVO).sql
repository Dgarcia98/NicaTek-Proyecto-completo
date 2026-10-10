USE NicaTek;
GO

CREATE OR ALTER TRIGGER tr_ActualizarStock_Desde_Kardex
ON Tbl_Movimientos_Inventario
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Actualiza stock en Tbl_Bodega_Producto (no en Tbl_Bodega directamente)
    UPDATE bp
    SET
        bp.Cantidad = bp.Cantidad + (
            CASE
                WHEN tm.Operacion = 'SUMA'  THEN i.Cantidad
                WHEN tm.Operacion = 'RESTA' THEN (i.Cantidad * -1)
                ELSE 0
            END
        ),
        bp.Fecha_Modificacion = GETDATE()
    FROM Tbl_Bodega_Producto bp
    INNER JOIN inserted i              ON bp.Id_Bodega_Producto = i.Id_Bodega_Producto
    INNER JOIN Cls_Tipo_Movimiento tm  ON i.Id_Tipo_Movimiento  = tm.Id_Tipo_Movimiento;
END
GO

-- Reactivar trigger
ENABLE TRIGGER tr_ActualizarStock_Desde_Kardex ON Tbl_Movimientos_Inventario;
GO