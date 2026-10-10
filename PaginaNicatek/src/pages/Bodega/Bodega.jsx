import { useEffect, useState } from 'react';
import { useOutletContext, useNavigate } from 'react-router-dom';
import {
    Warehouse, Activity, Plus, Search, X, LoaderCircle,
    Save, ChevronDown, ChevronUp, Package
} from 'lucide-react';
import Swal from 'sweetalert2';
import TablaConTarjetas from '../../components/TablaConTarjetas';
import api from '../../api/axios.Config';
import { tienePermiso, esAdmin } from '../../utils/permisos';

// ── Buscador reutilizable ─────────────────────────────────────────────────────
const Buscador = ({ valor, onChange, placeholder, modoOscuro }) => (
    <div className={`flex items-center gap-2 px-3 py-2 rounded-lg border mb-4 w-full max-w-sm
        ${modoOscuro ? 'bg-gray-700 border-gray-600' : 'bg-white border-gray-300'}`}>
        <Search size={16} className="text-gray-400 flex-shrink-0" />
        <input
            type="text" placeholder={placeholder} value={valor}
            onChange={e => onChange(e.target.value)}
            className={`flex-1 text-sm bg-transparent outline-none
                ${modoOscuro ? 'text-gray-100 placeholder-gray-500' : 'text-gray-800 placeholder-gray-400'}`}
        />
        {valor && (
            <button onClick={() => onChange('')}>
                <X size={14} className="text-gray-400 hover:text-gray-600" />
            </button>
        )}
    </div>
);

