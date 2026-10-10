import { useState, useEffect } from 'react';
import { useParams, useNavigate, useOutletContext } from 'react-router-dom';
import { Save, X, Plus, Trash2, ArrowLeft, DollarSign, LoaderCircle, AlertTriangle } from 'lucide-react';
import { toast } from 'react-hot-toast';
import Swal from 'sweetalert2';
import api from '../../api/axios.Config';

const EditarVenta = () => {
    const { ventaId }    = useParams();
    const navigate       = useNavigate();
    const { modoOscuro } = useOutletContext();

    const idUsuario     = parseInt(sessionStorage.getItem('idUsuario') || '1');
    const nombreUsuario = sessionStorage.getItem('nombreUsuario') || 'Usuario del Sistema';

    const [cargando,        setCargando]        = useState(true);
    const [guardando,       setGuardando]       = useState(false);
    const [venta,           setVenta]           = useState(null);
    const [clientes,        setClientes]        = useState([]);
    const [productos,       setProductos]       = useState([]);
    const [metodosPago,     setMetodosPago]     = useState([]);
    const [bodegaProductos, setBodegaProductos] = useState([]);
    const [detallesNuevos,  setDetallesNuevos]  = useState([]);

    const [form, setForm] = useState({
        id_Cliente:     '',
        id_Metodo_Pago: '',
        fecha_Venta:    '',
    });

    useEffect(() => {
        const cargar = async () => {
            try {
                setCargando(true);
                const [resVenta, resCli, resProd, resMp, resBP] = await Promise.all([
                    api.get(`/Venta/ObtenerVentaPorId/${ventaId}`),
                    api.get('/Cliente/ListarClientes'),
                    api.get('/Producto/ListarProducto'),
                    api.get('/MetodoPago/ListarMetodoPago'),
                    api.get('/BodegaProducto/Listar'),
                ]);

                const v = resVenta.data;

                if (v.id_Estado !== 5) {
                    toast.error('Solo se pueden editar ventas en estado Borrador.');
                    navigate(`/ventas/resumen/${ventaId}`, { replace: true });
                    return;
                }

                setVenta(v);
                setForm({
                    id_Cliente:     v.id_Cliente     || '',
                    id_Metodo_Pago: v.id_Metodo_Pago || '',
                    fecha_Venta:    v.fecha ? v.fecha.split('T')[0] : '',
                });

                const extraer = (r) => Array.isArray(r) ? r : (r?.datos || r?.data || []);
                setClientes(extraer(resCli.data));
                setProductos(extraer(resProd.data));
                setMetodosPago(extraer(resMp.data));
                setBodegaProductos(extraer(resBP.data));

            } catch (err) {
                toast.error('Error al cargar la venta.');
                navigate('/ventas', { replace: true });
            } finally { setCargando(false); }
        };
        cargar();
    }, [ventaId, navigate]);

    const getBP = (idProducto) =>
        bodegaProductos.find(b => b.id_Producto === parseInt(idProducto)) || null;

    const totalNuevos = detallesNuevos.reduce((s, d) => s + (parseFloat(d.subtotal) || 0), 0);
    const totalVenta  = totalNuevos;

    const hayStockInsuficiente = detallesNuevos.some(d => {
        const bp = getBP(d.id_Producto);
        return bp && parseInt(d.cantidad) > bp.cantidad;
    });

    const agregarLineaNueva = () =>
        setDetallesNuevos(prev => [...prev, { id_Producto: '', cantidad: '', precio_Unitario: '', subtotal: 0 }]);

    const eliminarLineaNueva = (i) =>
        setDetallesNuevos(prev => prev.filter((_, idx) => idx !== i));

    const handleDetalleChange = (index, e) => {
        const { name, value } = e.target;
        setDetallesNuevos(prev => {
            const copia   = [...prev];
            const updated = { ...copia[index], [name]: value };
            if (name === 'id_Producto') {
                const prod = productos.find(p => (p.id_Producto || p.id)?.toString() === value);
                if (prod) { updated.precio_Unitario = prod.precio || 0; if (!updated.cantidad) updated.cantidad = 1; }
                else { updated.precio_Unitario = ''; updated.cantidad = ''; }
            }
            const cant   = parseFloat(updated.cantidad)       || 0;
            const precio = parseFloat(updated.precio_Unitario) || 0;
            updated.subtotal = cant > 0 && precio >= 0 ? cant * precio : 0;
            copia[index]     = updated;
            return copia;
        });
    };

    // ── Payload cabecera ──────────────────────────────────────────────────────
    const buildPayloadCabecera = (idEstado) => {
        const cliente = clientes.find(c => (c.id_Cliente || c.id)?.toString() === form.id_Cliente?.toString());
        return {
            Id_Venta:           parseInt(ventaId),
            Fecha:              new Date(form.fecha_Venta).toISOString(),
            Id_Cliente:         parseInt(form.id_Cliente),
            Cliente:            cliente?.nombre_Completo || venta?.cliente || '',
            Id_Empleado:        idUsuario,
            Empleado:           nombreUsuario,
            Id_Metodo_Pago:     parseInt(form.id_Metodo_Pago),
            Total:              totalVenta || venta?.total || 0,
            Id_Estado:          idEstado,
            Estado:             idEstado === 4 ? 'Completado' : 'Borrador',
            Fecha_Creacion:     venta?.fecha_Creacion || new Date().toISOString(),
            Fecha_Modificacion: new Date().toISOString(),
            Id_Creador:         venta?.id_Creador || idUsuario,
            Id_Modificador:     idUsuario,
        };
    };

    // ── Guardar Borrador (solo cabecera, sin productos) ───────────────────────
    const guardarBorrador = async () => {
        if (!form.id_Cliente || !form.id_Metodo_Pago || !form.fecha_Venta) {
            toast.error('Completa todos los campos de cabecera.'); return;
        }
        try {
            setGuardando(true);
            await api.put(`/Venta/EditarVenta/${ventaId}`, buildPayloadCabecera(5));
            toast.success('Borrador actualizado.');
            navigate(`/ventas/resumen/${ventaId}`, { replace: true });
        } catch (err) {
            toast.error(err.response?.data?.msj || 'Error al guardar.');
        } finally { setGuardando(false); }
    };

    // ── Completar Venta (cabecera + detalles → descuenta stock) ──────────────
    const completarVenta = async () => {
        if (!form.id_Cliente || !form.id_Metodo_Pago || !form.fecha_Venta) {
            toast.error('Completa todos los campos de cabecera.'); return;
        }
        if (detallesNuevos.length === 0) {
            toast.error('Agrega al menos un producto.'); return;
        }
        if (hayStockInsuficiente) {
            toast.error('Hay productos con stock insuficiente.'); return;
        }
        const validos = detallesNuevos.every(d =>
            d.id_Producto && parseFloat(d.cantidad) > 0 && d.precio_Unitario !== ''
        );
        if (!validos) { toast.error('Completa todas las líneas de productos.'); return; }

        const result = await Swal.fire({
            title: '¿Completar venta?',
            text: 'El stock se descontará y la venta pasará a Completado.',
            icon: 'question', showCancelButton: true,
            confirmButtonColor: '#2563eb', cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, completar', cancelButtonText: 'Cancelar',
        });
        if (!result.isConfirmed) return;

        try {
            setGuardando(true);

            // 1. Actualizar cabecera a Completado (4)
            await api.put(`/Venta/EditarVenta/${ventaId}`, buildPayloadCabecera(4));

            // 2. Insertar detalles (el SP descuenta stock)
            await Promise.all(detallesNuevos.map(d => {
                const prod = productos.find(p => (p.id_Producto || p.id)?.toString() === d.id_Producto);
                const bp   = getBP(d.id_Producto);
                return api.post('/DetalleVenta/NuevoDetalleVenta', {
                    id_Detalle:                           0,
                    id_Venta:                             parseInt(ventaId),
                    total_Venta_General:                  totalVenta,
                    id_Producto:                          parseInt(d.id_Producto),
                    producto:                             prod?.nombre || 'N/A',
                    id_Bodega:                            bp?.id_Bodega || null,
                    cantidad:                             parseInt(d.cantidad),
                    precio_Unitario_Al_Momento_Venta:     parseFloat(d.precio_Unitario) || 0,
                    subtotal:                             parseFloat(d.subtotal) || 0,
                    id_Estado:                            1,
                    estado:                               'Activo',
                    id_Tipo_Movimiento:                   2,
                    fecha_Creacion:                       new Date().toISOString(),
                    fecha_Modificacion:                   null,
                    id_Creador:                           idUsuario,
                    id_Modificador:                       null,
                });
            }));

            await Swal.fire({
                icon: 'success', title: '¡Venta completada!',
                text: 'El stock fue descontado correctamente.',
                confirmButtonColor: '#2563eb', timer: 2000, timerProgressBar: true,
            });
            navigate('/ventas', { replace: true });
        } catch (err) {
            toast.error(err.response?.data?.msj || 'Error al completar.');
        } finally { setGuardando(false); }
    };

    const inputCls = `w-full border rounded-md p-2 text-sm focus:ring-blue-500 focus:border-blue-500 outline-none transition-colors
        ${modoOscuro ? 'bg-gray-700 border-gray-600 text-white placeholder-gray-400' : 'bg-white border-gray-300 text-gray-900'}`;
    const labelCls = `block text-xs font-medium mb-1 ${modoOscuro ? 'text-gray-300' : 'text-gray-600'}`;

    if (cargando) return (
        <div className="flex justify-center items-center py-32">
            <LoaderCircle size={36} className="animate-spin text-blue-500" />
        </div>
    );

    return (
        <div className={`p-6 ${modoOscuro ? 'bg-gray-900 text-gray-100' : 'bg-transparent text-gray-800'}`}>

            {/* Header */}
            <div className="flex justify-between items-center mb-6">
                <h1 className={`text-2xl font-bold flex items-center gap-2 ${modoOscuro ? 'text-white' : 'text-black'}`}>
                    <DollarSign size={24} className="text-blue-500" />
                    Editar Venta Borrador <span className="text-blue-500">FAC-{String(ventaId).padStart(6, '0')}</span>
                </h1>
                <button onClick={() => navigate(`/ventas/resumen/${ventaId}`, { replace: true })}
                    className={`flex items-center gap-2 px-4 py-2 rounded-lg border font-medium transition-all hover:scale-105
                        ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                    <ArrowLeft size={18} /> Volver
                </button>
            </div>

            <div className={`rounded-2xl shadow-md border p-6 mb-5 ${modoOscuro ? 'bg-gray-800 border-gray-700' : 'bg-white border-gray-200'}`}>

                {/* ── Cabecera ──────────────────────────────────────────── */}
                <h2 className={`text-base font-semibold mb-4 pb-3 border-b ${modoOscuro ? 'text-gray-100 border-gray-700' : 'text-gray-700 border-gray-100'}`}>
                    Datos de la Venta
                </h2>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-5 mb-6">
                    <div>
                        <label className={labelCls}>Cliente *</label>
                        <select value={form.id_Cliente}
                            onChange={e => setForm(f => ({ ...f, id_Cliente: e.target.value }))}
                            className={inputCls}>
                            <option value="">Selecciona un cliente</option>
                            {clientes.map((c, i) => {
                                const id = c.id_Cliente || c.id || i;
                                return <option key={`cli-${id}`} value={id}>{c.nombre_Completo || c.nombre || 'Sin nombre'}</option>;
                            })}
                        </select>
                    </div>
                    <div>
                        <label className={labelCls}>Método de Pago *</label>
                        <select value={form.id_Metodo_Pago}
                            onChange={e => setForm(f => ({ ...f, id_Metodo_Pago: e.target.value }))}
                            className={inputCls}>
                            <option value="">Seleccionar método</option>
                            {metodosPago.map((mp, i) => {
                                const id = mp.id_Metodo_Pago || mp.id || i;
                                return <option key={`mp-${id}`} value={id}>{mp.descripcion || mp.nombre}</option>;
                            })}
                        </select>
                    </div>
                    <div>
                        <label className={labelCls}>Fecha de Venta *</label>
                        <input type="date" value={form.fecha_Venta}
                            onChange={e => setForm(f => ({ ...f, fecha_Venta: e.target.value }))}
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
                            El stock se descuenta al completar la venta.
                        </p>
                    </div>
                    <button onClick={agregarLineaNueva}
                        className="flex items-center gap-2 bg-blue-600 text-white px-3 py-1.5 rounded-md text-sm hover:bg-blue-700 transition">
                        <Plus size={15} /> Agregar línea
                    </button>
                </div>

                {detallesNuevos.length === 0 ? (
                    <div className={`text-center py-8 rounded-lg border-2 border-dashed ${modoOscuro ? 'border-gray-600 text-gray-400' : 'border-gray-200 text-gray-400'}`}>
                        <p className="text-sm">Sin productos aún. Agregalos antes de completar la venta.</p>
                    </div>
                ) : (
                    <div className="space-y-3">
                        {detallesNuevos.map((detalle, index) => {
                            const bp               = getBP(detalle.id_Producto);
                            const stock            = bp ? bp.cantidad : null;
                            const stockBajo        = stock !== null && stock <= 5;
                            const stockInsuficiente = stock !== null && parseInt(detalle.cantidad) > stock;
                            const sinBodega        = detalle.id_Producto && !bp;

                            return (
                                <div key={index}
                                    className={`grid grid-cols-1 md:grid-cols-4 gap-4 p-4 rounded-md
                                        ${modoOscuro ? 'bg-gray-700 border border-gray-600' : 'bg-white border border-gray-200'}`}>
                                    <div className="flex flex-col">
                                        <label className={labelCls}>Producto</label>
                                        <select name="id_Producto" value={detalle.id_Producto}
                                            onChange={e => handleDetalleChange(index, e)} className={inputCls}>
                                            <option value="">Selecciona producto</option>
                                            {productos.map((p, pi) => {
                                                const id     = p.id_Producto || p.id || pi;
                                                const bpItem = bodegaProductos.find(b => b.id_Producto === id);
                                                const st     = bpItem ? bpItem.cantidad : '?';
                                                return <option key={`prod-${id}`} value={id}>{p.nombre} (Stock: {st})</option>;
                                            })}
                                        </select>
                                        {sinBodega && <p className="text-xs text-red-500 flex items-center gap-1 mt-1"><AlertTriangle size={12} /> Sin bodega</p>}
                                        {!sinBodega && detalle.id_Producto && stockBajo && !stockInsuficiente &&
                                            <p className="text-xs text-yellow-600 flex items-center gap-1 mt-1"><AlertTriangle size={12} /> Stock bajo: {stock}</p>}
                                        {stockInsuficiente && <p className="text-xs text-red-500 flex items-center gap-1 mt-1"><AlertTriangle size={12} /> Insuficiente: {stock}</p>}
                                    </div>
                                    <div className="flex flex-col">
                                        <label className={labelCls}>Cantidad</label>
                                        <input name="cantidad" type="number" min="1" value={detalle.cantidad}
                                            onChange={e => handleDetalleChange(index, e)}
                                            className={`${inputCls} ${stockInsuficiente ? 'border-red-400' : ''}`} />
                                    </div>
                                    <div className="flex flex-col">
                                        <label className={labelCls}>Precio Unitario</label>
                                        <input name="precio_Unitario" type="number" value={detalle.precio_Unitario}
                                            onChange={e => handleDetalleChange(index, e)} className={inputCls} />
                                    </div>
                                    <div className="flex flex-col">
                                        <label className={labelCls}>Subtotal</label>
                                        <div className="flex items-center gap-2">
                                            <input type="number" readOnly
                                                value={detalle.subtotal === '' ? '' : Number(detalle.subtotal).toFixed(2)}
                                                className={`p-2 border rounded-md w-full cursor-not-allowed ${modoOscuro ? 'bg-gray-600 border-gray-500 text-gray-300' : 'bg-gray-200 border-gray-300'}`} />
                                            <button onClick={() => eliminarLineaNueva(index)}
                                                className="text-red-500 hover:text-red-700">
                                                <Trash2 size={20} />
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                )}

                {detallesNuevos.length > 0 && (
                    <div className="flex justify-end items-center gap-3 mt-4 pt-4 border-t border-dashed">
                        <span className={`text-base font-bold ${modoOscuro ? 'text-gray-100' : 'text-gray-800'}`}>Total:</span>
                        <span className="text-2xl font-bold text-blue-500">${Number(totalVenta).toFixed(2)}</span>
                    </div>
                )}
            </div>

            {/* ── Botones ───────────────────────────────────────────────── */}
            <div className="flex justify-end gap-3">
                <button onClick={() => navigate(`/ventas/resumen/${ventaId}`, { replace: true })} disabled={guardando}
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
                <button onClick={completarVenta} disabled={guardando || hayStockInsuficiente}
                    title={hayStockInsuficiente ? 'Corrige el stock insuficiente' : ''}
                    className={`flex items-center gap-2 px-4 py-2 text-white rounded-md font-semibold transition
                        ${guardando || hayStockInsuficiente ? 'bg-gray-400 cursor-not-allowed' : 'bg-blue-600 hover:bg-blue-700'}`}>
                    {guardando ? <LoaderCircle size={16} className="animate-spin" /> : <Save size={16} />}
                    {hayStockInsuficiente ? 'Stock insuficiente' : 'Completar Venta'}
                </button>
            </div>
        </div>
    );
};

export default EditarVenta;