import { lazy, Suspense } from 'react';
import { BrowserRouter, Routes, Route, Navigate, useLocation } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import { AuthProvider, useAuth } from './context/AuthContext';
import DashboardLayout from './layouts/DashboardLayout';
import Login from './pages/Login';

/**
 * Rutas con carga diferida.
 *
 * Antes las 36 paginas se importaban de forma estatica: el usuario
 * descargaba el modulo de ventas, compras, bodega y usuarios (incluido
 * Chart.js) solo para ver la pantalla de login. Ahora cada pantalla llega
 * cuando se visita.
 *
 * Login y DashboardLayout quedan estaticos a proposito: son lo primero
 * que se ve y diferirlos solo agregaria un parpadeo.
 */
const LogoutScreen            = lazy(() => import('./pages/LogoutScreen'));
const Overview                = lazy(() => import('./pages/Overview'));

const Productos               = lazy(() => import('./pages/Productos/Productos.jsx'));
const FormularioAgregarProducto = lazy(() => import('./pages/Productos/FormularioAgregarProducto.jsx'));
const EditarProducto          = lazy(() => import('./pages/Productos/EditarProducto.jsx'));
const Marcas                  = lazy(() => import('./pages/Marcas/Marcas'));

const Categorias              = lazy(() => import('./pages/Categorias/Categorias'));
const AgregarCategoria        = lazy(() => import('./pages/Categorias/AgregarCategoria'));
const EditarCategoria         = lazy(() => import('./pages/Categorias/EditarCategoria'));

const Compras                 = lazy(() => import('./pages/Compras/Compras'));
const ResumenCompra           = lazy(() => import('./pages/Compras/ResumenCompra'));
const EditarCompra            = lazy(() => import('./pages/Compras/EditarCompra'));

const Ventas                  = lazy(() => import('./pages/Ventas/Ventas'));
const ResumenVenta            = lazy(() => import('./pages/Ventas/ResumenVenta'));
const EditarVenta             = lazy(() => import('./pages/Ventas/EditarVenta'));

const Bodega                  = lazy(() => import('./pages/Bodega/Bodega'));
const BodegaDetalle           = lazy(() => import('./pages/Bodega/BodegaDetalle'));

const Admins                  = lazy(() => import('./pages/usuarios/Admins'));
const CrearUsuario            = lazy(() => import('./pages/usuarios/CrearUsuario'));
const CrearEmpleado           = lazy(() => import('./pages/usuarios/CrearEmpleado'));
const VistaResumenUsuario     = lazy(() => import('./pages/usuarios/VistaResumenUsuario'));
const VistaResumenEmpleado    = lazy(() => import('./pages/usuarios/VistaResumenEmpleado'));

const Clientes                = lazy(() => import('./pages/usuarios/Clientes'));
const CrearCliente            = lazy(() => import('./pages/Clientes/CrearCliente'));
const VistaResumenCliente     = lazy(() => import('./pages/Clientes/VistaResumenCliente'));

const Proveedores             = lazy(() => import('./pages/usuarios/Proveedores'));
const CrearProveedor          = lazy(() => import('./pages/proveedores/CrearProveedor'));
const VistaResumenProveedor   = lazy(() => import('./pages/proveedores/VistaResumenProveedor'));

const Personas                = lazy(() => import('./pages/usuarios/Personas.jsx'));
const CrearPersona            = lazy(() => import('./pages/personas/CrearPersona'));
const VistaResumenPersona     = lazy(() => import('./pages/personas/VistaResumenPersona'));

const PantallaCarga = () => (
    <div className="flex h-screen items-center justify-center bg-gray-100 dark:bg-gray-900">
        <div
            className="h-10 w-10 animate-spin rounded-full border-4 border-cyan-500 border-t-transparent"
            role="status"
            aria-label="Cargando"
        />
    </div>
);

/**
 * Bloquea el acceso a las rutas del panel sin sesion.
 * `state.desde` deja volver a la pagina pedida despues de autenticarse.
 */
const RutaProtegida = ({ children }) => {
    const { estaAutenticado } = useAuth();
    const location = useLocation();

    if (!estaAutenticado) {
        return <Navigate to="/login" replace state={{ desde: location.pathname }} />;
    }
    return children;
};

