import { useEffect, useState } from 'react';
import { useOutletContext } from 'react-router-dom';
import { Tag, Layers, Box, Plus, Edit, ToggleRight, ToggleLeft, Search, X, LoaderCircle, Save } from 'lucide-react';
import Swal from 'sweetalert2';
import TablaConTarjetas from '../../components/TablaConTarjetas';
import api from '../../api/axios.Config';
import { tienePermiso } from '../../utils/permisos';

const MODAL_MARCA_VACIO  = { id_Marca: 0, nombre: '', id_Estado: 1 };
const MODAL_MOD_VACIO    = { id_Modelo: 0, nombre: '', id_Estado: 1 };
const MODAL_MM_VACIO     = { id_Marca_Modelo: 0, id_Marca: '', id_Modelo: '', id_Estado: 1 };

const Buscador = ({ valor, onChange, placeholder, modoOscuro }) => (
    <div className={`flex items-center gap-2 px-3 py-2 rounded-lg border mb-4 w-full max-w-sm ${modoOscuro ? 'bg-gray-700 border-gray-600' : 'bg-white border-gray-300'}`}>
        <Search size={16} className="text-gray-400" />
        <input type="text" placeholder={placeholder} value={valor} onChange={e => onChange(e.target.value)}
            className={`flex-1 text-sm bg-transparent outline-none ${modoOscuro ? 'text-gray-100 placeholder-gray-500' : 'text-gray-800 placeholder-gray-400'}`} />
        {valor && <button onClick={() => onChange('')}><X size={14} className="text-gray-400 hover:text-gray-600" /></button>}
    </div>
);

