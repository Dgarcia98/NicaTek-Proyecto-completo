import { useState, useEffect } from 'react';
import { useParams, useNavigate, useOutletContext } from 'react-router-dom';
import { Save, X, Plus, Trash2, ArrowLeft, ShoppingBag, LoaderCircle } from 'lucide-react';
import { toast } from 'react-hot-toast';
import Swal from 'sweetalert2';
import api from '../../api/axios.Config';

const EditarCompra = () => {
    const { compraId }   = useParams();
    const navigate       = useNavigate();
    const { modoOscuro } = useOutletContext();

    const idUsuario = parseInt(sessionStorage.getItem('idUsuario') || '1');

    const [cargando,  setCargando]  = useState(true);
    const [guardando, setGuardando] = useState(false);
    const [compra,    setCompra]    = useState(null);

    const [proveedores, setProveedores] = useState([]);
    const [bodegas,     setBodegas]     = useState([]);
    const [productos,   setProductos]   = useState([]);

    const [form, setForm] = useState({
        id_Proveedor:             '',
        numero_Factura_Proveedor: '',
        fecha_Compra:             '',
    });

    // Borrador: solo cabecera, sin detalles hasta completar
    const [detallesNuevos, setDetallesNuevos] = useState([]);

    useEffect(() => {
        const cargar = async () => {
            try {
                setCargando(true);
                const [resCompra, resProv, resProd, resB] = await Promise.all([
                    api.get(`/Compra/ObtenerCompraPorId/${compraId}`),
                    api.get('/Proveedores/listar'),
                    api.get('/Producto/ListarProducto'),
                    api.get('/Bodega/ListarBodega'),
                ]);

                const c = resCompra.data;

                if (c.id_Estado !== 5) {
                    toast.error('Solo se pueden editar compras en estado Borrador.');
                    navigate(`/compras/resumen/${compraId}`, { replace: true });
                    return;
                }

                setCompra(c);
                setForm({
                    id_Proveedor:             c.id_Proveedor             || '',
                    numero_Factura_Proveedor: c.numero_Factura_Proveedor || '',
                    fecha_Compra:             c.fecha_Compra ? c.fecha_Compra.split('T')[0] : '',
                });

                setProveedores(Array.isArray(resProv.data) ? resProv.data : (resProv.data?.datos || []));
                setProductos(Array.isArray(resProd.data) ? resProd.data.filter(p => p.id_Estado === 1) : []);
                setBodegas(Array.isArray(resB.data) ? resB.data : []);

            } catch (err) {
                toast.error('Error al cargar la compra.');
                navigate('/compras', { replace: true });
            } finally { setCargando(false); }
        };
        cargar();
    }, [compraId, navigate]);

    const totalNuevos = detallesNuevos.reduce((s, d) => s + (parseFloat(d.Subtotal) || 0), 0);
    const totalCompra = totalNuevos;

    const agregarLineaNueva = () =>
        setDetallesNuevos(prev => [...prev, { Id_Bodega: '', Id_Producto: '', Cantidad: 1, Precio_Unitario: 0, Subtotal: 0 }]);

    const eliminarLineaNueva = (index) =>
        setDetallesNuevos(prev => prev.filter((_, i) => i !== index));

    const actualizarLineaNueva = (index, campo, valor) => {
        setDetallesNuevos(prev => {
            const copia = [...prev];
            copia[index] = { ...copia[index], [campo]: valor };
            if (campo === 'Id_Bodega') copia[index].Id_Producto = '';
            if (campo === 'Cantidad' || campo === 'Precio_Unitario') {
                const cant   = parseFloat(campo === 'Cantidad'        ? valor : copia[index].Cantidad)        || 0;
                const precio = parseFloat(campo === 'Precio_Unitario' ? valor : copia[index].Precio_Unitario) || 0;
                copia[index].Subtotal = cant * precio;
            }
            return copia;
        });
    };

    // ── Payload cabecera ──────────────────────────────────────────────────────
    const buildPayloadCabecera = (idEstado) => {
        const provSel = proveedores.find(p => p.id_Proveedor === parseInt(form.id_Proveedor));
        return {
            Id_Compra:                parseInt(compraId),
            Id_Proveedor:             parseInt(form.id_Proveedor),
            Proveedor:                provSel?.nombre_Completo || compra?.proveedor || '',
            Numero_Factura_Proveedor: form.numero_Factura_Proveedor,
            Fecha_Compra:             new Date(form.fecha_Compra).toISOString(),
            Total_Compra:             totalCompra || compra?.total_Compra || 0,
            Id_Estado:                idEstado,
            Estado:                   idEstado === 4 ? 'Completado' : 'Borrador',
            Fecha_Creacion:           compra?.fecha_Creacion || new Date().toISOString(),
            Fecha_Modificacion:       new Date().toISOString(),
            Id_Creador:               compra?.id_Creador || idUsuario,
            Id_Modificador:           idUsuario,
        };
    };

    // ── Guardar Borrador (solo cabecera, sin productos) ───────────────────────
    const guardarBorrador = async () => {
        if (!form.id_Proveedor || !form.numero_Factura_Proveedor || !form.fecha_Compra) {
            toast.error('Completa todos los campos de cabecera.'); return;
        }
        try {
            setGuardando(true);
            await api.put(`/Compra/EditarCompra/${compraId}`, buildPayloadCabecera(5));
            toast.success('Borrador actualizado.');
            navigate(`/compras/resumen/${compraId}`, { replace: true });
        } catch (err) {
            toast.error(err.response?.data?.msj || 'Error al guardar.');
        } finally { setGuardando(false); }
    };

    // ── Completar Compra (actualiza cabecera + inserta detalles → mueve stock) ─
    const completarCompra = async () => {
        if (!form.id_Proveedor || !form.numero_Factura_Proveedor || !form.fecha_Compra) {
            toast.error('Completa todos los campos de cabecera.'); return;
        }
        if (detallesNuevos.length === 0) {
            toast.error('Agrega al menos un producto antes de completar.'); return;
        }
        const validos = detallesNuevos.every(d =>
            d.Id_Bodega && d.Id_Producto && Number(d.Cantidad) > 0 && Number(d.Precio_Unitario) > 0
        );
        if (!validos) { toast.error('Completa todas las líneas de productos.'); return; }

        const result = await Swal.fire({
            title: '¿Completar compra?',
            text: 'El stock se actualizará y la compra pasará a Completado.',
            icon: 'question', showCancelButton: true,
            confirmButtonColor: '#0891b2', cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, completar', cancelButtonText: 'Cancelar',
        });
        if (!result.isConfirmed) return;

        try {
            setGuardando(true);

            // 1. Actualizar cabecera a Completado
            await api.put(`/Compra/EditarCompra/${compraId}`, buildPayloadCabecera(4));

            // 2. Insertar detalles (SP mueve stock via trigger)
            await Promise.all(detallesNuevos.map(d => {
                const prodSel = productos.find(p => p.id_Producto === parseInt(d.Id_Producto));
                return api.post('/DetalleCompra/NuevoDetalleCompra', {
                    Id_Detalle_Compra:      0,
                    Id_Compra:              parseInt(compraId),
                    Id_Bodega:              parseInt(d.Id_Bodega),
                    Id_Producto:            parseInt(d.Id_Producto),
                    Producto:               prodSel?.nombre || '',
                    Cantidad:               parseInt(d.Cantidad),
                    Precio_Unitario_Compra: parseFloat(d.Precio_Unitario),
                    Subtotal_Compra:        parseFloat(d.Subtotal),
                    Fecha_Creacion:         new Date().toISOString(),
                    Fecha_Modificacion:     new Date().toISOString(),
                    Id_Creador:             idUsuario,
                    Id_Modificador:         idUsuario,
                });
            }));

            await Swal.fire({
                icon: 'success', title: '¡Compra completada!',
                text: 'El stock fue actualizado correctamente.',
                confirmButtonColor: '#0891b2', timer: 2000, timerProgressBar: true,
            });
            navigate('/compras', { replace: true });
        } catch (err) {
            toast.error(err.response?.data?.msj || 'Error al completar.');
        } finally { setGuardando(false); }
    };

    const inputCls = `w-full border rounded-md p-2 text-sm focus:ring-cyan-500 focus:border-cyan-500 outline-none transition-colors
        ${modoOscuro ? 'bg-gray-700 border-gray-600 text-white placeholder-gray-400' : 'bg-white border-gray-300 text-gray-900'}`;
    const labelCls = `block text-xs font-medium mb-1 ${modoOscuro ? 'text-gray-300' : 'text-gray-600'}`;

    if (cargando) return (
        <div className="flex justify-center items-center py-32">
            <LoaderCircle size={36} className="animate-spin text-cyan-500" />
        </div>
    );

    return (
        <div className={`p-6 ${modoOscuro ? 'bg-gray-900 text-gray-100' : 'bg-transparent text-gray-800'}`}>

            {/* Header */}
            <div className="flex justify-between items-center mb-6">
                <h1 className={`text-2xl font-bold flex items-center gap-2 ${modoOscuro ? 'text-white' : 'text-black'}`}>
                    <ShoppingBag size={24} className="text-cyan-500" />
                    Editar Compra Borrador <span className="text-cyan-500">#{compraId}</span>
                </h1>
                <button onClick={() => navigate(`/compras/resumen/${compraId}`, { replace: true })}
                    className={`flex items-center gap-2 px-4 py-2 rounded-lg border font-medium transition-all hover:scale-105
                        ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                    <ArrowLeft size={18} /> Volver
                </button>
            </div>

            <div className={`rounded-2xl shadow-md border p-6 mb-5 ${modoOscuro ? 'bg-gray-800 border-gray-700' : 'bg-white border-gray-200'}`}>

                {/* ── Cabecera ──────────────────────────────────────────── */}
                <h2 className={`text-base font-semibold mb-4 pb-3 border-b ${modoOscuro ? 'text-gray-100 border-gray-700' : 'text-gray-700 border-gray-100'}`}>
                    Datos de la Compra
                </h2>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-5 mb-6">
                    <div>
                        <label className={labelCls}>Proveedor *</label>
                        <select value={form.id_Proveedor}
                            onChange={e => setForm(f => ({ ...f, id_Proveedor: e.target.value }))}
                            className={inputCls}>
                            <option value="">Selecciona un proveedor</option>
                            {proveedores.map(p => (
                                <option key={p.id_Proveedor} value={p.id_Proveedor}>{p.nombre_Completo}</option>
                            ))}
                        </select>
                    </div>
                    <div>
                        <label className={labelCls}>No. Factura Proveedor *</label>
                        <input type="text" value={form.numero_Factura_Proveedor}
                            onChange={e => setForm(f => ({ ...f, numero_Factura_Proveedor: e.target.value }))}
                            className={inputCls} placeholder="ej: FAC-0001" />
                    </div>
                    <div>
                        <label className={labelCls}>Fecha de Compra *</label>
                        <input type="date" value={form.fecha_Compra}
                            onChange={e => setForm(f => ({ ...f, fecha_Compra: e.target.value }))}
                            className={inputCls} />
                    </div>
                </div>

                {/* ── Agregar productos (solo se insertan al completar) ──── */}
                <div className="flex justify-between items-center mb-3">
                    <div>
                        <h2 className={`text-base font-semibold ${modoOscuro ? 'text-gray-100' : 'text-gray-700'}`}>
                            Productos
                        </h2>
                        <p className={`text-xs mt-0.5 ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                            Los productos se registran en inventario al completar la compra.
                        </p>
                    </div>
                    <button type="button" onClick={agregarLineaNueva}
                        className="flex items-center gap-2 bg-cyan-600 text-white px-3 py-1.5 rounded-md text-sm hover:bg-cyan-700 transition">
                        <Plus size={15} /> Agregar línea
                    </button>
                </div>

                {detallesNuevos.length === 0 ? (
                    <div className={`text-center py-8 rounded-lg border-2 border-dashed ${modoOscuro ? 'border-gray-600 text-gray-400' : 'border-gray-200 text-gray-400'}`}>
                        <p className="text-sm">Sin productos aún. Agregalos antes de completar la compra.</p>
                    </div>
                ) : (
                    <div className="space-y-3">
                        {detallesNuevos.map((detalle, index) => (
                            <div key={index}
                                className={`grid grid-cols-1 md:grid-cols-12 gap-3 items-end p-3 rounded-lg
                                    ${modoOscuro ? 'bg-gray-700 border border-gray-600' : 'bg-white border border-gray-200'}`}>
                                <div className="md:col-span-3">
                                    <label className={labelCls}>Bodega *</label>
                                    <select value={detalle.Id_Bodega}
                                        onChange={e => actualizarLineaNueva(index, 'Id_Bodega', e.target.value)}
                                        className={inputCls}>
                                        <option value="">Selecciona bodega...</option>
                                        {bodegas.map(b => (
                                            <option key={b.id_Bodega} value={b.id_Bodega}>
                                                {b.nombre_Bodega || `Bodega #${b.id_Bodega}`}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                                <div className="md:col-span-3">
                                    <label className={labelCls}>Producto *</label>
                                    <select value={detalle.Id_Producto}
                                        onChange={e => actualizarLineaNueva(index, 'Id_Producto', e.target.value)}
                                        disabled={!detalle.Id_Bodega}
                                        className={`${inputCls} ${!detalle.Id_Bodega ? 'opacity-50 cursor-not-allowed' : ''}`}>
                                        <option value="">{detalle.Id_Bodega ? 'Selecciona producto...' : '— Elige bodega primero —'}</option>
                                        {detalle.Id_Bodega && productos.map(p => (
                                            <option key={p.id_Producto} value={p.id_Producto}>{p.nombre}</option>
                                        ))}
                                    </select>
                                </div>
                                <div className="md:col-span-2">
                                    <label className={labelCls}>Cantidad *</label>
                                    <input type="number" min="1" value={detalle.Cantidad}
                                        onChange={e => actualizarLineaNueva(index, 'Cantidad', e.target.value)}
                                        className={inputCls} />
                                </div>
                                <div className="md:col-span-2">
                                    <label className={labelCls}>Precio Unitario *</label>
                                    <input type="number" step="0.01" min="0.01" value={detalle.Precio_Unitario}
                                        onChange={e => actualizarLineaNueva(index, 'Precio_Unitario', e.target.value)}
                                        className={inputCls} />
                                </div>
                                <div className="md:col-span-2">
                                    <label className={labelCls}>Subtotal</label>
                                    <div className="flex items-center gap-1">
                                        <div className={`flex-1 border rounded-md p-2 text-right text-sm
                                            ${modoOscuro ? 'bg-gray-600 border-gray-500 text-gray-300' : 'bg-gray-100 border-gray-300 text-gray-600'}`}>
                                            {Number(detalle.Subtotal).toFixed(2)}
                                        </div>
                                        <button type="button" onClick={() => eliminarLineaNueva(index)}
                                            className="p-2 rounded-md text-red-400 hover:text-red-600">
                                            <Trash2 size={16} />
                                        </button>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                )}

                {/* Total */}
                {detallesNuevos.length > 0 && (
                    <div className="flex justify-end items-center gap-3 mt-4 pt-4 border-t border-dashed">
                        <span className={`text-base font-bold ${modoOscuro ? 'text-gray-100' : 'text-gray-800'}`}>Total:</span>
                        <span className="text-2xl font-bold text-cyan-500">
                            {Number(totalCompra).toLocaleString('es-NI', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                        </span>
                    </div>
                )}
            </div>

            {/* ── Botones ───────────────────────────────────────────────── */}
            <div className="flex justify-end gap-3">
                <button onClick={() => navigate(`/compras/resumen/${compraId}`, { replace: true })} disabled={guardando}
                    className={`flex items-center gap-2 px-4 py-2 rounded-md border transition
                        ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                    <X size={16} /> Cancelar
                </button>
                <button onClick={guardarBorrador} disabled={guardando}
                    className={`flex items-center gap-2 px-4 py-2 rounded-md border font-medium transition
                        ${modoOscuro ? 'border-gray-500 text-gray-300 hover:bg-gray-700' : 'border-gray-400 text-gray-600 hover:bg-gray-100'}
                        ${guardando ? 'opacity-60 cursor-not-allowed' : ''}`}>
                    {guardando ? <LoaderCircle size={16} className="animate-spin" /> : <Save size={16} />}
                    Guardar Borrador
                </button>
                <button onClick={completarCompra} disabled={guardando}
                    className={`flex items-center gap-2 px-4 py-2 bg-cyan-600 text-white rounded-md font-semibold hover:bg-cyan-700 transition
                        ${guardando ? 'opacity-60 cursor-not-allowed' : ''}`}>
                    {guardando ? <LoaderCircle size={16} className="animate-spin" /> : <Save size={16} />}
                    Completar Compra
                </button>
            </div>
        </div>
    );
};

export default EditarCompra;