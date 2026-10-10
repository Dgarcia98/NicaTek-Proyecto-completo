import { useEffect, useState } from 'react';
import { useOutletContext, useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, ShoppingBag, Printer,Edit } from 'lucide-react';
import api from '../../api/axios.Config';

const FilaDato = ({ label, valor, modoOscuro }) => (
    <div className="flex flex-wrap gap-x-2 py-2">
        <span className={`font-bold text-sm min-w-[160px] ${modoOscuro ? 'text-gray-200' : 'text-gray-800'}`}>{label}:</span>
        <span className={`text-sm ${modoOscuro ? 'text-gray-300' : 'text-gray-700'}`}>{valor}</span>
    </div>
);

const EstadoBadge = ({ estado, print = false }) => {
    const cfg = {
        'Completado': { cls: 'bg-green-100 text-green-700 border-green-200', print: 'COMPLETADO' },
        'Borrador':   { cls: 'bg-blue-100  text-blue-700  border-blue-200',  print: 'BORRADOR'   },
        'Anulado':    { cls: 'bg-red-100   text-red-700   border-red-200',   print: 'ANULADO'    },
        'Activo':     { cls: 'bg-green-100 text-green-700 border-green-200', print: 'COMPLETADO' },
    };
    const item = cfg[estado] || { cls: 'bg-gray-100 text-gray-700 border-gray-200', print: estado?.toUpperCase() || '—' };
    return (
        <span className={`px-3 py-1 text-xs font-bold rounded-full border ${item.cls}`}>
            {print ? item.print : (estado === 'Activo' ? 'Completado' : estado) || '—'}
        </span>
    );
};

