import { HiOfficeBuilding } from 'react-icons/hi';
import { FiPackage, FiLayers } from 'react-icons/fi';
import { TbStack3 } from 'react-icons/tb';

const CardBodega = ({ bodega, modoOscuro, onVerProductos }) => { // Componente para mostrar la información de una bodega
  return (
    <div // Retorna el componente CardBodega
      className={`rounded-lg shadow-md p-4 space-y-3 border transition-colors duration-300 
      ${modoOscuro ? 'bg-gray-800 text-white border-gray-700' : 'bg-white text-gray-800 border-gray-200'}`} // Estilos condicionales según el modo oscuro
    >
      <h3 className="text-lg font-bold flex items-center gap-2">
        <HiOfficeBuilding className="text-xl" />
        {bodega.nombre_Bodega}
      </h3>

      <div className="text-sm space-y-1"> {/* Información detallada de la bodega */}
        <p className="flex items-center gap-2">
          <FiPackage /> <strong>ID Bodega:</strong> {bodega.id_Bodega}
        </p>
        <p className="flex items-center gap-2">
          <FiLayers /> <strong>Productos únicos:</strong> {bodega.total_productos}
        </p>
        <p className="flex items-center gap-2">
          <TbStack3 /> <strong>Cantidad total:</strong> {bodega.cantidad}
        </p>
      </div>

      <button
        onClick={() => onVerProductos(bodega)} // Llama a la función pasada como prop al hacer clic
        className="text-sm text-blue-500 hover:underline pt-2"
      >
        Ver productos en esta bodega →
      </button>
    </div>
  );
};

export default CardBodega;
