import { useEffect, useState } from 'react';
import { useParams, useOutletContext } from 'react-router-dom';
import { ArrowLeft, Users, User, Edit, X, Save, LoaderCircle, ShieldCheck } from 'lucide-react';
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

const VistaResumenEmpleado = () => {
    const { id } = useParams();
    const { modoOscuro } = useOutletContext();

    const idUsuarioActual  = parseInt(sessionStorage.getItem('idUsuario') || '0');
    const idPersonaActual  = parseInt(sessionStorage.getItem('idPersona') || '0');

    const [cargando, setCargando] = useState(true);
    const [empleado, setEmpleado] = useState(null);
    const [cargos, setCargos] = useState([]);
    const [editando, setEditando] = useState(false);
    const [guardando, setGuardando] = useState(false);

    // ── Form empleado (para admins) ───────────────────────────────────────────
    const [form, setForm] = useState({
        id_Cargo: '', fecha_Contratacion: '', fecha_Despido: '', id_Estado: 1,
    });

    // ── Form usuario propio (para el empleado logueado) ───────────────────────
    const [formUsuario, setFormUsuario] = useState({ nombre_Usuario: '', contrasena: '', confirmar: '' });
    const [usuarioEmpleado, setUsuarioEmpleado] = useState(null);
    const [editandoUsuario, setEditandoUsuario] = useState(false);
    const [guardandoUsuario, setGuardandoUsuario] = useState(false);

    const [esMiEmpleado, setEsMiEmpleado] = useState(false);

    const cargarDatos = async () => {
        try {
            setCargando(true);
            const [resE, resCat, resU] = await Promise.all([
                api.get('/Empleado/ListarEmpleado'),
                api.get('/Catalogo/ListarCatalogo'),
                api.get('/Usuario/ListarUsuarios'),
            ]);
            const lista = Array.isArray(resE.data) ? resE.data : (resE.data?.datos || []);
            const e = lista.find(x => x.id_Empleado === parseInt(id));
            if (!e) throw new Error('Empleado no encontrado');
            setEmpleado(e);
            setForm({
                id_Cargo: e.id_Cargo || '',
                fecha_Contratacion: e.fecha_Contratacion ? e.fecha_Contratacion.split('T')[0] : '',
                fecha_Despido: e.fecha_Despido ? e.fecha_Despido.split('T')[0] : '',
                id_Estado: e.id_Estado || 1,
            });

            const catData = Array.isArray(resCat.data) ? resCat.data : [];
            setCargos(catData.filter(c => c.id_Tipo_Catalogo === 4));

            // ── Determinar si es el propio empleado ──
            const esMio = e.id_Persona === idPersonaActual;
            setEsMiEmpleado(esMio);

            // ── Cargar usuario vinculado al empleado ──
            const usuarios = Array.isArray(resU.data) ? resU.data : (resU.data?.datos || []);
            const u = usuarios.find(x => x.id_Persona === e.id_Persona);
            if (u) {
                setUsuarioEmpleado(u);
                setFormUsuario({ nombre_Usuario: u.nombre_Usuario || '', contrasena: '', confirmar: '' });
            }
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudieron cargar los datos.', confirmButtonColor: '#0891b2' });
        } finally {
            setCargando(false);
        }
    };

    useEffect(() => { cargarDatos(); }, [id]);

    // ── Guardar datos del empleado (admin) ────────────────────────────────────
    const guardar = async () => {
        if (!form.id_Cargo) {
            Swal.fire({ icon: 'warning', title: 'Selecciona un cargo', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardando(true);
            const cargoSeleccionado = cargos.find(c => c.id_Catalogo === parseInt(form.id_Cargo));
            await api.put(`/Empleado/EditarEmpleado/${id}`, {
                Id_Empleado: parseInt(id),
                Id_Persona: empleado.id_Persona,
                Nombre_Completo: empleado.nombre_Completo || '',
                Cedula: empleado.cedula || '',
                Fecha_Contratacion: new Date(form.fecha_Contratacion).toISOString(),
                Fecha_Despido: form.fecha_Despido ? new Date(form.fecha_Despido).toISOString() : null,
                Id_Cargo: parseInt(form.id_Cargo),
                Cargo: cargoSeleccionado?.nombre_Catalogo || empleado.cargo || '',
                Id_Estado: parseInt(form.id_Estado),
                Estado: parseInt(form.id_Estado) === 1 ? 'Activo' : 'Inactivo',
                Fecha_Creacion: empleado.fecha_Creacion,
                Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: empleado.id_Creador || 1,
                Id_Modificador: idUsuarioActual,
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

    // ── Guardar usuario propio (empleado) ─────────────────────────────────────
    const guardarUsuario = async () => {
        if (!formUsuario.nombre_Usuario.trim()) {
            Swal.fire({ icon: 'warning', title: 'El nombre de usuario es requerido', confirmButtonColor: '#0891b2' }); return;
        }
        if (formUsuario.contrasena && formUsuario.contrasena !== formUsuario.confirmar) {
            Swal.fire({ icon: 'warning', title: 'Las contraseñas no coinciden', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardandoUsuario(true);
            await api.put(`/Usuario/EditarUsuario/${usuarioEmpleado.id_Usuario}`, {
                Id_Usuario:          usuarioEmpleado.id_Usuario,
                Nombre_Usuario:      formUsuario.nombre_Usuario.trim(),
                Contrasena:          formUsuario.contrasena.trim() || usuarioEmpleado.contrasena || '',
                Id_Persona:          usuarioEmpleado.id_Persona,
                Id_Rol:              usuarioEmpleado.id_Rol,
                Id_Estado:           usuarioEmpleado.id_Estado,
                Id_Creador:          usuarioEmpleado.id_Creador || 1,
                Id_Modificador:      idUsuarioActual,
                Token:               usuarioEmpleado.token || '',
                Nombre_Rol:          usuarioEmpleado.nombre_Rol || '',
                Estado_Nombre:       usuarioEmpleado.estado_Nombre || 'Activo',
                Nombre_Persona:      usuarioEmpleado.nombre_Persona || '',
                Fecha_Ultimo_Acceso: usuarioEmpleado.fecha_Ultimo_Acceso || null,
                Fecha_Creacion:      usuarioEmpleado.fecha_Creacion,
                Fecha_Modificacion:  new Date().toISOString(),
            });
            sessionStorage.setItem('nombreUsuario', formUsuario.nombre_Usuario.trim());
            setEditandoUsuario(false);
            await cargarDatos();
            Swal.fire({ icon: 'success', title: 'Usuario actualizado', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al guardar usuario', confirmButtonColor: '#0891b2' });
        } finally {
            setGuardandoUsuario(false); }
    };

    if (cargando) return (
        <div className="flex justify-center items-center py-32">
            <LoaderCircle size={36} className="animate-spin text-cyan-500" />
        </div>
    );

    if (!empleado) return <div className="p-6 text-center text-red-500">No se encontró el empleado.</div>;

    const esActivo = empleado.id_Estado === 1;

    return (
        <div className={`p-6 max-w-3xl mx-auto ${modoOscuro ? 'text-gray-100' : 'text-gray-800'}`}>

            <div className="flex items-center justify-between mb-6">
                <div className="flex items-center gap-3">
                    <Users size={26} className="text-cyan-500" />
                    <div>
                        <h1 className="text-xl font-bold">{empleado.nombre_Completo}</h1>
                        <p className={`text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                            ID #{empleado.id_Empleado} · {empleado.cedula || '—'} · {empleado.cargo || '—'}
                            {esMiEmpleado && (
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

            {/* ── Datos del empleado — solo admin puede editar ── */}
            <Seccion icono={<Users size={16} className="text-cyan-500" />} titulo="Datos del Empleado" modoOscuro={modoOscuro}>
                {editando ? (
                    <>
                        <div className="grid grid-cols-2 gap-4">
                            <div className="col-span-2">
                                <label className={labelCls(modoOscuro)}>Cargo *</label>
                                <select className={inputCls(modoOscuro)} value={form.id_Cargo}
                                    onChange={e => setForm(f => ({ ...f, id_Cargo: e.target.value }))}>
                                    <option value="">Seleccionar...</option>
                                    {cargos.map(c => <option key={c.id_Catalogo} value={c.id_Catalogo}>{c.nombre_Catalogo}</option>)}
                                </select>
                            </div>
                            <div>
                                <label className={labelCls(modoOscuro)}>Fecha de Contratación *</label>
                                <input type="date" className={inputCls(modoOscuro)} value={form.fecha_Contratacion}
                                    onChange={e => setForm(f => ({ ...f, fecha_Contratacion: e.target.value }))} />
                            </div>
                            <div>
                                <label className={labelCls(modoOscuro)}>Fecha de Despido</label>
                                <input type="date" className={inputCls(modoOscuro)} value={form.fecha_Despido}
                                    onChange={e => setForm(f => ({ ...f, fecha_Despido: e.target.value }))} />
                            </div>
                            <div>
                                <label className={labelCls(modoOscuro)}>Estado</label>
                                <select className={inputCls(modoOscuro)} value={form.id_Estado}
                                    onChange={e => setForm(f => ({ ...f, id_Estado: parseInt(e.target.value) }))}>
                                    <option value={1}>Activo</option>
                                    <option value={2}>Inactivo</option>
                                </select>
                            </div>
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
                                <span className={labelCls(modoOscuro)}>Nombre completo</span>
                                <p className="font-medium">{empleado.nombre_Completo || '—'}</p>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Cédula</span>
                                <p className="font-medium">{empleado.cedula || '—'}</p>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Cargo</span>
                                <p className="font-medium">{empleado.cargo || '—'}</p>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Estado</span>
                                <span className={`inline-block mt-1 px-2 py-0.5 text-xs rounded-full border font-semibold ${esActivo ? 'bg-green-100 text-green-800 border-green-200' : 'bg-red-100 text-red-800 border-red-200'}`}>
                                    {esActivo ? 'Activo' : 'Inactivo'}
                                </span>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Fecha de Contratación</span>
                                <p className="font-medium">{empleado.fecha_Contratacion ? new Date(empleado.fecha_Contratacion).toLocaleDateString() : '—'}</p>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Fecha de Despido</span>
                                <p className="font-medium">{empleado.fecha_Despido ? new Date(empleado.fecha_Despido).toLocaleDateString() : '—'}</p>
                            </div>
                        </div>
                        {/* Solo admin puede editar datos del empleado */}
                        {!esMiEmpleado && (
                            <div className="flex justify-end mt-4">
                                <button onClick={() => setEditando(true)}
                                    className="flex items-center gap-1 px-3 py-1.5 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition">
                                    <Edit size={14} /> Editar
                                </button>
                            </div>
                        )}
                    </>
                )}
            </Seccion>

            {/* ── Usuario vinculado — empleado puede editar su nombre/contraseña ── */}
            {usuarioEmpleado && (
                <Seccion icono={<ShieldCheck size={16} className="text-cyan-500" />} titulo="Mi Usuario" modoOscuro={modoOscuro}>
                    {editandoUsuario ? (
                        <>
                            <div className="grid grid-cols-2 gap-4">
                                <div>
                                    <label className={labelCls(modoOscuro)}>Nombre de usuario *</label>
                                    <input className={inputCls(modoOscuro)} value={formUsuario.nombre_Usuario}
                                        onChange={e => setFormUsuario(f => ({ ...f, nombre_Usuario: e.target.value }))} />
                                </div>
                                <div>
                                    <label className={labelCls(modoOscuro)}>
                                        Nueva contraseña{' '}
                                        <span className={`font-normal ${modoOscuro ? 'text-gray-500' : 'text-gray-400'}`}>(vacío = no cambiar)</span>
                                    </label>
                                    <input type="password" className={inputCls(modoOscuro)} placeholder="••••••••"
                                        value={formUsuario.contrasena}
                                        onChange={e => setFormUsuario(f => ({ ...f, contrasena: e.target.value }))} />
                                </div>
                                {formUsuario.contrasena && (
                                    <div className="col-span-2">
                                        <label className={labelCls(modoOscuro)}>Confirmar contraseña</label>
                                        <input type="password"
                                            className={`${inputCls(modoOscuro)} ${formUsuario.confirmar && formUsuario.contrasena !== formUsuario.confirmar ? 'border-red-400' : ''}`}
                                            placeholder="••••••••" value={formUsuario.confirmar}
                                            onChange={e => setFormUsuario(f => ({ ...f, confirmar: e.target.value }))} />
                                        {formUsuario.confirmar && formUsuario.contrasena !== formUsuario.confirmar && (
                                            <p className="text-xs text-red-400 mt-1">Las contraseñas no coinciden</p>
                                        )}
                                    </div>
                                )}
                            </div>
                            <div className="flex gap-2 mt-4 justify-end">
                                <button onClick={() => setEditandoUsuario(false)}
                                    className={`flex items-center gap-1 px-3 py-1.5 rounded-lg text-sm border transition ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                                    <X size={14} /> Cancelar
                                </button>
                                <button onClick={guardarUsuario} disabled={guardandoUsuario}
                                    className="flex items-center gap-1 px-4 py-1.5 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition disabled:opacity-60">
                                    {guardandoUsuario ? <LoaderCircle size={14} className="animate-spin" /> : <Save size={14} />} Guardar
                                </button>
                            </div>
                        </>
                    ) : (
                        <>
                            <div className="grid grid-cols-2 gap-3 text-sm">
                                <div>
                                    <span className={labelCls(modoOscuro)}>Nombre de usuario</span>
                                    <p className="font-medium">{usuarioEmpleado.nombre_Usuario}</p>
                                </div>
                                <div>
                                    <span className={labelCls(modoOscuro)}>Rol</span>
                                    <p className="font-medium">{usuarioEmpleado.nombre_Rol || '—'}</p>
                                </div>
                                <div>
                                    <span className={labelCls(modoOscuro)}>Miembro desde</span>
                                    <p className="font-medium">{usuarioEmpleado.fecha_Creacion ? new Date(usuarioEmpleado.fecha_Creacion).toLocaleDateString() : '—'}</p>
                                </div>
                                <div>
                                    <span className={labelCls(modoOscuro)}>Último acceso</span>
                                    <p className="font-medium">{usuarioEmpleado.fecha_Ultimo_Acceso ? new Date(usuarioEmpleado.fecha_Ultimo_Acceso).toLocaleDateString() : '—'}</p>
                                </div>
                            </div>
                            {esMiEmpleado && (
                                <div className="flex justify-end mt-4">
                                    <button onClick={() => setEditandoUsuario(true)}
                                        className="flex items-center gap-1 px-3 py-1.5 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition">
                                        <Edit size={14} /> Editar mi usuario
                                    </button>
                                </div>
                            )}
                        </>
                    )}
                </Seccion>
            )}

            {/* ── Persona vinculada ── */}
            <Seccion icono={<User size={16} className="text-cyan-500" />} titulo="Persona Vinculada" modoOscuro={modoOscuro}>
                <div className="flex items-center justify-between">
                    <div className="text-sm">
                        <span className={labelCls(modoOscuro)}>Nombre completo</span>
                        <p className="font-medium">{empleado.nombre_Completo || '—'}</p>
                    </div>
                    <button onClick={() => window.location.href = `/usuarios/personas/resumen/${empleado.id_Persona}`}
                        className="flex items-center gap-1 px-3 py-1.5 border border-cyan-500 text-cyan-500 rounded-lg text-sm hover:bg-cyan-50 transition ml-4">
                        <User size={14} /> Ver persona
                    </button>
                </div>
            </Seccion>
        </div>
    );
};

export default VistaResumenEmpleado;