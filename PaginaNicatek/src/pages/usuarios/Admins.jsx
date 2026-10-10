import { useEffect, useState } from 'react';
import { useOutletContext, useNavigate } from 'react-router-dom';
import { ShieldCheck, Users, Search, X, LoaderCircle, Eye, ToggleRight, ToggleLeft, Plus, KeyRound, Save, Tag, Edit, Trash2 } from 'lucide-react';
import Swal from 'sweetalert2';
import TablaConTarjetas from '../../components/TablaConTarjetas';
import api from '../../api/axios.Config';
import { esAdmin } from '../../utils/permisos';

const TODOS_LOS_PERMISOS = [
    { id: 1, codigo: 'INV_READ',   nombre: 'Ver Inventario',    descripcion: 'Permite ver la lista de productos' },
    { id: 2, codigo: 'INV_CREATE', nombre: 'Crear Producto',     descripcion: 'Permite agregar nuevos productos' },
    { id: 3, codigo: 'INV_UPDATE', nombre: 'Editar Producto',    descripcion: 'Permite modificar productos existentes' },
    { id: 4, codigo: 'INV_DELETE', nombre: 'Eliminar Producto',  descripcion: 'Permite borrar/desactivar productos' },
    { id: 5, codigo: 'POS_SALE',   nombre: 'Realizar Venta',     descripcion: 'Permite acceder a la pantalla de ventas' },
    { id: 6, codigo: 'RPT_VIEW',   nombre: 'Ver Reportes',       descripcion: 'Acceso a reportes financieros' },
    { id: 7, codigo: 'USR_MANAGE', nombre: 'Gestionar Usuarios', descripcion: 'Crear o bloquear usuarios' },
];

const MODAL_VACIO = { id_Rol: 0, nombre_Rol: '', descripcion: '' };

