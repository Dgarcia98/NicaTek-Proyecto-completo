import { useEffect, useState } from 'react';
import { useOutletContext, useNavigate } from 'react-router-dom';
import { Plus, ShoppingBag, Eye, Search, X, ArrowUp, ArrowDown, Ban } from 'lucide-react';
import Swal from 'sweetalert2';
import TablaConTarjetas from '../../components/TablaConTarjetas';
import FormularioAgregarCompra from '../Compras/FormularioAgregarCompra';
import api from '../../api/axios.Config';
import { tienePermiso } from '../../utils/Permisos';

// Badge de estado reutilizable
const EstadoBadge = ({ estado }) => {
    const cfg = {
        'Completado': 'bg-green-100 text-green-800 border-green-200',
        'Borrador':   'bg-blue-100  text-blue-800  border-blue-200',
        'Anulado':    'bg-red-100   text-red-800   border-red-200',
    };
    return (
        <span className={`px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full border ${cfg[estado] || 'bg-gray-100 text-gray-800 border-gray-200'}`}>
            {estado || '—'}
        </span>
    );
};

const Compras = () => {
    const { modoOscuro } = useOutletContext();
    const navigate = useNavigate();

    const puedeComprar = tienePermiso('POS_SALE');
    const puedeAnular  = tienePermiso('INV_DELETE');
    const idUsuario    = parseInt(sessionStorage.getItem('idUsuario') || '1');

    const [compras,  setCompras]  = useState([]);
    const [cargando, setCargando] = useState(true);
    const [vista,    setVista]    = useState('lista');
    const [filtro,   setFiltro]   = useState('');
    const [ordenId,  setOrdenId]  = useState('desc');

    const obtenerDatos = async (terminoFiltro = '') => {
        setCargando(true);
        try {
            const endpoint = terminoFiltro.trim()
                ? `/Compra/ListarPorFiltro/${encodeURIComponent(terminoFiltro.trim())}`
                : '/Compra/ListarCompra';
            const res  = await api.get(endpoint);
            const data = res.data;
            let arr    = Array.isArray(data) ? data : (data?.exito && Array.isArray(data.datos) ? data.datos : []);
            setCompras(arr.sort((a, b) => ordenId === 'desc' ? b.id_Compra - a.id_Compra : a.id_Compra - b.id_Compra));
        } catch (error) {
            console.error('Error al obtener compras:', error.message);
            setCompras([]);
        } finally { setCargando(false); }
    };

    useEffect(() => {
        if (vista !== 'lista') return;
        const t = setTimeout(() => obtenerDatos(filtro), 500);
        return () => clearTimeout(t);
    }, [vista, filtro]);

    const toggleOrdenId = () => {
        const nuevo = ordenId === 'desc' ? 'asc' : 'desc';
        setOrdenId(nuevo);
        setCompras(prev => [...prev].sort((a, b) => nuevo === 'desc' ? b.id_Compra - a.id_Compra : a.id_Compra - b.id_Compra));
    };

    const handleAnularCompra = async (compra) => {
        if (!puedeAnular) return;
        const result = await Swal.fire({
            title: `¿Anular compra #${compra.id_Compra}?`,
            text: "Los productos se restarán del inventario y la compra pasará a Anulado.",
            icon: 'warning', showCancelButton: true,
            confirmButtonColor: '#d33', cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, anular', cancelButtonText: 'Cancelar'
        });
        if (!result.isConfirmed) return;
        try {
            await api.delete(`/Compra/EliminarCompra/${compra.id_Compra}/${idUsuario}`);
            Swal.fire('¡Anulada!', 'La compra ha sido anulada y el inventario ajustado.', 'success');
            obtenerDatos(filtro);
        } catch (error) {
            Swal.fire('Error', error.response?.data?.mensaje || 'Hubo un problema al procesar la anulación.', 'error');
        }
    };

    const filasFormateadas = compras.map(compra => {
        const estaAnulado   = compra.estado === 'Anulado';
        const estaBorrador  = compra.estado === 'Borrador';
        const puedeAnularFila = !estaAnulado && puedeAnular;

        return [
            compra.id_Compra,
            compra.proveedor,
            compra.numero_Factura_Proveedor,
            new Date(compra.fecha_Compra).toLocaleDateString(),
            `$${compra.total_Compra !== undefined ? compra.total_Compra.toFixed(2) : '0.00'}`,
            <EstadoBadge key={`est-${compra.id_Compra}`} estado={compra.estado} />,
            new Date(compra.fecha_Creacion).toLocaleDateString(),
            <div key={`acc-${compra.id_Compra}`} className="flex space-x-2">
                <button onClick={() => navigate(`/compras/resumen/${compra.id_Compra}`)}
                    className="text-gray-500 hover:text-cyan-600 transition-colors" title="Ver resumen">
                    <Eye size={20} />
                </button>
                <button
                    onClick={() => puedeAnularFila && handleAnularCompra(compra)}
                    disabled={!puedeAnularFila}
                    title={!puedeAnular ? 'Sin permiso' : estaAnulado ? 'Ya está anulada' : estaBorrador ? 'Anular borrador' : 'Anular compra'}
                    className={`${!puedeAnularFila ? 'text-gray-300 cursor-not-allowed opacity-60' : 'text-red-500 hover:text-red-700 transition-colors'}`}>
                    <Ban size={20} />
                </button>
            </div>
        ];
    });

    const encabezadosDinamicos = [
        <div key="header-id" onClick={toggleOrdenId}
            className="flex items-center cursor-pointer hover:text-cyan-500 transition-colors select-none">
            ID {ordenId === 'desc' ? <ArrowDown size={16} className="ml-1" /> : <ArrowUp size={16} className="ml-1" />}
        </div>,
        'Proveedor', 'Factura', 'Fecha', 'Total', 'Estado', 'Fecha de creación', 'Acciones'
    ];

    if (vista === 'agregar') {
        return <FormularioAgregarCompra
            onSuccess={() => { setVista('lista'); setFiltro(''); }}
            onCancel={() => { setVista('lista'); setFiltro(''); }} />;
    }

    return (
        <div className={`p-6 ${modoOscuro ? 'bg-gray-900 text-gray-100' : 'bg-transparent text-gray-800'}`}>
            <div className="flex justify-between items-center mb-6">
                <h1 className={`text-3xl font-bold flex items-center ${modoOscuro ? 'text-white' : 'text-black'}`}>
                    <ShoppingBag className="mr-3 text-cyan-500" size={28} /> Compras
                </h1>
                <button onClick={() => puedeComprar && setVista('agregar')}
                    disabled={!puedeComprar}
                    className={`flex items-center px-4 py-2 font-semibold rounded-lg shadow-md transition-all transform ${
                        puedeComprar ? 'bg-cyan-600 text-white hover:bg-cyan-500 hover:scale-105' : 'bg-gray-300 text-gray-500 cursor-not-allowed'
                    }`}>
                    <Plus size={20} className="mr-2" /> Agregar Compra
                </button>
            </div>

            <div className="mb-6 relative">
                <input type="text" placeholder="Buscar por número de factura o producto..."
                    value={filtro} onChange={(e) => setFiltro(e.target.value)}
                    className={`w-full pl-10 pr-10 py-3 border rounded-xl shadow-sm focus:ring-2 focus:ring-cyan-500 focus:outline-none transition-all ${
                        modoOscuro ? 'bg-gray-800 border-gray-700 text-gray-100 placeholder-gray-400' : 'bg-white border-gray-200 text-gray-900 placeholder-gray-400'
                    }`} />
                <Search className={`absolute left-3 top-1/2 -translate-y-1/2 ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`} size={20} />
                {filtro && <button onClick={() => setFiltro('')} className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-red-500"><X size={20} /></button>}
            </div>

            {cargando ? (
                <div className="flex flex-col justify-center items-center h-48 space-y-4">
                    <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-cyan-500"></div>
                    <p className={`${modoOscuro ? 'text-gray-400' : 'text-gray-600'} animate-pulse font-medium`}>Cargando compras...</p>
                </div>
            ) : compras.length === 0 ? (
                <div className="text-center py-20 text-gray-500">No se encontraron compras.</div>
            ) : (
                <TablaConTarjetas encabezados={encabezadosDinamicos} filas={filasFormateadas} modoOscuro={modoOscuro} />
            )}
        </div>
    );
};

export default Compras;