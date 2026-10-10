import { useEffect, useState } from 'react';
import { useOutletContext, useNavigate } from 'react-router-dom';
import { Users, Search, X, LoaderCircle, Eye, ToggleRight, ToggleLeft, Plus } from 'lucide-react';
import Swal from 'sweetalert2';
import TablaConTarjetas from '../../components/TablaConTarjetas';
import api from '../../api/axios.Config';
import { tienePermiso } from '../../utils/permisos';

const Proveedores = () => {
    const { modoOscuro } = useOutletContext();
    const navigate = useNavigate();

    const puedeCrear      = tienePermiso('POS_SALE');   // Empleado puede crear
    const puedeDesactivar = tienePermiso('USR_MANAGE'); // Solo admin puede desactivar

    const [proveedores, setProveedores] = useState([]);
    const [cargando, setCargando] = useState(true);
    const [filtro, setFiltro] = useState('');

    const fetchProveedores = async (termino = '') => {
        try {
            setCargando(true);
            const endpoint = termino.trim()
                ? `/Proveedores/buscar/${encodeURIComponent(termino.trim())}`
                : '/Proveedores/listar';
            const res = await api.get(endpoint);
            const data = Array.isArray(res.data) ? res.data : (res.data?.datos || []);
            setProveedores([...data].sort((a, b) => (b.id_Proveedor ?? 0) - (a.id_Proveedor ?? 0)));
        } catch (err) { console.error('Error al cargar proveedores:', err); setProveedores([]);
        } finally { setCargando(false); }
    };

    useEffect(() => { fetchProveedores(); }, []);
    useEffect(() => {
        const delay = setTimeout(() => fetchProveedores(filtro), 350);
        return () => clearTimeout(delay);
    }, [filtro]);

    const alternarEstado = async (proveedor) => {
        if (!puedeDesactivar) return;
        const esActivo = proveedor.id_Estado === 1;
        const nuevoEstado = esActivo ? 2 : 1;
        const nuevoNombre = esActivo ? 'Inactivo' : 'Activo';
        const result = await Swal.fire({
            icon: 'question', title: '¿Cambiar estado?', text: `El proveedor pasará a: ${nuevoNombre}`,
            showCancelButton: true, confirmButtonColor: '#0891b2', cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, cambiar', cancelButtonText: 'Cancelar',
        });
        if (!result.isConfirmed) return;
        try {
            await api.put(`/Proveedores/editar/${proveedor.id_Proveedor}`, {
                Id_Proveedor: proveedor.id_Proveedor, Id_Persona: proveedor.id_Persona,
                Cedula: proveedor.cedula || '', Primer_Nombre: proveedor.primer_Nombre || '',
                Segundo_Nombre: proveedor.segundo_Nombre || '', Primer_Apellido: proveedor.primer_Apellido || '',
                Segundo_Apellido: proveedor.segundo_Apellido || '', Nombre_Completo: proveedor.nombre_Completo || '',
                Id_Estado: nuevoEstado, Estado: nuevoNombre,
                FechaRegistro: proveedor.fechaRegistro || new Date().toISOString(),
                Fecha_Terminacion_Contrato: proveedor.fecha_Terminacion_Contrato || null,
            });
            Swal.fire({ icon: 'success', title: `Proveedor ${nuevoNombre}`, timer: 1200, timerProgressBar: true, showConfirmButton: false });
            fetchProveedores(filtro);
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al cambiar estado', confirmButtonColor: '#0891b2' });
        }
    };

    const filas = proveedores.map(p => {
        const esActivo = p.id_Estado === 1;
        return [
            p.id_Proveedor,
            p.nombre_Completo || `${p.primer_Nombre || ''} ${p.primer_Apellido || ''}`.trim(),
            p.cedula || '—',
            <span key={`ep-${p.id_Proveedor}`} className={`px-2 py-0.5 text-xs rounded-full border font-semibold ${esActivo ? 'bg-green-100 text-green-800 border-green-200' : 'bg-red-100 text-red-800 border-red-200'}`}>
                {esActivo ? 'Activo' : 'Inactivo'}
            </span>,
            p.fechaRegistro ? new Date(p.fechaRegistro).toLocaleDateString() : '—',
            p.fecha_Terminacion_Contrato ? new Date(p.fecha_Terminacion_Contrato).toLocaleDateString() : '—',
            <div key={`ap-${p.id_Proveedor}`} className="flex items-center gap-2">
                <button onClick={() => navigate(`/usuarios/proveedores/resumen/${p.id_Proveedor}`)}
                    title="Ver resumen" className="p-1.5 rounded-lg text-cyan-500 hover:bg-cyan-50 transition">
                    <Eye size={16} />
                </button>
                <button onClick={() => alternarEstado(p)} disabled={!puedeDesactivar}
                    title={puedeDesactivar ? (esActivo ? 'Desactivar' : 'Activar') : 'Sin permiso para cambiar estado'}
                    className={`p-1.5 rounded-lg transition ${!puedeDesactivar ? 'text-gray-300 cursor-not-allowed' : esActivo ? 'text-green-500 hover:bg-green-50' : 'text-red-400 hover:bg-red-50'}`}>
                    {esActivo ? <ToggleRight size={18} /> : <ToggleLeft size={18} />}
                </button>
            </div>
        ];
    });

    return (
        <div className={`p-6 transition-colors duration-300 ${modoOscuro ? 'bg-gray-800 text-white' : 'bg-gray-100 text-gray-800'}`}>
            <div className="flex items-center justify-between mb-5">
                <div className="flex items-center gap-2">
                    <Users size={22} className="text-cyan-500" />
                    <h1 className="text-2xl font-bold">Proveedores</h1>
                </div>
                <button onClick={() => puedeCrear && navigate('/usuarios/proveedores/nuevo')}
                    disabled={!puedeCrear}
                    title={puedeCrear ? 'Nuevo proveedor' : 'Sin permiso para crear'}
                    className={`flex items-center gap-2 px-4 py-2 rounded-lg text-sm font-semibold transition ${puedeCrear ? 'bg-cyan-600 text-white hover:bg-cyan-500' : 'bg-gray-300 text-gray-500 cursor-not-allowed'}`}>
                    <Plus size={16} /> Nuevo Proveedor
                </button>
            </div>
            <div className={`flex items-center gap-2 px-3 py-2 rounded-lg border mb-4 w-full max-w-sm ${modoOscuro ? 'bg-gray-700 border-gray-600' : 'bg-white border-gray-300'}`}>
                <Search size={16} className="text-gray-400" />
                <input type="text" placeholder="Buscar proveedor..." value={filtro} onChange={e => setFiltro(e.target.value)}
                    className={`flex-1 text-sm bg-transparent outline-none ${modoOscuro ? 'text-gray-100 placeholder-gray-500' : 'text-gray-800 placeholder-gray-400'}`} />
                {filtro && <button onClick={() => setFiltro('')}><X size={14} className="text-gray-400 hover:text-gray-600" /></button>}
            </div>
            {cargando ? (
                <div className="flex justify-center py-16"><LoaderCircle size={32} className="animate-spin text-cyan-500" /></div>
            ) : (
                <TablaConTarjetas encabezados={['ID', 'Nombre', 'Cédula', 'Estado', 'Registro', 'Fin Contrato', 'Acciones']} filas={filas} modoOscuro={modoOscuro} />
            )}
        </div>
    );
};

export default Proveedores;