const StatCard = ({ title, value, growth, icon, modoOscuro }) => {  // Agregado modoOscuro como prop
  const estilo = modoOscuro
    ? 'bg-gray-700 text-white'
    : 'bg-white text-gray-800';

  const mostrarCrecimiento = typeof growth === 'number' && !isNaN(growth) && growth > 0;

  return (
    <div className={`rounded shadow-md p-4 transition-colors duration-300 ${estilo}`}>
      <div className="flex justify-between items-center mb-2">
        <span className="text-xl">{icon}</span>
 
        {mostrarCrecimiento && ( // Condicional renderizado
          <span className="text-sm text-cyan-400"> 
            +{growth}% crecimiento
          </span>
        )}
      </div>
      <h3 className="text-lg font-semibold">{title}</h3>
      <p className="text-2xl font-bold">{isNaN(value) ? 0 : value}</p>
    </div>
  );
};

export default StatCard;
