import { useEffect, useState } from 'react';
import { useOutletContext } from 'react-router-dom';
import { User, MapPin, Phone, Mail, Plus, Trash2, Save, LoaderCircle, CheckCircle } from 'lucide-react';
import Swal from 'sweetalert2';
import api from '../../api/axios.Config';

const inputCls = (modoOscuro) =>
    `w-full px-3 py-2 rounded-lg border text-sm outline-none transition focus:ring-2 focus:ring-cyan-400 ${
        modoOscuro ? 'bg-gray-700 border-gray-600 text-gray-100' : 'bg-gray-50 border-gray-300 text-gray-800'
    }`;

const labelCls = (modoOscuro) =>
    `block text-xs font-medium mb-1 ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`;

const Seccion = ({ icono, titulo, modoOscuro, bloqueada, children }) => (
    <div className={`rounded-2xl shadow-md border p-6 mb-5 transition-opacity ${
        bloqueada ? 'opacity-40 pointer-events-none' : ''
    } ${modoOscuro ? 'bg-gray-800 border-gray-700' : 'bg-white border-gray-200'}`}>
        <h2 className={`text-base font-semibold flex items-center gap-2 mb-4 pb-3 border-b ${
            modoOscuro ? 'text-gray-100 border-gray-700' : 'text-gray-700 border-gray-100'
        }`}>
            {icono} {titulo}
            {bloqueada && <span className="ml-2 text-xs text-gray-400">(Guarda los datos personales primero)</span>}
        </h2>
        {children}
    </div>
);

