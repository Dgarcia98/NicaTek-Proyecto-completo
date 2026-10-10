import { useState, useEffect } from 'react';
import { NavLink } from 'react-router-dom';
import {
  LuLayoutDashboard, LuUsers, LuUserCheck, LuBox, LuFactory,
  LuShoppingBag, LuShoppingCart, LuChevronDown, LuUserCog,
  LuUserPlus, LuTags, LuWarehouse, LuTag, LuChevronsLeft,
} from 'react-icons/lu';
import api from '../api/axios.Config';

const Sidebar = ({ onClose, modoOscuro }) => {
  const [showRRHH, setShowRRHH] = useState(false);
  const [empresa, setEmpresa] = useState(null);

  useEffect(() => {
    api.get('/Configuracion/Obtener')
      .then(res => setEmpresa(res.data))
      .catch(() => {});
  }, []);

  const linkClass = 'flex items-center gap-2 px-3 py-2 rounded hover:text-cyan-300 transition text-sm';
  const logoValido = empresa?.ruta_Logo && empresa.ruta_Logo.trim().length > 1;

  return (
    <aside className={`fixed inset-y-0 left-0 w-64 flex flex-col transition-colors duration-300 z-40 ${
      modoOscuro ? 'bg-gray-800 text-white' : 'bg-cyan-800 text-white'}`}>

      {/* Logo / nombre empresa */}
      <div className={`px-6 py-5 border-b ${modoOscuro ? 'border-gray-700' : 'border-cyan-700'}`}>
        {logoValido ? (
          <img src={empresa.ruta_Logo} alt={empresa.nombre_Empresa} className="h-10 w-auto object-contain" />
        ) : (
          <div className="flex items-center gap-2">
            <div className="w-8 h-8 rounded-lg bg-cyan-500/20 border border-cyan-400/30 flex items-center justify-center">
              <span className="text-cyan-300 font-bold text-sm">
                {empresa?.nombre_Empresa?.[0] || 'N'}
              </span>
            </div>
            <div>
              <p className="text-sm font-bold leading-tight">{empresa?.nombre_Empresa || 'NicaTek'}</p>
              <p className="text-xs text-cyan-300/60">Sistema de Gestión</p>
            </div>
          </div>
        )}
      </div>

      {/* Navegación */}
      <nav className="flex flex-col gap-1 p-4 flex-1 overflow-y-auto">
        <NavLink to="/overview" className={({ isActive }) =>
          `${linkClass} ${isActive ? 'text-cyan-300 font-semibold bg-white/10 rounded' : ''}`}>
          <LuLayoutDashboard size={18} /> Resumen
        </NavLink>

        <button onClick={() => setShowRRHH(!showRRHH)} className={`${linkClass} font-semibold`}>
          <LuUsers size={18} /> Recursos Humanos
          <LuChevronDown className={`ml-auto transform transition-transform duration-200 ${showRRHH ? 'rotate-180' : ''}`} />
        </button>

        {showRRHH && (
          <div className="pl-6 flex flex-col gap-1">
            <NavLink to="/usuarios/admins" className={({ isActive }) =>
              `${linkClass} ${isActive ? 'text-cyan-300 font-semibold' : ''}`}>
              <LuUserCog size={16} /> Administración
            </NavLink>
            <NavLink to="/usuarios/clientes" className={({ isActive }) =>
              `${linkClass} ${isActive ? 'text-cyan-300 font-semibold' : ''}`}>
              <LuUserPlus size={16} /> Clientes
            </NavLink>
            <NavLink to="/usuarios/proveedores" className={({ isActive }) =>
              `${linkClass} ${isActive ? 'text-cyan-300 font-semibold' : ''}`}>
              <LuFactory size={16} /> Proveedores
            </NavLink>
            <NavLink to="/usuarios/personas" className={({ isActive }) =>
              `${linkClass} ${isActive ? 'text-cyan-300 font-semibold' : ''}`}>
              <LuUserCheck size={16} /> Personas
            </NavLink>
          </div>
        )}

        <NavLink to="/productos" className={({ isActive }) =>
          `${linkClass} ${isActive ? 'text-cyan-300 font-semibold' : ''}`}>
          <LuBox size={18} /> Productos
        </NavLink>
        <NavLink to="/categorias" className={({ isActive }) =>
          `${linkClass} ${isActive ? 'text-cyan-300 font-semibold' : ''}`}>
          <LuTags size={18} /> Categorías
        </NavLink>
        <NavLink to="/marcas" className={({ isActive }) =>
          `${linkClass} ${isActive ? 'text-cyan-300 font-semibold' : ''}`}>
          <LuTag size={18} /> Marcas y Modelos
        </NavLink>
        <NavLink to="/compras" className={({ isActive }) =>
          `${linkClass} ${isActive ? 'text-cyan-300 font-semibold' : ''}`}>
          <LuShoppingBag size={18} /> Compras
        </NavLink>
        <NavLink to="/ventas" className={({ isActive }) =>
          `${linkClass} ${isActive ? 'text-cyan-300 font-semibold' : ''}`}>
          <LuShoppingCart size={18} /> Ventas
        </NavLink>
        <NavLink to="/bodega" className={({ isActive }) =>
          `${linkClass} ${isActive ? 'text-cyan-300 font-semibold' : ''}`}>
          <LuWarehouse size={18} /> Bodega
        </NavLink>
      </nav>

      {/* ── Botón colapsar ───────────────────────────────────────────────── */}
      <button
        onClick={onClose}
        title="Colapsar menú"
        className={`flex items-center gap-2 px-5 py-4 text-sm font-medium border-t transition-colors duration-200
          ${modoOscuro
            ? 'border-gray-700 text-gray-400 hover:text-white hover:bg-gray-700'
            : 'border-cyan-700 text-cyan-300/70 hover:text-white hover:bg-cyan-700/50'}`}
      >
        <LuChevronsLeft size={18} />
        <span>Cerrar</span>
      </button>
    </aside>
  );
};

export default Sidebar;