const ResumenCompra = () => {
    const { compraId: id } = useParams();
    const navigate         = useNavigate();
    const { modoOscuro }   = useOutletContext();

    const [compra,           setCompra]           = useState(null);
    const [cargando,         setCargando]         = useState(true);
    const [detalleProductos, setDetalleProductos] = useState([]);
    const [cargandoDetalle,  setCargandoDetalle]  = useState(true);
    const [errorDetalle,     setErrorDetalle]     = useState(null);
    const [empresa,          setEmpresa]          = useState(null);
    const [nombreCreador,    setNombreCreador]    = useState('');

    useEffect(() => {
        api.get('/Configuracion/Obtener')
            .then(res => setEmpresa(res.data))
            .catch(err => console.error(err));
    }, []);

    useEffect(() => {
        if (!id) return;
        const fetch = async () => {
            try {
                setCargando(true);
                const res = await api.get(`/Compra/ObtenerCompraPorId/${id}`);
                setCompra(res.data);
            } catch (err) {
                setCompra(null);
            } finally { setCargando(false); }
        };
        fetch();
    }, [id]);

    useEffect(() => {
        if (!compra?.id_Creador) return;
        api.get('/Usuario/ListarUsuarios')
            .then(res => {
                const lista = Array.isArray(res.data) ? res.data : [];
                const user  = lista.find(u => u.id_Usuario === compra.id_Creador);
                setNombreCreador(user?.nombre_Persona || user?.nombre_Usuario || `Usuario #${compra.id_Creador}`);
            })
            .catch(() => setNombreCreador(`Usuario #${compra.id_Creador}`));
    }, [compra?.id_Creador]);

    useEffect(() => {
        if (!compra?.id_Compra) { setCargandoDetalle(false); return; }
        const fetch = async () => {
            setCargandoDetalle(true);
            try {
                const res  = await api.get(`/DetalleCompra/ListarPorFiltro/${compra.id_Compra}`);
                const data = res.data;
                let prods  = Array.isArray(data) ? data : (data.exito && Array.isArray(data.datos) ? data.datos : []);
                setDetalleProductos(prods.sort((a, b) => new Date(b.fecha_Creacion || 0) - new Date(a.fecha_Creacion || 0)));
            } catch (err) {
                setErrorDetalle(err.response?.data?.mensaje || err.message || 'Error al obtener detalles.');
            } finally { setCargandoDetalle(false); }
        };
        fetch();
    }, [compra?.id_Compra]);

    if (cargando) return (
        <div className="flex justify-center items-center py-32">
            <div className="w-8 h-8 border-2 border-cyan-500 border-t-transparent rounded-full animate-spin" />
        </div>
    );

    if (!compra) return (
        <div className={`p-6 ${modoOscuro ? 'bg-gray-900 text-gray-100' : 'bg-white text-gray-800'}`}>
            <button onClick={() => navigate('/compras')} className="flex items-center gap-2 mb-4 hover:text-cyan-500">
                <ArrowLeft size={20} />
            </button>
            <p className="text-center text-red-500">No se encontró la información de la compra.</p>
        </div>
    );

    const moneda   = empresa?.moneda_Simbolo || 'C$';
    const ruc      = empresa?.ruC_No || empresa?.ruc_No || '';
    const fmt      = (f) => f ? new Date(f).toLocaleDateString('es-NI', { day: '2-digit', month: '2-digit', year: 'numeric' }) : 'N/A';
    const fmtMonto = (n) => n !== undefined && n !== null
        ? Number(n).toLocaleString('es-NI', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) : '0.00';

    const getUsuarioJWT = () => {
        try {
            const token   = sessionStorage.getItem('token');
            if (!token)   return sessionStorage.getItem('nombreUsuario') || 'N/A';
            const payload = JSON.parse(atob(token.split('.')[1]));
            return payload.nombre_Usuario || payload.unique_name || payload.name || payload.sub || 'N/A';
        } catch { return 'N/A'; }
    };
    const creadoPor = nombreCreador || getUsuarioJWT();

    return (
        <>
            <style>{`
                @media print {
                    @page { size: A4; margin: 14mm; }
                    html, body { height: auto !important; overflow: visible !important; background: white !important; }
                    body * { visibility: hidden; }
                    #print-area, #print-area * { visibility: visible; }
                    #print-area { position: absolute; left: 0; top: 0; width: 100%; font-size: 11px; color: black !important; }
                    .no-print { display: none !important; }
                    #print-area table { width: 100%; border-collapse: collapse; }
                    #print-area th, #print-area td { border: 1px solid #d1d5db; padding: 4px 8px; font-size: 11px; }
                    #print-area thead th { background-color: #f3f4f6 !important; -webkit-print-color-adjust: exact; print-color-adjust: exact; }
                    .print-block { page-break-inside: avoid; }
                }
            `}</style>

            <div className={`min-h-screen p-4 sm:p-6 ${modoOscuro ? 'bg-gray-900 text-gray-100' : 'bg-gray-100 text-gray-800'}`}>

                {/* Barra superior */}
                <div className="flex justify-between items-center mb-6 no-print">
                    <div className="flex items-center gap-3">
                        <button onClick={() => navigate(-1)}
                            className={`p-2 rounded-full transition-colors ${modoOscuro ? 'hover:bg-gray-700' : 'hover:bg-gray-300'}`}>
                            <ArrowLeft size={22} />
                        </button>
                        <h1 className="text-xl font-bold flex items-center gap-2">
                            <ShoppingBag size={22} className="text-cyan-500" />
                            Resumen de Compra
                            <span className="text-cyan-500">#{compra.id_Compra}</span>
                        </h1>
                    </div>
                    <div className="flex items-center gap-3">
                        <EstadoBadge estado={compra.estado} />

                        {/* Botón Editar — solo si es Borrador */}
{compra.id_Estado === 5 && (
    <button onClick={() => navigate(`/compras/editar/${compra.id_Compra}`)}
        className={`flex items-center gap-2 px-4 py-2 text-sm font-medium rounded-lg border transition-colors
            ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
        <Edit size={16} /> Editar Borrador
    </button>
)}


                        <button onClick={() => window.print()}
                            className="flex items-center gap-2 px-4 py-2 bg-cyan-600 hover:bg-cyan-700 text-white text-sm font-medium rounded-lg transition-colors shadow-sm">
                            <Printer size={16} /> Imprimir
                        </button>
                    </div>
                </div>

                {/* Documento */}
                <div id="print-area" className={`w-full max-w-3xl mx-auto rounded-xl shadow-md ${modoOscuro ? 'bg-gray-800 text-gray-100' : 'bg-white text-gray-800'}`}>
                    <div className="px-8 py-8">

                        {/* Encabezado empresa */}
                        <div className="flex flex-col sm:flex-row sm:justify-between sm:items-start gap-4 mb-6 print-block">
                            <div>
                                <p className="text-xl font-bold">{empresa?.nombre_Empresa ?? '—'}</p>
                                <p className={`text-sm ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>RUC: {ruc || '—'}</p>
                                <p className={`text-sm ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>{empresa?.direccion}</p>
                                {empresa?.telefono       && <p className={`text-sm ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>Tel: {empresa.telefono}</p>}
                                {empresa?.correo_Empresa && <p className={`text-sm ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>{empresa.correo_Empresa}</p>}
                            </div>
                            <div className="sm:text-right shrink-0">
                                <p className={`text-xs font-bold uppercase tracking-widest ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>Comprobante de Compra</p>
                                <p className="text-3xl font-extrabold text-cyan-500">#{compra.id_Compra}</p>
                                <p className={`text-sm ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>{fmt(compra.fecha_Compra)}</p>
                                <div className="mt-2">
                                    <EstadoBadge estado={compra.estado} print />
                                </div>
                            </div>
                        </div>

                        <hr className={`mb-6 ${modoOscuro ? 'border-gray-600' : 'border-gray-200'}`} />

                        {/* Datos de la compra */}
                        <div className="mb-6 print-block">
                            <FilaDato label="Proveedor"         valor={compra.proveedor || 'N/A'}                modoOscuro={modoOscuro} />
                            <FilaDato label="Factura"           valor={compra.numero_Factura_Proveedor || 'N/A'} modoOscuro={modoOscuro} />
                            <FilaDato label="Fecha de Compra"   valor={fmt(compra.fecha_Compra)}                 modoOscuro={modoOscuro} />
                            <FilaDato label="Fecha de Registro" valor={fmt(compra.fecha_Creacion)}               modoOscuro={modoOscuro} />
                            <FilaDato label="Creado por"        valor={creadoPor}                                modoOscuro={modoOscuro} />
                            <FilaDato label="Total"             valor={`${moneda}${fmtMonto(compra.total_Compra)}`} modoOscuro={modoOscuro} />
                        </div>

                        {/* Tabla productos */}
                        <div className="print-block">
                            <p className="text-base font-bold mb-3">Detalle de Compra</p>
                            {cargandoDetalle ? (
                                <p className={`text-center py-6 text-sm ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>Cargando productos...</p>
                            ) : errorDetalle ? (
                                <p className="text-center text-red-500 py-6 text-sm">{errorDetalle}</p>
                            ) : detalleProductos.length > 0 ? (
                                <div className="overflow-x-auto">
                                    <table className={`min-w-full divide-y ${modoOscuro ? 'divide-gray-600' : 'divide-gray-200'}`}>
                                        <thead className={modoOscuro ? 'bg-gray-700' : 'bg-gray-50'}>
                                            <tr>
                                                {['Producto', 'Cantidad', 'Precio Unitario', 'Subtotal'].map(h => (
                                                    <th key={h} className={`px-4 py-3 text-left text-[10px] font-bold uppercase tracking-wider ${modoOscuro ? 'text-gray-300' : 'text-gray-500'}`}>{h}</th>
                                                ))}
                                            </tr>
                                        </thead>
                                        <tbody className={`divide-y ${modoOscuro ? 'divide-gray-700' : 'divide-gray-100'}`}>
                                            {detalleProductos.map((item, i) => (
                                                <tr key={i} className={modoOscuro ? 'hover:bg-gray-700/40' : 'hover:bg-gray-50'}>
                                                    <td className="px-4 py-3 text-sm font-medium">{item.producto || 'N/A'}</td>
                                                    <td className="px-4 py-3 text-sm">{item.cantidad ?? 'N/A'}</td>
                                                    <td className="px-4 py-3 text-sm">{item.precio_Unitario_Compra !== undefined ? `${moneda}${fmtMonto(item.precio_Unitario_Compra)}` : 'N/A'}</td>
                                                    <td className="px-4 py-3 text-sm">{item.subtotal_Compra !== undefined ? `${moneda}${fmtMonto(item.subtotal_Compra)}` : 'N/A'}</td>
                                                </tr>
                                            ))}
                                        </tbody>
                                        <tfoot>
                                            <tr className={modoOscuro ? 'bg-gray-700' : 'bg-gray-50'}>
                                                <td colSpan={3} className={`px-4 py-3 text-right text-xs font-bold uppercase ${modoOscuro ? 'text-gray-300' : 'text-gray-500'}`}>Total</td>
                                                <td className="px-4 py-3 text-sm font-extrabold text-cyan-500">{moneda}{fmtMonto(compra.total_Compra)}</td>
                                            </tr>
                                        </tfoot>
                                    </table>
                                </div>
                            ) : (
                                <p className={`text-center py-6 text-sm ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                                    {compra.id_Estado === 5
                                        ? 'Esta compra está en borrador. Complétala para ver los productos.'
                                        : 'No se encontraron productos para esta compra.'}
                                </p>
                            )}
                        </div>

                        {/* Pie de página */}
                        {empresa && (
                            <div className={`mt-8 pt-4 border-t text-center text-xs ${modoOscuro ? 'border-gray-600 text-gray-500' : 'border-gray-200 text-gray-400'}`}>
                                <p>{empresa.nombre_Empresa} · RUC: {ruc} · {empresa.direccion}</p>
                                <p>Documento generado el {new Date().toLocaleDateString('es-NI')}</p>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </>
    );
};

export default ResumenCompra;