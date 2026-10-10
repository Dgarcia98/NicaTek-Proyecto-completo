import { useState, useRef, useEffect } from 'react';
import { createPortal } from 'react-dom';
import { LuUser, LuMoon, LuSun, LuLogOut, LuPencilLine, LuBuilding2, LuX, LuSave, LuMenu } from 'react-icons/lu';
import { useNavigate } from 'react-router-dom';
import Swal from 'sweetalert2';
import api from '../api/axios.Config';
import { esAdmin } from '../utils/permisos';

const Modal = ({ titulo, icono, onClose, children, modoOscuro }) => {
    return createPortal(
        <div className="fixed inset-0 z-[9999] flex items-center justify-center bg-black/50 backdrop-blur-sm">
            <div className={`w-full max-w-md rounded-2xl shadow-xl p-6 mx-4 ${modoOscuro ? 'bg-gray-800 text-gray-100' : 'bg-white text-gray-800'}`}>
                <div className="flex items-center justify-between mb-5">
                    <h2 className="text-lg font-bold flex items-center gap-2">{icono} {titulo}</h2>
                    <button onClick={onClose} className={`p-1.5 rounded-lg transition ${modoOscuro ? 'hover:bg-gray-700' : 'hover:bg-gray-100'}`}>
                        <LuX size={18} />
                    </button>
                </div>
                {children}
            </div>
        </div>,
        document.body
    );
};

const inputCls = (modoOscuro) =>
    `w-full px-3 py-2 rounded-lg border text-sm outline-none focus:ring-2 focus:ring-cyan-400 transition ${modoOscuro ? 'bg-gray-700 border-gray-600 text-gray-100' : 'bg-gray-50 border-gray-300 text-gray-800'}`;
const labelCls = (modoOscuro) =>
    `block text-xs font-medium mb-1 ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`;

