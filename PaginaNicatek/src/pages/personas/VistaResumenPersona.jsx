import { useEffect, useState } from 'react';
import { useParams, useOutletContext } from 'react-router-dom';
import { ArrowLeft, User, MapPin, Phone, Mail, Edit, X, Save, LoaderCircle, Plus } from 'lucide-react';
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

const VistaResumenPersona = () => {
    const { id } = useParams();
    const { modoOscuro } = useOutletContext();

    const [cargando, setCargando] = useState(true);
    const [persona, setPersona] = useState(null);
    const [correos, setCorreos] = useState([]);
    const [direcciones, setDirecciones] = useState([]);
    const [telefonos, setTelefonos] = useState([]);
    const [catalogos, setCatalogos] = useState([]);
    const [guardando, setGuardando] = useState('');

    const [editandoPersona, setEditandoPersona] = useState(false);
    const [editandoCorreo, setEditandoCorreo] = useState(null);
    const [editandoDireccion, setEditandoDireccion] = useState(null);
    const [editandoTelefono, setEditandoTelefono] = useState(null);

    const [formPersona, setFormPersona] = useState({});
    const [formCorreo, setFormCorreo] = useState({});
    const [formDireccion, setFormDireccion] = useState({});
    const [formTelefono, setFormTelefono] = useState({});

    const [mostrarNuevoCorreo, setMostrarNuevoCorreo] = useState(false);
    const [mostrarNuevaDireccion, setMostrarNuevaDireccion] = useState(false);
    const [mostrarNuevoTelefono, setMostrarNuevoTelefono] = useState(false);
    const [nuevoCorreo, setNuevoCorreo] = useState({ direccion_Correo: '', id_Tipo_Correo: '' });
    const [nuevaDireccion, setNuevaDireccion] = useState({ direccion_Completa: '', id_Tipo_Direccion: '' });
    const [nuevoTelefono, setNuevoTelefono] = useState({ numero_Telefono: '', id_Tipo_Telefono: '' });

    const filtrarCatalogo = (tipo) => catalogos.filter(c => c.id_Tipo_Catalogo === tipo);

    const cargarDatos = async () => {
        try {
            setCargando(true);
            const [resPersona, resCorreos, resDirs, resTels, resCats] = await Promise.all([
                api.get(`/DatosPersonales/ObtenerDatosPersonalesPorId/${id}`),
                api.get('/Correo/ListarCorreo'),
                api.get('/Direccion/ListarDirecciones'),
                api.get('/Telefono/ListarTelefono'),
                api.get('/Catalogo/ListarCatalogo'),
            ]);

            const p = resPersona.data;
            setPersona(p);
            setFormPersona({
                primer_Nombre: p.primer_Nombre || '',
                segundo_Nombre: p.segundo_Nombre || '',
                primer_Apellido: p.primer_Apellido || '',
                segundo_Apellido: p.segundo_Apellido || '',
                dni: p.dni || '',
                id_Tipo_DNI: p.id_Tipo_DNI || '',
                id_Genero: p.id_Genero || '',
                id_Nacionalidad: p.id_Nacionalidad || '',
            });

            const cats = Array.isArray(resCats.data) ? resCats.data : (resCats.data?.datos || []);
            setCatalogos(cats);

            const idNum = parseInt(id);
            const allCorreos = Array.isArray(resCorreos.data) ? resCorreos.data : (resCorreos.data?.datos || []);
            const allDirs = Array.isArray(resDirs.data) ? resDirs.data : (resDirs.data?.datos || []);
            const allTels = Array.isArray(resTels.data) ? resTels.data : (resTels.data?.datos || []);

            setCorreos(allCorreos.filter(c => (c.id_Persona ?? c.Id_Persona) === idNum));
            setDirecciones(allDirs.filter(d => (d.id_Persona ?? d.Id_Persona) === idNum));
            setTelefonos(allTels.filter(t => (t.id_Persona ?? t.Id_Persona) === idNum));
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudieron cargar los datos.', confirmButtonColor: '#0891b2' });
        } finally {
            setCargando(false);
        }
    };

    useEffect(() => { cargarDatos(); }, [id]);

    const guardarPersona = async () => {
        try {
            setGuardando('persona');
            await api.put(`/DatosPersonales/EditarDatosPersonales/${id}`, {
                Id_Persona: parseInt(id),
                Primer_Nombre: formPersona.primer_Nombre,
                Segundo_Nombre: formPersona.segundo_Nombre || '',
                Primer_Apellido: formPersona.primer_Apellido,
                Segundo_Apellido: formPersona.segundo_Apellido || '',
                Id_Tipo_DNI: parseInt(formPersona.id_Tipo_DNI),
                Dni: formPersona.dni,
                Id_Genero: parseInt(formPersona.id_Genero),
                Id_Nacionalidad: parseInt(formPersona.id_Nacionalidad),
                Estado: persona.estado,
                Id_Modificador: 1,
            });
            setEditandoPersona(false);
            await cargarDatos();
            Swal.fire({ icon: 'success', title: 'Guardado', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al guardar', confirmButtonColor: '#0891b2' });
        } finally { setGuardando(''); }
    };

    const guardarCorreo = async () => {
        try {
            setGuardando('correo');
            await api.put(`/Correo/EditarCorreo/${editandoCorreo}`, {
                Id_Correo: editandoCorreo, Id_Persona: parseInt(id),
                Nombre_Completo: '', Direccion_Correo: formCorreo.direccion_Correo,
                Id_Tipo_Correo: parseInt(formCorreo.id_Tipo_Correo), Tipo_Correo: '',
                Activo: formCorreo.activo, EstadoActivo: '',
                Fecha_Creacion: new Date().toISOString(), Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: 1, Id_Modificador: 1,
            });
            setEditandoCorreo(null);
            await cargarDatos();
            Swal.fire({ icon: 'success', title: 'Guardado', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al guardar correo', confirmButtonColor: '#0891b2' });
        } finally { setGuardando(''); }
    };

    const guardarDireccion = async () => {
        try {
            setGuardando('direccion');
            await api.put(`/Direccion/EditarDireccion/${editandoDireccion}`, {
                Id_Direccion: editandoDireccion, Id_Persona: parseInt(id),
                Nombre_Completo: '', Tipo_Direccion: '', EstadoActivo: '',
                Direccion_Completa: formDireccion.direccion_Completa,
                Id_Tipo_Direccion: parseInt(formDireccion.id_Tipo_Direccion),
                Activo: formDireccion.activo,
                Fecha_Creacion: new Date().toISOString(), Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: 1, Id_Modificador: 1,
            });
            setEditandoDireccion(null);
            await cargarDatos();
            Swal.fire({ icon: 'success', title: 'Guardado', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al guardar dirección', confirmButtonColor: '#0891b2' });
        } finally { setGuardando(''); }
    };

    const guardarTelefono = async () => {
        try {
            setGuardando('telefono');
            await api.put(`/Telefono/EditarTelefono/${editandoTelefono}`, {
                Id_Telefono: editandoTelefono, Id_Persona: parseInt(id),
                Numero_Telefono: formTelefono.numero_Telefono,
                Id_Tipo_Telefono: parseInt(formTelefono.id_Tipo_Telefono),
                Tipo_Telefono: '', Estado: formTelefono.estado, Nombre_Estado: '',
                Fecha_Creacion: new Date().toISOString(), Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: 1, Id_Modificador: 1,
            });
            setEditandoTelefono(null);
            await cargarDatos();
            Swal.fire({ icon: 'success', title: 'Guardado', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al guardar teléfono', confirmButtonColor: '#0891b2' });
        } finally { setGuardando(''); }
    };

    const agregarCorreo = async () => {
        if (!nuevoCorreo.direccion_Correo || !nuevoCorreo.id_Tipo_Correo) {
            Swal.fire({ icon: 'warning', title: 'Completa todos los campos', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardando('nuevoCorreo');
            await api.post('/Correo/NuevoCorreo', {
                Id_Correo: 0, Id_Persona: parseInt(id), Nombre_Completo: '',
                Direccion_Correo: nuevoCorreo.direccion_Correo,
                Id_Tipo_Correo: parseInt(nuevoCorreo.id_Tipo_Correo),
                Tipo_Correo: '', Activo: true, EstadoActivo: '',
                Fecha_Creacion: new Date().toISOString(), Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: 1, Id_Modificador: 1,
            });
            setNuevoCorreo({ direccion_Correo: '', id_Tipo_Correo: '' });
            setMostrarNuevoCorreo(false);
            await cargarDatos();
            Swal.fire({ icon: 'success', title: 'Correo agregado', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al agregar correo', confirmButtonColor: '#0891b2' });
        } finally { setGuardando(''); }
    };

    const agregarDireccion = async () => {
        if (!nuevaDireccion.direccion_Completa || !nuevaDireccion.id_Tipo_Direccion) {
            Swal.fire({ icon: 'warning', title: 'Completa todos los campos', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardando('nuevaDireccion');
            await api.post('/Direccion/NuevaDireccion', {
                Id_Direccion: 0, Id_Persona: parseInt(id), Nombre_Completo: '',
                Tipo_Direccion: '', EstadoActivo: '',
                Direccion_Completa: nuevaDireccion.direccion_Completa,
                Id_Tipo_Direccion: parseInt(nuevaDireccion.id_Tipo_Direccion),
                Activo: true,
                Fecha_Creacion: new Date().toISOString(), Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: 1, Id_Modificador: 1,
            });
            setNuevaDireccion({ direccion_Completa: '', id_Tipo_Direccion: '' });
            setMostrarNuevaDireccion(false);
            await cargarDatos();
            Swal.fire({ icon: 'success', title: 'Dirección agregada', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al agregar dirección', confirmButtonColor: '#0891b2' });
        } finally { setGuardando(''); }
    };

    const agregarTelefono = async () => {
        if (!nuevoTelefono.numero_Telefono || !nuevoTelefono.id_Tipo_Telefono) {
            Swal.fire({ icon: 'warning', title: 'Completa todos los campos', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardando('nuevoTelefono');
            await api.post('/Telefono/NuevoTelefono', {
                Id_Telefono: 0, Id_Persona: parseInt(id),
                Numero_Telefono: nuevoTelefono.numero_Telefono,
                Id_Tipo_Telefono: parseInt(nuevoTelefono.id_Tipo_Telefono),
                Tipo_Telefono: '', Estado: true, Nombre_Estado: '',
                Fecha_Creacion: new Date().toISOString(), Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: 1, Id_Modificador: 1,
            });
            setNuevoTelefono({ numero_Telefono: '', id_Tipo_Telefono: '' });
            setMostrarNuevoTelefono(false);
            await cargarDatos();
            Swal.fire({ icon: 'success', title: 'Teléfono agregado', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al agregar teléfono', confirmButtonColor: '#0891b2' });
        } finally { setGuardando(''); }
    };

    const BotonesEdicion = ({ onGuardar, onCancelar, clave }) => (
        <div className="flex gap-2 mt-4 justify-end">
            <button onClick={onCancelar} className={`flex items-center gap-1 px-3 py-1.5 rounded-lg text-sm border transition ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                <X size={14} /> Cancelar
            </button>
            <button onClick={onGuardar} disabled={guardando === clave} className="flex items-center gap-1 px-4 py-1.5 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition disabled:opacity-60">
                {guardando === clave ? <LoaderCircle size={14} className="animate-spin" /> : <Save size={14} />} Guardar
            </button>
        </div>
    );

    if (cargando) return (
        <div className="flex justify-center items-center py-32">
            <LoaderCircle size={36} className="animate-spin text-cyan-500" />
        </div>
    );

    if (!persona) return (
        <div className="p-6 text-center text-red-500">No se encontró la persona.</div>
    );

    const nombreCompleto = [persona.primer_Nombre, persona.segundo_Nombre, persona.primer_Apellido, persona.segundo_Apellido]
        .filter(Boolean).join(' ');

    return (
        <div className={`p-6 max-w-3xl mx-auto ${modoOscuro ? 'text-gray-100' : 'text-gray-800'}`}>

            {/* Header */}
            <div className="flex items-center justify-between mb-6">
                <div className="flex items-center gap-3">
                    <User size={26} className="text-cyan-500" />
                    <div>
                        <h1 className="text-xl font-bold">{nombreCompleto}</h1>
                        <p className={`text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>ID #{persona.id_Persona} · {persona.dni}</p>
                    </div>
                </div>
                <button onClick={() => window.location.href = '/usuarios/personas'}
                    className={`flex items-center gap-1 px-3 py-2 rounded-lg border text-sm transition ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                    <ArrowLeft size={16} /> Volver
                </button>
            </div>

            {/* ── Datos Personales ── */}
            <Seccion icono={<User size={16} className="text-cyan-500" />} titulo="Datos Personales" modoOscuro={modoOscuro}>
                {editandoPersona ? (
                    <>
                        <div className="grid grid-cols-2 gap-4">
                            {[
                                { label: 'Primer Nombre *', key: 'primer_Nombre' },
                                { label: 'Segundo Nombre', key: 'segundo_Nombre' },
                                { label: 'Primer Apellido *', key: 'primer_Apellido' },
                                { label: 'Segundo Apellido', key: 'segundo_Apellido' },
                                { label: 'DNI *', key: 'dni' },
                            ].map(({ label, key }) => (
                                <div key={key}>
                                    <label className={labelCls(modoOscuro)}>{label}</label>
                                    <input className={inputCls(modoOscuro)} value={formPersona[key] || ''}
                                        onChange={e => setFormPersona(f => ({ ...f, [key]: e.target.value }))} />
                                </div>
                            ))}
                            <div>
                                <label className={labelCls(modoOscuro)}>Tipo DNI *</label>
                                <select className={inputCls(modoOscuro)} value={formPersona.id_Tipo_DNI}
                                    onChange={e => setFormPersona(f => ({ ...f, id_Tipo_DNI: e.target.value }))}>
                                    <option value="">Seleccionar...</option>
                                    {filtrarCatalogo(2).map(c => <option key={c.id_Catalogo} value={c.id_Catalogo}>{c.nombre_Catalogo}</option>)}
                                </select>
                            </div>
                            <div>
                                <label className={labelCls(modoOscuro)}>Género *</label>
                                <select className={inputCls(modoOscuro)} value={formPersona.id_Genero}
                                    onChange={e => setFormPersona(f => ({ ...f, id_Genero: e.target.value }))}>
                                    <option value="">Seleccionar...</option>
                                    {filtrarCatalogo(1).map(c => <option key={c.id_Catalogo} value={c.id_Catalogo}>{c.nombre_Catalogo}</option>)}
                                </select>
                            </div>
                            <div>
                                <label className={labelCls(modoOscuro)}>Nacionalidad *</label>
                                <select className={inputCls(modoOscuro)} value={formPersona.id_Nacionalidad}
                                    onChange={e => setFormPersona(f => ({ ...f, id_Nacionalidad: e.target.value }))}>
                                    <option value="">Seleccionar...</option>
                                    {filtrarCatalogo(3).map(c => <option key={c.id_Catalogo} value={c.id_Catalogo}>{c.nombre_Catalogo}</option>)}
                                </select>
                            </div>
                        </div>
                        <BotonesEdicion onGuardar={guardarPersona} onCancelar={() => setEditandoPersona(false)} clave="persona" />
                    </>
                ) : (
                    <>
                        <div className="grid grid-cols-2 gap-3 text-sm">
                            <div><span className={labelCls(modoOscuro)}>Nombre completo</span><p className="font-medium">{nombreCompleto}</p></div>
                            <div><span className={labelCls(modoOscuro)}>DNI</span><p className="font-medium">{persona.dni}</p></div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Estado</span>
                                <span className={`inline-block mt-1 px-2 py-0.5 text-xs rounded-full border font-semibold ${persona.estado ? 'bg-green-100 text-green-800 border-green-200' : 'bg-red-100 text-red-800 border-red-200'}`}>
                                    {persona.estado ? 'Activo' : 'Inactivo'}
                                </span>
                            </div>
                        </div>
                        <div className="flex justify-end mt-4">
                            <button onClick={() => persona.estado && setEditandoPersona(true)} disabled={!persona.estado}
                                className={`flex items-center gap-1 px-3 py-1.5 rounded-lg text-sm transition ${persona.estado ? 'bg-cyan-600 text-white hover:bg-cyan-500' : 'bg-gray-300 text-gray-500 cursor-not-allowed'}`}
                                title={!persona.estado ? 'Activa la persona para poder editarla' : ''}>
                                <Edit size={14} /> Editar
                            </button>
                        </div>
                    </>
                )}
            </Seccion>

            {/* ── Correos ── */}
            <Seccion icono={<Mail size={16} className="text-cyan-500" />} titulo="Correos Electrónicos" modoOscuro={modoOscuro}>
                {correos.length === 0 && <p className={`text-sm mb-3 ${modoOscuro ? 'text-gray-500' : 'text-gray-400'}`}>No hay correos registrados.</p>}
                {correos.map(c => (
                    <div key={c.id_Correo} className={`mb-3 p-3 rounded-lg border ${modoOscuro ? 'bg-gray-700 border-gray-600' : 'bg-gray-50 border-gray-200'}`}>
                        {editandoCorreo === c.id_Correo ? (
                            <>
                                <div className="grid grid-cols-2 gap-3">
                                    <div className="col-span-2">
                                        <label className={labelCls(modoOscuro)}>Correo electrónico</label>
                                        <input type="email" className={inputCls(modoOscuro)} value={formCorreo.direccion_Correo || ''}
                                            onChange={e => setFormCorreo(f => ({ ...f, direccion_Correo: e.target.value }))} />
                                    </div>
                                    <div>
                                        <label className={labelCls(modoOscuro)}>Tipo</label>
                                        <select className={inputCls(modoOscuro)} value={formCorreo.id_Tipo_Correo || ''}
                                            onChange={e => setFormCorreo(f => ({ ...f, id_Tipo_Correo: e.target.value }))}>
                                            <option value="">Seleccionar...</option>
                                            {filtrarCatalogo(5).map(cat => <option key={cat.id_Catalogo} value={cat.id_Catalogo}>{cat.nombre_Catalogo}</option>)}
                                        </select>
                                    </div>
                                    <div>
                                        <label className={labelCls(modoOscuro)}>Estado</label>
                                        <select className={inputCls(modoOscuro)} value={formCorreo.id_Estado || 1}
                                            onChange={e => setFormCorreo(f => ({ ...f, id_Estado: parseInt(e.target.value), activo: parseInt(e.target.value) === 1 }))}>
                                            <option value={1}>Activo</option>
                                            <option value={2}>Inactivo</option>
                                        </select>
                                    </div>
                                </div>
                                <BotonesEdicion onGuardar={guardarCorreo} onCancelar={() => setEditandoCorreo(null)} clave="correo" />
                            </>
                        ) : (
                            <div className="flex justify-between items-center">
                                <div>
                                    <p className="text-sm font-medium">{c.direccion_Correo}</p>
                                    <p className={`text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>{c.tipo_Correo} · {c.activo ? 'Activo' : 'Inactivo'}</p>
                                </div>
                                <button onClick={() => { if (!persona.estado) return; setEditandoCorreo(c.id_Correo); setFormCorreo({ direccion_Correo: c.direccion_Correo, id_Tipo_Correo: c.id_Tipo_Correo, activo: c.activo, id_Estado: c.activo ? 1 : 2 }); }}
                                    className={`p-1 ${persona.estado ? 'text-cyan-500 hover:text-cyan-400' : 'text-gray-400 cursor-not-allowed'}`}
                                    title={!persona.estado ? 'Activa la persona para editar' : ''}><Edit size={16} /></button>
                            </div>
                        )}
                    </div>
                ))}
                {mostrarNuevoCorreo ? (
                    <div className={`mt-3 p-3 rounded-lg border ${modoOscuro ? 'bg-gray-700 border-gray-600' : 'bg-gray-50 border-gray-200'}`}>
                        <div className="grid grid-cols-2 gap-3">
                            <div className="col-span-2">
                                <label className={labelCls(modoOscuro)}>Correo electrónico</label>
                                <input type="email" className={inputCls(modoOscuro)} placeholder="usuario@correo.com"
                                    value={nuevoCorreo.direccion_Correo}
                                    onChange={e => setNuevoCorreo(f => ({ ...f, direccion_Correo: e.target.value }))} />
                            </div>
                            <div>
                                <label className={labelCls(modoOscuro)}>Tipo</label>
                                <select className={inputCls(modoOscuro)} value={nuevoCorreo.id_Tipo_Correo}
                                    onChange={e => setNuevoCorreo(f => ({ ...f, id_Tipo_Correo: e.target.value }))}>
                                    <option value="">Seleccionar...</option>
                                    {filtrarCatalogo(5).map(c => <option key={c.id_Catalogo} value={c.id_Catalogo}>{c.nombre_Catalogo}</option>)}
                                </select>
                            </div>
                        </div>
                        <BotonesEdicion onGuardar={agregarCorreo} onCancelar={() => { setMostrarNuevoCorreo(false); setNuevoCorreo({ direccion_Correo: '', id_Tipo_Correo: '' }); }} clave="nuevoCorreo" />
                    </div>
                ) : (
                    <button onClick={() => persona.estado && setMostrarNuevoCorreo(true)} disabled={!persona.estado}
                        className={`mt-2 flex items-center gap-1 text-sm transition ${persona.estado ? 'text-cyan-500 hover:text-cyan-400' : 'text-gray-400 cursor-not-allowed'}`}
                        title={!persona.estado ? 'Activa la persona para agregar' : ''}>
                        <Plus size={14} /> Agregar correo
                    </button>
                )}
            </Seccion>

            {/* ── Direcciones ── */}
            <Seccion icono={<MapPin size={16} className="text-cyan-500" />} titulo="Direcciones" modoOscuro={modoOscuro}>
                {direcciones.length === 0 && <p className={`text-sm mb-3 ${modoOscuro ? 'text-gray-500' : 'text-gray-400'}`}>No hay direcciones registradas.</p>}
                {direcciones.map(d => (
                    <div key={d.id_Direccion} className={`mb-3 p-3 rounded-lg border ${modoOscuro ? 'bg-gray-700 border-gray-600' : 'bg-gray-50 border-gray-200'}`}>
                        {editandoDireccion === d.id_Direccion ? (
                            <>
                                <div className="grid grid-cols-2 gap-3">
                                    <div className="col-span-2">
                                        <label className={labelCls(modoOscuro)}>Dirección completa</label>
                                        <input className={inputCls(modoOscuro)} value={formDireccion.direccion_Completa || ''}
                                            onChange={e => setFormDireccion(f => ({ ...f, direccion_Completa: e.target.value }))} />
                                    </div>
                                    <div>
                                        <label className={labelCls(modoOscuro)}>Tipo</label>
                                        <select className={inputCls(modoOscuro)} value={formDireccion.id_Tipo_Direccion || ''}
                                            onChange={e => setFormDireccion(f => ({ ...f, id_Tipo_Direccion: e.target.value }))}>
                                            <option value="">Seleccionar...</option>
                                            {filtrarCatalogo(7).map(c => <option key={c.id_Catalogo} value={c.id_Catalogo}>{c.nombre_Catalogo}</option>)}
                                        </select>
                                    </div>
                                    <div>
                                        <label className={labelCls(modoOscuro)}>Estado</label>
                                        <select className={inputCls(modoOscuro)} value={formDireccion.id_Estado || 1}
                                            onChange={e => setFormDireccion(f => ({ ...f, id_Estado: parseInt(e.target.value), activo: parseInt(e.target.value) === 1 }))}>
                                            <option value={1}>Activo</option>
                                            <option value={2}>Inactivo</option>
                                        </select>
                                    </div>
                                </div>
                                <BotonesEdicion onGuardar={guardarDireccion} onCancelar={() => setEditandoDireccion(null)} clave="direccion" />
                            </>
                        ) : (
                            <div className="flex justify-between items-center">
                                <div>
                                    <p className="text-sm font-medium">{d.direccion_Completa}</p>
                                    <p className={`text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>{d.tipo_Direccion} · {d.activo ? 'Activo' : 'Inactivo'}</p>
                                </div>
                                <button onClick={() => { if (!persona.estado) return; setEditandoDireccion(d.id_Direccion); setFormDireccion({ direccion_Completa: d.direccion_Completa, id_Tipo_Direccion: d.id_Tipo_Direccion, activo: d.activo, id_Estado: d.activo ? 1 : 2 }); }}
                                    className={`p-1 ${persona.estado ? 'text-cyan-500 hover:text-cyan-400' : 'text-gray-400 cursor-not-allowed'}`}
                                    title={!persona.estado ? 'Activa la persona para editar' : ''}><Edit size={16} /></button>
                            </div>
                        )}
                    </div>
                ))}
                {mostrarNuevaDireccion ? (
                    <div className={`mt-3 p-3 rounded-lg border ${modoOscuro ? 'bg-gray-700 border-gray-600' : 'bg-gray-50 border-gray-200'}`}>
                        <div className="grid grid-cols-2 gap-3">
                            <div className="col-span-2">
                                <label className={labelCls(modoOscuro)}>Dirección completa</label>
                                <input className={inputCls(modoOscuro)} placeholder="Ej: Residencial Las Brisas, casa #12"
                                    value={nuevaDireccion.direccion_Completa}
                                    onChange={e => setNuevaDireccion(f => ({ ...f, direccion_Completa: e.target.value }))} />
                            </div>
                            <div>
                                <label className={labelCls(modoOscuro)}>Tipo</label>
                                <select className={inputCls(modoOscuro)} value={nuevaDireccion.id_Tipo_Direccion}
                                    onChange={e => setNuevaDireccion(f => ({ ...f, id_Tipo_Direccion: e.target.value }))}>
                                    <option value="">Seleccionar...</option>
                                    {filtrarCatalogo(7).map(c => <option key={c.id_Catalogo} value={c.id_Catalogo}>{c.nombre_Catalogo}</option>)}
                                </select>
                            </div>
                        </div>
                        <BotonesEdicion onGuardar={agregarDireccion} onCancelar={() => { setMostrarNuevaDireccion(false); setNuevaDireccion({ direccion_Completa: '', id_Tipo_Direccion: '' }); }} clave="nuevaDireccion" />
                    </div>
                ) : (
                    <button onClick={() => persona.estado && setMostrarNuevaDireccion(true)} disabled={!persona.estado}
                        className={`mt-2 flex items-center gap-1 text-sm transition ${persona.estado ? 'text-cyan-500 hover:text-cyan-400' : 'text-gray-400 cursor-not-allowed'}`}
                        title={!persona.estado ? 'Activa la persona para agregar' : ''}>
                        <Plus size={14} /> Agregar dirección
                    </button>
                )}
            </Seccion>

            {/* ── Teléfonos ── */}
            <Seccion icono={<Phone size={16} className="text-cyan-500" />} titulo="Teléfonos" modoOscuro={modoOscuro}>
                {telefonos.length === 0 && <p className={`text-sm mb-3 ${modoOscuro ? 'text-gray-500' : 'text-gray-400'}`}>No hay teléfonos registrados.</p>}
                {telefonos.map(t => (
                    <div key={t.id_Telefono} className={`mb-3 p-3 rounded-lg border ${modoOscuro ? 'bg-gray-700 border-gray-600' : 'bg-gray-50 border-gray-200'}`}>
                        {editandoTelefono === t.id_Telefono ? (
                            <>
                                <div className="grid grid-cols-2 gap-3">
                                    <div>
                                        <label className={labelCls(modoOscuro)}>Número</label>
                                        <input className={inputCls(modoOscuro)} value={formTelefono.numero_Telefono || ''}
                                            onChange={e => setFormTelefono(f => ({ ...f, numero_Telefono: e.target.value }))} />
                                    </div>
                                    <div>
                                        <label className={labelCls(modoOscuro)}>Tipo</label>
                                        <select className={inputCls(modoOscuro)} value={formTelefono.id_Tipo_Telefono || ''}
                                            onChange={e => setFormTelefono(f => ({ ...f, id_Tipo_Telefono: e.target.value }))}>
                                            <option value="">Seleccionar...</option>
                                            {filtrarCatalogo(6).map(c => <option key={c.id_Catalogo} value={c.id_Catalogo}>{c.nombre_Catalogo}</option>)}
                                        </select>
                                    </div>
                                    <div>
                                        <label className={labelCls(modoOscuro)}>Estado</label>
                                        <select className={inputCls(modoOscuro)} value={formTelefono.id_Estado || 1}
                                            onChange={e => setFormTelefono(f => ({ ...f, id_Estado: parseInt(e.target.value), estado: parseInt(e.target.value) === 1 }))}>
                                            <option value={1}>Activo</option>
                                            <option value={2}>Inactivo</option>
                                        </select>
                                    </div>
                                </div>
                                <BotonesEdicion onGuardar={guardarTelefono} onCancelar={() => setEditandoTelefono(null)} clave="telefono" />
                            </>
                        ) : (
                            <div className="flex justify-between items-center">
                                <div>
                                    <p className="text-sm font-medium">{t.numero_Telefono}</p>
                                    <p className={`text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>{t.tipo_Telefono} · {t.estado ? 'Activo' : 'Inactivo'}</p>
                                </div>
                                <button onClick={() => { if (!persona.estado) return; setEditandoTelefono(t.id_Telefono); setFormTelefono({ numero_Telefono: t.numero_Telefono, id_Tipo_Telefono: t.id_Tipo_Telefono, estado: t.estado, id_Estado: t.estado ? 1 : 2 }); }}
                                    className={`p-1 ${persona.estado ? 'text-cyan-500 hover:text-cyan-400' : 'text-gray-400 cursor-not-allowed'}`}
                                    title={!persona.estado ? 'Activa la persona para editar' : ''}><Edit size={16} /></button>
                            </div>
                        )}
                    </div>
                ))}
                {mostrarNuevoTelefono ? (
                    <div className={`mt-3 p-3 rounded-lg border ${modoOscuro ? 'bg-gray-700 border-gray-600' : 'bg-gray-50 border-gray-200'}`}>
                        <div className="grid grid-cols-2 gap-3">
                            <div>
                                <label className={labelCls(modoOscuro)}>Número</label>
                                <input className={inputCls(modoOscuro)} placeholder="Ej: 8888-8888"
                                    value={nuevoTelefono.numero_Telefono}
                                    onChange={e => setNuevoTelefono(f => ({ ...f, numero_Telefono: e.target.value }))} />
                            </div>
                            <div>
                                <label className={labelCls(modoOscuro)}>Tipo</label>
                                <select className={inputCls(modoOscuro)} value={nuevoTelefono.id_Tipo_Telefono}
                                    onChange={e => setNuevoTelefono(f => ({ ...f, id_Tipo_Telefono: e.target.value }))}>
                                    <option value="">Seleccionar...</option>
                                    {filtrarCatalogo(6).map(c => <option key={c.id_Catalogo} value={c.id_Catalogo}>{c.nombre_Catalogo}</option>)}
                                </select>
                            </div>
                        </div>
                        <BotonesEdicion onGuardar={agregarTelefono} onCancelar={() => { setMostrarNuevoTelefono(false); setNuevoTelefono({ numero_Telefono: '', id_Tipo_Telefono: '' }); }} clave="nuevoTelefono" />
                    </div>
                ) : (
                    <button onClick={() => persona.estado && setMostrarNuevoTelefono(true)} disabled={!persona.estado}
                        className={`mt-2 flex items-center gap-1 text-sm transition ${persona.estado ? 'text-cyan-500 hover:text-cyan-400' : 'text-gray-400 cursor-not-allowed'}`}
                        title={!persona.estado ? 'Activa la persona para agregar' : ''}>
                        <Plus size={14} /> Agregar teléfono
                    </button>
                )}
            </Seccion>

        </div>
    );
};

export default VistaResumenPersona;