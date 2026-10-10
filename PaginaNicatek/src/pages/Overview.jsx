import { useEffect, useState, useMemo } from 'react';
import { useOutletContext, useNavigate } from 'react-router-dom';
import {
    ShoppingBag, DollarSign, TrendingUp, TrendingDown, LoaderCircle,
    ArrowRight, Sun, Cloud, CloudRain, CloudSnow, Wind, Droplets,
    CloudLightning, CloudDrizzle, ShoppingCart, Receipt,
} from 'lucide-react';
import {
    Chart as ChartJS, CategoryScale, LinearScale, BarElement,
    ArcElement, Title, Tooltip, Legend,
} from 'chart.js';
import { Bar, Doughnut } from 'react-chartjs-2';
import api from '../api/axios.Config';

ChartJS.register(CategoryScale, LinearScale, BarElement, ArcElement, Title, Tooltip, Legend);

const LAT = 12.1364;
const LON = -86.2514;

const IconoClima = ({ code, size = 28 }) => {
    if (code === 0)  return <Sun          size={size} className="text-yellow-400"   />;
    if (code <= 2)   return <Cloud        size={size} className="text-gray-400"     />;
    if (code <= 3)   return <Cloud        size={size} className="text-gray-500"     />;
    if (code <= 49)  return <CloudDrizzle size={size} className="text-blue-300"     />;
    if (code <= 67)  return <CloudRain    size={size} className="text-blue-500"     />;
    if (code <= 77)  return <CloudSnow    size={size} className="text-blue-200"     />;
    if (code <= 82)  return <CloudRain    size={size} className="text-blue-400"     />;
    if (code <= 99)  return <CloudLightning size={size} className="text-purple-500" />;
    return <Wind size={size} className="text-gray-400" />;
};
const descClima = (c) => {
    if (c === 0) return 'Despejado';       if (c <= 2)  return 'Parcialmente nublado';
    if (c <= 3)  return 'Nublado';         if (c <= 49) return 'Llovizna';
    if (c <= 67) return 'Lluvia';          if (c <= 77) return 'Nieve';
    if (c <= 82) return 'Lluvia intensa';  if (c <= 99) return 'Tormenta';
    return 'Variable';
};

