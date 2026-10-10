import { useEffect, useState } from 'react';
import { useNavigate, useParams, useOutletContext } from 'react-router-dom';
import { ArrowLeft, Save, LayoutGrid, LoaderCircle } from 'lucide-react';
import Swal from 'sweetalert2';
import api from '../../api/axios.Config';

const EditarCategoria = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const { modoOscuro } = useOutletContext();

    const [nombre, setNombre] = useState('');
    const [idEstado, setIdEstado] = useState(1);
    const [nombreEstado, setNombreEstado] = useState('Activo');
    const [fechaCreacion, setFechaCreacion] = useState('');
    const [cargando, setCargando] = useState(true);
    const [guardando, setGuardando] = useState(false);

    // Cargar datos de la categoría al montar
    useEffect(() => {
        const fetchCategoria = async () => {
            try {
                setCargando(true);
                const res = await api.get(`/Categoria/ObtenerCategoriaPorId/${id}`);
                const cat = res.data?.datos ?? res.data;

                setNombre(cat.nombre || '');
                setIdEstado(cat.id_Estado ?? 1);
                setNombreEstado(cat.nombre_Estado || 'Activo');
                setFechaCreacion(cat.fecha_Creacion || new Date().toISOString());
            } catch (err) {
                console.error('Error al cargar categoría:', err);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'No se pudo cargar la categoría.',
                    confirmButtonColor: '#0891b2',
                }).then(() => navigate('/categorias'));
            } finally {
                setCargando(false);
            }
        };

        fetchCategoria();
    }, [id]);

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!nombre.trim()) {
            Swal.fire({
                icon: 'warning',
                title: 'Campo requerido',
                text: 'El nombre de la categoría no puede estar vacío.',
                confirmButtonColor: '#0891b2',
            });
            return;
        }

        const result = await Swal.fire({
            title: '¿Guardar cambios?',
            text: `Se actualizará la categoría "${nombre}".`,
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
            const categoriaActualizada = {
                Id_Categoria: Number(id),
                Nombre: nombre.trim(),
                Id_Estado: idEstado,
                Nombre_Estado: nombreEstado,
                Fecha_Creacion: fechaCreacion,
                Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: 1,
                Id_Modificador: 1,
            };

            await api.put(`/Categoria/EditarCategoria/${id}`, categoriaActualizada);

            await Swal.fire({
                icon: 'success',
                title: '¡Guardado!',
                text: 'La categoría fue actualizada correctamente.',
                confirmButtonColor: '#0891b2',
                timer: 2000,
                timerProgressBar: true,
            });

            window.location.href = '/categorias';
        } catch (err) {
            console.error('Error al guardar:', err);
            Swal.fire({
                icon: 'error',
                title: 'Error al guardar',
                text: err.message || 'No se pudo actualizar la categoría.',
                confirmButtonColor: '#0891b2',
            });
        } finally {
            setGuardando(false);
        }
    };

    // ── Loading skeleton ──────────────────────────────────────────────────────
    if (cargando) {
        return (
            <div className={`p-6 flex justify-center items-center h-64 ${modoOscuro ? 'bg-gray-900' : 'bg-transparent'}`}>
                <div className="flex flex-col items-center space-y-3">
                    <LoaderCircle className="animate-spin text-cyan-500" size={36} />
                    <p className={`text-sm font-medium ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                        Cargando categoría...
                    </p>
                </div>
            </div>
        );
    }

    // ── Form ──────────────────────────────────────────────────────────────────
    return (
        <div className={`p-6 ${modoOscuro ? 'bg-gray-900 text-gray-100' : 'bg-transparent text-gray-800'}`}>

            {/* Header */}
            <div className="flex justify-between items-center mb-6">
                <h1 className={`text-3xl font-bold flex items-center ${modoOscuro ? 'text-white' : 'text-black'}`}>
                    <LayoutGrid className="mr-3 text-cyan-500" size={28} />
                    Editar Categoría
                </h1>
                <button
                    onClick={() => window.location.href = '/categorias'}
                    className={`flex items-center px-4 py-2 rounded-lg border font-medium transition-all hover:scale-105 ${
                        modoOscuro
                            ? 'border-gray-600 text-gray-300 hover:bg-gray-700'
                            : 'border-gray-300 text-gray-600 hover:bg-gray-100'
                    }`}
                >
                    <ArrowLeft size={18} className="mr-2" />
                    Volver
                </button>
            </div>

            {/* Card */}
            <div className={`max-w-2xl rounded-2xl shadow-md border p-8 ${
                modoOscuro
                    ? 'bg-gray-800 border-gray-700'
                    : 'bg-white border-gray-200'
            }`}>
                <form onSubmit={handleSubmit} className="space-y-6">

                    {/* ID (solo lectura) */}
                    <div>
                        <label className={`block text-sm font-semibold mb-1 ${modoOscuro ? 'text-gray-300' : 'text-gray-600'}`}>
                            ID de Categoría
                        </label>
                        <input
                            type="text"
                            value={id}
                            disabled
                            className={`w-full px-4 py-2.5 rounded-xl border text-sm cursor-not-allowed ${
                                modoOscuro
                                    ? 'bg-gray-700 border-gray-600 text-gray-400'
                                    : 'bg-gray-100 border-gray-200 text-gray-400'
                            }`}
                        />
                    </div>

                    {/* Nombre */}
                    <div>
                        <label className={`block text-sm font-semibold mb-1 ${modoOscuro ? 'text-gray-300' : 'text-gray-700'}`}>
                            Nombre <span className="text-red-500">*</span>
                        </label>
                        <input
                            type="text"
                            value={nombre}
                            onChange={(e) => setNombre(e.target.value)}
                            placeholder="Nombre de la categoría"
                            required
                            className={`w-full px-4 py-2.5 rounded-xl border text-sm focus:outline-none focus:ring-2 focus:ring-cyan-500 transition-all ${
                                modoOscuro
                                    ? 'bg-gray-700 border-gray-600 text-gray-100 placeholder-gray-500'
                                    : 'bg-white border-gray-300 text-gray-900 placeholder-gray-400'
                            }`}
                        />
                    </div>

                    {/* Estado */}
                    <div>
                        <label className={`block text-sm font-semibold mb-1 ${modoOscuro ? 'text-gray-300' : 'text-gray-700'}`}>
                            Estado
                        </label>
                        <select
                            value={idEstado}
                            onChange={(e) => {
                                const val = Number(e.target.value);
                                setIdEstado(val);
                                setNombreEstado(val === 1 ? 'Activo' : 'Inactivo');
                            }}
                            className={`w-full px-4 py-2.5 rounded-xl border text-sm focus:outline-none focus:ring-2 focus:ring-cyan-500 transition-all ${
                                modoOscuro
                                    ? 'bg-gray-700 border-gray-600 text-gray-100'
                                    : 'bg-white border-gray-300 text-gray-900'
                            }`}
                        >
                            <option value={1}>Activo</option>
                            <option value={2}>Inactivo</option>
                        </select>

                        {/* Badge de estado actual */}
                        <div className="mt-2">
                            <span className={`px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full border ${
                                idEstado === 1
                                    ? 'bg-green-100 text-green-800 border-green-200'
                                    : 'bg-red-100 text-red-800 border-red-200'
                            }`}>
                                {nombreEstado}
                            </span>
                        </div>
                    </div>

                    {/* Botón guardar */}
                    <div className="flex justify-end pt-2">
                        <button
                            type="submit"
                            disabled={guardando}
                            className="flex items-center px-6 py-2.5 bg-cyan-600 text-white font-semibold rounded-lg shadow-md hover:bg-cyan-500 transition-all transform hover:scale-105 disabled:opacity-60 disabled:cursor-not-allowed disabled:transform-none"
                        >
                            {guardando
                                ? <LoaderCircle size={18} className="mr-2 animate-spin" />
                                : <Save size={18} className="mr-2" />
                            }
                            {guardando ? 'Guardando...' : 'Guardar Cambios'}
                        </button>
                    </div>

                </form>
            </div>
        </div>
    );
};

export default EditarCategoria;