const ModalBase = ({ titulo, icono, onClose, onGuardar, guardando, esNuevo, modoOscuro, children }) => (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm">
        <div className={`w-full max-w-sm rounded-2xl shadow-xl p-6 ${modoOscuro ? 'bg-gray-800 text-gray-100' : 'bg-white text-gray-800'}`}>
            <div className="flex items-center justify-between mb-5">
                <h2 className="text-lg font-bold flex items-center gap-2">{icono} {titulo}</h2>
                <button onClick={onClose} className="p-1.5 rounded-lg hover:bg-gray-100 transition"><X size={18} /></button>
            </div>
            <div className="space-y-4">{children}</div>
            <div className="flex justify-end gap-2 mt-6">
                <button onClick={onClose}
                    className={`px-4 py-2 rounded-lg text-sm border transition ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                    Cancelar
                </button>
                <button onClick={onGuardar} disabled={guardando}
                    className="flex items-center gap-2 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition disabled:opacity-60">
                    {guardando ? <LoaderCircle size={14} className="animate-spin" /> : <Save size={14} />}
                    {esNuevo ? 'Crear' : 'Guardar'}
                </button>
            </div>
        </div>
    </div>
);

const Marcas = () => {
    const { modoOscuro } = useOutletContext();

    const puedeCrear    = tienePermiso('INV_CREATE');
    const puedeEditar   = tienePermiso('INV_UPDATE');
    const puedeEliminar = tienePermiso('INV_DELETE');

    const [tabActiva, setTabActiva] = useState('marcas');
    const idUsuario = parseInt(sessionStorage.getItem('idUsuario') || '1');

    // ── Marcas ────────────────────────────────────────────
    const [marcas, setMarcas]         = useState([]);
    const [cargandoM, setCargandoM]   = useState(false);
    const [filtroM, setFiltroM]       = useState('');
    const [modalMarca, setModalMarca] = useState(null);
    const [guardandoM, setGuardandoM] = useState(false);

    // ── Modelos ───────────────────────────────────────────
    const [modelos, setModelos]           = useState([]);
    const [cargandoMod, setCargandoMod]   = useState(false);
    const [filtroMod, setFiltroMod]       = useState('');
    const [modalModelo, setModalModelo]   = useState(null);
    const [guardandoMod, setGuardandoMod] = useState(false);

    // ── MarcaModelo ───────────────────────────────────────
    const [marcaModelos, setMarcaModelos]   = useState([]);
    const [cargandoMM, setCargandoMM]       = useState(false);
    const [filtroMM, setFiltroMM]           = useState('');
    const [modalMM, setModalMM]             = useState(null);
    const [guardandoMM, setGuardandoMM]     = useState(false);
    const [todasMarcas, setTodasMarcas]     = useState([]);
    const [todosModelos, setTodosModelos]   = useState([]);

    // ── Fetch ─────────────────────────────────────────────
    const fetchMarcas = async (termino = '') => {
        try {
            setCargandoM(true);
            const endpoint = termino.trim() ? `/Marca/ListarPorNombre/${encodeURIComponent(termino.trim())}` : '/Marca/ListarMarca';
            const res = await api.get(endpoint);
            setMarcas(Array.isArray(res.data) ? [...res.data].sort((a, b) => (b.id_Marca ?? 0) - (a.id_Marca ?? 0)) : []);
        } catch (err) { console.error(err); setMarcas([]);
        } finally { setCargandoM(false); }
    };

    const fetchModelos = async (termino = '') => {
        try {
            setCargandoMod(true);
            const endpoint = termino.trim() ? `/Modelo/ListarPorNombre/${encodeURIComponent(termino.trim())}` : '/Modelo/ListarModelo';
            const res = await api.get(endpoint);
            setModelos(Array.isArray(res.data) ? [...res.data].sort((a, b) => (b.id_Modelo ?? 0) - (a.id_Modelo ?? 0)) : []);
        } catch (err) { console.error(err); setModelos([]);
        } finally { setCargandoMod(false); }
    };

    const fetchMarcaModelos = async (termino = '') => {
        try {
            setCargandoMM(true);
            const endpoint = termino.trim() ? `/MarcaModelo/ListarPorNombre/${encodeURIComponent(termino.trim())}` : '/MarcaModelo/ListarMarcaModelo';
            const res = await api.get(endpoint);
            setMarcaModelos(Array.isArray(res.data) ? [...res.data].sort((a, b) => (b.id_Marca_Modelo ?? 0) - (a.id_Marca_Modelo ?? 0)) : []);
        } catch (err) { console.error(err); setMarcaModelos([]);
        } finally { setCargandoMM(false); }
    };

    const fetchCatalogos = async () => {
        try {
            const [resMarcas, resModelos] = await Promise.all([api.get('/Marca/ListarMarca'), api.get('/Modelo/ListarModelo')]);
            setTodasMarcas(Array.isArray(resMarcas.data) ? resMarcas.data : []);
            setTodosModelos(Array.isArray(resModelos.data) ? resModelos.data : []);
        } catch (err) { console.error(err); }
    };

    useEffect(() => { if (tabActiva === 'marcas') fetchMarcas(); }, [tabActiva]);
    useEffect(() => { if (tabActiva === 'modelos') fetchModelos(); }, [tabActiva]);
    useEffect(() => { if (tabActiva === 'marcamodelo') { fetchMarcaModelos(); fetchCatalogos(); } }, [tabActiva]);
    useEffect(() => { const t = setTimeout(() => fetchMarcas(filtroM), 350); return () => clearTimeout(t); }, [filtroM]);
    useEffect(() => { const t = setTimeout(() => fetchModelos(filtroMod), 350); return () => clearTimeout(t); }, [filtroMod]);
    useEffect(() => { const t = setTimeout(() => fetchMarcaModelos(filtroMM), 350); return () => clearTimeout(t); }, [filtroMM]);

    // ── CRUD Marca ────────────────────────────────────────
    const guardarMarca = async () => {
        if (!modalMarca.nombre.trim()) {
            Swal.fire({ icon: 'warning', title: 'El nombre es requerido', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardandoM(true);
            const esNuevo = modalMarca.id_Marca === 0;
            const payload = {
                Id_Marca: modalMarca.id_Marca,
                Nombre: modalMarca.nombre.trim(),
                Id_Estado: modalMarca.id_Estado,
                Nombre_Estado: modalMarca.id_Estado === 1 ? 'Activo' : 'Inactivo',
                Fecha_Creacion: new Date().toISOString(),
                Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: idUsuario,
                Id_Modificador: idUsuario,
            };
            if (esNuevo) await api.post('/Marca/NuevoMarca', payload);
            else         await api.put(`/Marca/EditarMarca/${modalMarca.id_Marca}`, payload);
            setModalMarca(null);
            await fetchMarcas(filtroM);
            Swal.fire({ icon: 'success', title: esNuevo ? 'Marca creada' : 'Marca actualizada', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al guardar', confirmButtonColor: '#0891b2' });
        } finally { setGuardandoM(false); }
    };

    // ← TOGGLE CORRECTO: usa EditarMarca con el nuevo estado, NO EliminarMarca
    const toggleMarca = async (marca) => {
        if (!puedeEliminar) return;
        const esActivo = marca.id_Estado === 1;
        const nuevoEstado = esActivo ? 2 : 1;
        const result = await Swal.fire({
            icon: 'question', title: '¿Cambiar estado?',
            text: `La marca "${marca.nombre}" pasará a ${esActivo ? 'Inactivo' : 'Activo'}.`,
            showCancelButton: true, confirmButtonColor: '#0891b2', cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, cambiar', cancelButtonText: 'Cancelar',
        });
        if (!result.isConfirmed) return;
        try {
            await api.put(`/Marca/EditarMarca/${marca.id_Marca}`, {
                Id_Marca: marca.id_Marca,
                Nombre: marca.nombre,
                Id_Estado: nuevoEstado,
                Nombre_Estado: nuevoEstado === 1 ? 'Activo' : 'Inactivo',
                Fecha_Creacion: marca.fecha_Creacion || new Date().toISOString(),
                Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: marca.id_Creador || idUsuario,
                Id_Modificador: idUsuario,
            });
            await fetchMarcas(filtroM);
            Swal.fire({ icon: 'success', title: `Marca ${nuevoEstado === 1 ? 'activada' : 'desactivada'}`, timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al cambiar estado', confirmButtonColor: '#0891b2' });
        }
    };

    // ── CRUD Modelo ───────────────────────────────────────
    const guardarModelo = async () => {
        if (!modalModelo.nombre.trim()) {
            Swal.fire({ icon: 'warning', title: 'El nombre es requerido', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardandoMod(true);
            const esNuevo = modalModelo.id_Modelo === 0;
            const payload = {
                Id_Modelo: modalModelo.id_Modelo,
                Nombre: modalModelo.nombre.trim(),
                Id_Estado: modalModelo.id_Estado,
                Nombre_Estado: modalModelo.id_Estado === 1 ? 'Activo' : 'Inactivo',
                Fecha_Creacion: new Date().toISOString(),
                Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: idUsuario,
                Id_Modificador: idUsuario,
            };
            if (esNuevo) await api.post('/Modelo/NuevoModelo', payload);
            else         await api.put(`/Modelo/EditarModelo/${modalModelo.id_Modelo}`, payload);
            setModalModelo(null);
            await fetchModelos(filtroMod);
            Swal.fire({ icon: 'success', title: esNuevo ? 'Modelo creado' : 'Modelo actualizado', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al guardar', confirmButtonColor: '#0891b2' });
        } finally { setGuardandoMod(false); }
    };

    const toggleModelo = async (modelo) => {
        if (!puedeEliminar) return;
        const esActivo = modelo.id_Estado === 1;
        const nuevoEstado = esActivo ? 2 : 1;
        const result = await Swal.fire({
            icon: 'question', title: '¿Cambiar estado?',
            text: `El modelo "${modelo.nombre}" pasará a ${esActivo ? 'Inactivo' : 'Activo'}.`,
            showCancelButton: true, confirmButtonColor: '#0891b2', cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, cambiar', cancelButtonText: 'Cancelar',
        });
        if (!result.isConfirmed) return;
        try {
            await api.put(`/Modelo/EditarModelo/${modelo.id_Modelo}`, {
                Id_Modelo: modelo.id_Modelo,
                Nombre: modelo.nombre,
                Id_Estado: nuevoEstado,
                Nombre_Estado: nuevoEstado === 1 ? 'Activo' : 'Inactivo',
                Fecha_Creacion: modelo.fecha_Creacion || new Date().toISOString(),
                Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: modelo.id_Creador || idUsuario,
                Id_Modificador: idUsuario,
            });
            await fetchModelos(filtroMod);
            Swal.fire({ icon: 'success', title: `Modelo ${nuevoEstado === 1 ? 'activado' : 'desactivado'}`, timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al cambiar estado', confirmButtonColor: '#0891b2' });
        }
    };

    // ── CRUD MarcaModelo ──────────────────────────────────
    const guardarMM = async () => {
        if (!modalMM.id_Marca || !modalMM.id_Modelo) {
            Swal.fire({ icon: 'warning', title: 'Selecciona marca y modelo', confirmButtonColor: '#0891b2' }); return;
        }
        try {
            setGuardandoMM(true);
            const esNuevo = modalMM.id_Marca_Modelo === 0;
            const marcaSel  = todasMarcas.find(m => m.id_Marca === parseInt(modalMM.id_Marca));
            const modeloSel = todosModelos.find(m => m.id_Modelo === parseInt(modalMM.id_Modelo));
            const payload = {
                Id_Marca_Modelo: modalMM.id_Marca_Modelo,
                Id_Marca: parseInt(modalMM.id_Marca),
                Nombre_Marca: marcaSel?.nombre || '',
                Id_Modelo: parseInt(modalMM.id_Modelo),
                Nombre_Modelo: modeloSel?.nombre || '',
                Id_Estado: modalMM.id_Estado,
                Nombre_Estado: modalMM.id_Estado === 1 ? 'Activo' : 'Inactivo',
                Fecha_Creacion: new Date().toISOString(),
                Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: idUsuario,
                Id_Modificador: idUsuario,
            };
            if (esNuevo) await api.post('/MarcaModelo/NuevoMarcaModelo', payload);
            else         await api.put(`/MarcaModelo/EditarMarcaModelo/${modalMM.id_Marca_Modelo}`, payload);
            setModalMM(null);
            await fetchMarcaModelos(filtroMM);
            Swal.fire({ icon: 'success', title: esNuevo ? 'Relación creada' : 'Relación actualizada', timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al guardar', confirmButtonColor: '#0891b2' });
        } finally { setGuardandoMM(false); }
    };

    const toggleMM = async (mm) => {
        if (!puedeEliminar) return;
        const esActivo = mm.id_Estado === 1;
        const nuevoEstado = esActivo ? 2 : 1;
        const result = await Swal.fire({
            icon: 'question', title: '¿Cambiar estado?',
            text: `"${mm.nombre_Marca} - ${mm.nombre_Modelo}" pasará a ${esActivo ? 'Inactivo' : 'Activo'}.`,
            showCancelButton: true, confirmButtonColor: '#0891b2', cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, cambiar', cancelButtonText: 'Cancelar',
        });
        if (!result.isConfirmed) return;
        try {
            await api.put(`/MarcaModelo/EditarMarcaModelo/${mm.id_Marca_Modelo}`, {
                Id_Marca_Modelo: mm.id_Marca_Modelo,
                Id_Marca: mm.id_Marca, Nombre_Marca: mm.nombre_Marca || '',
                Id_Modelo: mm.id_Modelo, Nombre_Modelo: mm.nombre_Modelo || '',
                Id_Estado: nuevoEstado,
                Nombre_Estado: nuevoEstado === 1 ? 'Activo' : 'Inactivo',
                Fecha_Creacion: mm.fecha_Creacion || new Date().toISOString(),
                Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: mm.id_Creador || idUsuario,
                Id_Modificador: idUsuario,
            });
            await fetchMarcaModelos(filtroMM);
            Swal.fire({ icon: 'success', title: `Relación ${nuevoEstado === 1 ? 'activada' : 'desactivada'}`, timer: 1200, timerProgressBar: true, showConfirmButton: false });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al cambiar estado', confirmButtonColor: '#0891b2' });
        }
    };

    // ── Helpers UI ────────────────────────────────────────
    const badgeCls = (esActivo) =>
        `px-2 py-0.5 text-xs rounded-full border font-semibold ${esActivo ? 'bg-green-100 text-green-800 border-green-200' : 'bg-red-100 text-red-800 border-red-200'}`;

    const accionCls = (puede) =>
        `p-1.5 rounded-lg transition ${puede ? 'text-cyan-500 hover:bg-cyan-50' : 'text-gray-300 cursor-not-allowed'}`;

    const toggleCls = (puede, esActivo) =>
        `p-1.5 rounded-lg transition ${!puede ? 'text-gray-300 cursor-not-allowed' : esActivo ? 'text-green-500 hover:bg-green-50' : 'text-red-400 hover:bg-red-50'}`;

    // ── Filas ─────────────────────────────────────────────
    const filasM = marcas.map(m => {
        const esActivo = m.id_Estado === 1;
        return [
            m.id_Marca, m.nombre,
            <span key={`em-${m.id_Marca}`} className={badgeCls(esActivo)}>{esActivo ? 'Activo' : 'Inactivo'}</span>,
            m.fecha_Creacion ? new Date(m.fecha_Creacion).toLocaleDateString() : '—',
            <div key={`am-${m.id_Marca}`} className="flex items-center gap-2">
                <button onClick={() => puedeEditar && setModalMarca({ id_Marca: m.id_Marca, nombre: m.nombre, id_Estado: m.id_Estado })}
                    disabled={!puedeEditar} title={puedeEditar ? 'Editar' : 'Sin permiso'} className={accionCls(puedeEditar)}>
                    <Edit size={16} />
                </button>
                <button onClick={() => toggleMarca(m)} disabled={!puedeEliminar}
                    title={puedeEliminar ? (esActivo ? 'Desactivar' : 'Activar') : 'Sin permiso'} className={toggleCls(puedeEliminar, esActivo)}>
                    {esActivo ? <ToggleRight size={18} /> : <ToggleLeft size={18} />}
                </button>
            </div>
        ];
    });

    const filasMod = modelos.map(m => {
        const esActivo = m.id_Estado === 1;
        return [
            m.id_Modelo, m.nombre,
            <span key={`emod-${m.id_Modelo}`} className={badgeCls(esActivo)}>{esActivo ? 'Activo' : 'Inactivo'}</span>,
            m.fecha_Creacion ? new Date(m.fecha_Creacion).toLocaleDateString() : '—',
            <div key={`amod-${m.id_Modelo}`} className="flex items-center gap-2">
                <button onClick={() => puedeEditar && setModalModelo({ id_Modelo: m.id_Modelo, nombre: m.nombre, id_Estado: m.id_Estado })}
                    disabled={!puedeEditar} title={puedeEditar ? 'Editar' : 'Sin permiso'} className={accionCls(puedeEditar)}>
                    <Edit size={16} />
                </button>
                <button onClick={() => toggleModelo(m)} disabled={!puedeEliminar}
                    title={puedeEliminar ? (esActivo ? 'Desactivar' : 'Activar') : 'Sin permiso'} className={toggleCls(puedeEliminar, esActivo)}>
                    {esActivo ? <ToggleRight size={18} /> : <ToggleLeft size={18} />}
                </button>
            </div>
        ];
    });

    const filasMM = marcaModelos.map(mm => {
        const esActivo = mm.id_Estado === 1;
        return [
            mm.id_Marca_Modelo, mm.nombre_Marca || '—', mm.nombre_Modelo || '—',
            <span key={`emm-${mm.id_Marca_Modelo}`} className={badgeCls(esActivo)}>{esActivo ? 'Activo' : 'Inactivo'}</span>,
            mm.fecha_Creacion ? new Date(mm.fecha_Creacion).toLocaleDateString() : '—',
            <div key={`amm-${mm.id_Marca_Modelo}`} className="flex items-center gap-2">
                <button onClick={() => { if (puedeEditar) { fetchCatalogos(); setModalMM({ id_Marca_Modelo: mm.id_Marca_Modelo, id_Marca: mm.id_Marca, id_Modelo: mm.id_Modelo, id_Estado: mm.id_Estado }); } }}
                    disabled={!puedeEditar} title={puedeEditar ? 'Editar' : 'Sin permiso'} className={accionCls(puedeEditar)}>
                    <Edit size={16} />
                </button>
                <button onClick={() => toggleMM(mm)} disabled={!puedeEliminar}
                    title={puedeEliminar ? (esActivo ? 'Desactivar' : 'Activar') : 'Sin permiso'} className={toggleCls(puedeEliminar, esActivo)}>
                    {esActivo ? <ToggleRight size={18} /> : <ToggleLeft size={18} />}
                </button>
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
                    <Tag size={22} className="text-cyan-500" />
                    <h1 className="text-2xl font-bold">Marcas y Modelos</h1>
                </div>
                {tabActiva === 'marcas' && puedeCrear && (
                    <button onClick={() => setModalMarca({ ...MODAL_MARCA_VACIO })}
                        className="flex items-center gap-2 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm font-semibold hover:bg-cyan-500 transition">
                        <Plus size={16} /> Nueva Marca
                    </button>
                )}
                {tabActiva === 'modelos' && puedeCrear && (
                    <button onClick={() => setModalModelo({ ...MODAL_MOD_VACIO })}
                        className="flex items-center gap-2 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm font-semibold hover:bg-cyan-500 transition">
                        <Plus size={16} /> Nuevo Modelo
                    </button>
                )}
                {tabActiva === 'marcamodelo' && puedeCrear && (
                    <button onClick={() => { fetchCatalogos(); setModalMM({ ...MODAL_MM_VACIO }); }}
                        className="flex items-center gap-2 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm font-semibold hover:bg-cyan-500 transition">
                        <Plus size={16} /> Nueva Relación
                    </button>
                )}
            </div>

            {/* Tabs */}
            <div className={`flex gap-1 border-b mb-5 ${modoOscuro ? 'border-gray-700' : 'border-gray-200'}`}>
                <button className={tabCls('marcas')} onClick={() => setTabActiva('marcas')}>
                    <span className="flex items-center gap-1"><Tag size={14} /> Marcas</span>
                </button>
                <button className={tabCls('modelos')} onClick={() => setTabActiva('modelos')}>
                    <span className="flex items-center gap-1"><Box size={14} /> Modelos</span>
                </button>
                <button className={tabCls('marcamodelo')} onClick={() => setTabActiva('marcamodelo')}>
                    <span className="flex items-center gap-1"><Layers size={14} /> Marca - Modelo</span>
                </button>
            </div>

            {/* TAB MARCAS */}
            {tabActiva === 'marcas' && (
                <>
                    <Buscador valor={filtroM} onChange={setFiltroM} placeholder="Buscar marca..." modoOscuro={modoOscuro} />
                    {cargandoM ? <div className="flex justify-center py-16"><LoaderCircle size={32} className="animate-spin text-cyan-500" /></div>
                        : <TablaConTarjetas encabezados={['ID', 'Nombre', 'Estado', 'Creación', 'Acciones']} filas={filasM} modoOscuro={modoOscuro} />}
                </>
            )}

            {/* TAB MODELOS */}
            {tabActiva === 'modelos' && (
                <>
                    <Buscador valor={filtroMod} onChange={setFiltroMod} placeholder="Buscar modelo..." modoOscuro={modoOscuro} />
                    {cargandoMod ? <div className="flex justify-center py-16"><LoaderCircle size={32} className="animate-spin text-cyan-500" /></div>
                        : <TablaConTarjetas encabezados={['ID', 'Nombre', 'Estado', 'Creación', 'Acciones']} filas={filasMod} modoOscuro={modoOscuro} />}
                </>
            )}

            {/* TAB MARCA-MODELO */}
            {tabActiva === 'marcamodelo' && (
                <>
                    <Buscador valor={filtroMM} onChange={setFiltroMM} placeholder="Buscar marca o modelo..." modoOscuro={modoOscuro} />
                    {cargandoMM ? <div className="flex justify-center py-16"><LoaderCircle size={32} className="animate-spin text-cyan-500" /></div>
                        : <TablaConTarjetas encabezados={['ID', 'Marca', 'Modelo', 'Estado', 'Creación', 'Acciones']} filas={filasMM} modoOscuro={modoOscuro} />}
                </>
            )}

            {/* MODAL MARCA */}
            {modalMarca && (
                <ModalBase titulo={modalMarca.id_Marca === 0 ? 'Nueva Marca' : 'Editar Marca'}
                    icono={<Tag size={18} className="text-cyan-500" />}
                    onClose={() => setModalMarca(null)} onGuardar={guardarMarca}
                    guardando={guardandoM} esNuevo={modalMarca.id_Marca === 0} modoOscuro={modoOscuro}>
                    <div>
                        <label className={labelCls}>Nombre *</label>
                        <input className={inputCls} value={modalMarca.nombre} placeholder="ej: Samsung"
                            onChange={e => setModalMarca(m => ({ ...m, nombre: e.target.value }))} />
                    </div>
                    {modalMarca.id_Marca !== 0 && (
                        <div>
                            <label className={labelCls}>Estado</label>
                            <select className={inputCls} value={modalMarca.id_Estado}
                                onChange={e => setModalMarca(m => ({ ...m, id_Estado: parseInt(e.target.value) }))}>
                                <option value={1}>Activo</option>
                                <option value={2}>Inactivo</option>
                            </select>
                        </div>
                    )}
                </ModalBase>
            )}

            {/* MODAL MODELO */}
            {modalModelo && (
                <ModalBase titulo={modalModelo.id_Modelo === 0 ? 'Nuevo Modelo' : 'Editar Modelo'}
                    icono={<Box size={18} className="text-cyan-500" />}
                    onClose={() => setModalModelo(null)} onGuardar={guardarModelo}
                    guardando={guardandoMod} esNuevo={modalModelo.id_Modelo === 0} modoOscuro={modoOscuro}>
                    <div>
                        <label className={labelCls}>Nombre *</label>
                        <input className={inputCls} value={modalModelo.nombre} placeholder="ej: Galaxy S24"
                            onChange={e => setModalModelo(m => ({ ...m, nombre: e.target.value }))} />
                    </div>
                    {modalModelo.id_Modelo !== 0 && (
                        <div>
                            <label className={labelCls}>Estado</label>
                            <select className={inputCls} value={modalModelo.id_Estado}
                                onChange={e => setModalModelo(m => ({ ...m, id_Estado: parseInt(e.target.value) }))}>
                                <option value={1}>Activo</option>
                                <option value={2}>Inactivo</option>
                            </select>
                        </div>
                    )}
                </ModalBase>
            )}

            {/* MODAL MARCA-MODELO */}
            {modalMM && (
                <ModalBase titulo={modalMM.id_Marca_Modelo === 0 ? 'Nueva Relación' : 'Editar Relación'}
                    icono={<Layers size={18} className="text-cyan-500" />}
                    onClose={() => setModalMM(null)} onGuardar={guardarMM}
                    guardando={guardandoMM} esNuevo={modalMM.id_Marca_Modelo === 0} modoOscuro={modoOscuro}>
                    <div>
                        <label className={labelCls}>Marca *</label>
                        <select className={inputCls} value={modalMM.id_Marca}
                            onChange={e => setModalMM(m => ({ ...m, id_Marca: e.target.value }))}>
                            <option value="">Seleccionar...</option>
                            {todasMarcas.filter(m => m.id_Estado === 1).map(m => (
                                <option key={m.id_Marca} value={m.id_Marca}>{m.nombre}</option>
                            ))}
                        </select>
                    </div>
                    <div>
                        <label className={labelCls}>Modelo *</label>
                        <select className={inputCls} value={modalMM.id_Modelo}
                            onChange={e => setModalMM(m => ({ ...m, id_Modelo: e.target.value }))}>
                            <option value="">Seleccionar...</option>
                            {todosModelos.filter(m => m.id_Estado === 1).map(m => (
                                <option key={m.id_Modelo} value={m.id_Modelo}>{m.nombre}</option>
                            ))}
                        </select>
                    </div>
                    {modalMM.id_Marca_Modelo !== 0 && (
                        <div>
                            <label className={labelCls}>Estado</label>
                            <select className={inputCls} value={modalMM.id_Estado}
                                onChange={e => setModalMM(m => ({ ...m, id_Estado: parseInt(e.target.value) }))}>
                                <option value={1}>Activo</option>
                                <option value={2}>Inactivo</option>
                            </select>
                        </div>
                    )}
                </ModalBase>
            )}
        </div>
    );
};

export default Marcas;