const Bodega = () => {
    const { modoOscuro } = useOutletContext();
    const navigate       = useNavigate();

    const puedeLeer  = tienePermiso('INV_READ');
    const puedeAdmin = esAdmin() || tienePermiso('USR_MANAGE');
    const idUsuario  = parseInt(sessionStorage.getItem('idUsuario') || '1');

    const [tabActiva, setTabActiva] = useState('bodegas');

    // ── Bodegas (almacenes) ───────────────────────────────────────────────────
    const [bodegas,      setBodegas]      = useState([]);
    const [cargandoB,    setCargandoB]    = useState(false);
    const [filtroB,      setFiltroB]      = useState('');
    const [modalNuevo,   setModalNuevo]   = useState(false);
    const [modalEditar,  setModalEditar]  = useState(null);
    const [guardandoB,   setGuardandoB]   = useState(false);
    const [nombreNuevo,  setNombreNuevo]  = useState('');

    // ── Stock por bodega (Tbl_Bodega_Producto) ────────────────────────────────
    const [stockItems,     setStockItems]     = useState([]);
    const [cargandoStock,  setCargandoStock]  = useState(false);
    const [filtroStock,    setFiltroStock]    = useState('');
    const [bodegaAbierta,  setBodegaAbierta]  = useState(null);

    // ── Movimientos ───────────────────────────────────────────────────────────
    const [movimientos,  setMovimientos]  = useState([]);
    const [cargandoMov,  setCargandoMov]  = useState(false);
    const [filtroMov,    setFiltroMov]    = useState('');

    // ── Fetch bodegas ─────────────────────────────────────────────────────────
    const fetchBodegas = async (termino = '') => {
        try {
            setCargandoB(true);
            const ep = termino.trim()
                ? `/Bodega/ListarPorNombre/${encodeURIComponent(termino.trim())}`
                : '/Bodega/ListarBodega';
            const res = await api.get(ep);
            const data = Array.isArray(res.data) ? res.data : [];
            setBodegas([...data].sort((a, b) => a.id_Bodega - b.id_Bodega));
        } catch (err) { console.error(err); setBodegas([]);
        } finally { setCargandoB(false); }
    };

    // ── Fetch stock ───────────────────────────────────────────────────────────
    const fetchStock = async (termino = '') => {
        try {
            setCargandoStock(true);
            const ep = termino.trim()
                ? `/BodegaProducto/Filtrar/${encodeURIComponent(termino.trim())}`
                : '/BodegaProducto/Listar';
            const res = await api.get(ep);
            setStockItems(Array.isArray(res.data) ? res.data : []);
        } catch (err) { console.error(err); setStockItems([]);
        } finally { setCargandoStock(false); }
    };

    // ── Fetch movimientos ─────────────────────────────────────────────────────
    const fetchMovimientos = async (termino = '') => {
        try {
            setCargandoMov(true);
            const ep = termino.trim()
                ? `/MovimientoInventario/ListarPorFiltro/${encodeURIComponent(termino.trim())}`
                : '/MovimientoInventario/ListarMovimientos';
            const res = await api.get(ep);
            setMovimientos(Array.isArray(res.data) ? res.data : []);
        } catch (err) { console.error(err); setMovimientos([]);
        } finally { setCargandoMov(false); }
    };

    // ── Effects ───────────────────────────────────────────────────────────────
    useEffect(() => { if (puedeLeer) { fetchBodegas(); fetchStock(); } }, [puedeLeer]);
    useEffect(() => { if (tabActiva === 'movimientos' && puedeLeer) fetchMovimientos(); }, [tabActiva, puedeLeer]);
    useEffect(() => { const t = setTimeout(() => fetchBodegas(filtroB), 350);      return () => clearTimeout(t); }, [filtroB]);
    useEffect(() => { const t = setTimeout(() => fetchStock(filtroStock), 350);    return () => clearTimeout(t); }, [filtroStock]);
    useEffect(() => { const t = setTimeout(() => fetchMovimientos(filtroMov), 350); return () => clearTimeout(t); }, [filtroMov]);

    // ── Crear bodega ──────────────────────────────────────────────────────────
    const guardarNuevaBodega = async () => {
        if (!nombreNuevo.trim()) {
            Swal.fire({ icon: 'warning', title: 'El nombre es requerido', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardandoB(true);
            await api.post('/Bodega/NuevoBodega', {
                Id_Bodega:      0,
                Nombre_Bodega:  nombreNuevo.trim(),
                Id_Estado:      1,
                Id_Creador:     idUsuario,
                Id_Modificador: idUsuario,
            });
            setModalNuevo(false);
            setNombreNuevo('');
            await fetchBodegas(filtroB);
            Swal.fire({ icon: 'success', title: 'Bodega creada', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: err.response?.data?.msj || 'Error al crear bodega', confirmButtonColor: '#0891b2' });
        } finally { setGuardandoB(false); }
    };

    // ── Editar bodega ─────────────────────────────────────────────────────────
    const guardarEdicion = async () => {
        if (!modalEditar.nombre_Bodega?.trim()) {
            Swal.fire({ icon: 'warning', title: 'El nombre es requerido', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardandoB(true);
            await api.put(`/Bodega/EditarBodega/${modalEditar.id_Bodega}`, {
                Id_Bodega:      modalEditar.id_Bodega,
                Nombre_Bodega:  modalEditar.nombre_Bodega.trim(),
                Id_Estado:      modalEditar.id_Estado,
                Id_Modificador: idUsuario,
            });
            setModalEditar(null);
            await fetchBodegas(filtroB);
            Swal.fire({ icon: 'success', title: 'Bodega actualizada', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al guardar', confirmButtonColor: '#0891b2' });
        } finally { setGuardandoB(false); }
    };

    // ── Toggle estado bodega ──────────────────────────────────────────────────
    const toggleBodega = async (b, totalStock) => {
        if (!puedeAdmin) return;
        const esActivo    = b.id_Estado === 1;
        const nuevoEstado = esActivo ? 2 : 1;

        if (esActivo && totalStock > 0) {
            Swal.fire({
                icon: 'warning',
                title: 'Acción Bloqueada',
                text: `No puedes desactivar "${b.nombre_Bodega}" porque aún tiene ${totalStock} producto(s) en stock. Traslada o da salida a los productos primero.`,
                confirmButtonColor: '#0891b2'
            });
            return;
        }

        const result = await Swal.fire({
            icon: 'question', title: '¿Cambiar estado?',
            text: `"${b.nombre_Bodega}" pasará a ${esActivo ? 'Inactivo' : 'Activo'}.`,
            showCancelButton: true, confirmButtonColor: '#0891b2', cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, cambiar', cancelButtonText: 'Cancelar',
        });
        if (!result.isConfirmed) return;

        try {
            await api.put(`/Bodega/EditarBodega/${b.id_Bodega}`, {
                Id_Bodega:      b.id_Bodega,
                Nombre_Bodega:  b.nombre_Bodega,
                Id_Estado:      nuevoEstado,
                Id_Modificador: idUsuario,
            });
            await fetchBodegas(filtroB);
            Swal.fire({ icon: 'success', title: `Bodega ${nuevoEstado === 1 ? 'activada' : 'desactivada'}`, timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: err.response?.data?.msj || 'Error al cambiar estado', confirmButtonColor: '#0891b2' });
        }
    };

    // ── Productos de una bodega específica ────────────────────────────────────
    const productosEnBodega = (idBodega) =>
        stockItems.filter(s => s.id_Bodega === idBodega);

    // ── Ruta del movimiento — FIX: extrae solo el número de la referencia ─────
    const rutaMovimiento = (m) => {
        const rutas = { 1: 'compras', 2: 'ventas', 3: 'ventas', 4: 'compras', 5: 'ventas' };
        const modulo = rutas[m.id_Tipo_Movimiento];
        if (!modulo) return null;

        // Extrae solo dígitos: "#Venta #38" → "38", 69 → "69"
        const soloNumero = String(m.referencia).replace(/\D/g, '');
        if (!soloNumero) return null;

        return `/${modulo}/resumen/${soloNumero}`;
    };

    // ── Colores por tipo movimiento ───────────────────────────────────────────
    const colorTipo = (idTipo) => {
        const map = {
            1: 'bg-green-100 text-green-800 border-green-200',
            2: 'bg-blue-100 text-blue-800 border-blue-200',
            3: 'bg-cyan-100 text-cyan-800 border-cyan-200',
            4: 'bg-orange-100 text-orange-800 border-orange-200',
            5: 'bg-red-100 text-red-800 border-red-200',
            6: 'bg-purple-100 text-purple-800 border-purple-200',
            7: 'bg-pink-100 text-pink-800 border-pink-200',
            8: 'bg-yellow-100 text-yellow-800 border-yellow-200',
        };
        return map[idTipo] || 'bg-gray-100 text-gray-800 border-gray-200';
    };

    // ── Filas movimientos — FIX: muestra solo el número en la referencia ──────
    const filasMov = movimientos.map(m => {
        const ruta       = rutaMovimiento(m);
        const refNumero  = String(m.referencia).replace(/\D/g, '');

        return [
            m.id_Movimiento,
            m.fecha_Movimiento ? new Date(m.fecha_Movimiento).toLocaleDateString() : '—',
            m.producto || '—',
            <span key={`tmov-${m.id_Movimiento}`}
                className={`px-2 py-0.5 text-xs rounded-full border font-semibold ${colorTipo(m.id_Tipo_Movimiento)}`}>
                {m.tipo_Movimiento || '—'}
            </span>,
            m.cantidad,
            m.stock_Anterior ?? '—',
            m.stock_Actual   ?? '—',
            ruta ? (
                <button key={`ref-${m.id_Movimiento}`} onClick={() => navigate(ruta)}
                    className="text-cyan-500 hover:underline text-xs font-semibold">
                    #{refNumero}
                </button>
            ) : (
                <span key={`ref-${m.id_Movimiento}`}
                    className={`text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                    {m.referencia || '—'}
                </span>
            ),
            m.responsable || '—',
        ];
    });

    // ── Clases ────────────────────────────────────────────────────────────────
    const tabCls = (tab) =>
        `px-4 py-2 text-sm font-semibold border-b-2 transition ${tabActiva === tab
            ? 'border-cyan-500 text-cyan-500'
            : `border-transparent ${modoOscuro ? 'text-gray-400 hover:text-gray-200' : 'text-gray-500 hover:text-gray-700'}`}`;

    const inputCls = `w-full px-3 py-2 rounded-lg border text-sm outline-none focus:ring-2 focus:ring-cyan-400
        ${modoOscuro ? 'bg-gray-700 border-gray-600 text-gray-100' : 'bg-gray-50 border-gray-300 text-gray-800'}`;
    const labelCls = `block text-xs font-medium mb-1 ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`;

    // ── Sin permiso ───────────────────────────────────────────────────────────
    if (!puedeLeer) {
        return (
            <section className={`p-6 flex flex-col items-center justify-center h-64
                ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                <Warehouse size={48} className="mb-3 opacity-30" />
                <p className="text-lg font-semibold">Acceso restringido</p>
                <p className="text-sm">No tienes permiso para ver el inventario.</p>
            </section>
        );
    }

    return (
        <div className={`p-6 transition-colors duration-300
            ${modoOscuro ? 'bg-gray-800 text-white' : 'bg-gray-100 text-gray-800'}`}>

            {/* ── Header ─────────────────────────────────────────────────── */}
            <div className="flex items-center justify-between mb-5">
                <div className="flex items-center gap-2">
                    <Warehouse size={22} className="text-cyan-500" />
                    <h1 className="text-2xl font-bold">Bodega</h1>
                </div>
                {tabActiva === 'bodegas' && puedeAdmin && (
                    <button onClick={() => { setNombreNuevo(''); setModalNuevo(true); }}
                        className="flex items-center gap-2 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm font-semibold hover:bg-cyan-500 transition">
                        <Plus size={16} /> Nueva Bodega
                    </button>
                )}
            </div>

            {/* ── Tabs ───────────────────────────────────────────────────── */}
            <div className={`flex gap-1 border-b mb-5 ${modoOscuro ? 'border-gray-700' : 'border-gray-200'}`}>
                <button className={tabCls('bodegas')} onClick={() => setTabActiva('bodegas')}>
                    <span className="flex items-center gap-1"><Warehouse size={14} /> Bodegas</span>
                </button>
                <button className={tabCls('movimientos')} onClick={() => setTabActiva('movimientos')}>
                    <span className="flex items-center gap-1"><Activity size={14} /> Movimientos</span>
                </button>
            </div>

            {/* ── TAB BODEGAS ────────────────────────────────────────────── */}
            {tabActiva === 'bodegas' && (
                <>
                    <Buscador valor={filtroB} onChange={setFiltroB}
                        placeholder="Buscar bodega por nombre..." modoOscuro={modoOscuro} />

                    {cargandoB ? (
                        <div className="flex justify-center py-16">
                            <LoaderCircle size={32} className="animate-spin text-cyan-500" />
                        </div>
                    ) : bodegas.length === 0 ? (
                        <div className="text-center py-20 text-gray-500">No se encontraron bodegas.</div>
                    ) : (
                        <div className="space-y-3">
                            {bodegas.map(b => {
                                const esActivo   = b.id_Estado === 1;
                                const abierta    = bodegaAbierta === b.id_Bodega;
                                const productos  = productosEnBodega(b.id_Bodega);
                                const totalStock = productos.reduce((s, p) => s + (p.cantidad || 0), 0);

                                return (
                                    <div key={b.id_Bodega}
                                        className={`rounded-2xl border shadow-sm overflow-hidden transition
                                            ${modoOscuro ? 'bg-gray-700 border-gray-600' : 'bg-white border-gray-200'}`}>

                                        {/* Cabecera */}
                                        <div className="flex items-center justify-between px-5 py-4 gap-3">
                                            <div className="flex items-center gap-3 min-w-0">
                                                <Warehouse size={18} className="text-cyan-500 flex-shrink-0" />
                                                <div className="min-w-0">
                                                    <p className="font-bold text-sm truncate">{b.nombre_Bodega || '—'}</p>
                                                    <p className={`text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                                                        {b.total_Productos ?? productos.length} producto{(b.total_Productos ?? productos.length) !== 1 ? 's' : ''} · Stock total: {totalStock}
                                                    </p>
                                                </div>
                                            </div>

                                            <div className="flex items-center gap-2 flex-shrink-0">
                                                <span className={`px-2 py-0.5 text-xs rounded-full border font-semibold
                                                    ${esActivo ? 'bg-green-100 text-green-800 border-green-200' : 'bg-red-100 text-red-800 border-red-200'}`}>
                                                    {esActivo ? 'Activo' : 'Inactivo'}
                                                </span>
                                                {puedeAdmin && (
                                                    <>
                                                        <button onClick={() => setModalEditar({ ...b })}
                                                            className={`px-3 py-1.5 text-xs font-semibold rounded-lg border transition
                                                                ${modoOscuro ? 'border-gray-500 text-gray-300 hover:bg-gray-600' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                                                            Editar
                                                        </button>
                                                        <button onClick={() => toggleBodega(b, totalStock)}
                                                            className={`px-3 py-1.5 text-xs font-semibold rounded-lg border transition
                                                                ${esActivo ? 'border-red-300 text-red-400 hover:bg-red-50' : 'border-green-300 text-green-500 hover:bg-green-50'}`}>
                                                            {esActivo ? 'Desactivar' : 'Activar'}
                                                        </button>
                                                    </>
                                                )}
                                                <button onClick={() => setBodegaAbierta(abierta ? null : b.id_Bodega)}
                                                    className={`p-1.5 rounded-lg transition
                                                        ${modoOscuro ? 'hover:bg-gray-600 text-gray-300' : 'hover:bg-gray-100 text-gray-500'}`}>
                                                    {abierta ? <ChevronUp size={16} /> : <ChevronDown size={16} />}
                                                </button>
                                            </div>
                                        </div>

                                        {/* Productos expandibles */}
                                        {abierta && (
                                            <div className={`border-t px-5 py-4 ${modoOscuro ? 'border-gray-600' : 'border-gray-100'}`}>
                                                <Buscador valor={filtroStock} onChange={setFiltroStock}
                                                    placeholder="Buscar producto en bodega..." modoOscuro={modoOscuro} />

                                                {cargandoStock ? (
                                                    <div className="flex justify-center py-6">
                                                        <LoaderCircle size={24} className="animate-spin text-cyan-500" />
                                                    </div>
                                                ) : productos.length === 0 ? (
                                                    <div className={`text-center py-6 text-sm ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                                                        Esta bodega no tiene productos aún. Se agregan al registrar una compra.
                                                    </div>
                                                ) : (
                                                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
                                                        {productos.map(p => (
                                                            <div key={p.id_Bodega_Producto}
                                                                className={`rounded-xl border p-4 space-y-2
                                                                    ${modoOscuro ? 'bg-gray-600 border-gray-500' : 'bg-gray-50 border-gray-200'}`}>
                                                                <div className="flex items-start justify-between gap-2">
                                                                    <div className="min-w-0">
                                                                        <p className="font-semibold text-sm truncate">{p.nombre_Producto || '—'}</p>
                                                                        <p className={`text-xs truncate ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                                                                            {p.nombre_Marca || '—'} · {p.nombre_Modelo || '—'}
                                                                        </p>
                                                                    </div>
                                                                    <Package size={14} className="text-cyan-500 flex-shrink-0 mt-0.5" />
                                                                </div>
                                                                {p.nombre_Categoria && (
                                                                    <span className={`inline-block text-xs px-2 py-0.5 rounded-full border
                                                                        ${modoOscuro ? 'bg-gray-500 border-gray-400 text-gray-300' : 'bg-white border-gray-200 text-gray-500'}`}>
                                                                        {p.nombre_Categoria}
                                                                    </span>
                                                                )}
                                                                <div className={`flex items-center justify-between pt-2 border-t
                                                                    ${modoOscuro ? 'border-gray-500' : 'border-gray-100'}`}>
                                                                    <span className={`text-xs uppercase tracking-wide
                                                                        ${modoOscuro ? 'text-gray-400' : 'text-gray-400'}`}>Stock</span>
                                                                    <span className={`font-bold text-lg
                                                                        ${p.cantidad === 0 ? 'text-red-500' : 'text-green-500'}`}>
                                                                        {p.cantidad}
                                                                    </span>
                                                                </div>
                                                            </div>
                                                        ))}
                                                    </div>
                                                )}
                                            </div>
                                        )}
                                    </div>
                                );
                            })}
                        </div>
                    )}
                </>
            )}

            {/* ── TAB MOVIMIENTOS ────────────────────────────────────────── */}
            {tabActiva === 'movimientos' && (
                <>
                    <Buscador valor={filtroMov} onChange={setFiltroMov}
                        placeholder="Buscar por producto, tipo o referencia..." modoOscuro={modoOscuro} />
                    {cargandoMov ? (
                        <div className="flex justify-center py-16">
                            <LoaderCircle size={32} className="animate-spin text-cyan-500" />
                        </div>
                    ) : movimientos.length === 0 ? (
                        <div className="text-center py-20 text-gray-500">No se encontraron movimientos.</div>
                    ) : (
                        <TablaConTarjetas
                            encabezados={['ID','Fecha','Producto','Tipo','Cantidad','Stock Anterior','Stock Actual','Referencia','Responsable']}
                            filas={filasMov}
                            modoOscuro={modoOscuro}
                        />
                    )}
                </>
            )}

            {/* ── MODAL NUEVA BODEGA ──────────────────────────────────────── */}
            {modalNuevo && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm">
                    <div className={`w-full max-w-sm rounded-2xl shadow-xl p-6
                        ${modoOscuro ? 'bg-gray-800 text-gray-100' : 'bg-white text-gray-800'}`}>
                        <div className="flex items-center justify-between mb-5">
                            <h2 className="text-lg font-bold flex items-center gap-2">
                                <Warehouse size={18} className="text-cyan-500" /> Nueva Bodega
                            </h2>
                            <button onClick={() => setModalNuevo(false)}
                                className="p-1.5 rounded-lg hover:bg-gray-100 transition">
                                <X size={18} />
                            </button>
                        </div>
                        <div>
                            <label className={labelCls}>Nombre de la Bodega *</label>
                            <input
                                className={inputCls}
                                value={nombreNuevo}
                                onChange={e => setNombreNuevo(e.target.value)}
                                placeholder="ej: Bodega Principal, Almacén Norte..."
                                autoFocus
                                onKeyDown={e => e.key === 'Enter' && guardarNuevaBodega()}
                            />
                            <p className={`text-xs mt-2 ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                                ℹ Los productos se asignan a esta bodega al registrar compras.
                            </p>
                        </div>
                        <div className="flex justify-end gap-2 mt-6">
                            <button onClick={() => setModalNuevo(false)}
                                className={`px-4 py-2 rounded-lg text-sm border transition
                                    ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                                Cancelar
                            </button>
                            <button onClick={guardarNuevaBodega} disabled={guardandoB}
                                className="flex items-center gap-2 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition disabled:opacity-60">
                                {guardandoB ? <LoaderCircle size={14} className="animate-spin" /> : <Save size={14} />}
                                Crear
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* ── MODAL EDITAR BODEGA ─────────────────────────────────────── */}
            {modalEditar && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm">
                    <div className={`w-full max-w-sm rounded-2xl shadow-xl p-6
                        ${modoOscuro ? 'bg-gray-800 text-gray-100' : 'bg-white text-gray-800'}`}>
                        <div className="flex items-center justify-between mb-5">
                            <h2 className="text-lg font-bold flex items-center gap-2">
                                <Warehouse size={18} className="text-cyan-500" /> Editar Bodega
                            </h2>
                            <button onClick={() => setModalEditar(null)}
                                className="p-1.5 rounded-lg hover:bg-gray-100 transition">
                                <X size={18} />
                            </button>
                        </div>
                        <div>
                            <label className={labelCls}>Nombre de la Bodega *</label>
                            <input
                                className={inputCls}
                                value={modalEditar.nombre_Bodega || ''}
                                onChange={e => setModalEditar(m => ({ ...m, nombre_Bodega: e.target.value }))}
                                placeholder="ej: Bodega Principal"
                                autoFocus
                                onKeyDown={e => e.key === 'Enter' && guardarEdicion()}
                            />
                        </div>
                        <div className="flex justify-end gap-2 mt-6">
                            <button onClick={() => setModalEditar(null)}
                                className={`px-4 py-2 rounded-lg text-sm border transition
                                    ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                                Cancelar
                            </button>
                            <button onClick={guardarEdicion} disabled={guardandoB}
                                className="flex items-center gap-2 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition disabled:opacity-60">
                                {guardandoB ? <LoaderCircle size={14} className="animate-spin" /> : <Save size={14} />}
                                Guardar
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Bodega;