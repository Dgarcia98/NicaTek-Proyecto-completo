import { useState, useEffect } from 'react';
import { useOutletContext } from 'react-router-dom';
import { Users, Search, CheckCircle, Save, LoaderCircle, X } from 'lucide-react';
import Swal from 'sweetalert2';
import api from '../../api/axios.Config';

const inputCls = (modoOscuro) =>
    `w-full px-3 py-2 rounded-lg border text-sm outline-none transition focus:ring-2 focus:ring-cyan-400 ${modoOscuro ? 'bg-gray-700 border-gray-600 text-gray-100' : 'bg-gray-50 border-gray-300 text-gray-800'}`;
const labelCls = (modoOscuro) =>
    `block text-xs font-medium mb-1 ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`;

const CrearEmpleado = () => {
    const { modoOscuro } = useOutletContext();
    const [busqueda, setBusqueda] = useState('');
    const [resultados, setResultados] = useState([]);
    const [buscando, setBuscando] = useState(false);
    const [personaSeleccionada, setPersonaSeleccionada] = useState(null);
    const [cargos, setCargos] = useState([]);
    const [guardando, setGuardando] = useState(false);
    const [form, setForm] = useState({ id_Cargo: '', fecha_Contratacion: new Date().toISOString().split('T')[0], fecha_Despido: '' });

    useEffect(() => {
        api.get('/Catalogo/ListarCatalogo').then(res => {
            const data = Array.isArray(res.data) ? res.data : [];
            setCargos(data.filter(c => c.id_Tipo_Catalogo === 4));
        }).catch(console.error);
    }, []);

    const buscarPersona = async () => {
        if (!busqueda.trim()) return;
        try {
            setBuscando(true);
            const res = await api.get(`/DatosPersonales/ListarPorNombre/${encodeURIComponent(busqueda.trim())}`);
            const data = Array.isArray(res.data) ? res.data : (res.data?.datos || []);
            setResultados(data.filter(p => p.estado === true));
        } catch { Swal.fire({ icon: 'error', title: 'Error al buscar', confirmButtonColor: '#0891b2' }); }
        finally { setBuscando(false); }
    };

    const guardar = async () => {
        if (!personaSeleccionada) { Swal.fire({ icon: 'warning', title: 'Selecciona una persona', confirmButtonColor: '#0891b2' }); return; }
        if (!form.id_Cargo) { Swal.fire({ icon: 'warning', title: 'Selecciona un cargo', confirmButtonColor: '#0891b2' }); return; }
        if (!form.fecha_Contratacion) { Swal.fire({ icon: 'warning', title: 'Ingresa la fecha de contratación', confirmButtonColor: '#0891b2' }); return; }
        try {
            setGuardando(true);
            const cargoSeleccionado = cargos.find(c => c.id_Catalogo === parseInt(form.id_Cargo));
            const nombreCompleto = [personaSeleccionada.primer_Nombre, personaSeleccionada.segundo_Nombre,
                personaSeleccionada.primer_Apellido, personaSeleccionada.segundo_Apellido].filter(Boolean).join(' ');
            await api.post('/Empleado/NuevoEmpleado', {
                Id_Empleado: 0,
                Id_Persona: personaSeleccionada.id_Persona,
                Nombre_Completo: nombreCompleto,
                Cedula: personaSeleccionada.dni || '',
                Fecha_Contratacion: new Date(form.fecha_Contratacion).toISOString(),
                Fecha_Despido: form.fecha_Despido ? new Date(form.fecha_Despido).toISOString() : null,
                Id_Cargo: parseInt(form.id_Cargo),
                Cargo: cargoSeleccionado?.nombre_Catalogo || '',
                Id_Estado: 1,
                Estado: 'Activo',
                Fecha_Creacion: new Date().toISOString(),
                Fecha_Modificacion: new Date().toISOString(),
                Id_Creador: 1,
                Id_Modificador: 1,
            });
            await Swal.fire({ icon: 'success', title: '¡Empleado creado!', text: `${nombreCompleto} fue registrado.`, confirmButtonColor: '#0891b2' });
            window.location.href = '/usuarios/admins';
        } catch (err) {
            Swal.fire({ icon: 'error', title: 'Error al crear empleado', text: JSON.stringify(err.response?.data || err.message), confirmButtonColor: '#0891b2' });
        } finally { setGuardando(false); }
    };

    return (
        <div className={`p-6 max-w-2xl mx-auto ${modoOscuro ? 'text-gray-100' : 'text-gray-800'}`}>
            <div className="flex items-center justify-between mb-6">
                <div className="flex items-center gap-3">
                    <Users size={26} className="text-cyan-500" />
                    <h1 className="text-xl font-bold">Nuevo Empleado</h1>
                </div>
                <button onClick={() => window.location.href = '/usuarios/admins'}
                    className={`px-3 py-2 rounded-lg border text-sm transition ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                    ← Volver
                </button>
            </div>

            <div className={`rounded-2xl shadow-md border p-6 mb-5 ${modoOscuro ? 'bg-gray-800 border-gray-700' : 'bg-white border-gray-200'}`}>
                <h2 className={`text-base font-semibold mb-4 pb-3 border-b ${modoOscuro ? 'border-gray-700' : 'border-gray-100'}`}>1. Seleccionar Persona</h2>
                {personaSeleccionada ? (
                    <div className={`flex items-center justify-between p-3 rounded-lg border ${modoOscuro ? 'bg-gray-700 border-gray-600' : 'bg-cyan-50 border-cyan-200'}`}>
                        <div className="flex items-center gap-2">
                            <CheckCircle size={18} className="text-cyan-500" />
                            <div>
                                <p className="text-sm font-semibold">{[personaSeleccionada.primer_Nombre, personaSeleccionada.segundo_Nombre, personaSeleccionada.primer_Apellido, personaSeleccionada.segundo_Apellido].filter(Boolean).join(' ')}</p>
                                <p className={`text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>ID #{personaSeleccionada.id_Persona} · {personaSeleccionada.dni}</p>
                            </div>
                        </div>
                        <button onClick={() => setPersonaSeleccionada(null)} className="text-gray-400 hover:text-red-400 p-1"><X size={16} /></button>
                    </div>
                ) : (
                    <>
                        <div className="flex gap-2">
                            <input className={inputCls(modoOscuro)} placeholder="Buscar por nombre o cédula..."
                                value={busqueda} onChange={e => setBusqueda(e.target.value)} onKeyDown={e => e.key === 'Enter' && buscarPersona()} />
                            <button onClick={buscarPersona} disabled={buscando}
                                className="flex items-center gap-1 px-4 py-2 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition disabled:opacity-60">
                                {buscando ? <LoaderCircle size={15} className="animate-spin" /> : <Search size={15} />} Buscar
                            </button>
                        </div>
                        {resultados.length > 0 && (
                            <div className={`mt-2 rounded-lg border overflow-hidden ${modoOscuro ? 'border-gray-600' : 'border-gray-200'}`}>
                                {resultados.map(p => (
                                    <button key={p.id_Persona} onClick={() => { setPersonaSeleccionada(p); setResultados([]); setBusqueda(''); }}
                                        className={`w-full text-left px-4 py-3 text-sm border-b last:border-0 transition ${modoOscuro ? 'border-gray-600 hover:bg-gray-700' : 'border-gray-100 hover:bg-gray-50'}`}>
                                        <span className="font-medium">{[p.primer_Nombre, p.segundo_Nombre, p.primer_Apellido, p.segundo_Apellido].filter(Boolean).join(' ')}</span>
                                        <span className={`ml-2 text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>ID #{p.id_Persona} · {p.dni}</span>
                                    </button>
                                ))}
                            </div>
                        )}
                        {resultados.length === 0 && busqueda && !buscando && (
                            <p className={`mt-2 text-sm ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                                No se encontraron personas. <a href="/usuarios/personas/nueva" className="text-cyan-500 hover:underline">Crear nueva persona</a>
                            </p>
                        )}
                    </>
                )}
            </div>

            <div className={`rounded-2xl shadow-md border p-6 mb-5 ${modoOscuro ? 'bg-gray-800 border-gray-700' : 'bg-white border-gray-200'} ${!personaSeleccionada ? 'opacity-50 pointer-events-none' : ''}`}>
                <h2 className={`text-base font-semibold mb-4 pb-3 border-b ${modoOscuro ? 'border-gray-700' : 'border-gray-100'}`}>
                    2. Datos del Empleado
                    {!personaSeleccionada && <span className={`ml-2 text-xs font-normal ${modoOscuro ? 'text-gray-500' : 'text-gray-400'}`}>(Selecciona una persona primero)</span>}
                </h2>
                <div className="grid grid-cols-2 gap-4">
                    <div className="col-span-2">
                        <label className={labelCls(modoOscuro)}>Cargo *</label>
                        <select className={inputCls(modoOscuro)} value={form.id_Cargo} onChange={e => setForm(f => ({ ...f, id_Cargo: e.target.value }))}>
                            <option value="">Seleccionar cargo...</option>
                            {cargos.map(c => <option key={c.id_Catalogo} value={c.id_Catalogo}>{c.nombre_Catalogo}</option>)}
                        </select>
                    </div>
                    <div>
                        <label className={labelCls(modoOscuro)}>Fecha de Contratación *</label>
                        <input type="date" className={inputCls(modoOscuro)}
                            value={form.fecha_Contratacion} onChange={e => setForm(f => ({ ...f, fecha_Contratacion: e.target.value }))} />
                    </div>
                    <div>
                        <label className={labelCls(modoOscuro)}>Fecha de Despido</label>
                        <input type="date" className={inputCls(modoOscuro)}
                            value={form.fecha_Despido} onChange={e => setForm(f => ({ ...f, fecha_Despido: e.target.value }))} />
                    </div>
                </div>
            </div>

            <div className="flex justify-end">
                <button onClick={guardar} disabled={guardando || !personaSeleccionada}
                    className="flex items-center gap-2 px-6 py-2.5 bg-cyan-600 text-white rounded-lg text-sm font-semibold hover:bg-cyan-500 transition disabled:opacity-60">
                    {guardando ? <LoaderCircle size={16} className="animate-spin" /> : <Save size={16} />} Guardar Empleado
                </button>
            </div>
        </div>
    );
};

export default CrearEmpleado;