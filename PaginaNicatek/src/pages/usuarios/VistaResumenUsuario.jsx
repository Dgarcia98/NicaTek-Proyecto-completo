import { useEffect, useState } from 'react';
import { useParams, useOutletContext } from 'react-router-dom';
import { ArrowLeft, ShieldCheck, User, Edit, X, Save, LoaderCircle } from 'lucide-react';
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

const VistaResumenUsuario = () => {
    const { id } = useParams();
    const { modoOscuro } = useOutletContext();

    const idUsuarioActual   = parseInt(sessionStorage.getItem('idUsuario') || '0');
    const esMiPropioUsuario = parseInt(id) === idUsuarioActual;

    const [cargando, setCargando] = useState(true);
    const [usuario, setUsuario] = useState(null);
    const [roles, setRoles] = useState([]);
    const [editando, setEditando] = useState(false);
    const [guardando, setGuardando] = useState(false);
    const [form, setForm] = useState({
        nombre_Usuario: '',
        contrasena: '',
        id_Rol: '',
        id_Estado: 1,
    });

    const cargarDatos = async () => {
        try {
            setCargando(true);
            const [resU, resR] = await Promise.all([
                api.get('/Usuario/ListarUsuarios'),
                api.get('/Rol/ListarRoles'),
            ]);
            const lista = Array.isArray(resU.data) ? resU.data : (resU.data?.datos || []);
            const u = lista.find(x => x.id_Usuario === parseInt(id));
            if (!u) throw new Error('Usuario no encontrado');
            setUsuario(u);
            setForm({ nombre_Usuario: u.nombre_Usuario || '', contrasena: '', id_Rol: u.id_Rol || '', id_Estado: u.id_Estado || 1 });
            setRoles(Array.isArray(resR.data) ? resR.data : []);
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudieron cargar los datos.', confirmButtonColor: '#0891b2' });
        } finally {
            setCargando(false);
        }
    };

    useEffect(() => { cargarDatos(); }, [id]);

    const guardar = async () => {
        if (!form.nombre_Usuario.trim()) {
            Swal.fire({ icon: 'warning', title: 'El nombre de usuario es requerido', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardando(true);
            const rolSeleccionado = roles.find(r => r.id_Rol === parseInt(form.id_Rol));
            await api.put(`/Usuario/EditarUsuario/${id}`, {
                Id_Usuario:          parseInt(id),
                Nombre_Usuario:      form.nombre_Usuario.trim(),
                Contrasena:          form.contrasena.trim() || usuario.contrasena || '',
                Id_Persona:          usuario.id_Persona,
                Id_Rol:              parseInt(form.id_Rol),
                Id_Estado:           parseInt(form.id_Estado),
                Id_Creador:          usuario.id_Creador || 1,
                Token:               usuario.token || '',
                Nombre_Rol:          rolSeleccionado?.nombre_Rol || usuario.nombre_Rol || '',
                Estado_Nombre:       parseInt(form.id_Estado) === 1 ? 'Activo' : 'Inactivo',
                Nombre_Persona:      usuario.nombre_Persona || '',
                Fecha_Ultimo_Acceso: usuario.fecha_Ultimo_Acceso || null,
                Fecha_Creacion:      usuario.fecha_Creacion,
                Fecha_Modificacion:  new Date().toISOString(),
                Id_Modificador:      idUsuarioActual,
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

    if (!usuario) return <div className="p-6 text-center text-red-500">No se encontró el usuario.</div>;

    const esActivo = usuario.id_Estado === 1;

    return (
        <div className={`p-6 max-w-3xl mx-auto ${modoOscuro ? 'text-gray-100' : 'text-gray-800'}`}>

            <div className="flex items-center justify-between mb-6">
                <div className="flex items-center gap-3">
                    <ShieldCheck size={26} className="text-cyan-500" />
                    <div>
                        <h1 className="text-xl font-bold">{usuario.nombre_Usuario}</h1>
                        <p className={`text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                            ID #{usuario.id_Usuario} · {usuario.nombre_Rol || '—'}
                            {esMiPropioUsuario && (
                                <span className="ml-2 px-2 py-0.5 bg-cyan-100 text-cyan-700 border border-cyan-200 rounded-full text-[10px] font-semibold">
                                    Tú
                                </span>
                            )}
                        </p>
                    </div>
                </div>
                <button onClick={() => window.location.href = '/usuarios/admins'}
                    className={`flex items-center gap-1 px-3 py-2 rounded-lg border text-sm transition ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                    <ArrowLeft size={16} /> Volver
                </button>
            </div>

            {/* Datos del usuario */}
            <Seccion icono={<ShieldCheck size={16} className="text-cyan-500" />} titulo="Datos del Usuario" modoOscuro={modoOscuro}>
                {editando ? (
                    <>
                        <div className="grid grid-cols-2 gap-4">
                            {/* Nombre de usuario — siempre editable */}
                            <div>
                                <label className={labelCls(modoOscuro)}>Nombre de usuario *</label>
                                <input className={inputCls(modoOscuro)} value={form.nombre_Usuario}
                                    onChange={e => setForm(f => ({ ...f, nombre_Usuario: e.target.value }))} />
                            </div>

                            {/* Contraseña — siempre editable */}
                            <div>
                                <label className={labelCls(modoOscuro)}>
                                    Nueva contraseña{' '}
                                    <span className={`font-normal ${modoOscuro ? 'text-gray-500' : 'text-gray-400'}`}>(dejar vacío para no cambiar)</span>
                                </label>
                                <input type="password" className={inputCls(modoOscuro)} placeholder="••••••••"
                                    value={form.contrasena}
                                    onChange={e => setForm(f => ({ ...f, contrasena: e.target.value }))} />
                            </div>

                            {/* Rol — solo editable si NO es mi propio usuario */}
                            <div>
                                <label className={labelCls(modoOscuro)}>Rol</label>
                                {esMiPropioUsuario ? (
                                    <p className={`text-sm font-medium py-2 ${modoOscuro ? 'text-gray-200' : 'text-gray-700'}`}>
                                        {usuario.nombre_Rol || '—'}
                                        <span className={`ml-2 text-xs ${modoOscuro ? 'text-gray-500' : 'text-gray-400'}`}>(no modificable)</span>
                                    </p>
                                ) : (
                                    <select className={inputCls(modoOscuro)} value={form.id_Rol}
                                        onChange={e => setForm(f => ({ ...f, id_Rol: e.target.value }))}>
                                        <option value="">Seleccionar...</option>
                                        {roles.map(r => <option key={r.id_Rol} value={r.id_Rol}>{r.nombre_Rol}</option>)}
                                    </select>
                                )}
                            </div>

                            {/* Estado — solo editable si NO es mi propio usuario */}
                            {!esMiPropioUsuario && (
                                <div>
                                    <label className={labelCls(modoOscuro)}>Estado</label>
                                    <select className={inputCls(modoOscuro)} value={form.id_Estado}
                                        onChange={e => setForm(f => ({ ...f, id_Estado: parseInt(e.target.value) }))}>
                                        <option value={1}>Activo</option>
                                        <option value={2}>Inactivo</option>
                                    </select>
                                </div>
                            )}
                        </div>

                        <div className="flex gap-2 mt-4 justify-end">
                            <button onClick={() => setEditando(false)}
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
                                <span className={labelCls(modoOscuro)}>Nombre de usuario</span>
                                <p className="font-medium">{usuario.nombre_Usuario}</p>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Rol</span>
                                <p className="font-medium">{usuario.nombre_Rol || '—'}</p>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Estado</span>
                                <span className={`inline-block mt-1 px-2 py-0.5 text-xs rounded-full border font-semibold ${esActivo ? 'bg-green-100 text-green-800 border-green-200' : 'bg-red-100 text-red-800 border-red-200'}`}>
                                    {esActivo ? 'Activo' : 'Inactivo'}
                                </span>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Último acceso</span>
                                <p className="font-medium">{usuario.fecha_Ultimo_Acceso ? new Date(usuario.fecha_Ultimo_Acceso).toLocaleDateString() : '—'}</p>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Fecha de creación</span>
                                <p className="font-medium">{usuario.fecha_Creacion ? new Date(usuario.fecha_Creacion).toLocaleDateString() : '—'}</p>
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

            {/* Persona vinculada */}
            <Seccion icono={<User size={16} className="text-cyan-500" />} titulo="Persona Vinculada" modoOscuro={modoOscuro}>
                <div className="flex items-center justify-between">
                    <div className="text-sm">
                        <span className={labelCls(modoOscuro)}>Nombre</span>
                        <p className="font-medium">{usuario.nombre_Persona || '—'}</p>
                    </div>
                    <button onClick={() => window.location.href = `/usuarios/personas/resumen/${usuario.id_Persona}`}
                        className="flex items-center gap-1 px-3 py-1.5 border border-cyan-500 text-cyan-500 rounded-lg text-sm hover:bg-cyan-50 transition ml-4">
                        <User size={14} /> Ver persona
                    </button>
                </div>
            </Seccion>
        </div>
    );
};

export default VistaResumenUsuario;