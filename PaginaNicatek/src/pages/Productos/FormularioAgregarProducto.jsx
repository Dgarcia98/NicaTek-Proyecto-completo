import { useState, useEffect } from 'react';
import { useOutletContext, useNavigate } from 'react-router-dom';
import { ArrowLeft, Save, Package, Tag, Layers, DollarSign, LoaderCircle } from 'lucide-react';
import Swal from 'sweetalert2';
import api from '../../api/axios.Config';

const FormularioAgregarProducto = () => {
    const { modoOscuro } = useOutletContext();
    const navigate = useNavigate();

    const [categorias, setCategorias] = useState([]);
    const [marcasModelos, setMarcasModelos] = useState([]);
    const [cargandoDatos, setCargandoDatos] = useState(true);
    const [cargandoEnvio, setCargandoEnvio] = useState(false);

    const [formData, setFormData] = useState({
        codigo: '',
        nombre: '',
        descripcion: '',
        precio: '',
        precio_Compra: '',
        id_Categoria: '',
        id_Marca_Modelo: '',
    });

    // ── Cargar catálogos ──────────────────────────────────────────────────────
    useEffect(() => {
        const cargarCatalogos = async () => {
            try {
                const [resCat, resMM] = await Promise.all([
                    api.get('/Categoria/ListarCategoria'),
                    api.get('/MarcaModelo/ListarMarcaModelo'),
                ]);

                setCategorias(Array.isArray(resCat.data) ? resCat.data : (resCat.data?.datos || []));
                setMarcasModelos(Array.isArray(resMM.data) ? resMM.data : (resMM.data?.datos || []));
            } catch (error) {
                console.error('Error cargando catálogos:', error);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'No se pudieron cargar los catálogos.',
                    confirmButtonColor: '#0891b2',
                });
            } finally {
                setCargandoDatos(false);
            }
        };
        cargarCatalogos();
    }, []);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!formData.id_Categoria || !formData.id_Marca_Modelo) {
            Swal.fire({
                icon: 'warning',
                title: 'Campos requeridos',
                text: 'Por favor selecciona categoría y marca/modelo.',
                confirmButtonColor: '#0891b2',
            });
            return;
        }

        const result = await Swal.fire({
            title: '¿Guardar producto?',
            text: `Se creará el producto "${formData.nombre}".`,
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#0891b2',
            cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, guardar',
            cancelButtonText: 'Cancelar',
        });

        if (!result.isConfirmed) return;

        setCargandoEnvio(true);
        try {
            await api.post('/Producto/NuevoProducto', {
                Id_Producto:     0,
                Codigo:          formData.codigo.trim(),
                Nombre:          formData.nombre.trim(),
                Descripcion:     formData.descripcion.trim(),
                Precio:          parseFloat(formData.precio),
                Precio_Compra:   parseFloat(formData.precio_Compra) || 0,
                Id_Marca_Modelo: parseInt(formData.id_Marca_Modelo),
                Id_Proveedor:    null,
                Id_Categoria:    parseInt(formData.id_Categoria),
                Id_Estado:       1,
                Estado:          'Activo',
                Categoria:       categorias.find(c => c.id_Categoria === parseInt(formData.id_Categoria))?.nombre || '',
                Proveedor:       '',
                Nombre_Marca:    marcasModelos.find(mm => mm.id_Marca_Modelo === parseInt(formData.id_Marca_Modelo))?.nombre_Marca || '',
                Nombre_Modelo:   marcasModelos.find(mm => mm.id_Marca_Modelo === parseInt(formData.id_Marca_Modelo))?.nombre_Modelo || '',
                Fecha_Creacion:     new Date().toISOString(),
                Fecha_Modificacion: new Date().toISOString(),
                Id_Creador:      1,
                Id_Modificador:  1,
            });

            await Swal.fire({
                icon: 'success',
                title: '¡Producto creado!',
                text: 'El producto fue registrado correctamente.',
                confirmButtonColor: '#0891b2',
                timer: 2000,
                timerProgressBar: true,
            });

            window.location.href = '/productos';
        } catch (error) {
            Swal.fire({
                icon: 'error',
                title: 'Error al guardar',
                text: error.message || 'No se pudo registrar el producto.',
                confirmButtonColor: '#0891b2',
            });
        } finally {
            setCargandoEnvio(false);
        }
    };

    // ── Estilos ───────────────────────────────────────────────────────────────
    const inputClass = `w-full px-4 py-2.5 rounded-xl border text-sm focus:outline-none focus:ring-2 focus:ring-cyan-500 transition-all ${
        modoOscuro
            ? 'bg-gray-700 border-gray-600 text-white placeholder-gray-400'
            : 'bg-white border-gray-300 text-gray-900 placeholder-gray-500'
    }`;

    const labelClass = `block text-sm font-semibold mb-1 ${modoOscuro ? 'text-gray-300' : 'text-gray-700'}`;
    const iconClass  = `absolute left-3 top-1/2 -translate-y-1/2 ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`;

    return (
        <div className={`p-6 ${modoOscuro ? 'bg-gray-900 text-gray-100' : 'bg-transparent text-gray-800'}`}>

            {/* Header */}
            <div className="flex justify-between items-center mb-6">
                <h1 className={`text-3xl font-bold flex items-center ${modoOscuro ? 'text-white' : 'text-black'}`}>
                    <Package className="mr-3 text-cyan-500" size={28} /> Nuevo Producto
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

                {cargandoDatos ? (
                    <div className="flex flex-col items-center justify-center py-16 space-y-3">
                        <LoaderCircle className="animate-spin text-cyan-500" size={36} />
                        <p className={`text-sm font-medium ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                            Cargando catálogos...
                        </p>
                    </div>
                ) : (
                    <form onSubmit={handleSubmit} className="space-y-6">

                        {/* Fila 1: Código y Nombre */}
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <div>
                                <label className={labelClass}>Código <span className="text-red-500">*</span></label>
                                <div className="relative">
                                    <Tag size={16} className={iconClass} />
                                    <input
                                        type="text"
                                        name="codigo"
                                        value={formData.codigo}
                                        onChange={handleChange}
                                        required
                                        className={`${inputClass} pl-9`}
                                        placeholder="Ej: PROD-001"
                                    />
                                </div>
                            </div>
                            <div>
                                <label className={labelClass}>Nombre del Producto <span className="text-red-500">*</span></label>
                                <input
                                    type="text"
                                    name="nombre"
                                    value={formData.nombre}
                                    onChange={handleChange}
                                    required
                                    className={inputClass}
                                    placeholder="Ej: Laptop Gamer"
                                />
                            </div>
                        </div>

                        {/* Descripción */}
                        <div>
                            <label className={labelClass}>Descripción</label>
                            <textarea
                                name="descripcion"
                                value={formData.descripcion}
                                onChange={handleChange}
                                rows={3}
                                className={`${inputClass} resize-none`}
                                placeholder="Características del producto..."
                            />
                        </div>

                        <hr className={modoOscuro ? 'border-gray-700' : 'border-gray-200'} />

                        {/* Fila 2: Precios */}
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <div>
                                <label className={labelClass}>Precio de Venta <span className="text-red-500">*</span></label>
                                <div className="relative">
                                    <DollarSign size={16} className={iconClass} />
                                    <input
                                        type="number"
                                        name="precio"
                                        value={formData.precio}
                                        onChange={handleChange}
                                        required
                                        step="0.01"
                                        min="0"
                                        className={`${inputClass} pl-9`}
                                        placeholder="0.00"
                                    />
                                </div>
                            </div>
                            <div>
                                <label className={labelClass}>Precio de Compra</label>
                                <div className="relative">
                                    <DollarSign size={16} className={iconClass} />
                                    <input
                                        type="number"
                                        name="precio_Compra"
                                        value={formData.precio_Compra}
                                        onChange={handleChange}
                                        step="0.01"
                                        min="0"
                                        className={`${inputClass} pl-9`}
                                        placeholder="0.00"
                                    />
                                </div>
                            </div>
                        </div>

                        {/* Fila 3: Categoría y Marca/Modelo */}
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <div>
                                <label className={labelClass}>Categoría <span className="text-red-500">*</span></label>
                                <div className="relative">
                                    <Layers size={16} className={iconClass} />
                                    <select
                                        name="id_Categoria"
                                        value={formData.id_Categoria}
                                        onChange={handleChange}
                                        required
                                        className={`${inputClass} pl-9`}
                                    >
                                        <option value="">Seleccionar...</option>
                                        {categorias.map(c => (
                                            <option key={c.id_Categoria} value={c.id_Categoria}>{c.nombre}</option>
                                        ))}
                                    </select>
                                </div>
                            </div>
                            <div>
                                <label className={labelClass}>Marca / Modelo <span className="text-red-500">*</span></label>
                                <select
                                    name="id_Marca_Modelo"
                                    value={formData.id_Marca_Modelo}
                                    onChange={handleChange}
                                    required
                                    className={inputClass}
                                >
                                    <option value="">Seleccionar...</option>
                                    {marcasModelos.map(mm => (
                                        <option key={mm.id_Marca_Modelo} value={mm.id_Marca_Modelo}>
                                            {mm.nombre_Marca} — {mm.nombre_Modelo}
                                        </option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        <hr className={modoOscuro ? 'border-gray-700' : 'border-gray-200'} />

                        {/* Botones */}
                        <div className="flex justify-end gap-4">
                            <button
                                type="button"
                                onClick={() => window.location.href = '/productos'}
                                disabled={cargandoEnvio}
                                className={`px-6 py-2.5 rounded-lg font-medium border transition-colors ${
                                    modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-700 hover:bg-gray-100'
                                }`}
                            >
                                Cancelar
                            </button>
                            <button
                                type="submit"
                                disabled={cargandoEnvio}
                                className="flex items-center px-6 py-2.5 bg-cyan-600 text-white font-semibold rounded-lg shadow-md hover:bg-cyan-500 transition-all transform hover:scale-105 disabled:opacity-60 disabled:cursor-not-allowed disabled:transform-none"
                            >
                                {cargandoEnvio
                                    ? <LoaderCircle size={18} className="mr-2 animate-spin" />
                                    : <Save size={18} className="mr-2" />
                                }
                                {cargandoEnvio ? 'Guardando...' : 'Guardar Producto'}
                            </button>
                        </div>

                    </form>
                )}
            </div>
        </div>
    );
};

export default FormularioAgregarProducto;