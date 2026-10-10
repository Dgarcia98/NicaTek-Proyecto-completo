import { useEffect, useState } from 'react';
import api from '../../api/axios.Config'; 
import { useParams } from 'react-router-dom';
import { useOutletContext } from 'react-router-dom';
import CardProducto from '../../components/CardProducto';

const BodegaDetalle = () => {
  const { modoOscuro } = useOutletContext();
  const { categoria } = useParams();
  const [productos, setProductos] = useState([]);

  useEffect(() => {
    const cargarInventario = async () => {
      try {
        const res = await api.get('/Bodega/ListarBodega');
        
        const datos = res.data;

        
        if (Array.isArray(datos)) {
        
          const filtrados = datos.filter(p => p.nombre_Categoria === categoria);
          setProductos(filtrados);
        }
      } catch (error) {
        console.error("Error al cargar el inventario de la categoría:", error.message);
      }
    };

    cargarInventario();
  }, [categoria]);

  return (
    <section className={`p-6 space-y-6 transition-colors duration-300 ${modoOscuro ? 'text-white' : 'text-gray-800'}`}>
      <h2 className={`text-2xl font-bold ${modoOscuro ? 'text-white' : 'text-black'}`}>
        Productos en {categoria}
      </h2>

      {productos.length === 0 ? (
        <div className="text-center text-gray-500 py-10">No hay productos registrados en esta categoría.</div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {productos.map(p => (
            <CardProducto
              key={`${p.id_Producto}-${p.id_Bodega}`} 
              producto={{
                id: p.id_Producto,
                nombre: p.nombre_Producto,
                cantidad: p.cantidad,
                precio: p.precio_Producto,
                marca: p.nombre_Marca,
                modelo: p.nombre_Modelo, 
                categoria: p.nombre_Categoria,
              }}
              modoOscuro={modoOscuro}
            />
          ))}
        </div>
      )}
    </section>
  );
};

export default BodegaDetalle;