const Admins = () => {
    const { modoOscuro } = useOutletContext();
    const navigate = useNavigate();

    const [tabActiva, setTabActiva] = useState('usuarios');
    const soyAdmin = esAdmin();
    const idUsuarioActual = parseInt(sessionStorage.getItem('idUsuario') || '0');

    // ── Usuarios ──────────────────────────────────────────
    const [usuarios, setUsuarios] = useState([]);
    const [cargandoU, setCargandoU] = useState(true);
    const [filtroU, setFiltroU] = useState('');

    // ── Empleados ─────────────────────────────────────────
    const [empleados, setEmpleados] = useState([]);
    const [cargandoE, setCargandoE] = useState(true);
    const [filtroE, setFiltroE] = useState('');

    // ── Permisos ──────────────────────────────────────────
    const [rolSeleccionado, setRolSeleccionado] = useState(2);
    const [rolesDisponibles, setRolesDisponibles] = useState([]);
    const [permisosActivos, setPermisosActivos] = useState(new Set());
    const [permisosOriginales, setPermisosOriginales] = useState(new Set());
    const [cargandoP, setCargandoP] = useState(false);
    const [guardandoP, setGuardandoP] = useState(false);

    // ── Roles ─────────────────────────────────────────────
    const [roles, setRoles] = useState([]);
    const [cargandoR, setCargandoR] = useState(false);
    const [modalRol, setModalRol] = useState(null);
    const [guardandoR, setGuardandoR] = useState(false);

    // ── Fetch ─────────────────────────────────────────────
    const fetchUsuarios = async (termino = '') => {
        try {
            setCargandoU(true);
            const endpoint = termino.trim()
                ? `/Usuario/ListarPorFiltro/${encodeURIComponent(termino.trim())}`
                : '/Usuario/ListarUsuarios';
            const res = await api.get(endpoint);
            const data = Array.isArray(res.data) ? res.data : (res.data?.datos || []);
            setUsuarios([...data].sort((a, b) => (b.id_Usuario ?? 0) - (a.id_Usuario ?? 0)));
        } catch (err) { console.error(err); setUsuarios([]);
        } finally { setCargandoU(false); }
    };

    const fetchEmpleados = async (termino = '') => {
        try {
            setCargandoE(true);
            const endpoint = termino.trim()
                ? `/Empleado/FiltrarEmpleado/${encodeURIComponent(termino.trim())}`
                : '/Empleado/ListarEmpleado';
            const res = await api.get(endpoint);
            const data = Array.isArray(res.data) ? res.data : (res.data?.datos || []);
            setEmpleados([...data].sort((a, b) => (b.id_Empleado ?? 0) - (a.id_Empleado ?? 0)));
        } catch (err) { console.error(err); setEmpleados([]);
        } finally { setCargandoE(false); }
    };

    const fetchPermisos = async (idRol) => {
        try {
            setCargandoP(true);
            const res = await api.get(`/RolPermiso/ListarPorRol/${idRol}`);
            const data = Array.isArray(res.data) ? res.data : [];
            const activos = new Set(data.map(p => p.id_Permiso));
            setPermisosActivos(new Set(activos));
            setPermisosOriginales(new Set(activos));
        } catch (err) {
            console.error(err);
            setPermisosActivos(new Set());
            setPermisosOriginales(new Set());
        } finally { setCargandoP(false); }
    };

    const fetchRoles = async () => {
        try {
            setCargandoR(true);
            const res = await api.get('/Rol/ListarRoles');
            const data = Array.isArray(res.data) ? res.data : [];
            setRoles(data);
            setRolesDisponibles(data);
        } catch (err) { console.error(err); setRoles([]);
        } finally { setCargandoR(false); }
    };

    useEffect(() => { fetchUsuarios(); fetchEmpleados(); }, []);
    useEffect(() => { const d = setTimeout(() => fetchUsuarios(filtroU), 350); return () => clearTimeout(d); }, [filtroU]);
    useEffect(() => { const d = setTimeout(() => fetchEmpleados(filtroE), 350); return () => clearTimeout(d); }, [filtroE]);
    useEffect(() => { if (tabActiva === 'permisos') { fetchPermisos(rolSeleccionado); fetchRoles(); } }, [tabActiva, rolSeleccionado]);
    useEffect(() => { if (tabActiva === 'roles') fetchRoles(); }, [tabActiva]);

    // ── Permisos ──────────────────────────────────────────
    const togglePermiso = (idPermiso) => {
        setPermisosActivos(prev => {
            const nuevo = new Set(prev);
            nuevo.has(idPermiso) ? nuevo.delete(idPermiso) : nuevo.add(idPermiso);
            return nuevo;
        });
    };

    const hayCambios = () => {
        if (permisosActivos.size !== permisosOriginales.size) return true;
        for (const id of permisosActivos) if (!permisosOriginales.has(id)) return true;
        return false;
    };

    const guardarPermisos = async () => {
        try {
            setGuardandoP(true);
            const agregar = [...permisosActivos].filter(id => !permisosOriginales.has(id));
            const quitar  = [...permisosOriginales].filter(id => !permisosActivos.has(id));
            await Promise.all([
                ...agregar.map(id => api.post(`/RolPermiso/AsignarPermiso/${rolSeleccionado}/${id}`)),
                ...quitar.map(id  => api.delete(`/RolPermiso/RemoverPermiso/${rolSeleccionado}/${id}`)),
            ]);
            setPermisosOriginales(new Set(permisosActivos));
            Swal.fire({ icon: 'success', title: 'Permisos actualizados', timer: 1400, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al guardar permisos', confirmButtonColor: '#0891b2' });
        } finally { setGuardandoP(false); }
    };

    // ── Roles CRUD ────────────────────────────────────────
    const abrirModalNuevo = () => setModalRol({ ...MODAL_VACIO });
    const abrirModalEditar = (rol) => setModalRol({ id_Rol: rol.id_Rol, nombre_Rol: rol.nombre_Rol, descripcion: rol.descripcion || '' });
    const cerrarModal = () => setModalRol(null);

    const guardarRol = async () => {
        if (!modalRol.nombre_Rol.trim()) {
            Swal.fire({ icon: 'warning', title: 'El nombre del rol es requerido', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardandoR(true);
            const esNuevo = modalRol.id_Rol === 0;
            const payload = {
                Id_Rol: modalRol.id_Rol,
                Nombre_Rol: modalRol.nombre_Rol.trim(),
                Descripcion: modalRol.descripcion.trim() || null,
                Id_Creador: idUsuarioActual,
                Id_Modificador: idUsuarioActual,
                Activo: true,
                Fecha_Creacion: new Date().toISOString(),
                Fecha_Modificacion: new Date().toISOString(),
            };
            if (esNuevo) {
                await api.post('/Rol/NuevoRol', payload);
            } else {
                await api.put(`/Rol/EditarRol/${modalRol.id_Rol}`, payload);
            }
            cerrarModal();
            await fetchRoles();
            Swal.fire({ icon: 'success', title: esNuevo ? 'Rol creado' : 'Rol actualizado', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al guardar rol', confirmButtonColor: '#0891b2' });
        } finally { setGuardandoR(false); }
    };

    const eliminarRol = async (rol) => {
        const result = await Swal.fire({
            icon: 'warning', title: `¿Desactivar rol "${rol.nombre_Rol}"?`,
            text: 'Los usuarios con este rol no podrán iniciar sesión.',
            showCancelButton: true, confirmButtonColor: '#d33', cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, desactivar', cancelButtonText: 'Cancelar',
        });
        if (!result.isConfirmed) return;
        try {
            await api.delete(`/Rol/EliminarRol/${rol.id_Rol}/1`);
            await fetchRoles();
            Swal.fire({ icon: 'success', title: 'Rol desactivado', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al desactivar rol', confirmButtonColor: '#0891b2' });
        }
    };

    const reactivarRol = async (rol) => {
        const result = await Swal.fire({
            icon: 'question', title: `¿Reactivar rol "${rol.nombre_Rol}"?`,
            text: 'Los usuarios con este rol podrán volver a iniciar sesión.',
            showCancelButton: true, confirmButtonColor: '#0891b2', cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, reactivar', cancelButtonText: 'Cancelar',
        });
        if (!result.isConfirmed) return;
        try {
            await api.put(`/Rol/EditarRol/${rol.id_Rol}`, {
                Id_Rol: rol.id_Rol,
                Nombre_Rol: rol.nombre_Rol,
                Descripcion: rol.descripcion || null,
                Activo: true,
                Id_Creador: rol.id_Creador || 1,
                Id_Modificador: idUsuarioActual,
                Fecha_Creacion: rol.fecha_Creacion,
                Fecha_Modificacion: new Date().toISOString(),
            });
            await fetchRoles();
            Swal.fire({ icon: 'success', title: 'Rol reactivado', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al reactivar rol', confirmButtonColor: '#0891b2' });
        }
    };

    // ── Toggles ───────────────────────────────────────────
    const toggleUsuario = async (u) => {
        if (u.id_Usuario === idUsuarioActual) {
            Swal.fire({ icon: 'warning', title: 'Acción no permitida', text: 'No puedes desactivar tu propio usuario.', confirmButtonColor: '#0891b2' });
            return;
        }
        const esActivo = u.id_Estado === 1;
        const nuevoEstado = esActivo ? 2 : 1;
        const result = await Swal.fire({
            icon: 'question', title: '¿Cambiar estado?', text: `El usuario pasará a: ${esActivo ? 'Inactivo' : 'Activo'}`,
            showCancelButton: true, confirmButtonColor: '#0891b2', cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, cambiar', cancelButtonText: 'Cancelar',
        });
        if (!result.isConfirmed) return;
        try {
            await api.put(`/Usuario/EditarUsuario/${u.id_Usuario}`, {
                Id_Usuario: u.id_Usuario, Nombre_Usuario: u.nombre_Usuario,
                Contrasena: u.contrasena || '', Id_Persona: u.id_Persona,
                Id_Rol: u.id_Rol, Id_Estado: nuevoEstado, Id_Creador: u.id_Creador || 1,
                Token: u.token || '', Nombre_Rol: u.nombre_Rol || '',
                Estado_Nombre: nuevoEstado === 1 ? 'Activo' : 'Inactivo',
                Nombre_Persona: u.nombre_Persona || '', Fecha_Ultimo_Acceso: u.fecha_Ultimo_Acceso || null,
                Fecha_Creacion: u.fecha_Creacion, Fecha_Modificacion: new Date().toISOString(),
                Id_Modificador: idUsuarioActual,
            });
            Swal.fire({ icon: 'success', title: `Usuario ${nuevoEstado === 1 ? 'activado' : 'desactivado'}`, timer: 1200, timerProgressBar: true, showConfirmButton: false });
            fetchUsuarios(filtroU);
        } catch { Swal.fire({ icon: 'error', title: 'Error al cambiar estado', confirmButtonColor: '#0891b2' }); }
    };

    const toggleEmpleado = async (e) => {
        const esActivo = e.id_Estado === 1;
        const nuevoEstado = esActivo ? 2 : 1;
        const result = await Swal.fire({
            icon: 'question', title: '¿Cambiar estado?', text: `El empleado pasará a: ${esActivo ? 'Inactivo' : 'Activo'}`,
            showCancelButton: true, confirmButtonColor: '#0891b2', cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, cambiar', cancelButtonText: 'Cancelar',
        });
        if (!result.isConfirmed) return;
        try {
            await api.put(`/Empleado/EditarEmpleado/${e.id_Empleado}`, {
                Id_Empleado: e.id_Empleado, Id_Persona: e.id_Persona,
                Nombre_Completo: e.nombre_Completo || '', Cedula: e.cedula || '',
                Fecha_Contratacion: e.fecha_Contratacion, Fecha_Despido: e.fecha_Despido || null,
                Id_Cargo: e.id_Cargo, Cargo: e.cargo || '', Id_Estado: nuevoEstado,
                Estado: nuevoEstado === 1 ? 'Activo' : 'Inactivo',
                Fecha_Creacion: e.fecha_Creacion, Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: e.id_Creador || 1, Id_Modificador: idUsuarioActual,
            });
            Swal.fire({ icon: 'success', title: `Empleado ${nuevoEstado === 1 ? 'activado' : 'desactivado'}`, timer: 1200, timerProgressBar: true, showConfirmButton: false });
            fetchEmpleados(filtroE);
        } catch { Swal.fire({ icon: 'error', title: 'Error al cambiar estado', confirmButtonColor: '#0891b2' }); }
    };

    // ── Filas ─────────────────────────────────────────────
    const filasU = usuarios.map(u => {
        const esActivo = u.id_Estado === 1;
        const esMiUsuario = u.id_Usuario === idUsuarioActual;
        return [
            u.id_Usuario, u.nombre_Usuario, u.nombre_Persona || '—', u.nombre_Rol || '—',
            <span key={`eu-${u.id_Usuario}`} className={`px-2 py-0.5 text-xs rounded-full border font-semibold ${esActivo ? 'bg-green-100 text-green-800 border-green-200' : 'bg-red-100 text-red-800 border-red-200'}`}>
                {esActivo ? 'Activo' : 'Inactivo'}
            </span>,
            u.fecha_Ultimo_Acceso ? new Date(u.fecha_Ultimo_Acceso).toLocaleDateString() : '—',
            <div key={`au-${u.id_Usuario}`} className="flex items-center gap-2">
                {soyAdmin ? (
                    <>
                        <button onClick={() => navigate(`/usuarios/admins/resumen/${u.id_Usuario}`)}
                            className="p-1.5 rounded-lg text-cyan-500 hover:bg-cyan-50 transition" title="Ver resumen">
                            <Eye size={16} />
                        </button>
                        <button onClick={() => toggleUsuario(u)}
                            disabled={esMiUsuario}
                            title={esMiUsuario ? 'No puedes desactivarte a ti mismo' : 'Cambiar estado'}
                            className={`p-1.5 rounded-lg transition ${
                                esMiUsuario
                                    ? 'text-gray-300 cursor-not-allowed opacity-50'
                                    : esActivo ? 'text-green-500 hover:bg-green-50' : 'text-red-400 hover:bg-red-50'
                            }`}>
                            {esActivo ? <ToggleRight size={18} /> : <ToggleLeft size={18} />}
                        </button>
                    </>
                ) : esMiUsuario ? (
                    // ── Empleado solo puede ver su propio resumen ──
                    <button onClick={() => navigate(`/usuarios/admins/resumen/${u.id_Usuario}`)}
                        className="p-1.5 rounded-lg text-cyan-500 hover:bg-cyan-50 transition" title="Ver mi perfil">
                        <Eye size={16} />
                    </button>
                ) : (
                    <span className={`text-xs italic ${modoOscuro ? 'text-gray-600' : 'text-gray-400'}`}>Sin acceso</span>
                )}
            </div>
        ];
    });

    const filasE = empleados.map(e => {
        const esActivo = e.id_Estado === 1;
        const idPersonaActual = parseInt(sessionStorage.getItem('idPersona') || '0');
        const esMiEmpleado = e.id_Persona === idPersonaActual;
        return [
            e.id_Empleado, e.nombre_Completo || '—', e.cedula || '—', e.cargo || '—',
            <span key={`ee-${e.id_Empleado}`} className={`px-2 py-0.5 text-xs rounded-full border font-semibold ${esActivo ? 'bg-green-100 text-green-800 border-green-200' : 'bg-red-100 text-red-800 border-red-200'}`}>
                {esActivo ? 'Activo' : 'Inactivo'}
            </span>,
            e.fecha_Contratacion ? new Date(e.fecha_Contratacion).toLocaleDateString() : '—',
            <div key={`ae-${e.id_Empleado}`} className="flex items-center gap-2">
                {soyAdmin ? (
                    <>
                        <button onClick={() => navigate(`/usuarios/admins/empleado/resumen/${e.id_Empleado}`)}
                            className="p-1.5 rounded-lg text-cyan-500 hover:bg-cyan-50 transition" title="Ver resumen">
                            <Eye size={16} />
                        </button>
                        <button onClick={() => toggleEmpleado(e)}
                            className={`p-1.5 rounded-lg transition ${esActivo ? 'text-green-500 hover:bg-green-50' : 'text-red-400 hover:bg-red-50'}`}
                            title="Cambiar estado">
                            {esActivo ? <ToggleRight size={18} /> : <ToggleLeft size={18} />}
                        </button>
                    </>
                ) : esMiEmpleado ? (
                    // ── Empleado solo puede ver su propio resumen ──
                    <button onClick={() => navigate(`/usuarios/admins/empleado/resumen/${e.id_Empleado}`)}
                        className="p-1.5 rounded-lg text-cyan-500 hover:bg-cyan-50 transition" title="Ver mi perfil">
                        <Eye size={16} />
                    </button>
                ) : (
                    <span className={`text-xs italic ${modoOscuro ? 'text-gray-600' : 'text-gray-400'}`}>Sin acceso</span>
                )}
            </div>
        ];
    });

    const filasR = roles.map(r => {
        const rolActivo = r.activo === true || r.activo === 1 || r.activo === 'true';
        return [
            r.id_Rol,
            r.nombre_Rol,
            r.descripcion || '—',
            <span key={`er-${r.id_Rol}`} className={`px-2 py-0.5 text-xs rounded-full border font-semibold ${rolActivo ? 'bg-green-100 text-green-800 border-green-200' : 'bg-red-100 text-red-800 border-red-200'}`}>
                {rolActivo ? 'Activo' : 'Inactivo'}
            </span>,
            <div key={`ar-${r.id_Rol}`} className="flex items-center gap-2">
                <button onClick={() => abrirModalEditar(r)}
                    className="p-1.5 rounded-lg text-cyan-500 hover:bg-cyan-50 transition" title="Editar">
                    <Edit size={16} />
                </button>
                {rolActivo ? (
                    <button onClick={() => eliminarRol(r)} title="Desactivar"
                        className="p-1.5 rounded-lg transition text-red-400 hover:bg-red-50 hover:text-red-600">
                        <Trash2 size={16} />
                    </button>
                ) : (
                    <button onClick={() => reactivarRol(r)} title="Reactivar"
                        className="p-1.5 rounded-lg transition text-green-500 hover:bg-green-50 hover:text-green-700">
                        <ToggleLeft size={18} />
                    </button>
                )}
            </div>
        ];
    });

    const tabCls = (tab) =>
        `px-4 py-2 text-sm font-semibold border-b-2 transition ${tabActiva === tab
            ? 'border-cyan-500 text-cyan-500'
            : `border-transparent ${modoOscuro ? 'text-gray-400 hover:text-gray-200' : 'text-gray-500 hover:text-gray-700'}`}`;

    const inputCls = `w-full px-3 py-2 rounded-lg border text-sm outline-none focus:ring-2 focus:ring-cyan-400 ${modoOscuro ? 'bg-gray-700 border-gray-600 text-gray-100' : 'bg-gray-50 border-gray-300 text-gray-800'}`;
    const labelCls = `block text-xs font-medium mb-1 ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`;

    return (
        <div className={`p-6 transition-colors duration-300 ${modoOscuro ? 'bg-gray-800 text-white' : 'bg-gray-100 text-gray-800'}`}>

            {/* Header */}
            <div className="flex items-center justify-between mb-5">
                <div className="flex items-center gap-2">
                    <ShieldCheck size={22} className="text-cyan-500" />
                    <h1 className="text-2xl font-bold">Administración</h1>
                </div>
                {soyAdmin && tabActiva === 'usuarios' && (
                    <button onClick={() => navigate('/usuarios/admins/nuevo-usuario')}
                        className="flex items-center gap-2 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm font-semibold hover:bg-cyan-500 transition">
                        <Plus size={16} /> Nuevo Usuario
                    </button>
                )}
                {soyAdmin && tabActiva === 'empleados' && (
                    <button onClick={() => navigate('/usuarios/admins/nuevo-empleado')}
                        className="flex items-center gap-2 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm font-semibold hover:bg-cyan-500 transition">
                        <Plus size={16} /> Nuevo Empleado
                    </button>
                )}
                {soyAdmin && tabActiva === 'roles' && (
                    <button onClick={abrirModalNuevo}
                        className="flex items-center gap-2 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm font-semibold hover:bg-cyan-500 transition">
                        <Plus size={16} /> Nuevo Rol
                    </button>
                )}
            </div>

            {/* Tabs */}
            <div className={`flex gap-1 border-b mb-5 ${modoOscuro ? 'border-gray-700' : 'border-gray-200'}`}>
                <button className={tabCls('usuarios')} onClick={() => setTabActiva('usuarios')}>
                    <span className="flex items-center gap-1"><ShieldCheck size={14} /> Usuarios</span>
                </button>
                <button className={tabCls('empleados')} onClick={() => setTabActiva('empleados')}>
                    <span className="flex items-center gap-1"><Users size={14} /> Empleados</span>
                </button>
                {soyAdmin && (
                    <>
                        <button className={tabCls('roles')} onClick={() => setTabActiva('roles')}>
                            <span className="flex items-center gap-1"><Tag size={14} /> Roles</span>
                        </button>
                        <button className={tabCls('permisos')} onClick={() => setTabActiva('permisos')}>
                            <span className="flex items-center gap-1"><KeyRound size={14} /> Permisos por Rol</span>
                        </button>
                    </>
                )}
            </div>

            {/* TAB USUARIOS */}
            {tabActiva === 'usuarios' && (
                <>
                    <div className={`flex items-center gap-2 px-3 py-2 rounded-lg border mb-4 w-full max-w-sm ${modoOscuro ? 'bg-gray-700 border-gray-600' : 'bg-white border-gray-300'}`}>
                        <Search size={16} className="text-gray-400" />
                        <input type="text" placeholder="Buscar usuario..." value={filtroU} onChange={e => setFiltroU(e.target.value)}
                            className={`flex-1 text-sm bg-transparent outline-none ${modoOscuro ? 'text-gray-100 placeholder-gray-500' : 'text-gray-800 placeholder-gray-400'}`} />
                        {filtroU && <button onClick={() => setFiltroU('')}><X size={14} className="text-gray-400 hover:text-gray-600" /></button>}
                    </div>
                    {cargandoU ? <div className="flex justify-center py-16"><LoaderCircle size={32} className="animate-spin text-cyan-500" /></div>
                        : <TablaConTarjetas encabezados={['ID', 'Usuario', 'Persona', 'Rol', 'Estado', 'Último Acceso', 'Acciones']} filas={filasU} modoOscuro={modoOscuro} />}
                </>
            )}

            {/* TAB EMPLEADOS */}
            {tabActiva === 'empleados' && (
                <>
                    <div className={`flex items-center gap-2 px-3 py-2 rounded-lg border mb-4 w-full max-w-sm ${modoOscuro ? 'bg-gray-700 border-gray-600' : 'bg-white border-gray-300'}`}>
                        <Search size={16} className="text-gray-400" />
                        <input type="text" placeholder="Buscar empleado..." value={filtroE} onChange={e => setFiltroE(e.target.value)}
                            className={`flex-1 text-sm bg-transparent outline-none ${modoOscuro ? 'text-gray-100 placeholder-gray-500' : 'text-gray-800 placeholder-gray-400'}`} />
                        {filtroE && <button onClick={() => setFiltroE('')}><X size={14} className="text-gray-400 hover:text-gray-600" /></button>}
                    </div>
                    {cargandoE ? <div className="flex justify-center py-16"><LoaderCircle size={32} className="animate-spin text-cyan-500" /></div>
                        : <TablaConTarjetas encabezados={['ID', 'Nombre', 'Cédula', 'Cargo', 'Estado', 'Contratación', 'Acciones']} filas={filasE} modoOscuro={modoOscuro} />}
                </>
            )}

            {/* TAB ROLES */}
            {tabActiva === 'roles' && (
                <>
                    {cargandoR ? <div className="flex justify-center py-16"><LoaderCircle size={32} className="animate-spin text-cyan-500" /></div>
                        : <TablaConTarjetas encabezados={['ID', 'Nombre', 'Descripción', 'Estado', 'Acciones']} filas={filasR} modoOscuro={modoOscuro} />}
                </>
            )}

            {/* TAB PERMISOS */}
            {tabActiva === 'permisos' && (
                <div className="max-w-2xl">
                    <div className="flex items-center gap-3 mb-6">
                        <span className={`text-sm font-medium ${modoOscuro ? 'text-gray-300' : 'text-gray-600'}`}>Rol:</span>
                        <div className="flex gap-2 flex-wrap">
                            {rolesDisponibles.map(r => (
                                <button key={r.id_Rol} onClick={() => setRolSeleccionado(r.id_Rol)}
                                    className={`px-4 py-1.5 rounded-full text-sm font-semibold border transition ${
                                        rolSeleccionado === r.id_Rol
                                            ? 'bg-cyan-600 text-white border-cyan-600'
                                            : modoOscuro ? 'border-gray-600 text-gray-300 hover:border-cyan-500' : 'border-gray-300 text-gray-600 hover:border-cyan-500'
                                    }`}>
                                    {r.nombre_Rol}
                                </button>
                            ))}
                        </div>
                    </div>
                    {cargandoP ? <div className="flex justify-center py-16"><LoaderCircle size={32} className="animate-spin text-cyan-500" /></div> : (
                        <>
                            <div className="space-y-3 mb-6">
                                {TODOS_LOS_PERMISOS.map(p => {
                                    const activo = permisosActivos.has(p.id);
                                    return (
                                        <div key={p.id} onClick={() => togglePermiso(p.id)}
                                            className={`flex items-center justify-between p-4 rounded-xl border cursor-pointer transition select-none ${
                                                activo ? modoOscuro ? 'bg-cyan-900/30 border-cyan-600' : 'bg-cyan-50 border-cyan-300'
                                                : modoOscuro ? 'bg-gray-700 border-gray-600 hover:border-gray-500' : 'bg-white border-gray-200 hover:border-gray-300'
                                            }`}>
                                            <div>
                                                <p className={`text-sm font-semibold ${activo ? 'text-cyan-500' : modoOscuro ? 'text-gray-200' : 'text-gray-700'}`}>{p.nombre}</p>
                                                <p className={`text-xs mt-0.5 ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>{p.descripcion}</p>
                                            </div>
                                            <div className={`w-5 h-5 rounded border-2 flex items-center justify-center flex-shrink-0 transition ${activo ? 'bg-cyan-500 border-cyan-500' : modoOscuro ? 'border-gray-500' : 'border-gray-300'}`}>
                                                {activo && <svg className="w-3 h-3 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={3}><path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" /></svg>}
                                            </div>
                                        </div>
                                    );
                                })}
                            </div>
                            <div className="flex items-center gap-3">
                                <button onClick={guardarPermisos} disabled={!hayCambios() || guardandoP}
                                    className={`flex items-center gap-2 px-5 py-2 rounded-lg text-sm font-semibold transition ${hayCambios() && !guardandoP ? 'bg-cyan-600 text-white hover:bg-cyan-500' : 'bg-gray-300 text-gray-500 cursor-not-allowed'}`}>
                                    {guardandoP ? <LoaderCircle size={16} className="animate-spin" /> : <Save size={16} />}
                                    Guardar cambios
                                </button>
                                {hayCambios() && <span className={`text-xs ${modoOscuro ? 'text-yellow-400' : 'text-yellow-600'}`}>⚠ Tienes cambios sin guardar</span>}
                            </div>
                        </>
                    )}
                </div>
            )}

            {/* MODAL ROL */}
            {modalRol && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm">
                    <div className={`w-full max-w-md rounded-2xl shadow-xl p-6 ${modoOscuro ? 'bg-gray-800 text-gray-100' : 'bg-white text-gray-800'}`}>
                        <div className="flex items-center justify-between mb-5">
                            <h2 className="text-lg font-bold flex items-center gap-2">
                                <Tag size={18} className="text-cyan-500" />
                                {modalRol.id_Rol === 0 ? 'Nuevo Rol' : 'Editar Rol'}
                            </h2>
                            <button onClick={cerrarModal} className={`p-1.5 rounded-lg hover:bg-gray-100 transition ${modoOscuro ? 'hover:bg-gray-700' : ''}`}>
                                <X size={18} />
                            </button>
                        </div>
                        <div className="space-y-4">
                            <div>
                                <label className={labelCls}>Nombre del Rol *</label>
                                <input className={inputCls} value={modalRol.nombre_Rol}
                                    onChange={e => setModalRol(m => ({ ...m, nombre_Rol: e.target.value }))}
                                    placeholder="ej: Supervisor" />
                            </div>
                            <div>
                                <label className={labelCls}>Descripción</label>
                                <textarea className={`${inputCls} resize-none`} rows={3} value={modalRol.descripcion}
                                    onChange={e => setModalRol(m => ({ ...m, descripcion: e.target.value }))}
                                    placeholder="Descripción del rol..." />
                            </div>
                        </div>
                        <div className="flex justify-end gap-2 mt-6">
                            <button onClick={cerrarModal}
                                className={`px-4 py-2 rounded-lg text-sm border transition ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                                Cancelar
                            </button>
                            <button onClick={guardarRol} disabled={guardandoR}
                                className="flex items-center gap-2 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition disabled:opacity-60">
                                {guardandoR ? <LoaderCircle size={14} className="animate-spin" /> : <Save size={14} />}
                                {modalRol.id_Rol === 0 ? 'Crear' : 'Guardar'}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Admins;