const StatCard = ({ titulo, valor, sub, icono, color, modoOscuro, onClick, tendencia }) => {
    const sube   = tendencia > 0;
    const neutro = tendencia === 0 || tendencia == null;
    return (
        <div onClick={onClick}
            className={`rounded-2xl border p-5 flex items-center gap-4 shadow-sm transition hover:shadow-md
                ${onClick ? 'cursor-pointer' : ''}
                ${modoOscuro ? 'bg-gray-800 border-gray-700' : 'bg-white border-gray-200'}`}>
            <div className={`p-3 rounded-xl flex-shrink-0 ${color}`}>{icono}</div>
            <div className="flex-1 min-w-0">
                <p className={`text-xs font-medium uppercase tracking-wider truncate
                    ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>{titulo}</p>
                <p className="text-2xl font-bold mt-0.5 truncate">{valor}</p>
                {sub && <p className={`text-xs mt-0.5 ${modoOscuro ? 'text-gray-500' : 'text-gray-400'}`}>{sub}</p>}
                {!neutro && (
                    <p className={`text-xs flex items-center gap-0.5 mt-0.5 font-medium
                        ${sube ? 'text-green-500' : 'text-red-400'}`}>
                        {sube ? <TrendingUp size={12}/> : <TrendingDown size={12}/>}
                        {Math.abs(tendencia).toFixed(1)}% vs mes anterior
                    </p>
                )}
            </div>
        </div>
    );
};

const ConteoCard = ({ titulo, total, activas, anuladas, icono, colorActivo, modoOscuro, onClick }) => (
    <div onClick={onClick}
        className={`rounded-2xl border p-5 shadow-sm transition hover:shadow-md
            ${onClick ? 'cursor-pointer' : ''}
            ${modoOscuro ? 'bg-gray-800 border-gray-700' : 'bg-white border-gray-200'}`}>
        <div className="flex items-center gap-2 mb-3">
            {icono}
            <p className={`text-xs font-semibold uppercase tracking-wider
                ${modoOscuro ? 'text-gray-400' : 'text-gray-500'}`}>{titulo}</p>
        </div>
        <p className="text-3xl font-bold mb-3">{total}</p>
        <div className="flex gap-2">
            <div className={`flex-1 rounded-xl px-3 py-2 ${modoOscuro ? 'bg-gray-700' : 'bg-gray-50'}`}>
                <p className={`text-[10px] uppercase font-bold ${colorActivo}`}>Activas</p>
                <p className="text-lg font-bold">{activas}</p>
            </div>
            <div className={`flex-1 rounded-xl px-3 py-2 ${modoOscuro ? 'bg-gray-700' : 'bg-gray-50'}`}>
                <p className="text-[10px] uppercase font-bold text-red-400">Anuladas</p>
                <p className="text-lg font-bold">{anuladas}</p>
            </div>
        </div>
    </div>
);

const MESES        = ['Ene','Feb','Mar','Abr','May','Jun','Jul','Ago','Sep','Oct','Nov','Dic'];
const DONUT_COLORS = ['#06b6d4','#3b82f6','#8b5cf6','#10b981','#f59e0b','#ef4444','#ec4899','#14b8a6'];

const Overview = () => {
    const { modoOscuro } = useOutletContext();
    const navigate       = useNavigate();

    const [cargando,      setCargando]      = useState(true);
    const [empresa,       setEmpresa]       = useState(null);
    const [clima,         setClima]         = useState(null);
    const [cargandoClima, setCargandoClima] = useState(true);
    const [rango,         setRango]         = useState('mes');

    // Metricas ya agregadas por SQL (sp_Dashboard_Resumen).
    //
    // Antes esta pantalla pedia /Venta/ListarVentas, /Compra/ListarCompra y
    // /DetalleVenta/ListarDetalleVentas COMPLETOS y sumaba todo aqui con
    // filter/reduce. Con 10.000 ventas y 40.000 detalles eran varios MB por
    // cada carga del dashboard, y el calculo del top de productos recorria
    // todo en el hilo principal. Ahora bajan ~20 filas.
    const [resumen, setResumen] = useState(null);

    useEffect(() => {
        (async () => {
            try {
                setCargandoClima(true);
                const url = `https://api.open-meteo.com/v1/forecast?latitude=${LAT}&longitude=${LON}`
                    + `&current=temperature_2m,relative_humidity_2m,weathercode,windspeed_10m`
                    + `&daily=temperature_2m_max,temperature_2m_min,weathercode`
                    + `&timezone=America%2FManagua&forecast_days=4`;
                setClima(await (await fetch(url)).json());
            } catch (e) { console.warn('Clima:', e.message); }
            finally { setCargandoClima(false); }
        })();
    }, []);

    const ahora = new Date();

    // El rango se resuelve en el servidor: solo se recalculan las fechas y
    // se vuelve a pedir el resumen.
    const { desde, hasta } = useMemo(() => {
        const d = new Date();
        if      (rango === 'semana') d.setDate(d.getDate() - 7);
        else if (rango === 'mes')    d.setDate(1);
        else if (rango === 'anio')   { d.setMonth(0); d.setDate(1); }
        else                         d.setFullYear(2000);
        d.setHours(0, 0, 0, 0);
        return { desde: d.toISOString(), hasta: new Date().toISOString() };
    }, [rango]);

    useEffect(() => {
        let cancelado = false;
        (async () => {
            try {
                setCargando(true);
                const [resE, resR] = await Promise.allSettled([
                    api.get('/Configuracion/Obtener'),
                    api.get('/Dashboard/Resumen', { params: { desde, hasta } }),
                ]);
                if (cancelado) return;
                if (resE.status === 'fulfilled') setEmpresa(resE.value.data);
                if (resR.status === 'fulfilled') setResumen(resR.value.data);
                else console.warn('Dashboard:', resR.reason?.message);
            } finally {
                if (!cancelado) setCargando(false);
            }
        })();
        return () => { cancelado = true; };
    }, [desde, hasta]);

    // Memorizados porque `?? []` crea un arreglo nuevo en cada render y
    // eso invalidaria los useMemo que dependen de ellos mas abajo.
    const totales        = resumen?.totales ?? {};
    const serieMensual   = useMemo(() => resumen?.serieMensual  ?? [], [resumen]);
    const topProductos   = useMemo(() => resumen?.topProductos  ?? [], [resumen]);
    const ultimasVentas  = resumen?.ultimasVentas  ?? [];
    const ultimasCompras = resumen?.ultimasCompras ?? [];

    const totalVentas  = Number(totales.total_Ventas  ?? 0);
    const totalCompras = Number(totales.total_Compras ?? 0);
    const cantVentas   = Number(totales.cantidad_Ventas ?? 0);
    const factPromedio = cantVentas > 0 ? totalVentas / cantVentas : 0;

    const ventasAnuladas   = Number(totales.ventas_Anuladas ?? 0);
    const comprasAnuladas  = Number(totales.compras_Anuladas ?? 0);
    const ventasTotales    = Number(totales.ventas_Totales_Historico ?? 0);
    const comprasTotales   = Number(totales.compras_Totales_Historico ?? 0);

    // La tendencia se deriva de la serie mensual que ya vino agregada:
    // ultimo mes contra el anterior.
    const calcTend = (campo) => {
        if (serieMensual.length < 2) return null;
        const act = Number(serieMensual[serieMensual.length - 1]?.[campo] ?? 0);
        const ant = Number(serieMensual[serieMensual.length - 2]?.[campo] ?? 0);
        return ant === 0 ? null : ((act - ant) / ant) * 100;
    };
    const tendVentas  = calcTend('ventas');
    const tendCompras = calcTend('compras');

    const moneda = empresa?.moneda_Simbolo || 'C$';
    const fmt    = (n) => Number(n).toLocaleString('es-NI', { minimumFractionDigits:2, maximumFractionDigits:2 });

    const datosBarras = useMemo(() => ({
        labels: serieMensual.map(m => `${MESES[(m.mes ?? 1) - 1]} ${String(m.anio ?? '').slice(2)}`),
        dV:     serieMensual.map(m => Number(m.ventas  ?? 0)),
        dC:     serieMensual.map(m => Number(m.compras ?? 0)),
    }), [serieMensual]);

    const datosDonaProductos = useMemo(() => ({
        labels: topProductos.map(p => p.producto || 'Sin nombre'),
        data:   topProductos.map(p => Number(p.unidades ?? 0)),
    }), [topProductos]);

    const gridC = modoOscuro ? 'rgba(255,255,255,0.07)' : 'rgba(0,0,0,0.06)';
    const tickC = modoOscuro ? '#9ca3af' : '#6b7280';
    const tipBg = modoOscuro ? '#1f2937' : '#fff';
    const tipTx = modoOscuro ? '#f3f4f6' : '#111827';

    const optsBar = {
        responsive:true, maintainAspectRatio:false,
        plugins: {
            legend: { labels:{ color:tickC, boxWidth:12, font:{size:11} } },
            tooltip:{ backgroundColor:tipBg, titleColor:tipTx, bodyColor:tipTx,
                callbacks:{ label: ctx => ` ${moneda}${fmt(ctx.parsed.y)}` } },
        },
        scales:{
            x:{ ticks:{color:tickC, font:{size:11}}, grid:{color:gridC} },
            y:{ ticks:{color:tickC, font:{size:11}, callback: v=>`${moneda}${Number(v).toLocaleString('es-NI')}`}, grid:{color:gridC} },
        },
    };
    const optsDona = (pos='bottom') => ({
        responsive:true, maintainAspectRatio:false, cutout:'65%',
        plugins:{
            legend:{ position:pos, labels:{color:tickC, boxWidth:12, font:{size:11}} },
            tooltip:{ backgroundColor:tipBg, titleColor:tipTx, bodyColor:tipTx },
        },
    });

    const cardBg = modoOscuro ? 'bg-gray-800 border-gray-700' : 'bg-white border-gray-200';

    if (cargando) return (
        <div className="flex justify-center items-center py-32">
            <LoaderCircle size={36} className="animate-spin text-cyan-500"/>
        </div>
    );

    return (
        <div className={`space-y-6 ${modoOscuro ? 'text-white' : 'text-gray-800'}`}>

            {/* Bienvenida + Clima */}
            <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-4">
                <div>
                    <h1 className="text-2xl font-bold">
                        {empresa?.nombre_Empresa ? `Bienvenido — ${empresa.nombre_Empresa}` : 'Dashboard'}
                    </h1>
                    <p className={`text-sm mt-0.5 ${modoOscuro ? 'text-gray-400':'text-gray-500'}`}>
                        {ahora.toLocaleDateString('es-ES',{weekday:'long',year:'numeric',month:'long',day:'numeric'})}
                    </p>
                </div>
                <div className={`rounded-2xl border px-5 py-3 flex items-center gap-4 shadow-sm ${cardBg}`}>
                    {cargandoClima ? (
                        <div className="flex items-center gap-2">
                            <LoaderCircle size={16} className="animate-spin text-cyan-500"/>
                            <span className={`text-xs ${modoOscuro ? 'text-gray-400':'text-gray-500'}`}>Cargando clima…</span>
                        </div>
                    ) : clima?.current ? (
                        <>
                            <IconoClima code={clima.current.weathercode} size={32}/>
                            <div>
                                <p className="text-xl font-bold">{Math.round(clima.current.temperature_2m)}°C</p>
                                <p className={`text-xs font-medium ${modoOscuro ? 'text-gray-300':'text-gray-600'}`}>
                                    {descClima(clima.current.weathercode)} · Managua
                                </p>
                                <div className={`flex items-center gap-3 mt-1 text-xs ${modoOscuro ? 'text-gray-400':'text-gray-500'}`}>
                                    <span className="flex items-center gap-0.5"><Droplets size={11}/>{clima.current.relative_humidity_2m}%</span>
                                    <span className="flex items-center gap-0.5"><Wind size={11}/>{Math.round(clima.current.windspeed_10m)} km/h</span>
                                </div>
                            </div>
                            <div className={`flex gap-3 ml-2 pl-3 border-l ${modoOscuro ? 'border-gray-700':'border-gray-100'}`}>
                                {clima.daily?.time?.slice(1,4).map((dia,i) => (
                                    <div key={dia} className="flex flex-col items-center gap-0.5">
                                        <span className={`text-[10px] ${modoOscuro ? 'text-gray-400':'text-gray-500'}`}>
                                            {new Date(dia+'T12:00:00').toLocaleDateString('es-NI',{weekday:'short'})}
                                        </span>
                                        <IconoClima code={clima.daily.weathercode[i+1]} size={14}/>
                                        <span className="text-[10px] font-semibold">{Math.round(clima.daily.temperature_2m_max[i+1])}°</span>
                                    </div>
                                ))}
                            </div>
                        </>
                    ) : (
                        <p className={`text-xs ${modoOscuro ? 'text-gray-400':'text-gray-500'}`}>Clima no disponible</p>
                    )}
                </div>
            </div>

            {/* Filtro período */}
            <div className="flex items-center gap-2 flex-wrap">
                <span className={`text-xs font-semibold uppercase tracking-wider mr-1 ${modoOscuro ? 'text-gray-400':'text-gray-500'}`}>Período:</span>
                {[{key:'semana',label:'Esta semana'},{key:'mes',label:'Este mes'},{key:'anio',label:'Este año'},{key:'todo',label:'Todo'}].map(({key,label}) => (
                    <button key={key} onClick={() => setRango(key)}
                        className={`px-3 py-1 rounded-full text-xs font-medium transition-colors ${
                            rango===key ? 'bg-cyan-600 text-white shadow-sm'
                                : modoOscuro ? 'bg-gray-700 text-gray-300 hover:bg-gray-600'
                                             : 'bg-gray-100 text-gray-600 hover:bg-gray-200'}`}>
                        {label}
                    </button>
                ))}
            </div>

            {/* Stats montos */}
            <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                <StatCard titulo="Total Ventas"    valor={`${moneda}${fmt(totalVentas)}`}
                    icono={<DollarSign size={20} className="text-blue-600"/>}   color="bg-blue-100"
                    modoOscuro={modoOscuro} onClick={() => navigate('/ventas')} tendencia={tendVentas}/>
                <StatCard titulo="Total Compras"   valor={`${moneda}${fmt(totalCompras)}`}
                    icono={<ShoppingBag size={20} className="text-cyan-600"/>}  color="bg-cyan-100"
                    modoOscuro={modoOscuro} onClick={() => navigate('/compras')} tendencia={tendCompras}/>
                <StatCard titulo="Factura Promedio" valor={`${moneda}${fmt(factPromedio)}`}
                    sub="Por venta activa en el período"
                    icono={<TrendingUp size={20} className="text-purple-600"/>} color="bg-purple-100"
                    modoOscuro={modoOscuro}/>
            </div>

            {/* Barras + Dona balance */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <div className={`lg:col-span-2 rounded-2xl border p-5 shadow-sm ${cardBg}`}>
                    <h2 className="text-sm font-bold mb-4">Ventas vs Compras — últimos 6 meses</h2>
                    <div className="h-56">
                        <Bar data={{ labels:datosBarras.labels, datasets:[
                            { label:'Ventas',  data:datosBarras.dV, backgroundColor:'rgba(59,130,246,0.85)', borderRadius:6 },
                            { label:'Compras', data:datosBarras.dC, backgroundColor:'rgba(6,182,212,0.85)',  borderRadius:6 },
                        ]}} options={optsBar}/>
                    </div>
                </div>
                <div className={`rounded-2xl border p-5 shadow-sm ${cardBg}`}>
                    <h2 className="text-sm font-bold mb-1">Balance del período</h2>
                    <p className={`text-xs mb-3 ${modoOscuro ? 'text-gray-500':'text-gray-400'}`}>Ventas vs Compras (monto)</p>
                    <div className="h-52 flex items-center justify-center">
                        {totalVentas+totalCompras > 0 ? (
                            <Doughnut data={{ labels:['Ventas','Compras'], datasets:[{
                                data:[totalVentas,totalCompras],
                                backgroundColor:['rgba(59,130,246,0.85)','rgba(6,182,212,0.85)'],
                                borderWidth:0, hoverOffset:6 }]}}
                            options={optsDona('bottom')}/>
                        ) : <p className={`text-xs ${modoOscuro ? 'text-gray-500':'text-gray-400'}`}>Sin datos</p>}
                    </div>
                </div>
            </div>

            {/* Dona productos + Últimas ventas + Últimas compras */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <div className={`rounded-2xl border p-5 shadow-sm ${cardBg}`}>
                    <h2 className="text-sm font-bold mb-1">Productos más vendidos</h2>
                    <p className={`text-xs mb-3 ${modoOscuro ? 'text-gray-500':'text-gray-400'}`}>Por unidades (activas)</p>
                    <div className="h-52 flex items-center justify-center">
                        {datosDonaProductos.data.length > 0 ? (
                            <Doughnut data={{ labels:datosDonaProductos.labels, datasets:[{
                                data:datosDonaProductos.data, backgroundColor:DONUT_COLORS,
                                borderWidth:0, hoverOffset:6 }]}}
                            options={optsDona('bottom')}/>
                        ) : <p className={`text-xs text-center ${modoOscuro ? 'text-gray-500':'text-gray-400'}`}>Sin detalles disponibles</p>}
                    </div>
                </div>

                <div className={`rounded-2xl border p-5 shadow-sm ${cardBg}`}>
                    <div className="flex items-center justify-between mb-3">
                        <h2 className="text-sm font-bold flex items-center gap-2"><DollarSign size={15} className="text-blue-500"/> Últimas Ventas</h2>
                        <button onClick={() => navigate('/ventas')} className="text-xs text-cyan-500 hover:underline flex items-center gap-1">Ver todas <ArrowRight size={11}/></button>
                    </div>
                    <div className="space-y-2">
                        {ultimasVentas.map(v => (
                            <div key={v.id_Venta} className={`flex items-center justify-between px-3 py-2 rounded-lg ${modoOscuro ? 'bg-gray-700':'bg-gray-50'}`}>
                                <div className="min-w-0">
                                    {/* El numero de factura lo genera la base (columna
                                        calculada), ya no se arma en el cliente. */}
                                    <p className="text-sm font-medium truncate">{v.numero_Factura || `FAC-${String(v.id_Venta).padStart(6,'0')}`}</p>
                                    <p className={`text-xs truncate ${modoOscuro ? 'text-gray-400':'text-gray-500'}`}>{v.cliente||'—'}</p>
                                </div>
                                <div className="text-right flex-shrink-0 ml-2">
                                    <p className="text-sm font-bold text-blue-500">{moneda}{fmt(v.total)}</p>
                                    <p className={`text-xs ${modoOscuro ? 'text-gray-400':'text-gray-500'}`}>{new Date(v.fecha).toLocaleDateString('es-NI')}</p>
                                </div>
                            </div>
                        ))}
                        {ultimasVentas.length===0 && <p className={`text-xs text-center py-4 ${modoOscuro ? 'text-gray-500':'text-gray-400'}`}>Sin ventas.</p>}
                    </div>
                </div>

                <div className={`rounded-2xl border p-5 shadow-sm ${cardBg}`}>
                    <div className="flex items-center justify-between mb-3">
                        <h2 className="text-sm font-bold flex items-center gap-2"><ShoppingBag size={15} className="text-cyan-500"/> Últimas Compras</h2>
                        <button onClick={() => navigate('/compras')} className="text-xs text-cyan-500 hover:underline flex items-center gap-1">Ver todas <ArrowRight size={11}/></button>
                    </div>
                    <div className="space-y-2">
                        {ultimasCompras.map(c => (
                            <div key={c.id_Compra} className={`flex items-center justify-between px-3 py-2 rounded-lg ${modoOscuro ? 'bg-gray-700':'bg-gray-50'}`}>
                                <div className="min-w-0">
                                    <p className="text-sm font-medium truncate">#{c.id_Compra} {c.numero_Factura_Proveedor}</p>
                                    <p className={`text-xs truncate ${modoOscuro ? 'text-gray-400':'text-gray-500'}`}>{c.proveedor||'—'}</p>
                                </div>
                                <div className="text-right flex-shrink-0 ml-2">
                                    <p className="text-sm font-bold text-cyan-500">{moneda}{fmt(c.total_Compra)}</p>
                                    <p className={`text-xs ${modoOscuro ? 'text-gray-400':'text-gray-500'}`}>{new Date(c.fecha_Compra).toLocaleDateString('es-NI')}</p>
                                </div>
                            </div>
                        ))}
                        {ultimasCompras.length===0 && <p className={`text-xs text-center py-4 ${modoOscuro ? 'text-gray-500':'text-gray-400'}`}>Sin compras.</p>}
                    </div>
                </div>
            </div>

            {/* ConteoCards al final */}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <ConteoCard titulo="Ventas realizadas"  total={ventasTotales}
                    activas={ventasTotales - ventasAnuladas}  anuladas={ventasAnuladas}
                    icono={<ShoppingCart size={16} className="text-blue-500"/>}
                    colorActivo="text-blue-500" modoOscuro={modoOscuro} onClick={() => navigate('/ventas')}/>
                <ConteoCard titulo="Compras realizadas" total={comprasTotales}
                    activas={comprasTotales - comprasAnuladas} anuladas={comprasAnuladas}
                    icono={<Receipt size={16} className="text-cyan-500"/>}
                    colorActivo="text-cyan-500" modoOscuro={modoOscuro} onClick={() => navigate('/compras')}/>
            </div>

        </div>
    );
};

export default Overview;