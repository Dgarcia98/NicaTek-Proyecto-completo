import { useEffect, useState } from "react";
import { useOutletContext, useNavigate } from "react-router-dom";
import { Plus, DollarSign, Eye, Search, X, ArrowUp, ArrowDown, Ban } from "lucide-react";
import Swal from 'sweetalert2';
import TablaConTarjetas from "../../components/TablaConTarjetas";
import FormularioAgregarVenta from "./FormularioAgregarVenta";
import api, { mensajeDeError } from "../../api/axios.Config";
import { tienePermiso } from "../../utils/Permisos";

const EstadoBadge = ({ estado }) => {
    const cfg = {
        'Completado': 'bg-green-100 text-green-800 border-green-200',
        'Borrador':   'bg-blue-100  text-blue-800  border-blue-200',
        'Anulado':    'bg-red-100   text-red-800   border-red-200',
    };
    return (
        <span className={`px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full border ${cfg[estado] || 'bg-gray-100 text-gray-800 border-gray-200'}`}>
            {estado || '—'}
        </span>
    );
};

const Ventas = () => {
    const { modoOscuro } = useOutletContext();
    const navigate = useNavigate();

    const puedeVender = tienePermiso('POS_SALE');
    const puedeAnular = tienePermiso('INV_DELETE');

    const [ventas,        setVentas]        = useState([]);
    const [cargando,      setCargando]      = useState(true);
    const [vista,         setVista]         = useState("lista");
    const [filtro,        setFiltro]        = useState("");
    const [ordenFactura,  setOrdenFactura]  = useState("desc");

    const obtenerDatos = async (terminoFiltro = "") => {
        setCargando(true);
        try {
            const endpoint = terminoFiltro.trim()
                ? `/Venta/ListarPorFiltro/${encodeURIComponent(terminoFiltro.trim())}`
                : "/Venta/ListarVentas";
            const res  = await api.get(endpoint);
            const data = res.data;
            let arr    = Array.isArray(data) ? data : (data?.exito && Array.isArray(data.datos) ? data.datos : []);
            setVentas(arr.sort((a, b) => ordenFactura === "desc" ? b.id_Venta - a.id_Venta : a.id_Venta - b.id_Venta));
        } catch (error) {
            console.error("Error al obtener ventas:", error.message);
            setVentas([]);
        } finally { setCargando(false); }
    };

    useEffect(() => {
        if (vista !== "lista") return;
        const t = setTimeout(() => obtenerDatos(filtro), 500);
        return () => clearTimeout(t);
    }, [vista, filtro]);

    const toggleOrdenFactura = () => {
        const nuevo = ordenFactura === "desc" ? "asc" : "desc";
        setOrdenFactura(nuevo);
        setVentas(prev => [...prev].sort((a, b) => nuevo === "desc" ? b.id_Venta - a.id_Venta : a.id_Venta - b.id_Venta));
    };

    const handleAnularVenta = async (idVenta) => {
        if (!puedeAnular) return;
        // El idModificador ya no se envia: la API lo toma del token. Antes
        // se mandaba `sessionStorage.getItem('idUsuario') || 1`, asi que un
        // fallo silencioso atribuia la anulacion al usuario 1 (el admin).
        const confirmacion  = await Swal.fire({
            title: '¿Anular Venta?',
            text: `¿Estás seguro de que deseas anular la factura #${idVenta}?`,
            icon: 'warning', showCancelButton: true,
            confirmButtonColor: '#d33', cancelButtonColor: '#3085d6',
            confirmButtonText: 'Sí, anular', cancelButtonText: 'Cancelar'
        });
        if (!confirmacion.isConfirmed) return;
        try {
            await api.delete(`/Venta/EliminarVenta/${idVenta}`);
            Swal.fire('¡Anulada!', 'La venta ha sido anulada y el inventario restaurado.', 'success');
            obtenerDatos(filtro);
        } catch (error) {
            // error.response.data es un objeto: pasarlo directo mostraba
            // "[object Object]" en el modal.
            Swal.fire('Error', mensajeDeError(error, 'Hubo un problema al anular la venta.'), 'error');
        }
    };

    const filasFormateadas = ventas.map((venta) => {
        const numeroFactura   = `FAC-${String(venta.id_Venta).padStart(6, "0")}`;
        const metodoPagoTexto = venta.metodo_Pago || venta.metodoPago || venta.nombreMetodoPago || "N/A";
        const estaAnulada     = venta.id_Estado === 3 || venta.estado === "Anulado";
        const puedeAnularFila = !estaAnulada && puedeAnular;

        let tituloAnular = 'Anular venta';
        if (!puedeAnular)      tituloAnular = 'Sin permiso';
        else if (estaAnulada)  tituloAnular = 'Ya está anulada';

        return [
            numeroFactura,
            venta.cliente,
            new Date(venta.fecha).toLocaleDateString(),
            metodoPagoTexto,
            `$${venta.total !== undefined ? venta.total.toFixed(2) : "0.00"}`,
            <EstadoBadge key={`est-${venta.id_Venta}`} estado={venta.estado} />,
            <div key={venta.id_Venta} className="flex space-x-2">
                {/* Botones de solo icono: sin aria-label son invisibles
                    para un lector de pantalla. */}
                <button type="button" onClick={() => navigate(`/ventas/resumen/${venta.id_Venta}`)}
                    className="text-gray-500 hover:text-blue-600 transition-colors"
                    title="Ver resumen" aria-label={`Ver resumen de la factura ${numeroFactura}`}>
                    <Eye size={20} aria-hidden="true" />
                </button>
                <button
                    type="button"
                    onClick={() => puedeAnularFila && handleAnularVenta(venta.id_Venta)}
                    disabled={!puedeAnularFila}
                    title={tituloAnular}
                    aria-label={`${tituloAnular}: factura ${numeroFactura}`}
                    className={`transition-colors ${!puedeAnularFila ? 'text-gray-300 cursor-not-allowed' : 'text-red-500 hover:text-red-700'}`}>
                    <Ban size={20} aria-hidden="true" />
                </button>
            </div>,
        ];
    });

    const encabezadosDinamicos = [
        <div key="header-factura" onClick={toggleOrdenFactura}
            className="flex items-center cursor-pointer hover:text-blue-500 transition-colors select-none">
            Factura {ordenFactura === "desc" ? <ArrowDown size={16} className="ml-1" /> : <ArrowUp size={16} className="ml-1" />}
        </div>,
        "Cliente", "Fecha", "Método Pago", "Total", "Estado", "Acciones",
    ];

    if (vista === "agregar") {
        return <FormularioAgregarVenta
            onSuccess={() => { setVista("lista"); setFiltro(""); }}
            onCancel={() => { setVista("lista"); setFiltro(""); }} />;
    }

    return (
        <div className={`p-6 ${modoOscuro ? "bg-gray-900 text-gray-100" : "bg-transparent text-gray-800"}`}>
            <div className="flex justify-between items-center mb-6">
                <h1 className={`text-3xl font-bold flex items-center ${modoOscuro ? "text-white" : "text-black"}`}>
                    <DollarSign className="mr-3 text-blue-500" size={28} /> Ventas
                </h1>
                <button onClick={() => puedeVender && setVista("agregar")}
                    disabled={!puedeVender}
                    className={`flex items-center px-4 py-2 font-semibold rounded-lg shadow-md transition-all transform ${
                        puedeVender ? 'bg-blue-600 text-white hover:bg-blue-500 hover:scale-105' : 'bg-gray-300 text-gray-500 cursor-not-allowed'
                    }`}>
                    <Plus size={20} className="mr-2" /> Agregar Venta
                </button>
            </div>

            <div className="mb-6 relative">
                <input type="text" placeholder="Buscar por cliente, ID o estado..."
                    value={filtro} onChange={(e) => setFiltro(e.target.value)}
                    className={`w-full pl-10 pr-10 py-3 border rounded-xl shadow-sm focus:ring-2 focus:ring-blue-500 focus:outline-none transition-all ${
                        modoOscuro ? "bg-gray-800 border-gray-700 text-gray-100 placeholder-gray-400" : "bg-white border-gray-200 text-gray-900 placeholder-gray-400"
                    }`} />
                <Search className={`absolute left-3 top-1/2 -translate-y-1/2 ${modoOscuro ? "text-gray-400" : "text-gray-500"}`} size={20} />
                {filtro && <button onClick={() => setFiltro("")} className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-red-500"><X size={20} /></button>}
            </div>

            {cargando ? (
                <div className="flex flex-col justify-center items-center h-48 space-y-4">
                    <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-blue-500"></div>
                    <p className={`${modoOscuro ? "text-gray-400" : "text-gray-600"} animate-pulse font-medium`}>Cargando ventas...</p>
                </div>
            ) : ventas.length === 0 ? (
                <div className="text-center py-20 text-gray-500">No se encontraron ventas.</div>
            ) : (
                <TablaConTarjetas encabezados={encabezadosDinamicos} filas={filasFormateadas} modoOscuro={modoOscuro} />
            )}
        </div>
    );
};

export default Ventas;