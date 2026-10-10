import { useEffect, useState } from 'react';
import { useParams, useOutletContext } from 'react-router-dom';
import { ArrowLeft, Users, User, Edit, X, Save, LoaderCircle } from 'lucide-react';
import Swal from 'sweetalert2';
import api from '../../api/axios.Config';

const Seccion = ({ icono, titulo, modoOscuro, children }) => (
    <div className={`rounded-2xl shadow-md border p-6 mb-5 ${modoOscuro ? 'bg-gray-800 border-gray-700' : 'bg-white border-gray-200'}`}>
        <h2 className={`text-base font-semibold flex items-center gap-2 mb-4 pb-3 border-b ${modoOscuro ? 'text-gray-100 border-gray-700' : 'text-gray-700 border-gray-100'}`}>
            {icono} {titulo}
        </h2>
        {children}
    </div>
);

const inputCls = (modoOscuro) =>
    `w-full px-3 py-2 rounded-lg border text-sm outline-none transition focus:ring-2 focus:ring-cyan-400 ${modoOscuro ? 'bg-gray-700 border-gray-600 text-gray-100' : 'bg-gray-50 border-gray-300 text-gray-800'}`;

const labelCls = (modoOscuro) =>
    `block text-xs font-medium mb-1 ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`;

const VistaResumenCliente = () => {
    const { id } = useParams();
    const { modoOscuro } = useOutletContext();

    const [cargando, setCargando] = useState(true);
    const [cliente, setCliente] = useState(null);
    const [editando, setEditando] = useState(false);
    const [guardando, setGuardando] = useState(false);
    const [idEstado, setIdEstado] = useState(1);

    const cargarDatos = async () => {
        try {
            setCargando(true);
            const res = await api.get(`/Cliente/ListarPorFiltro/${id}`);
            const lista = Array.isArray(res.data) ? res.data : (res.data?.datos || []);
            const c = lista.find(x => x.id_Cliente === parseInt(id));
            if (!c) throw new Error("Cliente no encontrado");
            setCliente(c);
            setIdEstado(c.id_Estado || 1);
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudieron cargar los datos.', confirmButtonColor: '#0891b2' });
        } finally {
            setCargando(false);
        }
    };

    useEffect(() => { cargarDatos(); }, [id]);

    const guardar = async () => {
        try {
            setGuardando(true);
            await api.put(`/Cliente/EditarCliente/${id}`, {
                Id_Cliente: parseInt(id),
                Id_Persona: cliente.id_Persona,
                Nombre_Completo: cliente.nombre_Completo || '',
                Id_Estado: idEstado,
                Nombre_Estado: idEstado === 1 ? 'Activo' : 'Inactivo',
                Fecha_Creacion: cliente.fecha_Creacion,
                Id_Creador: cliente.id_Creador || 1,
                Fecha_Modificacion: new Date().toISOString(),
                Id_Modificador: 1,
                Estado: idEstado === 1,
            });
            setEditando(false);
            await cargarDatos();
            Swal.fire({ icon: 'success', title: 'Guardado', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al guardar', confirmButtonColor: '#0891b2' });
        } finally {
            setGuardando(false);
        }
    };

    if (cargando) return (
        <div className="flex justify-center items-center py-32">
            <LoaderCircle size={36} className="animate-spin text-cyan-500" />
        </div>
    );

    if (!cliente) return (
        <div className="p-6 text-center text-red-500">No se encontró el cliente.</div>
    );

    const esActivo = cliente.id_Estado === 1;

    return (
        <div className={`p-6 max-w-3xl mx-auto ${modoOscuro ? 'text-gray-100' : 'text-gray-800'}`}>

            {/* Header */}
            <div className="flex items-center justify-between mb-6">
                <div className="flex items-center gap-3">
                    <Users size={26} className="text-cyan-500" />
                    <div>
                        <h1 className="text-xl font-bold">{cliente.nombre_Completo || '—'}</h1>
                        <p className={`text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                            ID Cliente #{cliente.id_Cliente}
                        </p>
                    </div>
                </div>
                <button onClick={() => window.location.href = '/usuarios/clientes'}
                    className={`flex items-center gap-1 px-3 py-2 rounded-lg border text-sm transition ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                    <ArrowLeft size={16} /> Volver
                </button>
            </div>

            {/* ── Datos del Cliente ── */}
            <Seccion icono={<Users size={16} className="text-cyan-500" />} titulo="Datos del Cliente" modoOscuro={modoOscuro}>
                {editando ? (
                    <>
                        <div className="max-w-xs">
                            <label className={labelCls(modoOscuro)}>Estado</label>
                            <select className={inputCls(modoOscuro)} value={idEstado}
                                onChange={e => setIdEstado(parseInt(e.target.value))}>
                                <option value={1}>Activo</option>
                                <option value={2}>Inactivo</option>
                            </select>
                        </div>
                        <div className="flex gap-2 mt-4 justify-end">
                            <button onClick={() => { setEditando(false); setIdEstado(cliente.id_Estado); }}
                                className={`flex items-center gap-1 px-3 py-1.5 rounded-lg text-sm border transition ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                                <X size={14} /> Cancelar
                            </button>
                            <button onClick={guardar} disabled={guardando}
                                className="flex items-center gap-1 px-4 py-1.5 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition disabled:opacity-60">
                                {guardando ? <LoaderCircle size={14} className="animate-spin" /> : <Save size={14} />} Guardar
                            </button>
                        </div>
                    </>
                ) : (
                    <>
                        <div className="grid grid-cols-2 gap-3 text-sm">
                            <div>
                                <span className={labelCls(modoOscuro)}>Nombre completo</span>
                                <p className="font-medium">{cliente.nombre_Completo || '—'}</p>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Estado</span>
                                <span className={`inline-block mt-1 px-2 py-0.5 text-xs rounded-full border font-semibold ${esActivo ? 'bg-green-100 text-green-800 border-green-200' : 'bg-red-100 text-red-800 border-red-200'}`}>
                                    {esActivo ? 'Activo' : 'Inactivo'}
                                </span>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Fecha de Registro</span>
                                <p className="font-medium">
                                    {cliente.fecha_Creacion ? new Date(cliente.fecha_Creacion).toLocaleDateString() : '—'}
                                </p>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Última Modificación</span>
                                <p className="font-medium">
                                    {cliente.fecha_Modificacion ? new Date(cliente.fecha_Modificacion).toLocaleDateString() : '—'}
                                </p>
                            </div>
                        </div>
                        <div className="flex justify-end mt-4">
                            <button onClick={() => setEditando(true)}
                                className="flex items-center gap-1 px-3 py-1.5 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition">
                                <Edit size={14} /> Editar
                            </button>
                        </div>
                    </>
                )}
            </Seccion>

            {/* ── Persona vinculada ── */}
            <Seccion icono={<User size={16} className="text-cyan-500" />} titulo="Persona Vinculada" modoOscuro={modoOscuro}>
                <div className="flex items-center justify-between">
                    <div className="text-sm">
                        <span className={labelCls(modoOscuro)}>Nombre completo</span>
                        <p className="font-medium">{cliente.nombre_Completo || '—'}</p>
                    </div>
                    <button
                        onClick={() => window.location.href = `/usuarios/personas/resumen/${cliente.id_Persona}`}
                        className="flex items-center gap-1 px-3 py-1.5 border border-cyan-500 text-cyan-500 rounded-lg text-sm hover:bg-cyan-50 transition ml-4">
                        <User size={14} /> Ver persona
                    </button>
                </div>
            </Seccion>

        </div>
    );
};

export default VistaResumenCliente;