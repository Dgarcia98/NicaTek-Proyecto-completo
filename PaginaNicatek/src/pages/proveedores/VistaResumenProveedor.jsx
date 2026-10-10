import { useEffect, useState } from 'react';
import { useParams, useOutletContext } from 'react-router-dom';
import { ArrowLeft, Truck, User, Edit, X, Save, LoaderCircle } from 'lucide-react';
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

const VistaResumenProveedor = () => {
    const { id } = useParams();
    const { modoOscuro } = useOutletContext();

    const [cargando, setCargando] = useState(true);
    const [proveedor, setProveedor] = useState(null);
    const [editando, setEditando] = useState(false);
    const [guardando, setGuardando] = useState(false);
    const [form, setForm] = useState({});

    const cargarDatos = async () => {
        try {
            setCargando(true);
            const res = await api.get(`/Proveedores/obtener/${id}`);
            const p = res.data;
            setProveedor(p);
            setForm({
                fechaRegistro: p.fechaRegistro ? p.fechaRegistro.split('T')[0] : '',
                fecha_Terminacion_Contrato: p.fecha_Terminacion_Contrato ? p.fecha_Terminacion_Contrato.split('T')[0] : '',
                id_Estado: p.id_Estado || 1,
            });
        } catch (err) {
            console.error(err);
            Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudieron cargar los datos.', confirmButtonColor: '#0891b2' });
        } finally {
            setCargando(false);
        }
    };

    useEffect(() => { cargarDatos(); }, [id]);

    const guardar = async () => {
        try {
            setGuardando(true);
            await api.put(`/Proveedores/editar/${id}`, {
                Id_Proveedor: parseInt(id),
                Id_Persona: proveedor.id_Persona,
                Cedula: proveedor.cedula || '',
                Primer_Nombre: proveedor.primer_Nombre || '',
                Segundo_Nombre: proveedor.segundo_Nombre || '',
                Primer_Apellido: proveedor.primer_Apellido || '',
                Segundo_Apellido: proveedor.segundo_Apellido || '',
                Nombre_Completo: proveedor.nombre_Completo || '',
                Id_Estado: parseInt(form.id_Estado),
                Estado: form.id_Estado == 1 ? 'Activo' : 'Inactivo',
                FechaRegistro: new Date(form.fechaRegistro).toISOString(),
                Fecha_Terminacion_Contrato: form.fecha_Terminacion_Contrato
                    ? new Date(form.fecha_Terminacion_Contrato).toISOString()
                    : null,
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

    if (cargando) return (
        <div className="flex justify-center items-center py-32">
            <LoaderCircle size={36} className="animate-spin text-cyan-500" />
        </div>
    );

    if (!proveedor) return (
        <div className="p-6 text-center text-red-500">No se encontró el proveedor.</div>
    );

    const esActivo = proveedor.id_Estado === 1;
    const nombreCompleto = proveedor.nombre_Completo ||
        [proveedor.primer_Nombre, proveedor.segundo_Nombre, proveedor.primer_Apellido, proveedor.segundo_Apellido]
            .filter(Boolean).join(' ');

    return (
        <div className={`p-6 max-w-3xl mx-auto ${modoOscuro ? 'text-gray-100' : 'text-gray-800'}`}>

            {/* Header */}
            <div className="flex items-center justify-between mb-6">
                <div className="flex items-center gap-3">
                    <Truck size={26} className="text-cyan-500" />
                    <div>
                        <h1 className="text-xl font-bold">{nombreCompleto}</h1>
                        <p className={`text-xs ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>
                            ID #{proveedor.id_Proveedor} · {proveedor.cedula}
                        </p>
                    </div>
                </div>
                <button onClick={() => window.location.href = '/usuarios/proveedores'}
                    className={`flex items-center gap-1 px-3 py-2 rounded-lg border text-sm transition ${modoOscuro ? 'border-gray-600 text-gray-300 hover:bg-gray-700' : 'border-gray-300 text-gray-600 hover:bg-gray-100'}`}>
                    <ArrowLeft size={16} /> Volver
                </button>
            </div>

            {/* ── Datos del Proveedor ── */}
            <Seccion icono={<Truck size={16} className="text-cyan-500" />} titulo="Datos del Proveedor" modoOscuro={modoOscuro}>
                {editando ? (
                    <>
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
                                <p className="font-medium">{nombreCompleto}</p>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Cédula</span>
                                <p className="font-medium">{proveedor.cedula || '—'}</p>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Fecha de Registro</span>
                                <p className="font-medium">
                                    {proveedor.fechaRegistro ? new Date(proveedor.fechaRegistro).toLocaleDateString() : '—'}
                                </p>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Fecha Fin de Contrato</span>
                                <p className="font-medium">
                                    {proveedor.fecha_Terminacion_Contrato ? new Date(proveedor.fecha_Terminacion_Contrato).toLocaleDateString() : '—'}
                                </p>
                            </div>
                            <div>
                                <span className={labelCls(modoOscuro)}>Estado</span>
                                <span className={`inline-block mt-1 px-2 py-0.5 text-xs rounded-full border font-semibold ${esActivo ? 'bg-green-100 text-green-800 border-green-200' : 'bg-red-100 text-red-800 border-red-200'}`}>
                                    {esActivo ? 'Activo' : 'Inactivo'}
                                </span>
                            </div>
                        </div>
                        <div className="flex justify-end mt-4">
                            <button onClick={() => setEditando(true)}
                                className="flex items-center gap-1 px-3 py-1.5 bg-cyan-600 text-white rounded-lg text-sm hover:bg-cyan-500 transition">
                                <Edit size={14} /> Editar
                            </button>
                        </div>
                    </>
                )}
            </Seccion>

            {/* ── Persona vinculada ── */}
            <Seccion icono={<User size={16} className="text-cyan-500" />} titulo="Persona Vinculada" modoOscuro={modoOscuro}>
                <div className="flex items-center justify-between">
                    <div className="grid grid-cols-2 gap-3 text-sm flex-1">
                        <div>
                            <span className={labelCls(modoOscuro)}>Nombre completo</span>
                            <p className="font-medium">{nombreCompleto}</p>
                        </div>
                        <div>
                            <span className={labelCls(modoOscuro)}>Cédula / DNI</span>
                            <p className="font-medium">{proveedor.cedula || '—'}</p>
                        </div>
                    </div>
                    <button
                        onClick={() => window.location.href = `/usuarios/personas/resumen/${proveedor.id_Persona}`}
                        className="flex items-center gap-1 px-3 py-1.5 border border-cyan-500 text-cyan-500 rounded-lg text-sm hover:bg-cyan-50 transition ml-4">
                        <User size={14} /> Ver persona
                    </button>
                </div>
            </Seccion>

        </div>
    );
};

export default VistaResumenProveedor;