const ItemLista = ({ texto, subtexto, onEliminar, modoOscuro }) => (
    <div className={`flex justify-between items-center px-3 py-2 rounded-lg border mb-2 ${
        modoOscuro ? 'bg-gray-700 border-gray-600' : 'bg-gray-50 border-gray-200'
    }`}>
        <div>
            <p className="text-sm font-medium">{texto}</p>
            {subtexto && <p className={`text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>{subtexto}</p>}
        </div>
        <button onClick={onEliminar} className="text-red-400 hover:text-red-600 p-1 transition">
            <Trash2 size={15} />
        </button>
    </div>
);

const CrearPersona = () => {
    const { modoOscuro } = useOutletContext();

    const [catalogos, setCatalogos] = useState([]);
    const [cargandoCatalogos, setCargandoCatalogos] = useState(true);
    const [idPersonaCreada, setIdPersonaCreada] = useState(null);
    const [guardandoPersona, setGuardandoPersona] = useState(false);

    // Forms
    const [formPersona, setFormPersona] = useState({
        primer_Nombre: '', segundo_Nombre: '', primer_Apellido: '',
        segundo_Apellido: '', dni: '', id_Tipo_DNI: '', id_Genero: '', id_Nacionalidad: '',
    });

    // Listas acumuladas
    const [correos, setCorreos] = useState([]);
    const [direcciones, setDirecciones] = useState([]);
    const [telefonos, setTelefonos] = useState([]);

    // Forms de nuevos items
    const [nuevoCorreo, setNuevoCorreo] = useState({ direccion_Correo: '', id_Tipo_Correo: '' });
    const [nuevaDireccion, setNuevaDireccion] = useState({ direccion_Completa: '', id_Tipo_Direccion: '' });
    const [nuevoTelefono, setNuevoTelefono] = useState({ numero_Telefono: '', id_Tipo_Telefono: '' });

    const [guardandoExtra, setGuardandoExtra] = useState('');

    // ── Cargar catálogos ──────────────────────────────────────────────────────
    useEffect(() => {
        const cargarCatalogos = async () => {
            try {
                const res = await api.get('/Catalogo/ListarCatalogo');
                setCatalogos(Array.isArray(res.data) ? res.data : []);
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudieron cargar los catálogos.', confirmButtonColor: '#0891b2' });
            } finally {
                setCargandoCatalogos(false);
            }
        };
        cargarCatalogos();
    }, []);

    const filtrarCatalogo = (idTipo) =>
        catalogos.filter(c => c.id_Tipo_Catalogo === idTipo);

    // ── Guardar persona ───────────────────────────────────────────────────────
    const guardarPersona = async () => {
        const { primer_Nombre, primer_Apellido, dni, id_Tipo_DNI, id_Genero, id_Nacionalidad } = formPersona;
        if (!primer_Nombre || !primer_Apellido || !dni || !id_Tipo_DNI || !id_Genero || !id_Nacionalidad) {
            Swal.fire({ icon: 'warning', title: 'Campos requeridos', text: 'Completa todos los campos obligatorios.', confirmButtonColor: '#0891b2' });
            return;
        }

        try {
            setGuardandoPersona(true);
            const res = await api.post('/DatosPersonales/NuevoDatosPersonales', {
                Id_Persona: 0,
                Primer_Nombre: formPersona.primer_Nombre,
                Segundo_Nombre: formPersona.segundo_Nombre || '',
                Primer_Apellido: formPersona.primer_Apellido,
                Segundo_Apellido: formPersona.segundo_Apellido || '',
                Id_Tipo_DNI: parseInt(formPersona.id_Tipo_DNI),
                Dni: formPersona.dni,
                Id_Genero: parseInt(formPersona.id_Genero),
                Id_Nacionalidad: parseInt(formPersona.id_Nacionalidad),
                Id_Creador: 1,
                Id_Modificador: 1,
                Estado: true,
            });

            // El backend no devuelve el ID, buscamos la persona por DNI
            const resBuscar = await api.get(`/DatosPersonales/ListarPorNombre/${encodeURIComponent(formPersona.dni)}`);
            const lista = Array.isArray(resBuscar.data) ? resBuscar.data : (resBuscar.data?.datos || []);
            const personaCreada = lista.find(p => p.dni === formPersona.dni || p.Dni === formPersona.dni);
            const idCreado = parseInt(personaCreada?.id_Persona || personaCreada?.idPersona || 0);

            if (!idCreado) {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo obtener el ID de la persona creada.', confirmButtonColor: '#0891b2' });
                return;
            }
            setIdPersonaCreada(idCreado);

            Swal.fire({
                icon: 'success',
                title: '¡Persona creada!',
                text: 'Ahora puedes agregar correos, direcciones y teléfonos.',
                confirmButtonColor: '#0891b2',
                timer: 2000,
                timerProgressBar: true,
            });
        } catch {
            Swal.fire({ icon: 'error', title: 'Error al crear persona', confirmButtonColor: '#0891b2' });
        } finally {
            setGuardandoPersona(false);
        }
    };

    // ── Agregar correo ────────────────────────────────────────────────────────
    const agregarCorreo = async () => {
        if (!nuevoCorreo.direccion_Correo || !nuevoCorreo.id_Tipo_Correo) {
            Swal.fire({ icon: 'warning', title: 'Completa el correo', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardandoExtra('correo');
            await api.post('/Correo/NuevoCorreo', {
                Id_Correo: 0,
                Id_Persona: idPersonaCreada,
                Nombre_Completo: '',
                Direccion_Correo: nuevoCorreo.direccion_Correo,
                Id_Tipo_Correo: parseInt(nuevoCorreo.id_Tipo_Correo),
                Tipo_Correo: '',
                Activo: true,
                EstadoActivo: '',
                Fecha_Creacion: new Date().toISOString(),
                Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: 1,
                Id_Modificador: 0,
            });
            const tipoNombre = filtrarCatalogo(5).find(c => c.id_Catalogo === parseInt(nuevoCorreo.id_Tipo_Correo))?.nombre_Catalogo || '';
            setCorreos(prev => [...prev, { ...nuevoCorreo, tipoNombre }]);
            setNuevoCorreo({ direccion_Correo: '', id_Tipo_Correo: '' });
        } catch {
            Swal.fire({ icon: 'error', title: 'Error al guardar correo', confirmButtonColor: '#0891b2' });
        } finally {
            setGuardandoExtra('');
        }
    };

    // ── Agregar dirección ─────────────────────────────────────────────────────
    const agregarDireccion = async () => {
        if (!nuevaDireccion.direccion_Completa || !nuevaDireccion.id_Tipo_Direccion) {
            Swal.fire({ icon: 'warning', title: 'Completa la dirección', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardandoExtra('direccion');
            await api.post('/Direccion/NuevaDireccion', {
                Id_Direccion: 0,
                Id_Persona: idPersonaCreada,
                Nombre_Completo: '',
                Tipo_Direccion: '',
                EstadoActivo: '',
                Direccion_Completa: nuevaDireccion.direccion_Completa,
                Id_Tipo_Direccion: parseInt(nuevaDireccion.id_Tipo_Direccion),
                Activo: true,
                Fecha_Creacion: new Date().toISOString(),
                Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: 1,
                Id_Modificador: 0,
            });
            const tipoNombre = filtrarCatalogo(7).find(c => c.id_Catalogo === parseInt(nuevaDireccion.id_Tipo_Direccion))?.nombre_Catalogo || '';
            setDirecciones(prev => [...prev, { ...nuevaDireccion, tipoNombre }]);
            setNuevaDireccion({ direccion_Completa: '', id_Tipo_Direccion: '' });
        } catch (err) {
            console.error('Error dirección:', err.response?.data || err.message);
            Swal.fire({ icon: 'error', title: 'Error al guardar dirección', text: JSON.stringify(err.response?.data || err.message), confirmButtonColor: '#0891b2' });
        } finally {
            setGuardandoExtra('');
        }
    };

    // ── Agregar teléfono ──────────────────────────────────────────────────────
    const agregarTelefono = async () => {
        if (!nuevoTelefono.numero_Telefono || !nuevoTelefono.id_Tipo_Telefono) {
            Swal.fire({ icon: 'warning', title: 'Completa el teléfono', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardandoExtra('telefono');
            await api.post('/Telefono/NuevoTelefono', {
                Id_Telefono: 0,
                Id_Persona: idPersonaCreada,
                Numero_Telefono: nuevoTelefono.numero_Telefono,
                Id_Tipo_Telefono: parseInt(nuevoTelefono.id_Tipo_Telefono),
                Tipo_Telefono: '',
                Estado: true,
                Nombre_Estado: '',
                Fecha_Creacion: new Date().toISOString(),
                Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: 1,
                Id_Modificador: 0,
            });
            const tipoNombre = filtrarCatalogo(6).find(c => c.id_Catalogo === parseInt(nuevoTelefono.id_Tipo_Telefono))?.nombre_Catalogo || '';
            setTelefonos(prev => [...prev, { ...nuevoTelefono, tipoNombre }]);
            setNuevoTelefono({ numero_Telefono: '', id_Tipo_Telefono: '' });
        } catch (err) {
            console.error('Error teléfono:', err.response?.data || err.message);
            Swal.fire({ icon: 'error', title: 'Error al guardar teléfono', text: JSON.stringify(err.response?.data || err.message), confirmButtonColor: '#0891b2' });
        } finally {
            setGuardandoExtra('');
        }
    };

    const finalizar = () => {
        window.location.href = '/usuarios/personas';
    };

    if (cargandoCatalogos) return (
        <div className="flex justify-center items-center py-32">
            <LoaderCircle size={36} className="animate-spin text-cyan-500" />
        </div>
    );

    return (
        <div className={`p-6 max-w-3xl mx-auto ${modoOscuro ? 'text-gray-100' : 'text-gray-800'}`}>

            {/* Header */}
            <div className="flex items-center justify-between mb-6">
                <div className="flex items-center gap-3">
                    <User size={26} className="text-cyan-500" />
                    <h1 className="text-xl font-bold">Nueva Persona</h1>
                </div>
                <div className="flex items-center gap-3">
                    {idPersonaCreada && (
                        <span className="flex items-center gap-1 text-green-600 text-sm font-medium">
                            <CheckCircle size={16} /> ID #{String(idPersonaCreada)} creado
                        </span>
                    )}
                    <button
                        onClick={() => window.location.href = '/usuarios/personas'}
                        className={`px-3 py-2 rounded-lg border text-sm transition ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}
                    >
                        ← Volver
                    </button>
                </div>
            </div>

            {/* ── Datos Personales ── */}
            <Seccion icono={<User size={16} className="text-cyan-500" />} titulo="Datos Personales" modoOscuro={modoOscuro} bloqueada={false}>
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
                            <input
                                disabled={!!idPersonaCreada}
                                className={`${inputCls(modoOscuro)} ${idPersonaCreada ? 'opacity-60 cursor-not-allowed' : ''}`}
                                value={formPersona[key]}
                                onChange={e => setFormPersona(f => ({ ...f, [key]: e.target.value }))}
                            />
                        </div>
                    ))}

                    <div>
                        <label className={labelCls(modoOscuro)}>Tipo DNI *</label>
                        <select disabled={!!idPersonaCreada} className={`${inputCls(modoOscuro)} ${idPersonaCreada ? 'opacity-60 cursor-not-allowed' : ''}`}
                            value={formPersona.id_Tipo_DNI} onChange={e => setFormPersona(f => ({ ...f, id_Tipo_DNI: e.target.value }))}>
                            <option value="">Seleccionar...</option>
                            {filtrarCatalogo(2).map(c => <option key={c.id_Catalogo} value={c.id_Catalogo}>{c.nombre_Catalogo}</option>)}
                        </select>
                    </div>

                    <div>
                        <label className={labelCls(modoOscuro)}>Género *</label>
                        <select disabled={!!idPersonaCreada} className={`${inputCls(modoOscuro)} ${idPersonaCreada ? 'opacity-60 cursor-not-allowed' : ''}`}
                            value={formPersona.id_Genero} onChange={e => setFormPersona(f => ({ ...f, id_Genero: e.target.value }))}>
                            <option value="">Seleccionar...</option>
                            {filtrarCatalogo(1).map(c => <option key={c.id_Catalogo} value={c.id_Catalogo}>{c.nombre_Catalogo}</option>)}
                        </select>
                    </div>

                    <div>
                        <label className={labelCls(modoOscuro)}>Nacionalidad *</label>
                        <select disabled={!!idPersonaCreada} className={`${inputCls(modoOscuro)} ${idPersonaCreada ? 'opacity-60 cursor-not-allowed' : ''}`}
                            value={formPersona.id_Nacionalidad} onChange={e => setFormPersona(f => ({ ...f, id_Nacionalidad: e.target.value }))}>
                            <option value="">Seleccionar...</option>
                            {filtrarCatalogo(3).map(c => <option key={c.id_Catalogo} value={c.id_Catalogo}>{c.nombre_Catalogo}</option>)}
                        </select>
                    </div>
                </div>

                {!idPersonaCreada && (
                    <div className="flex justify-end mt-4">
                        <button onClick={guardarPersona} disabled={guardandoPersona}
                            className="flex items-center gap-2 px-5 py-2 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition disabled:opacity-60">
                            {guardandoPersona ? <LoaderCircle size={15} className="animate-spin" /> : <Save size={15} />}
                            Guardar Persona
                        </button>
                    </div>
                )}
            </Seccion>

            {/* ── Correos ── */}
            <Seccion icono={<Mail size={16} className="text-cyan-500" />} titulo="Correos Electrónicos" modoOscuro={modoOscuro} bloqueada={!idPersonaCreada}>
                {correos.map((c, i) => (
                    <ItemLista key={i} texto={c.direccion_Correo} subtexto={c.tipoNombre} modoOscuro={modoOscuro}
                        onEliminar={() => setCorreos(prev => prev.filter((_, idx) => idx !== i))} />
                ))}
                <div className="grid grid-cols-2 gap-3 mt-3">
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
                    <div className="flex items-end">
                        <button onClick={agregarCorreo} disabled={guardandoExtra === 'correo'}
                            className="w-full flex items-center justify-center gap-1 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition disabled:opacity-60">
                            {guardandoExtra === 'correo' ? <LoaderCircle size={14} className="animate-spin" /> : <Plus size={14} />}
                            Agregar
                        </button>
                    </div>
                </div>
            </Seccion>

            {/* ── Direcciones ── */}
            <Seccion icono={<MapPin size={16} className="text-cyan-500" />} titulo="Direcciones" modoOscuro={modoOscuro} bloqueada={!idPersonaCreada}>
                {direcciones.map((d, i) => (
                    <ItemLista key={i} texto={d.direccion_Completa} subtexto={d.tipoNombre} modoOscuro={modoOscuro}
                        onEliminar={() => setDirecciones(prev => prev.filter((_, idx) => idx !== i))} />
                ))}
                <div className="grid grid-cols-2 gap-3 mt-3">
                    <div className="col-span-2">
                        <label className={labelCls(modoOscuro)}>Dirección completa</label>
                        <input className={inputCls(modoOscuro)} placeholder="Residencial Las Brisas, casa #12"
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
                    <div className="flex items-end">
                        <button onClick={agregarDireccion} disabled={guardandoExtra === 'direccion'}
                            className="w-full flex items-center justify-center gap-1 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition disabled:opacity-60">
                            {guardandoExtra === 'direccion' ? <LoaderCircle size={14} className="animate-spin" /> : <Plus size={14} />}
                            Agregar
                        </button>
                    </div>
                </div>
            </Seccion>

            {/* ── Teléfonos ── */}
            <Seccion icono={<Phone size={16} className="text-cyan-500" />} titulo="Teléfonos" modoOscuro={modoOscuro} bloqueada={!idPersonaCreada}>
                {telefonos.map((t, i) => (
                    <ItemLista key={i} texto={t.numero_Telefono} subtexto={t.tipoNombre} modoOscuro={modoOscuro}
                        onEliminar={() => setTelefonos(prev => prev.filter((_, idx) => idx !== i))} />
                ))}
                <div className="grid grid-cols-2 gap-3 mt-3">
                    <div>
                        <label className={labelCls(modoOscuro)}>Número</label>
                        <input type="tel" className={inputCls(modoOscuro)} placeholder="8756-9012"
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
                    <div className="col-span-2 flex justify-end">
                        <button onClick={agregarTelefono} disabled={guardandoExtra === 'telefono'}
                            className="flex items-center gap-1 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition disabled:opacity-60">
                            {guardandoExtra === 'telefono' ? <LoaderCircle size={14} className="animate-spin" /> : <Plus size={14} />}
                            Agregar
                        </button>
                    </div>
                </div>
            </Seccion>

            {/* ── Finalizar ── */}
            {idPersonaCreada && (
                <div className="flex justify-end mt-2">
                    <button onClick={finalizar}
                        className="flex items-center gap-2 px-6 py-2.5 bg-green-600 text-white font-semibold rounded-lg shadow hover:bg-green-500 transition">
                        <CheckCircle size={18} /> Finalizar
                    </button>
                </div>
            )}
        </div>
    );
};

export default CrearPersona;