import { useState, useEffect, useMemo } from 'react';
import { useOutletContext } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { Save, X, Plus, Trash2, AlertTriangle, FileText } from 'lucide-react';
import { toast } from 'react-hot-toast';
import api, { mensajeDeError } from '../../api/axios.Config';

const LINEA_VACIA = { id_Producto: '', id_Bodega: '', cantidad: '' };

const FormularioAgregarVenta = ({ onSuccess, onCancel }) => {
    const { modoOscuro } = useOutletContext();
    const { register, handleSubmit, formState: { errors } } = useForm();

    const [clientes,        setClientes]        = useState([]);
    const [productos,       setProductos]       = useState([]);
    const [metodosPago,     setMetodosPago]     = useState([]);
    const [bodegaProductos, setBodegaProductos] = useState([]);
    const [detallesVenta,   setDetallesVenta]   = useState([{ ...LINEA_VACIA }]);
    const [cargandoDatos,   setCargandoDatos]   = useState(true);
    const [isSubmitting,    setIsSubmitting]    = useState(false);

    const nombreUsuario = sessionStorage.getItem('nombreUsuario') || 'Usuario del Sistema';

    useEffect(() => {
        const fetchDatos = async () => {
            setCargandoDatos(true);
            try {
                // Ya no se piden todas las ventas: se usaban solo para
                // calcular el proximo numero de factura con Math.max(ids)+1,
                // que ademas daba el MISMO numero a dos cajeros que abrieran
                // el formulario a la vez. El numero real lo genera la base
                // (columna calculada Numero_Factura) y llega en la respuesta.
                const [resClientes, resProductos, resMetodosPago, resBodegaProductos] = await Promise.all([
                    api.get('/Cliente/ListarClientes'),
                    api.get('/Producto/ListarProducto'),
                    api.get('/MetodoPago/ListarMetodoPago'),
                    api.get('/BodegaProducto/Listar'),
                ]);
                const extraer = (r) => Array.isArray(r) ? r : (r?.datos || r?.data || []);
                setClientes(extraer(resClientes.data));
                setProductos(extraer(resProductos.data));
                setMetodosPago(extraer(resMetodosPago.data));
                setBodegaProductos(extraer(resBodegaProductos.data));
            } catch (error) {
                toast.error(mensajeDeError(error, 'Error de conexión al cargar datos.'));
            } finally { setCargandoDatos(false); }
        };
        fetchDatos();
    }, []);

    // ── Inventario por producto ───────────────────────────────────────────────
    // Antes se hacia bodegaProductos.find(b => b.id_Producto === id), que
    // devuelve la PRIMERA bodega encontrada. Con el modelo multi-bodega eso
    // mostraba el stock de una sola bodega como si fuera el total: un
    // producto con 2 unidades en la bodega A y 50 en la B aparecia como
    // "Stock: 2" y bloqueaba la venta.
    const bodegasPorProducto = useMemo(() => {
        const mapa = new Map();
        for (const bp of bodegaProductos) {
            if (!mapa.has(bp.id_Producto)) mapa.set(bp.id_Producto, []);
            mapa.get(bp.id_Producto).push(bp);
        }
        // Mas stock primero: es la bodega que el servidor elegiria por defecto.
        for (const lista of mapa.values()) lista.sort((a, b) => b.cantidad - a.cantidad);
        return mapa;
    }, [bodegaProductos]);

    const bodegasDe    = (idProducto) => bodegasPorProducto.get(parseInt(idProducto)) || [];
    const stockTotalDe = (idProducto) => bodegasDe(idProducto).reduce((s, b) => s + (b.cantidad || 0), 0);

    /** Stock relevante para una linea: el de su bodega, o el total si no eligio. */
    const stockDisponible = (linea) => {
        const lista = bodegasDe(linea.id_Producto);
        if (!lista.length) return null;
        if (!linea.id_Bodega) return lista.reduce((s, b) => s + (b.cantidad || 0), 0);
        const bp = lista.find(b => String(b.id_Bodega) === String(linea.id_Bodega));
        return bp ? bp.cantidad : 0;
    };

    const precioDe = (idProducto) => {
        const p = productos.find(x => String(x.id_Producto || x.id) === String(idProducto));
        return p ? Number(p.precio) || 0 : 0;
    };

    const nombreDe = (idProducto) => {
        const p = productos.find(x => String(x.id_Producto || x.id) === String(idProducto));
        return p?.nombre || 'Producto';
    };

    const subtotalDe = (linea) => (parseInt(linea.cantidad) || 0) * precioDe(linea.id_Producto);

    // Solo referencial: el total que se cobra lo calcula el servidor con los
    // precios de catalogo. Antes se enviaba este numero y la API lo guardaba
    // tal cual, asi que se podia postear una venta de C$50.000 con total 1.
    const totalEstimado = detallesVenta.reduce((t, d) => t + subtotalDe(d), 0);

    const hayStockInsuficiente = detallesVenta.some(d => {
        const stock = stockDisponible(d);
        return stock !== null && parseInt(d.cantidad) > stock;
    });

    const handleAgregarProducto = () => setDetallesVenta([...detallesVenta, { ...LINEA_VACIA }]);

    const handleEliminarProducto = (index) => {
        if (detallesVenta.length <= 1) { toast.error('No puedes eliminar la única fila.'); return; }
        setDetallesVenta(detallesVenta.filter((_, i) => i !== index));
    };

    const handleDetalleChange = (index, event) => {
        const { name, value } = event.target;
        setDetallesVenta(prev => prev.map((d, i) => {
            if (i !== index) return d;
            const updated = { ...d, [name]: value };
            if (name === 'id_Producto') {
                const lista = bodegasDe(value);
                // Con una sola bodega se preselecciona; con varias, el
                // usuario elige de donde sale la mercaderia.
                updated.id_Bodega = lista.length === 1 ? String(lista[0].id_Bodega) : '';
                if (!updated.cantidad) updated.cantidad = 1;
            }
            return updated;
        }));
    };

    const validarDetalles = () => {
        if (!detallesVenta[0]?.id_Producto) { toast.error('Agrega al menos un producto.'); return false; }

        for (const d of detallesVenta) {
            if (!d.id_Producto || !(parseInt(d.cantidad) > 0)) {
                toast.error('Completa todos los campos de productos.'); return false;
            }
            const lista = bodegasDe(d.id_Producto);
            if (!lista.length) {
                toast.error(`"${nombreDe(d.id_Producto)}" no tiene registro en bodega.`); return false;
            }
            if (lista.length > 1 && !d.id_Bodega) {
                toast.error(`Elige la bodega para "${nombreDe(d.id_Producto)}".`); return false;
            }
            const stock = stockDisponible(d);
            if (parseInt(d.cantidad) > stock) {
                toast.error(`Stock insuficiente para "${nombreDe(d.id_Producto)}". Disponible: ${stock}`);
                return false;
            }
        }
        return true;
    };

    /**
     * Envia la venta COMPLETA en una sola llamada.
     *
     * Antes se hacia POST /Venta/NuevaVenta y despues un POST por cada
     * linea con Promise.allSettled: si fallaban algunas, quedaba una venta
     * "Completado" con detalles parciales y el stock ya descontado, sin
     * ninguna compensacion. Ahora la API lo resuelve en una transaccion:
     * entra todo o no entra nada.
     */
    const enviarVenta = async (formData, idEstado) => {
        setIsSubmitting(true);
        try {
            const { data } = await api.post('/Venta/Registrar', {
                id_Cliente:     parseInt(formData.id_Cliente),
                id_Metodo_Pago: formData.id_Metodo_Pago ? parseInt(formData.id_Metodo_Pago) : null,
                fecha:          formData.fecha_Venta ? new Date(formData.fecha_Venta).toISOString() : null,
                id_Estado:      idEstado,
                detalles: detallesVenta.map(d => ({
                    id_Producto: parseInt(d.id_Producto),
                    id_Bodega:   d.id_Bodega ? parseInt(d.id_Bodega) : null,
                    cantidad:    parseInt(d.cantidad),
                    // Sin precio ni subtotal: los pone el servidor.
                })),
            });

            toast.success(data?.mensaje || 'Venta registrada correctamente.');
            // Guarda por si el componente se monta sin props (ver App.jsx).
            onSuccess?.();
        } catch (error) {
            toast.error(mensajeDeError(error, 'No se pudo registrar la venta.'));
        } finally {
            setIsSubmitting(false);
        }
    };

    const guardarBorrador = handleSubmit((formData) => {
        if (isSubmitting) return;
        return enviarVenta(formData, 5); // 5 = Borrador, no mueve inventario
    });

    const onSubmit = (formData) => {
        if (!validarDetalles()) return;
        return enviarVenta(formData, 4); // 4 = Completado
    };

    const inputCls    = `w-full border rounded-md p-2 focus:outline-none focus:ring-2 focus:ring-blue-500 ${modoOscuro ? 'bg-gray-700 border-gray-600 text-white' : 'bg-white border-gray-300 text-gray-900'}`;
    const labelCls    = `mb-1 text-sm font-medium ${modoOscuro ? 'text-gray-300' : 'text-gray-700'}`;
    const readonlyCls = `w-full border rounded-md p-2 cursor-not-allowed ${modoOscuro ? 'bg-gray-700 border-gray-600 text-gray-400' : 'bg-gray-100 border-gray-300 text-gray-500'}`;

    return (
        <div className={`p-6 rounded-md shadow-md ${modoOscuro ? 'bg-gray-800 text-gray-100' : 'bg-gray-50 text-gray-800'}`}>
            <h2 className="text-2xl font-bold mb-6">Agregar Nueva Venta</h2>
            {cargandoDatos ? (
                <div className="flex justify-center items-center h-48">
                    <p className={modoOscuro ? 'text-gray-400' : 'text-gray-600'}>Cargando datos...</p>
                </div>
            ) : (
                <form onSubmit={handleSubmit(onSubmit)}>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
                        <div className="flex flex-col">
                            <label className={labelCls} htmlFor="venta-cliente">Cliente</label>
                            <select id="venta-cliente" {...register('id_Cliente', { required: 'El cliente es requerido' })} className={inputCls}>
                                <option value="">Selecciona un cliente</option>
                                {clientes.map((c, i) => {
                                    const id = c.id_Cliente || c.id || i;
                                    return <option key={`cli-${id}`} value={id}>{c.nombre_Completo || c.nombre || 'Sin nombre'}</option>;
                                })}
                            </select>
                            {errors.id_Cliente && <span className="text-red-500 text-xs mt-1">{errors.id_Cliente.message}</span>}
                        </div>
                        <div className="flex flex-col">
                            <label className={labelCls} htmlFor="venta-fecha">Fecha de Venta</label>
                            <input id="venta-fecha" type="date" {...register('fecha_Venta', { required: 'La fecha es requerida' })} className={inputCls} />
                            {errors.fecha_Venta && <span className="text-red-500 text-xs mt-1">{errors.fecha_Venta.message}</span>}
                        </div>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
                        <div className="flex flex-col">
                            <label className={labelCls} htmlFor="venta-metodo">Método de Pago</label>
                            <select id="venta-metodo" {...register('id_Metodo_Pago', { required: 'El método de pago es requerido' })} className={inputCls}>
                                <option value="">Seleccionar método</option>
                                {metodosPago.map((mp, i) => {
                                    const id = mp.id_Metodo_Pago || mp.id || i;
                                    return <option key={`mp-${id}`} value={id}>{mp.descripcion || mp.nombre || 'Sin descripción'}</option>;
                                })}
                            </select>
                            {errors.id_Metodo_Pago && <span className="text-red-500 text-xs mt-1">{errors.id_Metodo_Pago.message}</span>}
                        </div>
                        <div className="flex flex-col">
                            <label className={labelCls} htmlFor="venta-factura">N° Factura</label>
                            <input id="venta-factura" type="text" value="Se asigna al guardar" disabled className={readonlyCls} />
                        </div>
                        <div className="flex flex-col">
                            <label className={labelCls} htmlFor="venta-usuario">Creado por</label>
                            <input id="venta-usuario" type="text" value={nombreUsuario} disabled className={readonlyCls} />
                        </div>
                    </div>

                    <div className={`mt-4 p-4 rounded-md ${modoOscuro ? 'bg-gray-700' : 'bg-gray-100'}`}>
                        <div className="flex justify-between items-center mb-4">
                            <h3 className="text-lg font-bold">Detalles de la Venta</h3>
                            <button type="button" onClick={handleAgregarProducto}
                                className="flex items-center px-3 py-1 bg-blue-600 text-white font-semibold rounded-md hover:bg-blue-700 transition">
                                <Plus size={16} className="mr-1" aria-hidden="true" /> Producto
                            </button>
                        </div>

                        <div className="space-y-3">
                            {detallesVenta.map((detalle, index) => {
                                const listaBodegas     = bodegasDe(detalle.id_Producto);
                                const stock            = stockDisponible(detalle);
                                const stockBajo        = stock !== null && stock <= 5;
                                const stockInsuficiente = stock !== null && parseInt(detalle.cantidad) > stock;
                                const sinBodega        = detalle.id_Producto && listaBodegas.length === 0;
                                const variasBodegas    = listaBodegas.length > 1;

                                return (
                                    <div key={index} className={`grid grid-cols-1 md:grid-cols-5 gap-4 p-4 rounded-md ${modoOscuro ? 'bg-gray-600' : 'bg-white'}`}>
                                        <div className="flex flex-col">
                                            <label className={labelCls} htmlFor={`prod-${index}`}>Producto</label>
                                            <select id={`prod-${index}`} name="id_Producto" value={detalle.id_Producto}
                                                onChange={(e) => handleDetalleChange(index, e)} className={inputCls}>
                                                <option value="">Selecciona producto</option>
                                                {productos.map((p, pi) => {
                                                    const id = p.id_Producto || p.id || pi;
                                                    // Stock TOTAL sumando todas las bodegas.
                                                    const st = stockTotalDe(id);
                                                    return <option key={`prod-${id}`} value={id}>{p.nombre} (Stock: {st})</option>;
                                                })}
                                            </select>
                                            {sinBodega && <p className="text-xs text-red-500 flex items-center gap-1 mt-1"><AlertTriangle size={12} aria-hidden="true" /> Sin registro en bodega</p>}
                                            {!sinBodega && detalle.id_Producto && stockBajo && !stockInsuficiente && (
                                                <p className="text-xs text-yellow-600 flex items-center gap-1 mt-1"><AlertTriangle size={12} aria-hidden="true" /> Stock bajo: {stock} unidades</p>
                                            )}
                                            {stockInsuficiente && <p className="text-xs text-red-500 flex items-center gap-1 mt-1"><AlertTriangle size={12} aria-hidden="true" /> Stock insuficiente. Disponible: {stock}</p>}
                                        </div>

                                        {/* Selector de bodega: solo aparece si el producto vive en
                                            mas de una. Antes no existia y siempre se enviaba la
                                            primera bodega encontrada. */}
                                        <div className="flex flex-col">
                                            <label className={labelCls} htmlFor={`bod-${index}`}>Bodega</label>
                                            <select id={`bod-${index}`} name="id_Bodega" value={detalle.id_Bodega}
                                                onChange={(e) => handleDetalleChange(index, e)}
                                                disabled={!variasBodegas}
                                                className={variasBodegas ? inputCls : readonlyCls}>
                                                {variasBodegas && <option value="">Elegir bodega…</option>}
                                                {listaBodegas.map(b => (
                                                    <option key={`bod-${index}-${b.id_Bodega}`} value={b.id_Bodega}>
                                                        {b.nombre_Bodega || `Bodega ${b.id_Bodega}`} ({b.cantidad})
                                                    </option>
                                                ))}
                                                {!listaBodegas.length && <option value="">—</option>}
                                            </select>
                                        </div>

                                        <div className="flex flex-col">
                                            <label className={labelCls} htmlFor={`cant-${index}`}>Cantidad</label>
                                            <input id={`cant-${index}`} name="cantidad" type="number" min="1" value={detalle.cantidad}
                                                onChange={(e) => handleDetalleChange(index, e)}
                                                className={`${inputCls} ${stockInsuficiente ? 'border-red-400 focus:ring-red-400' : ''}`} />
                                        </div>

                                        <div className="flex flex-col">
                                            <label className={labelCls} htmlFor={`precio-${index}`}>Precio Unitario</label>
                                            {/* Solo lectura: el precio sale del catalogo en el servidor. */}
                                            <input id={`precio-${index}`} type="text" readOnly
                                                value={detalle.id_Producto ? precioDe(detalle.id_Producto).toFixed(2) : ''}
                                                className={readonlyCls} />
                                        </div>

                                        <div className="flex flex-col">
                                            <label className={labelCls} htmlFor={`sub-${index}`}>Subtotal</label>
                                            <div className="flex items-center gap-2">
                                                <input id={`sub-${index}`} type="text" readOnly
                                                    value={subtotalDe(detalle).toFixed(2)}
                                                    className={readonlyCls} />
                                                <button type="button" onClick={() => handleEliminarProducto(index)}
                                                    aria-label={`Eliminar línea ${index + 1}`}
                                                    className="text-red-500 hover:text-red-700">
                                                    <Trash2 size={20} aria-hidden="true" />
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                );
                            })}
                        </div>

                        <div className="mt-4 text-right">
                            <span className="font-bold text-lg">Total estimado: {totalEstimado.toFixed(2)}</span>
                            <p className={`text-xs mt-0.5 ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                                El monto final se calcula en el servidor con los precios de catálogo.
                            </p>
                        </div>
                    </div>

                    <div className="flex justify-end space-x-4 mt-6">
                        <button type="button" onClick={onCancel} disabled={isSubmitting}
                            className="flex items-center px-4 py-2 bg-gray-500 text-white font-semibold rounded-md hover:bg-gray-600 transition disabled:opacity-50">
                            <X size={20} className="mr-2" aria-hidden="true" /> Cancelar
                        </button>
                        <button type="button" onClick={guardarBorrador} disabled={isSubmitting}
                            className={`flex items-center px-4 py-2 font-semibold rounded-md border transition disabled:opacity-50
                                ${modoOscuro ? 'border-gray-500 text-gray-300 hover:bg-gray-700' : 'border-gray-400 text-gray-600 hover:bg-gray-100'}`}>
                            <FileText size={20} className="mr-2" aria-hidden="true" /> Guardar Borrador
                        </button>
                        <button type="submit"
                            disabled={isSubmitting || hayStockInsuficiente}
                            title={hayStockInsuficiente ? 'Corrige el stock insuficiente antes de guardar' : ''}
                            className={`flex items-center px-4 py-2 text-white font-semibold rounded-md transition disabled:opacity-50 disabled:cursor-not-allowed ${
                                hayStockInsuficiente ? 'bg-gray-400' : 'bg-green-600 hover:bg-green-700'
                            }`}>
                            <Save size={20} className="mr-2" aria-hidden="true" />
                            {isSubmitting ? 'Guardando...' : hayStockInsuficiente ? 'Stock insuficiente' : 'Completar Venta'}
                        </button>
                    </div>
                </form>
            )}
        </div>
    );
};

export default FormularioAgregarVenta;
