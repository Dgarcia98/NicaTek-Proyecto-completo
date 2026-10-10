import { useEffect, useState } from 'react';
import { useOutletContext, useNavigate } from 'react-router-dom';
import { LayoutGrid, LoaderCircle, Plus, Edit, ToggleRight, ToggleLeft } from 'lucide-react';
import Swal from 'sweetalert2';
import TablaConTarjetas from '../../components/TablaConTarjetas';
import api from '../../api/axios.Config';
import { tienePermiso } from '../../utils/Permisos';

const Categorias = () => {
    const [categorias, setCategorias] = useState([]);
    const [cargando, setCargando] = useState(true);
    const { modoOscuro } = useOutletContext();
    const navigate = useNavigate();

    const puedeCrear    = tienePermiso('INV_CREATE');
    const puedeEditar   = tienePermiso('INV_UPDATE');
    const puedeEliminar = tienePermiso('INV_DELETE');

    const fetchCategorias = async () => {
        try {
            setCargando(true);
            const res = await api.get('/Categoria/ListarCategoria');
            const data = Array.isArray(res.data) ? res.data : (res.data.datos || []);
            setCategorias(data.map(cat => ({
                id: cat.id_Categoria, nombre: cat.nombre,
                estado: cat.nombre_Estado, idEstado: cat.id_Estado, fecha: cat.fecha_Modificacion
            })));
        } catch (err) { console.error("Error al cargar:", err);
        } finally { setCargando(false); }
    };

    useEffect(() => { fetchCategorias(); }, []);

    const alternarEstado = async (cat) => {
        if (!puedeEliminar) return;
        const esActivo = cat.estado === 'Activo';
        const nuevoEstado = esActivo ? 2 : 1;
        const nuevoNombreEstado = esActivo ? 'Inactivo' : 'Activo';
        const result = await Swal.fire({
            title: `¿Cambiar estado?`, text: `La categoría "${cat.nombre}" pasará a ${nuevoNombreEstado}.`,
            icon: 'warning', showCancelButton: true, confirmButtonColor: '#0891b2', cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, cambiar', cancelButtonText: 'Cancelar',
        });
        if (!result.isConfirmed) return;
        try {
            await api.put(`/Categoria/EditarCategoria/${cat.id}`, {
                Id_Categoria: cat.id, Nombre: cat.nombre, Id_Estado: nuevoEstado,
                Nombre_Estado: nuevoNombreEstado, Fecha_Creacion: new Date().toISOString(),
                Fecha_Modificacion: new Date().toISOString(), Id_Creador: 1, Id_Modificador: 1,
            });
            await Swal.fire({ icon: 'success', title: '¡Estado actualizado!', confirmButtonColor: '#0891b2', timer: 1500, timerProgressBar: true });
            fetchCategorias();
        } catch { Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cambiar el estado.', confirmButtonColor: '#0891b2' }); }
    };

    return (
        <div className={`p-6 ${modoOscuro ? 'bg-gray-900 text-white' : 'bg-gray-50'}`}>
            <div className="flex justify-between mb-6">
                <h1 className="text-2xl font-bold flex items-center"><LayoutGrid className="mr-2" /> Categorías</h1>
                <button onClick={() => puedeCrear && navigate('/categorias/nueva')} disabled={!puedeCrear}
                    title={puedeCrear ? 'Nueva categoría' : 'Sin permiso para crear'}
                    className={`px-4 py-2 rounded-lg flex items-center transition-colors ${puedeCrear ? 'bg-blue-600 text-white hover:bg-blue-700' : 'bg-gray-300 text-gray-500 cursor-not-allowed'}`}>
                    <Plus size={20} className="mr-2" /> Nueva
                </button>
            </div>
            {cargando ? (
                <div className="flex justify-center py-10"><LoaderCircle className="animate-spin" /></div>
            ) : (
                <TablaConTarjetas
                    encabezados={['ID', 'Nombre', 'Estado', 'Última mod.', 'Acciones']}
                    filas={categorias.map(cat => [
                        cat.id, cat.nombre,
                        <span key={`estado-${cat.id}`} className={`px-3 py-1 rounded-full text-xs font-semibold border ${cat.estado === 'Activo' ? 'bg-green-100 text-green-800 border-green-200' : 'bg-red-100 text-red-800 border-red-200'}`}>{cat.estado}</span>,
                        new Date(cat.fecha).toLocaleDateString(),
                        <div key={`acc-${cat.id}`} className="flex space-x-2">
                            <button onClick={() => puedeEditar && navigate(`/categorias/editar/${cat.id}`)} disabled={!puedeEditar}
                                title={puedeEditar ? 'Editar' : 'Sin permiso'} className={`p-1 ${puedeEditar ? 'text-blue-500 hover:text-blue-700' : 'text-gray-300 cursor-not-allowed'}`}>
                                <Edit size={18} />
                            </button>
                            <button onClick={() => alternarEstado(cat)} disabled={!puedeEliminar}
                                title={puedeEliminar ? 'Cambiar estado' : 'Sin permiso'}
                                className={`p-1 hover:scale-110 transition-transform ${!puedeEliminar ? 'text-gray-300 cursor-not-allowed' : cat.estado === 'Activo' ? 'text-green-600 hover:text-green-800' : 'text-gray-400 hover:text-gray-600'}`}>
                                {cat.estado === 'Activo' ? <ToggleRight size={20} /> : <ToggleLeft size={20} />}
                            </button>
                        </div>
                    ])}
                    modoOscuro={modoOscuro}
                />
            )}
        </div>
    );
};

export default Categorias;