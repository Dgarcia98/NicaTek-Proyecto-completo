const CardProducto = ({ producto, modoOscuro }) => { // Componente para mostrar la información de un producto
  const {
    nombre,
    precio,
    proveedor,
    categoria,
  } = producto;

  const tieneNombre = nombre && nombre.trim() !== ""; // Verifica si el nombre existe y no está vacío
  const tienePrecio = typeof precio === 'number' && !isNaN(precio); // Verifica si el precio es un número válido
  const tieneProveedor = proveedor && proveedor.trim() !== ""; // Verifica si el proveedor existe y no está vacío
  const tieneCategoria = categoria && categoria.trim() !== ""; // Verifica si la categoría existe y no está vacía

  const fondo = modoOscuro ? 'bg-gray-800' : 'bg-white';
  const textoPrincipal = modoOscuro ? 'text-white' : 'text-gray-800';
  const textoLabel = modoOscuro ? 'text-gray-300' : 'text-gray-500';
  const borde = modoOscuro ? 'border-gray-600' : 'border-gray-200';

  return ( // Retorna el componente CardProducto
    <div className={`border ${borde} rounded-lg shadow p-4 hover:shadow-md transition duration-200 ${fondo} ${textoPrincipal}`}>
      {tieneNombre && (
        <h3 className="text-xl font-semibold mb-2">{nombre}</h3>
      )}

      {tienePrecio && ( // Verifica si el precio es un número válido
        <p className="text-sm mb-1">
          <span className={`font-medium ${textoLabel}`}>Precio:</span>{" "}
          {precio.toLocaleString('en-US', { style: 'currency', currency: 'USD' })}
        </p>
      )}

      {tieneProveedor && ( // Verifica si el proveedor existe y no está vacío
        <p className="text-sm mb-1">
          <span className={`font-medium ${textoLabel}`}>Proveedor:</span> {proveedor}
        </p>
      )}

      {tieneCategoria && (
        <p className="text-sm">
          <span className={`font-medium ${textoLabel}`}>Categoría:</span> {categoria}
        </p>
      )}
    </div>
  );
};

export default CardProducto;
