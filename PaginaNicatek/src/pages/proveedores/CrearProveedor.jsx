import { useState } from 'react';
import { useOutletContext } from 'react-router-dom';
import { Truck, Search, CheckCircle, Save, LoaderCircle, X } from 'lucide-react';
import Swal from 'sweetalert2';
import api from '../../api/axios.Config';

const inputCls = (modoOscuro) =>
    `w-full px-3 py-2 rounded-lg border text-sm outline-none transition focus:ring-2 focus:ring-cyan-400 ${modoOscuro ? 'bg-gray-700 border-gray-600 text-gray-100' : 'bg-gray-50 border-gray-300 text-gray-800'}`;

const labelCls = (modoOscuro) =>
    `block text-xs font-medium mb-1 ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`;

const CrearProveedor = () => {
    const { modoOscuro } = useOutletContext();

    const [busqueda, setBusqueda] = useState('');
    const [resultados, setResultados] = useState([]);
    const [buscando, setBuscando] = useState(false);
    const [personaSeleccionada, setPersonaSeleccionada] = useState(null);
    const [guardando, setGuardando] = useState(false);

    const [form, setForm] = useState({
        fechaRegistro: new Date().toISOString().split('T')[0],
        fecha_Terminacion_Contrato: '',
    });

    const buscarPersona = async () => {
        if (!busqueda.trim()) return;
        try {
            setBuscando(true);
            const res = await api.get(`/DatosPersonales/ListarPorNombre/${encodeURIComponent(busqueda.trim())}`);
            const data = Array.isArray(res.data) ? res.data : (res.data?.datos || []);
            setResultados(data.filter(p => p.estado === true));
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al buscar', confirmButtonColor: '#0891b2' });
        } finally {
            setBuscando(false);
        }
    };

    const seleccionarPersona = (persona) => {
        setPersonaSeleccionada(persona);
        setResultados([]);
        setBusqueda('');
    };

    const guardarProveedor = async () => {
        if (!personaSeleccionada) {
            Swal.fire({ icon: 'warning', title: 'Selecciona una persona', confirmButtonColor: '#0891b2' }); return;
        }
        if (!form.fechaRegistro) {
            Swal.fire({ icon: 'warning', title: 'Ingresa la fecha de registro', confirmButtonColor: '#0891b2' }); return;
        }

        try {
            setGuardando(true);
            const nombreCompleto = [
                personaSeleccionada.primer_Nombre,
                personaSeleccionada.segundo_Nombre,
                personaSeleccionada.primer_Apellido,
                personaSeleccionada.segundo_Apellido,
            ].filter(Boolean).join(' ');

            await api.post('/Proveedores/insertar', {
                Id_Proveedor: 0,
                Id_Persona: personaSeleccionada.id_Persona,
                Cedula: personaSeleccionada.dni || '',
                Primer_Nombre: personaSeleccionada.primer_Nombre || '',
                Segundo_Nombre: personaSeleccionada.segundo_Nombre || '',
                Primer_Apellido: personaSeleccionada.primer_Apellido || '',
                Segundo_Apellido: personaSeleccionada.segundo_Apellido || '',
                Nombre_Completo: nombreCompleto,
                Id_Estado: 1,
                Estado: 'Activo',
                FechaRegistro: new Date(form.fechaRegistro).toISOString(),
                Fecha_Terminacion_Contrato: form.fecha_Terminacion_Contrato
                    ? new Date(form.fecha_Terminacion_Contrato).toISOString()
                    : null,
            });

            await Swal.fire({
                icon: 'success',
                title: '¡Proveedor creado!',
                text: `${nombreCompleto} fue registrado como proveedor.`,
                confirmButtonColor: '#0891b2',
            });

            window.location.href = '/usuarios/proveedores';
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error al crear proveedor', text: JSON.stringify(err.response?.data || err.message), confirmButtonColor: '#0891b2' });
        } finally {
            setGuardando(false);
        }
    };

    return (
        <div className={`p-6 max-w-2xl mx-auto ${modoOscuro ? 'text-gray-100' : 'text-gray-800'}`}>

            {/* Header */}
            <div className="flex items-center justify-between mb-6">
                <div className="flex items-center gap-3">
                    <Truck size={26} className="text-cyan-500" />
                    <h1 className="text-xl font-bold">Nuevo Proveedor</h1>
                </div>
                <button onClick={() => window.location.href = '/usuarios/proveedores'}
                    className={`px-3 py-2 rounded-lg border text-sm transition ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                    ← Volver
                </button>
            </div>

            {/* Paso 1: Buscar persona */}
            <div className={`rounded-2xl shadow-md border p-6 mb-5 ${modoOscuro ? 'bg-gray-800 border-gray-700' : 'bg-white border-gray-200'}`}>
                <h2 className={`text-base font-semibold mb-4 pb-3 border-b ${modoOscuro ? 'text-gray-100 border-gray-700' : 'text-gray-700 border-gray-100'}`}>
                    1. Seleccionar Persona
                </h2>

                {personaSeleccionada ? (
                    <div className={`flex items-center justify-between p-3 rounded-lg border ${modoOscuro ? 'bg-gray-700 border-gray-600' : 'bg-cyan-50 border-cyan-200'}`}>
                        <div className="flex items-center gap-2">
                            <CheckCircle size={18} className="text-cyan-500" />
                            <div>
                                <p className="text-sm font-semibold">
                                    {[personaSeleccionada.primer_Nombre, personaSeleccionada.segundo_Nombre,
                                      personaSeleccionada.primer_Apellido, personaSeleccionada.segundo_Apellido]
                                        .filter(Boolean).join(' ')}
                                </p>
                                <p className={`text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                                    ID #{personaSeleccionada.id_Persona} · {personaSeleccionada.dni}
                                </p>
                            </div>
                        </div>
                        <button onClick={() => setPersonaSeleccionada(null)}
                            className="text-gray-400 hover:text-red-400 transition p-1">
                            <X size={16} />
                        </button>
                    </div>
                ) : (
                    <>
                        <div className="flex gap-2">
                            <input
                                className={inputCls(modoOscuro)}
                                placeholder="Buscar por nombre o cédula..."
                                value={busqueda}
                                onChange={e => setBusqueda(e.target.value)}
                                onKeyDown={e => e.key === 'Enter' && buscarPersona()}
                            />
                            <button onClick={buscarPersona} disabled={buscando}
                                className="flex items-center gap-1 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition disabled:opacity-60">
                                {buscando ? <LoaderCircle size={15} className="animate-spin" /> : <Search size={15} />}
                                Buscar
                            </button>
                        </div>

                        {resultados.length > 0 && (
                            <div className={`mt-2 rounded-lg border overflow-hidden ${modoOscuro ? 'border-gray-600' : 'border-gray-200'}`}>
                                {resultados.map(p => (
                                    <button key={p.id_Persona} onClick={() => seleccionarPersona(p)}
                                        className={`w-full text-left px-4 py-3 text-sm border-b last:border-0 transition ${modoOscuro ? 'border-gray-600 hover:bg-gray-700' : 'border-gray-100 hover:bg-gray-50'}`}>
                                        <span className="font-medium">
                                            {[p.primer_Nombre, p.segundo_Nombre, p.primer_Apellido, p.segundo_Apellido].filter(Boolean).join(' ')}
                                        </span>
                                        <span className={`ml-2 text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                                            ID #{p.id_Persona} · {p.dni}
                                        </span>
                                    </button>
                                ))}
                            </div>
                        )}

                        {resultados.length === 0 && busqueda && !buscando && (
                            <p className={`mt-2 text-sm ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                                No se encontraron personas activas. <a href="/usuarios/personas/nueva" className="text-cyan-500 hover:underline">Crear nueva persona</a>
                            </p>
                        )}
                    </>
                )}
            </div>

            {/* Paso 2: Fechas del contrato */}
            <div className={`rounded-2xl shadow-md border p-6 mb-5 ${modoOscuro ? 'bg-gray-800 border-gray-700' : 'bg-white border-gray-200'} ${!personaSeleccionada ? 'opacity-50 pointer-events-none' : ''}`}>
                <h2 className={`text-base font-semibold mb-4 pb-3 border-b ${modoOscuro ? 'text-gray-100 border-gray-700' : 'text-gray-700 border-gray-100'}`}>
                    2. Datos del Contrato
                    {!personaSeleccionada && <span className={`ml-2 text-xs font-normal ${modoOscuro ? 'text-gray-500' : 'text-gray-400'}`}>(Selecciona una persona primero)</span>}
                </h2>
                <div className="grid grid-cols-2 gap-4">
                    <div>
                        <label className={labelCls(modoOscuro)}>Fecha de Registro *</label>
                        <input type="date" className={inputCls(modoOscuro)}
                            value={form.fechaRegistro}
                            onChange={e => setForm(f => ({ ...f, fechaRegistro: e.target.value }))} />
                    </div>
                    <div>
                        <label className={labelCls(modoOscuro)}>Fecha Fin de Contrato</label>
                        <input type="date" className={inputCls(modoOscuro)}
                            value={form.fecha_Terminacion_Contrato}
                            onChange={e => setForm(f => ({ ...f, fecha_Terminacion_Contrato: e.target.value }))} />
                    </div>
                </div>
            </div>

            {/* Botón guardar */}
            <div className="flex justify-end">
                <button onClick={guardarProveedor} disabled={guardando || !personaSeleccionada}
                    className="flex items-center gap-2 px-6 py-2.5 bg-cyan-600 text-white rounded-lg text-sm font-semibold hover:bg-cyan-500 transition disabled:opacity-60">
                    {guardando ? <LoaderCircle size={16} className="animate-spin" /> : <Save size={16} />}
                    Guardar Proveedor
                </button>
            </div>

        </div>
    );
};

export default CrearProveedor;