/** Redirige al panel si ya hay sesion (evita ver el login estando dentro). */
const SoloInvitados = ({ children }) => {
    const { estaAutenticado } = useAuth();
    return estaAutenticado ? <Navigate to="/overview" replace /> : children;
};

const Rutas = () => {
    const { estaAutenticado } = useAuth();

    return (
        <Suspense fallback={<PantallaCarga />}>
            <Routes>
                <Route path="/login" element={<SoloInvitados><Login /></SoloInvitados>} />
                <Route path="/logout" element={<LogoutScreen />} />

                <Route
                    path="/"
                    element={<RutaProtegida><DashboardLayout /></RutaProtegida>}
                >
                    <Route index element={<Navigate to="/overview" replace />} />
                    <Route path="overview" element={<Overview />} />

                    <Route path="productos" element={<Productos />} />
                    <Route path="productos/nuevo" element={<FormularioAgregarProducto />} />
                    <Route path="productos/editar/:id" element={<EditarProducto />} />
                    <Route path="marcas" element={<Marcas />} />

                    <Route path="categorias" element={<Categorias />} />
                    <Route path="categorias/nueva" element={<AgregarCategoria />} />
                    <Route path="categorias/editar/:id" element={<EditarCategoria />} />

                    <Route path="compras" element={<Compras />} />
                    <Route path="compras/resumen/:compraId" element={<ResumenCompra />} />
                    <Route path="compras/editar/:compraId" element={<EditarCompra />} />

                    <Route path="ventas" element={<Ventas />} />
                    <Route path="ventas/resumen/:ventaId" element={<ResumenVenta />} />
                    <Route path="ventas/editar/:ventaId" element={<EditarVenta />} />
                    {/*
                      La ruta /ventas/nueva se elimino: renderizaba
                      <FormularioAgregarVenta /> sin las props onSuccess y
                      onCancel, asi que al guardar con exito reventaba con
                      "onSuccess is not a function" y dejaba la pantalla en
                      blanco -- despues de haber creado la venta.
                      El alta real se abre desde Ventas.jsx.
                    */}
                    <Route path="ventas/nueva" element={<Navigate to="/ventas" replace />} />

                    <Route path="bodega" element={<Bodega />} />
                    <Route path="bodega/:categoria" element={<BodegaDetalle />} />

                    <Route path="usuarios/admins" element={<Admins />} />
                    <Route path="usuarios/admins/nuevo-usuario" element={<CrearUsuario />} />
                    <Route path="usuarios/admins/nuevo-empleado" element={<CrearEmpleado />} />
                    <Route path="usuarios/admins/resumen/:id" element={<VistaResumenUsuario />} />
                    <Route path="usuarios/admins/empleado/resumen/:id" element={<VistaResumenEmpleado />} />

                    <Route path="usuarios/clientes" element={<Clientes />} />
                    <Route path="usuarios/clientes/nuevo" element={<CrearCliente />} />
                    <Route path="usuarios/clientes/resumen/:id" element={<VistaResumenCliente />} />

                    <Route path="usuarios/proveedores" element={<Proveedores />} />
                    <Route path="usuarios/proveedores/nuevo" element={<CrearProveedor />} />
                    <Route path="usuarios/proveedores/resumen/:id" element={<VistaResumenProveedor />} />

                    <Route path="usuarios/personas" element={<Personas />} />
                    <Route path="usuarios/personas/nueva" element={<CrearPersona />} />
                    <Route path="usuarios/personas/resumen/:id" element={<VistaResumenPersona />} />
                </Route>

                <Route path="*" element={<Navigate to={estaAutenticado ? '/overview' : '/login'} replace />} />
            </Routes>
        </Suspense>
    );
};

const App = () => (
    <BrowserRouter>
        <AuthProvider>
            <Rutas />
            <Toaster
                position="top-right"
                toastOptions={{
                    duration: 4000,
                    className: 'dark:!bg-gray-800 dark:!text-gray-100',
                }}
            />
        </AuthProvider>
    </BrowserRouter>
);

export default App;
