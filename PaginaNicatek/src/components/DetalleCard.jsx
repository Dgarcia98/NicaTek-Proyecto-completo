import React from 'react';

// Un componente simple para mostrar un título y un valor en una tarjeta
const DetalleCard = ({ titulo, valor, modoOscuro }) => {
    return (
        <div 
            className={`p-4 rounded-lg shadow-md ${
                modoOscuro 
                    ? 'bg-gray-800 text-gray-200 border border-gray-700' 
                    : 'bg-white text-gray-800 border border-gray-200'
            }`} // Estilos condicionales según el modo oscuro
        >
            <h3 className="text-sm font-semibold text-gray-400 uppercase tracking-wider mb-1">
                {titulo}
            </h3>
            <p className="text-lg font-bold">
                {valor}
            </p>
        </div>
    );
};

export default DetalleCard;
