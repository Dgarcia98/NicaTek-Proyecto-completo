import { useEffect, useState } from 'react';
import { useNavigate, useParams, useOutletContext } from 'react-router-dom';
import { ArrowLeft, Save, Package, LoaderCircle } from 'lucide-react';
import Swal from 'sweetalert2';
import api from '../../api/axios.Config';

const EditarProducto = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const { modoOscuro } = useOutletContext();

    const [form, setForm] = useState({
        codigo: '',
        nombre: '',
        descripcion: '',
        precio: '',
        precio_Compra: '',
        id_Marca_Modelo: '',
        id_Categoria: '',
        id_Estado: 1,
        nombre_Estado: 'Activo',
        fecha_Creacion: '',
    });

    const [categorias, setCategorias] = useState([]);
    const [marcasModelos, setMarcasModelos] = useState([]);
    const [cargando, setCargando] = useState(true);
    const [guardando, setGuardando] = useState(false);

    useEffect(() => {
        const cargarDatos = async () => {
            try {
                setCargando(true);
                const [resProd, resCat, resMM] = await Promise.all([
                    api.get(`/Producto/ObtenerProductoPorId/${id}`),
                    api.get('/Categoria/ListarCategoria'),
                    api.get('/MarcaModelo/ListarMarcaModelo'),
                ]);

                const p = resProd.data?.datos ?? resProd.data;
                setForm({
                    codigo:          p.codigo          || p.Codigo          || '',
                    nombre:          p.nombre          || p.Nombre          || '',
                    descripcion:     p.descripcion     || p.Descripcion     || '',
                    precio:          p.precio          || p.Precio          || '',
                    precio_Compra:   p.precio_Compra   || p.Precio_Compra   || '',
                    id_Marca_Modelo: p.id_Marca_Modelo || p.Id_Marca_Modelo || '',
                    id_Categoria:    p.id_Categoria    || p.Id_Categoria    || '',
                    id_Estado:       p.id_Estado       || p.Id_Estado       || 1,
                    nombre_Estado:   p.estado          || p.nombre_Estado   || 'Activo',
                    fecha_Creacion:  p.fecha_Creacion  || p.Fecha_Creacion  || new Date().toISOString(),
                });

                setCategorias(Array.isArray(resCat.data) ? resCat.data : (resCat.data?.datos || []));
                setMarcasModelos(Array.isArray(resMM.data) ? resMM.data : (resMM.data?.datos || []));
            } catch (err) {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar el producto.', confirmButtonColor: '#0891b2' })
                    .then(() => navigate('/productos'));
            } finally {
                setCargando(false);
            }
        };
        cargarDatos();
    }, [id]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setForm(prev => ({ ...prev, [name]: value }));
    };

    const handleEstadoChange = (e) => {
        const val = Number(e.target.value);
        setForm(prev => ({ ...prev, id_Estado: val, nombre_Estado: val === 1 ? 'Activo' : 'Inactivo' }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!form.nombre.trim() || !form.codigo.trim()) {
            Swal.fire({ icon: 'warning', title: 'Campos requeridos', text: 'El nombre y el código son obligatorios.', confirmButtonColor: '#0891b2' });
            return;
        }

        const result = await Swal.fire({
            title: '¿Guardar cambios?',
            text: `Se actualizará el producto "${form.nombre}".`,
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#0891b2',
            cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, guardar',
            cancelButtonText: 'Cancelar',
        });
        if (!result.isConfirmed) return;

        setGuardando(true);
        try {
            await api.put(`/Producto/EditarProducto/${id}`, {
                Id_Producto:     Number(id),
                Codigo:          form.codigo.trim(),
                Nombre:          form.nombre.trim(),
                Descripcion:     form.descripcion.trim(),
                Precio:          Number(form.precio),
                Precio_Compra:   Number(form.precio_Compra),
                Id_Marca_Modelo: Number(form.id_Marca_Modelo),
                Id_Proveedor:    null,                          // no se gestiona desde aquí
                Id_Categoria:    form.id_Categoria ? Number(form.id_Categoria) : null,
                Id_Estado:       form.id_Estado,
                Estado:          form.nombre_Estado,
                Categoria:       categorias.find(c => c.id_Categoria === Number(form.id_Categoria))?.nombre || '',
                Proveedor:       '',
                Nombre_Marca:    marcasModelos.find(mm => mm.id_Marca_Modelo === Number(form.id_Marca_Modelo))?.nombre_Marca || '',
                Nombre_Modelo:   marcasModelos.find(mm => mm.id_Marca_Modelo === Number(form.id_Marca_Modelo))?.nombre_Modelo || '',
                Fecha_Creacion:     form.fecha_Creacion,
                Fecha_Modificacion: new Date().toISOString(),
                Id_Creador:      1,
                Id_Modificador:  1,
            });

            await Swal.fire({
                icon: 'success',
                title: '¡Guardado!',
                text: 'Producto actualizado correctamente.',
                confirmButtonColor: '#0891b2',
                timer: 2000,
                timerProgressBar: true,
            });

            window.location.href = '/productos';
        } catch (err) {
            Swal.fire({ icon: 'error', title: 'Error al guardar', text: err.message || 'No se pudo actualizar.', confirmButtonColor: '#0891b2' });
        } finally {
            setGuardando(false);
        }
    };

    const inputClass = `w-full px-4 py-2.5 rounded-xl border text-sm focus:outline-none focus:ring-2 focus:ring-cyan-500 transition-all ${
        modoOscuro ? 'bg-gray-700 border-gray-600 text-gray-100 placeholder-gray-500' : 'bg-white border-gray-300 text-gray-900 placeholder-gray-400'
    }`;
    const labelClass = `block text-sm font-semibold mb-1 ${modoOscuro ? 'text-gray-300' : 'text-gray-700'}`;

    if (cargando) {
        return (
            <div className={`p-6 flex justify-center items-center h-64 ${modoOscuro ? 'bg-gray-900' : 'bg-transparent'}`}>
                <div className="flex flex-col items-center space-y-3">
                    <LoaderCircle className="animate-spin text-cyan-500" size={36} />
                    <p className={`text-sm font-medium ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>Cargando producto...</p>
                </div>
            </div>
        );
    }

    return (
        <div className={`p-6 ${modoOscuro ? 'bg-gray-900 text-gray-100' : 'bg-transparent text-gray-800'}`}>

            {/* Header */}
            <div className="flex justify-between items-center mb-6">
                <h1 className={`text-3xl font-bold flex items-center ${modoOscuro ? 'text-white' : 'text-black'}`}>
                    <Package className="mr-3 text-cyan-500" size={28} /> Editar Producto
                </h1>
                <button
                    onClick={() => window.location.href = '/productos'}
                    className={`flex items-center px-4 py-2 rounded-lg border font-medium transition-all hover:scale-105 ${
                        modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'
                    }`}
                >
                    <ArrowLeft size={18} className="mr-2" /> Volver
                </button>
            </div>

            {/* Card */}
            <div className={`max-w-4xl rounded-2xl shadow-md border p-8 ${modoOscuro ? 'bg-gray-800 border-gray-700' : 'bg-white border-gray-200'}`}>
                <form onSubmit={handleSubmit} className="space-y-5">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-5">

                        {/* ID - solo lectura */}
                        <div>
                            <label className={labelClass}>ID Producto</label>
                            <input type="text" value={id} disabled className={`${inputClass} cursor-not-allowed opacity-60`} />
                        </div>

                        {/* Código */}
                        <div>
                            <label className={labelClass}>Código <span className="text-red-500">*</span></label>
                            <input type="text" name="codigo" value={form.codigo} onChange={handleChange} required className={inputClass} placeholder="Ej: LP-AIR-M2" />
                        </div>

                        {/* Nombre */}
                        <div className="md:col-span-2">
                            <label className={labelClass}>Nombre <span className="text-red-500">*</span></label>
                            <input type="text" name="nombre" value={form.nombre} onChange={handleChange} required className={inputClass} placeholder="Nombre del producto" />
                        </div>

                        {/* Descripción */}
                        <div className="md:col-span-2">
                            <label className={labelClass}>Descripción</label>
                            <textarea name="descripcion" value={form.descripcion} onChange={handleChange} rows={3} className={`${inputClass} resize-none`} placeholder="Descripción del producto" />
                        </div>

                        {/* Precio Venta */}
                        <div>
                            <label className={labelClass}>Precio de Venta <span className="text-red-500">*</span></label>
                            <input type="number" name="precio" value={form.precio} onChange={handleChange} required min="0" step="0.01" className={inputClass} placeholder="0.00" />
                        </div>

                        {/* Precio Compra */}
                        <div>
                            <label className={labelClass}>Precio de Compra</label>
                            <input type="number" name="precio_Compra" value={form.precio_Compra} onChange={handleChange} min="0" step="0.01" className={inputClass} placeholder="0.00" />
                        </div>

                        {/* Marca / Modelo */}
                        <div>
                            <label className={labelClass}>Marca / Modelo <span className="text-red-500">*</span></label>
                            <select name="id_Marca_Modelo" value={form.id_Marca_Modelo} onChange={handleChange} required className={inputClass}>
                                <option value="">Seleccionar...</option>
                                {marcasModelos.map(mm => (
                                    <option key={mm.id_Marca_Modelo} value={mm.id_Marca_Modelo}>
                                        {mm.nombre_Marca} — {mm.nombre_Modelo}
                                    </option>
                                ))}
                            </select>
                        </div>

                        {/* Categoría */}
                        <div>
                            <label className={labelClass}>Categoría</label>
                            <select name="id_Categoria" value={form.id_Categoria} onChange={handleChange} className={inputClass}>
                                <option value="">Sin categoría</option>
                                {categorias.map(cat => (
                                    <option key={cat.id_Categoria} value={cat.id_Categoria}>
                                        {cat.nombre}
                                    </option>
                                ))}
                            </select>
                        </div>

                        {/* Estado */}
                        <div>
                            <label className={labelClass}>Estado</label>
                            <select value={form.id_Estado} onChange={handleEstadoChange} className={inputClass}>
                                <option value={1}>Activo</option>
                                <option value={2}>Inactivo</option>
                            </select>
                            <div className="mt-2">
                                <span className={`px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full border ${
                                    form.id_Estado === 1 ? 'bg-green-100 text-green-800 border-green-200' : 'bg-red-100 text-red-800 border-red-200'
                                }`}>
                                    {form.nombre_Estado}
                                </span>
                            </div>
                        </div>
                    </div>

                    {/* Botón guardar */}
                    <div className="flex justify-end pt-2">
                        <button type="submit" disabled={guardando} className="flex items-center px-6 py-2.5 bg-cyan-600 text-white font-semibold rounded-lg shadow-md hover:bg-cyan-500 transition-all transform hover:scale-105 disabled:opacity-60 disabled:cursor-not-allowed disabled:transform-none">
                            {guardando ? <LoaderCircle size={18} className="mr-2 animate-spin" /> : <Save size={18} className="mr-2" />}
                            {guardando ? 'Guardando...' : 'Guardar Cambios'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default EditarProducto;