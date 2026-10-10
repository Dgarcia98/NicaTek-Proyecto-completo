import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useOutletContext } from 'react-router-dom';
import { Save, X, Plus, Trash2, FileText } from 'lucide-react';
import { toast } from 'react-hot-toast';
import api from '../../api/axios.Config';

const FormularioAgregarCompra = ({ onCancel, onSuccess }) => {
    const outletContext = useOutletContext();
    const modoOscuro    = outletContext?.modoOscuro || false;

    const { register, handleSubmit, formState: { errors } } = useForm({
        defaultValues: { fecha_Compra: new Date().toISOString().split('T')[0] }
    });

    const [proveedores, setProveedores] = useState([]);
    const [bodegas,     setBodegas]     = useState([]);
    const [productos,   setProductos]   = useState([]);
    const [isSaving,    setIsSaving]    = useState(false);

    const idUsuario = parseInt(sessionStorage.getItem('idUsuario') || '1');

    const [detalles, setDetalles] = useState([
        { Id_Bodega: '', Id_Producto: '', Cantidad: 1, Precio_Unitario: 0, Subtotal: 0 }
    ]);

    useEffect(() => {
        const cargar = async () => {
            try {
                const [resP, resProd, resB] = await Promise.all([
                    api.get('/Proveedores/listar'),
                    api.get('/Producto/ListarProducto'),
                    api.get('/Bodega/ListarBodega'),
                ]);
                setProveedores(resP.data || []);
                setProductos(Array.isArray(resProd.data)
                    ? resProd.data.filter(p => p.id_Estado === 1) : []);
                setBodegas(Array.isArray(resB.data) ? resB.data : []);
            } catch (err) {
                toast.error('Error al cargar los datos iniciales.');
            }
        };
        cargar();
    }, []);

    const agregarDetalle = () =>
        setDetalles(prev => [...prev, { Id_Bodega: '', Id_Producto: '', Cantidad: 1, Precio_Unitario: 0, Subtotal: 0 }]);

    const eliminarDetalle = (index) => {
        if (detalles.length === 1) { toast.error('Debe haber al menos un producto.'); return; }
        setDetalles(prev => prev.filter((_, i) => i !== index));
    };

    const actualizarDetalle = (index, campo, valor) => {
        setDetalles(prev => {
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

    const totalCompra = detalles.reduce((acc, d) => acc + (parseFloat(d.Subtotal) || 0), 0);

    // ── Guardar como Borrador (solo cabecera, sin detalles, sin stock) ─────────
    const guardarBorrador = handleSubmit(async (data) => {
        if (isSaving) return;
        try {
            setIsSaving(true);
            const idProv  = parseInt(data.id_Proveedor);
            const provSel = proveedores.find(p => p.id_Proveedor === idProv);

            await api.post('/Compra/NuevoCompra', {
                Id_Compra:                0,
                Id_Proveedor:             idProv,
                Proveedor:                provSel?.nombre_Completo || '',
                Numero_Factura_Proveedor: data.numero_Factura_Proveedor,
                Fecha_Compra:             new Date(data.fecha_Compra).toISOString(),
                Total_Compra:             totalCompra,
                Id_Estado:                5, // Borrador
                Estado:                   'Borrador',
                Fecha_Creacion:           new Date().toISOString(),
                Fecha_Modificacion:       new Date().toISOString(),
                Id_Creador:               idUsuario,
                Id_Modificador:           idUsuario,
            });

            toast.success('Compra guardada como borrador.');
            if (onSuccess) onSuccess();
        } catch (err) {
            toast.error(`Error: ${err.response?.data?.msj || err.message}`);
        } finally {
            setIsSaving(false);
        }
    });

    // ── Guardar y Completar (cabecera + detalles + mueve stock) ──────────────
    const onSubmit = async (data) => {
        if (isSaving) return;

        const valido = detalles.every(d =>
            d.Id_Bodega !== '' && d.Id_Producto !== '' &&
            Number(d.Cantidad) > 0 && Number(d.Precio_Unitario) > 0
        );
        if (!valido) {
            toast.error('Completa todos los campos: bodega, producto, cantidad y precio.');
            return;
        }

        try {
            setIsSaving(true);
            const idProv  = parseInt(data.id_Proveedor);
            const provSel = proveedores.find(p => p.id_Proveedor === idProv);

            // 1. Crear cabecera como Completado (4)
            const resCompra = await api.post('/Compra/NuevoCompra', {
                Id_Compra:                0,
                Id_Proveedor:             idProv,
                Proveedor:                provSel?.nombre_Completo || '',
                Numero_Factura_Proveedor: data.numero_Factura_Proveedor,
                Fecha_Compra:             new Date(data.fecha_Compra).toISOString(),
                Total_Compra:             totalCompra,
                Id_Estado:                4, // Completado
                Estado:                   'Completado',
                Fecha_Creacion:           new Date().toISOString(),
                Fecha_Modificacion:       new Date().toISOString(),
                Id_Creador:               idUsuario,
                Id_Modificador:           idUsuario,
            });

            const idCompra = resCompra.data?.id_Compra || resCompra.data?.Id_Compra;
            if (!idCompra || idCompra === 0)
                throw new Error(`No se obtuvo el ID de compra. Respuesta: ${JSON.stringify(resCompra.data)}`);

            // 2. Insertar detalles (el SP mueve stock via trigger)
            await Promise.all(detalles.map(d => {
                const prodSel = productos.find(p => p.id_Producto === parseInt(d.Id_Producto));
                return api.post('/DetalleCompra/NuevoDetalleCompra', {
                    Id_Detalle_Compra:      0,
                    Id_Compra:              idCompra,
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

            toast.success('Compra completada y stock actualizado correctamente.');
            if (onSuccess) onSuccess();
        } catch (err) {
            toast.error(`Error: ${err.response?.data?.msj || err.message}`);
        } finally {
            setIsSaving(false);
        }
    };

    const inputCls = `w-full border rounded-md p-2 text-sm focus:ring-cyan-500 focus:border-cyan-500 outline-none transition-colors
        ${modoOscuro ? 'bg-gray-700 border-gray-600 text-white placeholder-gray-400' : 'bg-white border-gray-300 text-gray-900'}`;
    const labelCls = `block text-xs font-medium mb-1 ${modoOscuro ? 'text-gray-300' : 'text-gray-600'}`;

    return (
        <div className={`p-6 rounded-lg shadow-sm transition-colors ${modoOscuro ? 'bg-gray-800' : 'bg-white'}`}>
            <h2 className={`text-2xl font-bold mb-6 ${modoOscuro ? 'text-gray-100' : 'text-gray-800'}`}>
                Agregar Nueva Compra
            </h2>

            <form onSubmit={handleSubmit(onSubmit)}>

                {/* ── Cabecera ──────────────────────────────────────────── */}
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
                    <div>
                        <label className={labelCls}>Proveedor *</label>
                        <select {...register('id_Proveedor', { required: 'Requerido' })} className={inputCls}>
                            <option value="">Selecciona un proveedor</option>
                            {proveedores.map(p => (
                                <option key={p.id_Proveedor} value={p.id_Proveedor}>{p.nombre_Completo}</option>
                            ))}
                        </select>
                        {errors.id_Proveedor && <p className="text-red-400 text-xs mt-1">{errors.id_Proveedor.message}</p>}
                    </div>
                    <div>
                        <label className={labelCls}>No. Factura Proveedor *</label>
                        <input type="text"
                            {...register('numero_Factura_Proveedor', { required: 'Requerido' })}
                            className={inputCls} placeholder="ej: FAC-0001" />
                        {errors.numero_Factura_Proveedor && <p className="text-red-400 text-xs mt-1">{errors.numero_Factura_Proveedor.message}</p>}
                    </div>
                    <div>
                        <label className={labelCls}>Fecha de Compra *</label>
                        <input type="date"
                            {...register('fecha_Compra', { required: 'Requerido' })}
                            className={inputCls} />
                        {errors.fecha_Compra && <p className="text-red-400 text-xs mt-1">{errors.fecha_Compra.message}</p>}
                    </div>
                </div>

                {/* ── Detalle de productos ──────────────────────────────── */}
                <div className={`p-4 rounded-lg border mb-6 ${modoOscuro ? 'bg-gray-900 border-gray-700' : 'bg-gray-50 border-gray-200'}`}>
                    <div className="flex justify-between items-center mb-4">
                        <h3 className={`text-base font-semibold ${modoOscuro ? 'text-gray-100' : 'text-gray-800'}`}>
                            Productos a Comprar
                        </h3>
                        <button type="button" onClick={agregarDetalle}
                            className="flex items-center gap-2 bg-cyan-600 text-white px-3 py-1.5 rounded-md text-sm hover:bg-cyan-700 transition">
                            <Plus size={15} /> Agregar línea
                        </button>
                    </div>

                    <div className="space-y-3">
                        {detalles.map((detalle, index) => (
                            <div key={index}
                                className={`grid grid-cols-1 md:grid-cols-12 gap-3 items-end p-3 rounded-lg
                                    ${modoOscuro ? 'bg-gray-800' : 'bg-white border border-gray-200'}`}>
                                <div className="md:col-span-3">
                                    <label className={labelCls}>Bodega destino *</label>
                                    <select value={detalle.Id_Bodega}
                                        onChange={e => actualizarDetalle(index, 'Id_Bodega', e.target.value)}
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
                                        onChange={e => actualizarDetalle(index, 'Id_Producto', e.target.value)}
                                        disabled={!detalle.Id_Bodega}
                                        className={`${inputCls} ${!detalle.Id_Bodega ? 'opacity-50 cursor-not-allowed' : ''}`}>
                                        <option value="">
                                            {detalle.Id_Bodega ? 'Selecciona producto...' : '— Elige bodega primero —'}
                                        </option>
                                        {detalle.Id_Bodega && productos.map(p => (
                                            <option key={p.id_Producto} value={p.id_Producto}>{p.nombre}</option>
                                        ))}
                                    </select>
                                </div>
                                <div className="md:col-span-2">
                                    <label className={labelCls}>Cantidad *</label>
                                    <input type="number" min="1" value={detalle.Cantidad}
                                        onChange={e => actualizarDetalle(index, 'Cantidad', e.target.value)}
                                        className={inputCls} />
                                </div>
                                <div className="md:col-span-2">
                                    <label className={labelCls}>Precio Unitario *</label>
                                    <input type="number" step="0.01" min="0.01" value={detalle.Precio_Unitario}
                                        onChange={e => actualizarDetalle(index, 'Precio_Unitario', e.target.value)}
                                        className={inputCls} />
                                </div>
                                <div className="md:col-span-2">
                                    <label className={labelCls}>Subtotal</label>
                                    <div className="flex items-center gap-1">
                                        <div className={`flex-1 border rounded-md p-2 text-right text-sm
                                            ${modoOscuro ? 'bg-gray-700 border-gray-600 text-gray-300' : 'bg-gray-100 border-gray-300 text-gray-600'}`}>
                                            {Number(detalle.Subtotal).toFixed(2)}
                                        </div>
                                        <button type="button" onClick={() => eliminarDetalle(index)}
                                            className={`p-2 rounded-md ${modoOscuro ? 'text-red-400 hover:bg-gray-700' : 'text-red-500 hover:bg-red-50'}`}>
                                            <Trash2 size={16} />
                                        </button>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>

                    <div className="mt-5 flex justify-end items-center gap-3">
                        <span className={`text-base font-bold ${modoOscuro ? 'text-gray-100' : 'text-gray-800'}`}>Total:</span>
                        <span className="text-2xl font-bold text-cyan-500">
                            {Number(totalCompra).toLocaleString('es-NI', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                        </span>
                    </div>
                </div>

                {/* ── Botones ───────────────────────────────────────────── */}
                <div className="flex justify-end gap-3">
                    <button type="button" onClick={onCancel}
                        className={`flex items-center gap-2 px-4 py-2 text-white rounded-md transition-colors
                            ${modoOscuro ? 'bg-gray-600 hover:bg-gray-500' : 'bg-gray-500 hover:bg-gray-600'}`}>
                        <X size={16} /> Cancelar
                    </button>
                    {/* Botón Borrador — solo guarda cabecera, sin stock */}
                    <button type="button" onClick={guardarBorrador} disabled={isSaving}
                        className={`flex items-center gap-2 px-4 py-2 rounded-md border transition-colors
                            ${modoOscuro
                                ? 'border-gray-500 text-gray-300 hover:bg-gray-700'
                                : 'border-gray-400 text-gray-600 hover:bg-gray-100'}
                            ${isSaving ? 'opacity-60 cursor-not-allowed' : ''}`}>
                        <FileText size={16} /> Guardar Borrador
                    </button>
                    {/* Botón Completar — guarda cabecera + detalles + mueve stock */}
                    <button type="submit" disabled={isSaving}
                        className={`flex items-center gap-2 px-4 py-2 text-white rounded-md transition-colors
                            ${isSaving ? 'bg-cyan-400 cursor-not-allowed' : 'bg-cyan-600 hover:bg-cyan-700'}`}>
                        <Save size={16} /> {isSaving ? 'Guardando...' : 'Completar Compra'}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default FormularioAgregarCompra;