const Topbar = ({ toggleSidebar, toggleModoOscuro, modoOscuro }) => {
    const navigate = useNavigate();
    const dropdownRef = useRef(null);
    const soyAdmin = esAdmin();

    const [openDropdown, setOpenDropdown] = useState(false);
    const [modalPerfil, setModalPerfil] = useState(false);
    const [modalEmpresa, setModalEmpresa] = useState(false);

    const [perfil, setPerfil] = useState({ nombre_Usuario: '', contrasena: '', confirmar: '' });
    const [guardandoPerfil, setGuardandoPerfil] = useState(false);
    // Guardamos datos completos del usuario para el PUT
    const [usuarioActual, setUsuarioActual] = useState(null);

    const [empresa, setEmpresa] = useState({
        id_Configuracion: 0, nombre_Empresa: '', ruC_No: '',
        direccion: '', telefono: '', correo_Empresa: '',
        moneda_Simbolo: 'C$', ruta_Logo: ''
    });
    const [guardandoEmpresa, setGuardandoEmpresa] = useState(false);
    const [cargandoEmpresa, setCargandoEmpresa] = useState(false);

    const idUsuario = parseInt(sessionStorage.getItem('idUsuario') || '1');
    const nombreCompleto = sessionStorage.getItem('nombreUsuario') || 'Usuario';
    const primerNombre = nombreCompleto.split(' ')[0];

    useEffect(() => {
        const handleClickOutside = (e) => {
            if (dropdownRef.current && !dropdownRef.current.contains(e.target)) setOpenDropdown(false);
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    const abrirPerfil = async () => {
        setOpenDropdown(false);
        setPerfil({ nombre_Usuario: sessionStorage.getItem('nombreUsuario') || '', contrasena: '', confirmar: '' });
        setModalPerfil(true);
        // ── Cargar datos completos del usuario para el PUT ──
        try {
            const res = await api.get('/Usuario/ListarUsuarios');
            const lista = Array.isArray(res.data) ? res.data : (res.data?.datos || []);
            const u = lista.find(x => x.id_Usuario === idUsuario);
            if (u) setUsuarioActual(u);
        } catch (err) { console.error(err); }
    };

    const guardarPerfil = async () => {
        if (!perfil.nombre_Usuario.trim()) {
            Swal.fire({ icon: 'warning', title: 'El nombre de usuario es requerido', confirmButtonColor: '#0891b2' }); return;
        }
        if (perfil.contrasena && perfil.contrasena !== perfil.confirmar) {
            Swal.fire({ icon: 'warning', title: 'Las contraseñas no coinciden', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardandoPerfil(true);
            await api.put(`/Usuario/EditarUsuario/${idUsuario}`, {
                Id_Usuario:          idUsuario,
                Nombre_Usuario:      perfil.nombre_Usuario.trim(),
                // ── Si no escribe contraseña, mandar la actual del servidor ──
                Contrasena:          perfil.contrasena.trim() || usuarioActual?.contrasena || '',
                Id_Persona:          usuarioActual?.id_Persona || 0,
                Id_Rol:              parseInt(sessionStorage.getItem('idRol') || '1'),
                Id_Estado:           1,
                Id_Creador:          usuarioActual?.id_Creador || idUsuario,
                Id_Modificador:      idUsuario,
                Token:               usuarioActual?.token || '',
                Nombre_Rol:          sessionStorage.getItem('nombreRol') || '',
                Estado_Nombre:       'Activo',
                Nombre_Persona:      usuarioActual?.nombre_Persona || nombreCompleto,
                Fecha_Ultimo_Acceso: usuarioActual?.fecha_Ultimo_Acceso || null,
                Fecha_Creacion:      usuarioActual?.fecha_Creacion || new Date().toISOString(),
                Fecha_Modificacion:  new Date().toISOString(),
            });
            sessionStorage.setItem('nombreUsuario', perfil.nombre_Usuario.trim());
            setModalPerfil(false);
            Swal.fire({ icon: 'success', title: 'Perfil actualizado', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al guardar', confirmButtonColor: '#0891b2' });
        } finally { setGuardandoPerfil(false); }
    };

    const abrirEmpresa = async () => {
        setOpenDropdown(false);
        setModalEmpresa(true);
        try {
            setCargandoEmpresa(true);
            const res = await api.get('/Configuracion/Obtener');
            if (res.data) setEmpresa(res.data);
        } catch (err) { console.error(err);
        } finally { setCargandoEmpresa(false); }
    };

    const guardarEmpresa = async () => {
        if (!empresa.nombre_Empresa?.trim()) {
            Swal.fire({ icon: 'warning', title: 'El nombre de la empresa es requerido', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardandoEmpresa(true);
            const payload = {
                Id_Configuracion: empresa.id_Configuracion,
                Nombre_Empresa:   empresa.nombre_Empresa,
                RUC_No:           empresa.ruC_No || '',
                Direccion:        empresa.direccion || '',
                Telefono:         empresa.telefono || null,
                Correo_Empresa:   empresa.correo_Empresa || null,
                Ruta_Logo:        (empresa.ruta_Logo && empresa.ruta_Logo.trim().length > 1) ? empresa.ruta_Logo.trim() : null,
                Moneda_Simbolo:   empresa.moneda_Simbolo || 'C$',
                Id_Modificador:   idUsuario,
                Fecha_Modificacion: new Date().toISOString(),
            };
            if (empresa.id_Configuracion > 0) {
                await api.put(`/Configuracion/Actualizar/${empresa.id_Configuracion}`, payload);
            } else {
                await api.post('/Configuracion/Crear', payload);
            }
            setModalEmpresa(false);
            Swal.fire({ icon: 'success', title: 'Configuración guardada', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al guardar', confirmButtonColor: '#0891b2' });
        } finally { setGuardandoEmpresa(false); }
    };

    return (
        <>
            {/* ── Header ───────────────────────────────────────────────────── */}
            <header className={`sticky top-0 z-40 px-4 py-3 shadow-sm rounded-md flex items-center justify-between transition-colors duration-300 ${
                modoOscuro ? 'bg-gray-800 text-white' : 'bg-white text-gray-700'}`}>

                <button
                    onClick={toggleSidebar}
                    title="Expandir / colapsar menú"
                    className={`p-2 rounded-lg transition-colors duration-200 ${
                        modoOscuro ? 'hover:bg-gray-700 text-gray-300' : 'hover:bg-gray-100 text-gray-500'}`}
                >
                    <LuMenu size={20} />
                </button>

                <div className="flex items-center gap-4" ref={dropdownRef}>
                    <button onClick={toggleModoOscuro}
                        className={`transition ${modoOscuro ? 'text-yellow-300 hover:text-yellow-400' : 'text-gray-500 hover:text-cyan-600'}`}>
                        {modoOscuro ? <LuSun size={20} /> : <LuMoon size={20} />}
                    </button>

                    <button onClick={() => setOpenDropdown(p => !p)} className="flex items-center gap-2">
                        <LuUser size={20} />
                        <span className="text-sm font-medium flex items-center gap-1">
                            {primerNombre}
                            <span className={`text-xs transform transition-transform duration-300 ${openDropdown ? 'rotate-180' : ''}`}>▼</span>
                        </span>
                    </button>

                    {openDropdown && (
                        <div className={`absolute top-14 right-6 shadow-lg rounded-xl w-52 py-2 z-50 border ${
                            modoOscuro ? 'bg-gray-800 border-gray-700' : 'bg-white border-gray-200'}`}>
                            <div className={`px-4 py-2 mb-1 border-b ${modoOscuro ? 'border-gray-700' : 'border-gray-100'}`}>
                                <p className={`text-xs font-semibold truncate ${modoOscuro ? 'text-gray-200' : 'text-gray-700'}`}>{nombreCompleto}</p>
                                <p className={`text-xs ${modoOscuro ? 'text-gray-500' : 'text-gray-400'}`}>{sessionStorage.getItem('nombreRol') || '—'}</p>
                            </div>
                            <button onClick={abrirPerfil}
                                className={`w-full text-left px-4 py-2 flex items-center gap-2 text-sm transition ${
                                    modoOscuro ? 'text-gray-300 hover:bg-gray-700' : 'text-gray-700 hover:bg-gray-50'}`}>
                                <LuPencilLine size={15} /> Editar perfil
                            </button>
                            <button onClick={abrirEmpresa}
                                className={`w-full text-left px-4 py-2 flex items-center gap-2 text-sm transition ${
                                    modoOscuro ? 'text-gray-300 hover:bg-gray-700' : 'text-gray-700 hover:bg-gray-50'}`}>
                                <LuBuilding2 size={15} /> Configurar empresa
                            </button>
                            <div className={`mx-3 my-1 border-t ${modoOscuro ? 'border-gray-700' : 'border-gray-100'}`} />
                            <button onClick={() => navigate('/logout')}
                                className={`w-full text-left px-4 py-2 flex items-center gap-2 text-sm text-red-400 transition ${
                                    modoOscuro ? 'hover:bg-red-900/20' : 'hover:bg-red-50'}`}>
                                <LuLogOut size={15} /> Cerrar sesión
                            </button>
                        </div>
                    )}
                </div>
            </header>

            {/* ── Modal Perfil ──────────────────────────────────────────────── */}
            {modalPerfil && (
                <Modal titulo="Editar mi perfil" icono={<LuPencilLine size={18} className="text-cyan-500" />}
                    onClose={() => setModalPerfil(false)} modoOscuro={modoOscuro}>
                    <div className="space-y-4">
                        <div>
                            <label className={labelCls(modoOscuro)}>Nombre de usuario *</label>
                            <input className={inputCls(modoOscuro)} value={perfil.nombre_Usuario}
                                onChange={e => setPerfil(p => ({ ...p, nombre_Usuario: e.target.value }))} />
                        </div>
                        <div>
                            <label className={labelCls(modoOscuro)}>
                                Nueva contraseña{' '}
                                <span className={`font-normal ${modoOscuro ? 'text-gray-500' : 'text-gray-400'}`}>(dejar vacío para no cambiar)</span>
                            </label>
                            <input type="password" className={inputCls(modoOscuro)} placeholder="••••••••"
                                value={perfil.contrasena}
                                onChange={e => setPerfil(p => ({ ...p, contrasena: e.target.value }))} />
                        </div>
                        {perfil.contrasena && (
                            <div>
                                <label className={labelCls(modoOscuro)}>Confirmar contraseña</label>
                                <input type="password"
                                    className={`${inputCls(modoOscuro)} ${perfil.confirmar && perfil.contrasena !== perfil.confirmar ? 'border-red-400' : ''}`}
                                    placeholder="••••••••" value={perfil.confirmar}
                                    onChange={e => setPerfil(p => ({ ...p, confirmar: e.target.value }))} />
                                {perfil.confirmar && perfil.contrasena !== perfil.confirmar && (
                                    <p className="text-xs text-red-400 mt-1">Las contraseñas no coinciden</p>
                                )}
                            </div>
                        )}
                    </div>
                    <div className="flex justify-end gap-2 mt-6">
                        <button onClick={() => setModalPerfil(false)}
                            className={`px-4 py-2 rounded-lg text-sm border transition ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                            Cancelar
                        </button>
                        <button onClick={guardarPerfil} disabled={guardandoPerfil}
                            className="flex items-center gap-2 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition disabled:opacity-60">
                            {guardandoPerfil
                                ? <div className="w-3.5 h-3.5 border-2 border-white border-t-transparent rounded-full animate-spin" />
                                : <LuSave size={14} />}
                            Guardar
                        </button>
                    </div>
                </Modal>
            )}

            {/* ── Modal Empresa ─────────────────────────────────────────────── */}
            {modalEmpresa && (
                <Modal titulo="Configuración de empresa" icono={<LuBuilding2 size={18} className="text-cyan-500" />}
                    onClose={() => setModalEmpresa(false)} modoOscuro={modoOscuro}>
                    {cargandoEmpresa ? (
                        <div className="flex justify-center py-8">
                            <div className="w-7 h-7 border-2 border-cyan-500 border-t-transparent rounded-full animate-spin" />
                        </div>
                    ) : (
                        <div className="space-y-3">
                            <div className="grid grid-cols-2 gap-3">
                                <div className="col-span-2">
                                    <label className={labelCls(modoOscuro)}>Nombre de la empresa *</label>
                                    <input className={inputCls(modoOscuro)} value={empresa.nombre_Empresa || ''}
                                        disabled={!soyAdmin}
                                        onChange={e => setEmpresa(em => ({ ...em, nombre_Empresa: e.target.value }))} />
                                </div>
                                <div>
                                    <label className={labelCls(modoOscuro)}>RUC / No. Registro</label>
                                    <input className={inputCls(modoOscuro)} value={empresa.ruC_No || ''}
                                        disabled={!soyAdmin}
                                        onChange={e => setEmpresa(em => ({ ...em, ruC_No: e.target.value }))} />
                                </div>
                                <div>
                                    <label className={labelCls(modoOscuro)}>Símbolo de moneda</label>
                                    <input className={inputCls(modoOscuro)} value={empresa.moneda_Simbolo || ''}
                                        disabled={!soyAdmin}
                                        onChange={e => setEmpresa(em => ({ ...em, moneda_Simbolo: e.target.value }))} />
                                </div>
                                <div className="col-span-2">
                                    <label className={labelCls(modoOscuro)}>Dirección</label>
                                    <input className={inputCls(modoOscuro)} value={empresa.direccion || ''}
                                        disabled={!soyAdmin}
                                        onChange={e => setEmpresa(em => ({ ...em, direccion: e.target.value }))} />
                                </div>
                                <div>
                                    <label className={labelCls(modoOscuro)}>Teléfono</label>
                                    <input className={inputCls(modoOscuro)} value={empresa.telefono || ''}
                                        disabled={!soyAdmin}
                                        onChange={e => setEmpresa(em => ({ ...em, telefono: e.target.value }))} />
                                </div>
                                <div>
                                    <label className={labelCls(modoOscuro)}>Correo</label>
                                    <input className={inputCls(modoOscuro)} value={empresa.correo_Empresa || ''}
                                        disabled={!soyAdmin}
                                        onChange={e => setEmpresa(em => ({ ...em, correo_Empresa: e.target.value }))} />
                                </div>
                                <div className="col-span-2">
                                    <label className={labelCls(modoOscuro)}>Logo de la empresa</label>
                                    {empresa.ruta_Logo && empresa.ruta_Logo.trim().length > 1 && (
                                        <div className="mb-2 flex items-center gap-3">
                                            <img src={empresa.ruta_Logo} alt="Logo" className="h-10 w-auto object-contain rounded" />
                                            <span className={`text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>Logo actual</span>
                                        </div>
                                    )}
                                    {soyAdmin && (
                                        <>
                                            <input className={inputCls(modoOscuro)}
                                                placeholder="/logo.png  o  https://mi-dominio.com/logo.png"
                                                value={empresa.ruta_Logo || ''}
                                                onChange={e => setEmpresa(em => ({ ...em, ruta_Logo: e.target.value }))} />
                                            <p className={`text-xs mt-1 ${modoOscuro ? 'text-gray-500' : 'text-gray-400'}`}>
                                                Coloca tu imagen en <code>public/</code> y escribe <code>/nombre-archivo.png</code>
                                            </p>
                                        </>
                                    )}
                                </div>
                            </div>
                            {!soyAdmin && (
                                <p className={`text-xs text-center pt-1 ${modoOscuro ? 'text-gray-500' : 'text-gray-400'}`}>
                                    Solo los administradores pueden editar esta información.
                                </p>
                            )}
                        </div>
                    )}
                    <div className="flex justify-end gap-2 mt-6">
                        <button onClick={() => setModalEmpresa(false)}
                            className={`px-4 py-2 rounded-lg text-sm border transition ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                            {soyAdmin ? 'Cancelar' : 'Cerrar'}
                        </button>
                        {soyAdmin && (
                            <button onClick={guardarEmpresa} disabled={guardandoEmpresa || cargandoEmpresa}
                                className="flex items-center gap-2 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition disabled:opacity-60">
                                {guardandoEmpresa
                                    ? <div className="w-3.5 h-3.5 border-2 border-white border-t-transparent rounded-full animate-spin" />
                                    : <LuSave size={14} />}
                                Guardar
                            </button>
                        )}
                    </div>
                </Modal>
            )}
        </>
    );
};

export default Topbar;