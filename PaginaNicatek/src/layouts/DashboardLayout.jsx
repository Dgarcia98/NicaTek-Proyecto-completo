import { useState, useEffect } from 'react';
import Sidebar from '../components/Sidebar';
import MiniSidebar from '../components/MiniSidebar';
import Topbar from '../components/Topbar';
import { Outlet, useNavigate } from 'react-router-dom';
import { AlertTriangle, X } from 'lucide-react';
import api from '../api/axios.Config';

const DashboardLayout = () => {
    // ── Sidebar: cerrado en móvil, persiste en sessionStorage en desktop ─────
    const [sidebarOpen, setSidebarOpen] = useState(() => {
        const esMobil = window.innerWidth < 768;
        if (esMobil) return false; // siempre cerrado en móvil
        const saved = sessionStorage.getItem('sidebarOpen');
        return saved !== null ? saved === 'true' : true;
    });

    // ── Modo oscuro ───────────────────────────────────────────────────────────
    // Ahora en localStorage (antes sessionStorage: la preferencia se perdia
    // al cerrar la pestana) y respetando prefers-color-scheme la primera vez.
    const [modoOscuro, setModoOscuro] = useState(() => {
        const saved = localStorage.getItem('modoOscuro') ?? sessionStorage.getItem('modoOscuro');
        if (saved !== null) return saved === 'true';
        return window.matchMedia?.('(prefers-color-scheme: dark)').matches ?? false;
    });

    const [alertasStock,  setAlertasStock]  = useState([]);
    const [bannerVisible, setBannerVisible] = useState(false);
    const navigate = useNavigate();

    // Persistir sidebarOpen al cambiar (solo en desktop)
    useEffect(() => {
        if (window.innerWidth >= 768) {
            sessionStorage.setItem('sidebarOpen', String(sidebarOpen));
        }
    }, [sidebarOpen]);

    // Persistir modoOscuro y sincronizar la clase `dark` en <html>.
    //
    // tailwind.config.js ya tenia darkMode: 'class' configurado pero nadie
    // ponia la clase, asi que las ~600 repeticiones de
    // `${modoOscuro ? 'bg-gray-800' : 'bg-white'}` repartidas por 46
    // archivos hacian a mano lo que Tailwind resuelve solo.
    //
    // Con esto, el prefijo `dark:` ya funciona en toda la app y las
    // pantallas se pueden ir migrando de a poco a `bg-white dark:bg-gray-800`
    // sin romper nada: el prop modoOscuro sigue disponible por contexto
    // para las que aun no se migraron.
    useEffect(() => {
        localStorage.setItem('modoOscuro', String(modoOscuro));
        document.documentElement.classList.toggle('dark', modoOscuro);
        document.documentElement.style.colorScheme = modoOscuro ? 'dark' : 'light';
    }, [modoOscuro]);

    // Seguimiento real del ancho de pantalla.
    // Antes el layout leia window.innerWidth durante el render, sin volver
    // a renderizar al cambiar el tamano: el overlay del sidebar quedaba
    // desincronizado al rotar el telefono o redimensionar la ventana.
    const [esMovil, setEsMovil] = useState(() => window.innerWidth < 768);

    useEffect(() => {
        const mq = window.matchMedia('(max-width: 767px)');
        const alCambiar = (e) => {
            setEsMovil(e.matches);
            if (e.matches) setSidebarOpen(false);
        };
        setEsMovil(mq.matches);
        mq.addEventListener('change', alCambiar);
        return () => mq.removeEventListener('change', alCambiar);
    }, []);

    // ── Alertas de stock ──────────────────────────────────────────────────────
useEffect(() => {
    const fetchAlertas = async () => {
        try {
            const res  = await api.get('/BodegaProducto/Listar');
            const data = Array.isArray(res.data) ? res.data : [];
            const bajos = data.filter(b => b.cantidad <= 5);
            if (bajos.length > 0) {
                setAlertasStock(bajos);
                setBannerVisible(true);
            }
        } catch (err) {
            console.warn('No se pudieron cargar alertas de stock:', err.message);
        }
    };
    fetchAlertas();
}, []);

    return (
        <div className="min-h-screen relative transition-colors duration-300
            bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-white">

            {/* ── Overlay para cerrar sidebar en móvil ──────────────────── */}
            {/* <button> y no <div>: cerrar con Escape o Tab+Enter tambien
                debe funcionar, no solo con el mouse. */}
            {sidebarOpen && esMovil && (
                <button
                    type="button"
                    aria-label="Cerrar menú"
                    className="fixed inset-0 z-30 bg-black/40"
                    onClick={() => setSidebarOpen(false)}
                />
            )}

            {/* ── Sidebar expandido ─────────────────────────────────────── */}
            <div className={`fixed inset-y-0 left-0 w-64 z-40 transform transition-transform duration-300
                ${sidebarOpen ? 'translate-x-0' : '-translate-x-full'} overflow-y-auto`}>
                <Sidebar onClose={() => setSidebarOpen(false)} modoOscuro={modoOscuro} />
            </div>

            {/* ── Mini sidebar (colapsado) ──────────────────────────────── */}
            {!sidebarOpen && (
                <MiniSidebar onOpen={() => setSidebarOpen(true)} modoOscuro={modoOscuro} />
            )}

            {/* ── Contenido principal ───────────────────────────────────── */}
            <div className={`transition-all duration-300 ${sidebarOpen ? 'md:ml-64 ml-0' : 'ml-16'}`}>
                <Topbar
                    toggleSidebar={() => setSidebarOpen(prev => !prev)}
                    toggleModoOscuro={() => setModoOscuro(prev => !prev)}
                    modoOscuro={modoOscuro}
                />

                {/* ── Banner stock bajo ─────────────────────────────────── */}
                {/* Los colores del banner estaban fijados en claro
                    (bg-yellow-50 / text-yellow-800) y quedaban ilegibles en
                    modo oscuro. Ahora tienen variante dark:. */}
                {bannerVisible && alertasStock.length > 0 && (
                    <div
                        role="status"
                        className="mx-6 mt-4 rounded-xl border px-4 py-3 flex items-center justify-between gap-3
                            border-yellow-300 bg-yellow-50
                            dark:border-yellow-700/60 dark:bg-yellow-500/10"
                    >
                        <div className="flex items-center gap-3 flex-1 min-w-0">
                            <AlertTriangle size={20} className="text-yellow-600 dark:text-yellow-400 flex-shrink-0" aria-hidden="true" />
                            <div className="flex-1 min-w-0">
                                <span className="text-sm font-semibold text-yellow-800 dark:text-yellow-200">
                                    {alertasStock.length} producto{alertasStock.length > 1 ? 's' : ''} con stock bajo (≤5 unidades):
                                </span>
                                <span className="text-sm text-yellow-700 dark:text-yellow-300/90 ml-2 truncate">
                                    {alertasStock.slice(0, 3).map(a =>
                                        `${a.nombre_Producto || '—'} (${a.cantidad})`
                                    ).join(', ')}
                                    {alertasStock.length > 3 && ` y ${alertasStock.length - 3} más...`}
                                </span>
                            </div>
                        </div>
                        <div className="flex items-center gap-2 flex-shrink-0">
                            <button type="button" onClick={() => navigate('/bodega')}
                                className="text-xs font-semibold border px-3 py-1 rounded-lg transition
                                    text-yellow-700 border-yellow-400 hover:bg-yellow-100
                                    dark:text-yellow-300 dark:border-yellow-600 dark:hover:bg-yellow-500/20">
                                Ver bodega
                            </button>
                            <button type="button" onClick={() => setBannerVisible(false)}
                                aria-label="Cerrar aviso de stock bajo"
                                className="text-yellow-500 hover:text-yellow-700 dark:hover:text-yellow-300 transition">
                                <X size={16} aria-hidden="true" />
                            </button>
                        </div>
                    </div>
                )}

                <div className="p-6">
                    <Outlet context={{ modoOscuro }} />
                </div>
            </div>
        </div>
    );
};

export default DashboardLayout;