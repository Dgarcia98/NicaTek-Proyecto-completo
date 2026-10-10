using System.ComponentModel.DataAnnotations;

namespace GRUPO_DDR_APPLICATION.DTOs
{
    /// <summary>
    /// Registro de una venta completa en UNA sola llamada.
    ///
    /// Reemplaza el flujo anterior, en el que el navegador creaba la
    /// cabecera y luego disparaba un POST por cada linea con
    /// Promise.allSettled. Si alguna linea fallaba quedaba una venta
    /// "Completado" con detalles parciales, stock parcialmente descontado
    /// y un total que no cuadraba, sin ninguna compensacion.
    ///
    /// Fijate en lo que NO esta aqui: ni Total, ni Precio_Unitario, ni
    /// Id_Creador. Los tres los pone el servidor:
    ///   - el precio se lee de Tbl_Producto
    ///   - el total se suma de los precios reales
    ///   - el creador sale del token
    /// </summary>
    public class VentaRegistroDTO
    {
        [Required(ErrorMessage = "El cliente es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Cliente invalido.")]
        public int Id_Cliente { get; set; }

        public int? Id_Metodo_Pago { get; set; }

        public DateTime? Fecha { get; set; }

        /// <summary>4 = Completado (mueve inventario), 5 = Borrador (no lo mueve).</summary>
        public int Id_Estado { get; set; } = 4;

        [Required(ErrorMessage = "La venta debe tener al menos un producto.")]
        [MinLength(1, ErrorMessage = "La venta debe tener al menos un producto.")]
        public List<DetalleVentaRegistroDTO> Detalles { get; set; } = new();
    }

    public class DetalleVentaRegistroDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Producto invalido.")]
        public int Id_Producto { get; set; }

        /// <summary>Null = el servidor elige la bodega con mas stock.</summary>
        public int? Id_Bodega { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; }
    }

    /// <summary>
    /// Registro de una compra completa en una sola transaccion.
    /// Aqui el precio unitario SI lo aporta el cliente: el costo se negocia
    /// con el proveedor y no existe en el catalogo. El subtotal y el total
    /// igual se recalculan en el servidor.
    /// </summary>
    public class CompraRegistroDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Proveedor invalido.")]
        public int Id_Proveedor { get; set; }

        [Required(ErrorMessage = "El numero de factura del proveedor es obligatorio.")]
        [StringLength(50)]
        public string Numero_Factura_Proveedor { get; set; } = string.Empty;

        public DateTime? Fecha_Compra { get; set; }

        public int Id_Estado { get; set; } = 4;

        [Required(ErrorMessage = "La compra debe tener al menos un producto.")]
        [MinLength(1, ErrorMessage = "La compra debe tener al menos un producto.")]
        public List<DetalleCompraRegistroDTO> Detalles { get; set; } = new();
    }

    public class DetalleCompraRegistroDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Producto invalido.")]
        public int Id_Producto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Bodega invalida.")]
        public int Id_Bodega { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; }

        [Range(0, 9999999999.99, ErrorMessage = "Precio de compra invalido.")]
        public decimal Precio_Unitario_Compra { get; set; }
    }

    /// <summary>Resultado de un registro transaccional.</summary>
    public class ResultadoRegistroDTO
    {
        public bool Exito { get; set; }
        public int Id { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
