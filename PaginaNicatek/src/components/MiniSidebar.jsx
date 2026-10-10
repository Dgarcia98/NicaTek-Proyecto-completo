import { useState, useEffect } from 'react';
import { NavLink } from 'react-router-dom';
import {
  LuLayoutDashboard, LuUsers, LuUserCheck, LuBox, LuFactory,
  LuShoppingBag, LuShoppingCart, LuChevronDown, LuChevronRight,
  LuWarehouse, LuTags, LuTag, LuChevronsRight,
} from 'react-icons/lu';
import api from '../api/axios.Config';

const MiniSidebar = ({ onOpen, modoOscuro }) => {
  const [isRRHHOpen, setIsRRHHOpen] = useState(false);
  const [empresa, setEmpresa] = useState(null);

  useEffect(() => {
    api.get('/Configuracion/Obtener')
      .then(res => setEmpresa(res.data))
      .catch(() => {});
  }, []);

  const iconClass = ({ isActive } = {}) =>
    `text-white hover:text-cyan-300 transition duration-200 ${isActive ? 'text-cyan-300' : ''}`;

  const logoValido = empresa?.ruta_Logo && empresa.ruta_Logo.trim().length > 1;

  return (
    <aside className={`fixed inset-y-0 left-0 w-16 z-40 flex flex-col items-center shadow-md transition-colors duration-300 ${
      modoOscuro ? 'bg-gray-800' : 'bg-cyan-900'}`}>

      {/* Logo pequeño */}
      <div className="py-5 flex items-center justify-center w-full border-b border-cyan-700/50">
        {logoValido ? (
          <img src={empresa.ruta_Logo} alt="Logo" className="w-8 h-8 object-contain rounded"
               title={empresa.nombre_Empresa} />
        ) : (
          <div className="w-8 h-8 rounded-lg bg-cyan-500/20 border border-cyan-400/30 flex items-center justify-center"
               title={empresa?.nombre_Empresa || 'NicaTek'}>
            <span className="text-cyan-300 font-bold text-sm">{empresa?.nombre_Empresa?.[0] || 'N'}</span>
          </div>
        )}
      </div>

      {/* Iconos navegación */}
      <nav className="flex flex-col items-center gap-5 flex-1 py-4">
        <NavLink to="/overview" title="Resumen"
          className={({ isActive }) => `transition duration-200 ${isActive ? 'text-cyan-300' : 'text-white hover:text-cyan-300'}`}>
          <LuLayoutDashboard size={22} />
        </NavLink>

        <button onClick={() => setIsRRHHOpen(!isRRHHOpen)} title="Recursos Humanos"
          className="text-white hover:text-cyan-300 transition duration-200">
          {isRRHHOpen ? <LuChevronDown size={22} /> : <LuChevronRight size={22} />}
        </button>

        {isRRHHOpen && (
          <>
            <NavLink to="/usuarios/admins" title="Administración"
              className={({ isActive }) => `ml-0 transition duration-200 ${isActive ? 'text-cyan-300' : 'text-cyan-200 hover:text-white'}`}>
              <LuUsers size={18} />
            </NavLink>
            <NavLink to="/usuarios/clientes" title="Clientes"
              className={({ isActive }) => `transition duration-200 ${isActive ? 'text-cyan-300' : 'text-cyan-200 hover:text-white'}`}>
              <LuUserCheck size={18} />
            </NavLink>
            <NavLink to="/usuarios/proveedores" title="Proveedores"
              className={({ isActive }) => `transition duration-200 ${isActive ? 'text-cyan-300' : 'text-cyan-200 hover:text-white'}`}>
              <LuFactory size={18} />
            </NavLink>
          </>
        )}

        <NavLink to="/productos" title="Productos"
          className={({ isActive }) => `transition duration-200 ${isActive ? 'text-cyan-300' : 'text-white hover:text-cyan-300'}`}>
          <LuBox size={22} />
        </NavLink>
        <NavLink to="/categorias" title="Categorías"
          className={({ isActive }) => `transition duration-200 ${isActive ? 'text-cyan-300' : 'text-white hover:text-cyan-300'}`}>
          <LuTags size={22} />
        </NavLink>
        <NavLink to="/marcas" title="Marcas y Modelos"
          className={({ isActive }) => `transition duration-200 ${isActive ? 'text-cyan-300' : 'text-white hover:text-cyan-300'}`}>
          <LuTag size={22} />
        </NavLink>
        <NavLink to="/compras" title="Compras"
          className={({ isActive }) => `transition duration-200 ${isActive ? 'text-cyan-300' : 'text-white hover:text-cyan-300'}`}>
          <LuShoppingBag size={22} />
        </NavLink>
        <NavLink to="/ventas" title="Ventas"
          className={({ isActive }) => `transition duration-200 ${isActive ? 'text-cyan-300' : 'text-white hover:text-cyan-300'}`}>
          <LuShoppingCart size={22} />
        </NavLink>
        <NavLink to="/bodega" title="Bodega"
          className={({ isActive }) => `transition duration-200 ${isActive ? 'text-cyan-300' : 'text-white hover:text-cyan-300'}`}>
          <LuWarehouse size={22} />
        </NavLink>
      </nav>

      {/* ── Botón expandir ───────────────────────────────────────────────── */}
      <button
        onClick={() => onOpen?.()}
        title="Expandir menú"
        className={`w-full flex items-center justify-center py-4 border-t transition-colors duration-200
          ${modoOscuro
            ? 'border-gray-700 text-gray-400 hover:text-white hover:bg-gray-700'
            : 'border-cyan-700/50 text-cyan-300/70 hover:text-white hover:bg-cyan-700/50'}`}
      >
        <LuChevronsRight size={20} />
      </button>
    </aside>
  );
};

export default MiniSidebar;