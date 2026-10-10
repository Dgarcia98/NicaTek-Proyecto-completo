import { useState } from 'react';
import { ChevronUp, ChevronDown, ChevronsUpDown } from 'lucide-react';

const TablaConTarjetas = ({ encabezados, filas, modoOscuro }) => {
  const [sortCol, setSortCol] = useState(null);
  const [sortDir, setSortDir] = useState('asc');

  const handleSort = (i) => {
    if (sortCol === i) {
      setSortDir(d => d === 'asc' ? 'desc' : 'asc');
    } else {
      setSortCol(i);
      setSortDir('asc');
    }
  };

  // Guardamos el índice original de cada fila ANTES de ordenar
  const filasConIndice = filas.map((fila, originalIdx) => ({ fila, originalIdx }));

  const filasOrdenadas = sortCol === null
    ? filasConIndice
    : [...filasConIndice].sort((a, b) => {
        const va = a.fila[sortCol];
        const vb = b.fila[sortCol];
        if (typeof va === 'object' && va !== null && !Array.isArray(va)) return 0;
        const na = parseFloat(va);
        const nb = parseFloat(vb);
        if (!isNaN(na) && !isNaN(nb)) return sortDir === 'asc' ? na - nb : nb - na;
        return sortDir === 'asc'
          ? String(va ?? '').localeCompare(String(vb ?? ''))
          : String(vb ?? '').localeCompare(String(va ?? ''));
      });

  return (
    <div className="overflow-x-auto w-full">
      <table className="table-auto w-full border-separate border-spacing-y-2 text-sm">
        <thead>
          <tr className={`${modoOscuro ? 'bg-gray-800 text-gray-200' : 'bg-gray-100 text-gray-700'}`}>
            {encabezados.map((text, i) => (
              <th key={i} className="px-4 py-2 text-left">
                <button
                  onClick={() => handleSort(i)}
                  className="flex items-center gap-1 hover:text-cyan-500 transition-colors"
                >
                  {text}
                  {sortCol === i
                    ? sortDir === 'asc'
                      ? <ChevronUp size={14} />
                      : <ChevronDown size={14} />
                    : <ChevronsUpDown size={14} className="opacity-40" />
                  }
                </button>
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {filasOrdenadas.length > 0 ? (
            filasOrdenadas.map(({ fila, originalIdx }) => (
              // key usa el índice ORIGINAL — React preserva los closures JSX correctamente
              <tr
                key={`row-orig-${originalIdx}`}
                className={`transition duration-200 rounded-lg shadow-sm relative z-[1] ${
                  modoOscuro
                    ? 'bg-gray-900 hover:shadow-lg hover:bg-gray-800'
                    : 'bg-white hover:shadow-lg hover:bg-gray-50'
                }`}
              >
                {fila.map((valor, i) => (
                  <td key={i} className="px-4 py-3">{valor}</td>
                ))}
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan={encabezados.length}
                  className={`px-4 py-2 text-center italic ${
                    modoOscuro ? 'text-gray-400 bg-gray-900' : 'text-gray-600 bg-white'
                  }`}>
                No hay datos disponibles.
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
};

export default TablaConTarjetas;