import { useEffect, useState } from 'react';
import { useOutletContext, useNavigate } from 'react-router-dom';
import { Package, Plus, Edit, ToggleRight, ToggleLeft, Search, X, LoaderCircle } from 'lucide-react';
import Swal from 'sweetalert2';
import TablaConTarjetas from '../../components/TablaConTarjetas';
import api from '../../api/axios.Config';
import { tienePermiso } from '../../utils/Permisos';

const Productos = () => {
    const { modoOscuro } = useOutletContext();
    const navigate = useNavigate();

    const puedeCrear    = tienePermiso('INV_CREATE');
    const puedeEditar   = tienePermiso('INV_UPDATE');
    const puedeEliminar = tienePermiso('INV_DELETE');

    const [productos, setProductos] = useState([]);
    const [cargando, setCargando] = useState(true);
    const [filtro, setFiltro] = useState('');

    const fetchProductos = async (termino = '') => {
        try {
            setCargando(true);
            const endpoint = termino.trim()
                ? `/Producto/ListarPorNombre/${encodeURIComponent(termino.trim())}`
                : '/Producto/ListarProducto';
            const res = await api.get(endpoint);
            const data = Array.isArray(res.data) ? res.data : (res.data?.datos || []);
            setProductos(data);
        } catch (err) {
            console.error('Error al cargar productos:', err);
            setProductos([]);
        } finally {
            setCargando(false);
        }
    };

    useEffect(() => {
        const t = setTimeout(() => fetchProductos(filtro), 400);
        return () => clearTimeout(t);
    }, [filtro]);

    const alternarEstado = async (producto) => {
        if (!puedeEliminar) return;

        const esActivo = producto.estado === 'Activo' || producto.id_Estado === 1;
        const nuevoEstado = esActivo ? 2 : 1;
        const nuevoNombreEstado = esActivo ? 'Inactivo' : 'Activo';

        const result = await Swal.fire({
            title: '¿Cambiar estado?',
            text: `El producto "${producto.nombre}" pasará a ${nuevoNombreEstado}.`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#0891b2',
            cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, cambiar',
            cancelButtonText: 'Cancelar',
        });

        if (!result.isConfirmed) return;

        try {
            await api.put(`/Producto/EditarProducto/${producto.id_Producto}`, {
                Id_Producto: producto.id_Producto,
                Codigo: producto.codigo,
                Nombre: producto.nombre,
                Descripcion: producto.descripcion,
                Precio: producto.precio,
                Precio_Compra: producto.precio_Compra,
                Id_Marca_Modelo: producto.id_Marca_Modelo,
                Id_Proveedor: producto.id_Proveedor,
                Id_Categoria: producto.id_Categoria,
                Id_Estado: nuevoEstado,
                Estado: nuevoNombreEstado,
                Categoria: producto.categoria || '',
                Proveedor: producto.nombre_Proveedor || producto.proveedor || '',
                Nombre_Marca: producto.nombre_Marca || '',
                Nombre_Modelo: producto.nombre_Modelo || '',
                Id_Creador: 1,
                Id_Modificador: 1,
            });

            await Swal.fire({
                icon: 'success',
                title: '¡Estado actualizado!',
                text: `El producto ahora está ${nuevoNombreEstado}.`,
                confirmButtonColor: '#0891b2',
                timer: 1500,
                timerProgressBar: true,
            });

            fetchProductos(filtro);
        } catch (err) {
            Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cambiar el estado.', confirmButtonColor: '#0891b2' });
        }
    };

    const filasFormateadas = productos.map(p => {
        const esActivo = p.estado === 'Activo' || p.id_Estado === 1;

        return [
            p.id_Producto,
            p.codigo,
            p.nombre,
            p.descripcion?.length > 40 ? p.descripcion.substring(0, 40) + '...' : (p.descripcion || '—'),
            `$${Number(p.precio).toFixed(2)}`,
            `$${Number(p.precio_Compra).toFixed(2)}`,
            p.nombre_Marca || '—',
            p.nombre_Modelo || '—',

            <span key={`estado-${p.id_Producto}`} className={`px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full border ${
                esActivo
                    ? 'bg-green-100 text-green-800 border-green-200'
                    : 'bg-red-100 text-red-800 border-red-200'
            }`}>
                {p.estado || (esActivo ? 'Activo' : 'Inactivo')}
            </span>,

            <div key={`acc-${p.id_Producto}`} className="flex space-x-2">
                {/* Editar — requiere INV_UPDATE */}
                <button
                    onClick={() => puedeEditar && navigate(`/productos/editar/${p.id_Producto}`)}
                    disabled={!puedeEditar}
                    title={puedeEditar ? 'Editar' : 'Sin permiso para editar'}
                    className={`p-1 transition-colors ${
                        puedeEditar
                            ? 'text-gray-500 hover:text-cyan-600'
                            : 'text-gray-300 cursor-not-allowed'
                    }`}>
                    <Edit size={18} />
                </button>

                {/* Toggle estado — requiere INV_DELETE */}
                <button
                    onClick={() => alternarEstado(p)}
                    disabled={!puedeEliminar}
                    title={puedeEliminar ? 'Cambiar estado' : 'Sin permiso para cambiar estado'}
                    className={`p-1 transition-transform ${
                        !puedeEliminar
                            ? 'text-gray-300 cursor-not-allowed'
                            : esActivo
                                ? 'text-green-600 hover:text-green-800 hover:scale-110'
                                : 'text-gray-400 hover:text-gray-600 hover:scale-110'
                    }`}>
                    {esActivo ? <ToggleRight size={20} /> : <ToggleLeft size={20} />}
                </button>
            </div>
        ];
    });

    return (
        <div className={`p-6 ${modoOscuro ? 'bg-gray-900 text-gray-100' : 'bg-transparent text-gray-800'}`}>
            
            <div className="flex justify-between items-center mb-6">
                <h1 className={`text-3xl font-bold flex items-center ${modoOscuro ? 'text-white' : 'text-black'}`}>
                    <Package className="mr-3 text-cyan-500" size={28} /> Productos
                </h1>

                {/* Nuevo Producto — requiere INV_CREATE */}
                <button
                    onClick={() => puedeCrear && navigate('/productos/nuevo')}
                    disabled={!puedeCrear}
                    title={puedeCrear ? 'Agregar producto' : 'Sin permiso para crear productos'}
                    className={`flex items-center px-4 py-2 font-semibold rounded-lg shadow-md transition-all transform ${
                        puedeCrear
                            ? 'bg-cyan-600 text-white hover:bg-cyan-500 hover:scale-105'
                            : 'bg-gray-300 text-gray-500 cursor-not-allowed'
                    }`}>
                    <Plus size={20} className="mr-2" />
                    Agregar Producto
                </button>
            </div>

            {/* Buscador */}
            <div className="mb-6 relative">
                <input
                    type="text"
                    placeholder="Buscar por nombre, código o categoría..."
                    value={filtro}
                    onChange={(e) => setFiltro(e.target.value)}
                    className={`w-full pl-10 pr-10 py-3 border rounded-xl shadow-sm focus:ring-2 focus:ring-cyan-500 focus:outline-none transition-all ${
                        modoOscuro
                            ? 'bg-gray-800 border-gray-700 text-gray-100 placeholder-gray-400'
                            : 'bg-white border-gray-200 text-gray-900 placeholder-gray-400'
                    }`}
                />
                <Search className={`absolute left-3 top-1/2 -translate-y-1/2 ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`} size={20} />
                {filtro && (
                    <button onClick={() => setFiltro('')} className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-red-500 transition-colors">
                        <X size={20} />
                    </button>
                )}
            </div>

            {/* Tabla */}
            {cargando ? (
                <div className="flex flex-col justify-center items-center h-48 space-y-4">
                    <LoaderCircle className="animate-spin text-cyan-500" size={36} />
                    <p className={`text-sm font-medium animate-pulse ${modoOscuro ? 'text-gray-400' : 'text-gray-600'}`}>
                        Cargando productos...
                    </p>
                </div>
            ) : productos.length === 0 ? (
                <div className="text-center py-20 text-gray-500">
                    No se encontraron productos.
                </div>
            ) : (
                <TablaConTarjetas
                    encabezados={['ID', 'Código', 'Nombre', 'Descripción', 'Precio Venta', 'Precio Compra', 'Marca', 'Modelo', 'Estado', 'Acciones']}
                    filas={filasFormateadas}
                    modoOscuro={modoOscuro}
                />
            )}
        </div>
    );
};

export default Productos;