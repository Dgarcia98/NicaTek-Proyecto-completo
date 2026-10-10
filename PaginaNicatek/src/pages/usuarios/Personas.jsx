import { useEffect, useState } from 'react';
import { useOutletContext, useNavigate } from 'react-router-dom';
import { Users, Search, X, LoaderCircle, Eye, ToggleRight, ToggleLeft } from 'lucide-react';
import Swal from 'sweetalert2';
import TablaConTarjetas from '../../components/TablaConTarjetas';
import api from '../../api/axios.Config';
import { tienePermiso } from '../../utils/permisos';

const Personas = () => {
    const { modoOscuro } = useOutletContext();
    const navigate = useNavigate();

    const puedeCrear      = tienePermiso('POS_SALE');
    const puedeDesactivar = tienePermiso('USR_MANAGE');

    // ID de persona del usuario logueado
    const idPersonaActual = parseInt(sessionStorage.getItem('idPersona') || '0');

    const [personas, setPersonas] = useState([]);
    const [cargando, setCargando] = useState(true);
    const [filtro, setFiltro] = useState('');

    const fetchPersonas = async (termino = '') => {
        try {
            setCargando(true);
            const endpoint = termino.trim()
                ? `/DatosPersonales/ListarPorNombre/${encodeURIComponent(termino.trim())}`
                : '/DatosPersonales/ListarDatosPersonales';
            const res = await api.get(endpoint);
            const data = Array.isArray(res.data) ? res.data : (res.data?.datos || []);
            setPersonas(data); // ← guardar sin ordenar, el sort va en el render
        } catch (err) { console.error('Error al cargar personas:', err); setPersonas([]);
        } finally { setCargando(false); }
    };

    useEffect(() => { fetchPersonas(); }, []);
    useEffect(() => {
        const delay = setTimeout(() => fetchPersonas(filtro), 350);
        return () => clearTimeout(delay);
    }, [filtro]);

    const alternarEstado = async (persona) => {
        if (!puedeDesactivar) return;

        // ── No permitir desactivar la propia persona ──
        if (persona.id_Persona === idPersonaActual) {
            Swal.fire({
                icon: 'warning',
                title: 'Acción no permitida',
                text: 'No puedes desactivar tu propia persona.',
                confirmButtonColor: '#0891b2'
            });
            return;
        }

        const esActivo = persona.estado === true || persona.estado === 1;
        const nuevoEstado = !esActivo;
        const nuevoNombreEstado = nuevoEstado ? 'Activo' : 'Inactivo';
        const result = await Swal.fire({
            icon: 'question', title: '¿Cambiar estado?', text: `La persona pasará a: ${nuevoNombreEstado}`,
            showCancelButton: true, confirmButtonColor: '#0891b2', cancelButtonColor: '#6b7280',
            confirmButtonText: 'Sí, cambiar', cancelButtonText: 'Cancelar',
        });
        if (!result.isConfirmed) return;
        try {
            const idUsuario = parseInt(sessionStorage.getItem('idUsuario') || '1');
            await api.put(`/DatosPersonales/EditarDatosPersonales/${persona.id_Persona}`, {
                Id_Persona: persona.id_Persona, Primer_Nombre: persona.primer_Nombre,
                Segundo_Nombre: persona.segundo_Nombre || '', Primer_Apellido: persona.primer_Apellido,
                Segundo_Apellido: persona.segundo_Apellido || '', Id_Tipo_DNI: persona.id_Tipo_DNI,
                DNI: persona.dni, Id_Genero: persona.id_Genero, Id_Nacionalidad: persona.id_Nacionalidad,
                Estado: nuevoEstado, Id_Modificador: idUsuario,
            });
            await Swal.fire({ icon: 'success', title: '¡Estado actualizado!', confirmButtonColor: '#0891b2', timer: 1500, timerProgressBar: true });
            fetchPersonas(filtro);
        } catch { Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cambiar el estado.', confirmButtonColor: '#0891b2' }); }
    };

    const filasFormateadas = [...personas]
        .sort((a, b) => (b.id_Persona ?? 0) - (a.id_Persona ?? 0))
        .map(p => {
        const nombreCompleto = [p.primer_Nombre, p.segundo_Nombre, p.primer_Apellido, p.segundo_Apellido].filter(Boolean).join(' ');
        const esActivo = p.estado === true || p.estado === 1;
        const esMiPersona = p.id_Persona === idPersonaActual;
        const idPersona = p.id_Persona; // ← guardado explícitamente para evitar bug de closure

        return [
            idPersona,
            nombreCompleto || '—',
            p.dni || '—',
            <span key={`ep-${idPersona}`} className={`px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full border ${esActivo ? 'bg-green-100 text-green-800 border-green-200' : 'bg-red-100 text-red-800 border-red-200'}`}>
                {esActivo ? 'Activo' : 'Inactivo'}
            </span>,
            <div key={`ap-${idPersona}`} className="flex space-x-2">
                <button onClick={() => navigate(`/usuarios/personas/resumen/${idPersona}`)}
                    className="text-gray-500 hover:text-cyan-600 transition-colors p-1" title="Ver resumen">
                    <Eye size={18} />
                </button>
                <button
                    onClick={() => alternarEstado(p)}
                    disabled={!puedeDesactivar || esMiPersona}
                    title={
                        !puedeDesactivar ? 'Sin permiso para cambiar estado'
                        : esMiPersona ? 'No puedes desactivar tu propia persona'
                        : 'Cambiar estado'
                    }
                    className={`p-1 hover:scale-110 transition-transform ${
                        !puedeDesactivar || esMiPersona
                            ? 'text-gray-300 cursor-not-allowed opacity-50'
                            : esActivo ? 'text-green-600 hover:text-green-800' : 'text-gray-400 hover:text-gray-600'
                    }`}>
                    {esActivo ? <ToggleRight size={20} /> : <ToggleLeft size={20} />}
                </button>
            </div>
        ];
    });

    return (
        <div className={`p-6 ${modoOscuro ? 'bg-gray-900 text-gray-100' : 'bg-transparent text-gray-800'}`}>
            <div className="flex items-center justify-between mb-6">
                <div className="flex items-center space-x-3">
                    <Users size={28} className="text-cyan-500" />
                    <h1 className="text-2xl font-bold">Personas</h1>
                </div>
                <button onClick={() => puedeCrear && navigate('/usuarios/personas/nueva')}
                    disabled={!puedeCrear}
                    title={puedeCrear ? 'Agregar persona' : 'Sin permiso para crear'}
                    className={`flex items-center px-4 py-2 font-semibold rounded-lg shadow transition-all transform ${puedeCrear ? 'bg-cyan-600 text-white hover:bg-cyan-500 hover:scale-105' : 'bg-gray-300 text-gray-500 cursor-not-allowed'}`}>
                    <span className="mr-1 text-lg font-bold">+</span> Agregar Persona
                </button>
            </div>
            <div className={`flex items-center border rounded-xl px-4 py-2 mb-6 shadow-sm ${modoOscuro ? 'bg-gray-800 border-gray-600' : 'bg-white border-gray-200'}`}>
                <Search size={18} className="text-gray-400 mr-2 shrink-0" />
                <input type="text" value={filtro} onChange={e => setFiltro(e.target.value)}
                    placeholder="Buscar por nombre o DNI..."
                    className={`w-full bg-transparent outline-none text-sm ${modoOscuro ? 'text-gray-100 placeholder-gray-500' : 'text-gray-700 placeholder-gray-400'}`} />
                {filtro && <button onClick={() => setFiltro('')} className="ml-2 text-gray-400 hover:text-gray-600"><X size={16} /></button>}
            </div>
            {cargando ? (
                <div className="flex justify-center items-center py-20"><LoaderCircle size={32} className="animate-spin text-cyan-500" /></div>
            ) : personas.length === 0 ? (
                <div className={`text-center py-16 rounded-xl border-2 border-dashed ${modoOscuro ? 'border-gray-700 text-gray-500' : 'border-gray-200 text-gray-400'}`}>
                    <Users size={40} className="mx-auto mb-3 opacity-40" />
                    <p className="text-sm">No se encontraron personas.</p>
                </div>
            ) : (
                <TablaConTarjetas encabezados={['ID', 'Nombre Completo', 'DNI', 'Estado', 'Acciones']} filas={filasFormateadas} modoOscuro={modoOscuro} />
            )}
        </div>
    );
};

export default Personas;