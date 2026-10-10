import { createContext, useContext, useState, useCallback, useEffect, useMemo } from 'react';
import { setOnSesionExpirada } from '../api/axios.Config';

/**
 * Estado de sesion de la aplicacion.
 *
 * Antes App.jsx leia `sessionStorage.getItem('token')` UNA sola vez al
 * montar, fuera de todo estado de React. Por eso el login terminaba con
 * `window.location.reload()`: era la unica forma de que el router se
 * enterara de que ya habia sesion. Eso recargaba el bundle entero y
 * provocaba el parpadeo blanco.
 *
 * Con el token en estado, el cambio de sesion vuelve a renderizar solo lo
 * necesario y la recarga sobra.
 */
const AuthContext = createContext(null);

const CLAVES = ['token', 'nombreUsuario', 'idUsuario', 'idRol', 'nombreRol', 'idPersona', 'permisos'];

const leerSesionGuardada = () => {
    const token = sessionStorage.getItem('token');
    if (!token) return null;

    let permisos = [];
    try {
        permisos = JSON.parse(sessionStorage.getItem('permisos') || '[]');
    } catch {
        permisos = [];
    }

    return {
        token,
        nombreUsuario: sessionStorage.getItem('nombreUsuario') || '',
        idUsuario: Number(sessionStorage.getItem('idUsuario')) || null,
        idRol: Number(sessionStorage.getItem('idRol')) || null,
        nombreRol: sessionStorage.getItem('nombreRol') || '',
        idPersona: sessionStorage.getItem('idPersona') || '',
        permisos,
    };
};

export const AuthProvider = ({ children }) => {
    const [usuario, setUsuario] = useState(leerSesionGuardada);

    const cerrarSesion = useCallback(() => {
        CLAVES.forEach(k => sessionStorage.removeItem(k));
        setUsuario(null);
    }, []);

    const iniciarSesion = useCallback((datos) => {
        sessionStorage.setItem('token', datos.token);
        sessionStorage.setItem('nombreUsuario', datos.nombreUsuario ?? '');
        sessionStorage.setItem('idUsuario', datos.idUsuario ?? '');
        sessionStorage.setItem('idRol', datos.idRol ?? '');
        sessionStorage.setItem('nombreRol', datos.nombreRol ?? '');
        sessionStorage.setItem('idPersona', datos.idPersona ?? '');
        sessionStorage.setItem('permisos', JSON.stringify(datos.permisos ?? []));
        setUsuario(datos);
    }, []);

    // El interceptor de axios vive fuera de React. Le pasamos el callback
    // para que un 401 (token vencido o revocado) limpie la sesion y el
    // router mande al login por si solo, en vez de dejar cada pantalla
    // fallando con un toast generico.
    useEffect(() => {
        setOnSesionExpirada(() => cerrarSesion());
        return () => setOnSesionExpirada(null);
    }, [cerrarSesion]);

    // Memorizado: sin esto el objeto seria nuevo en cada render y forzaria
    // a re-renderizar todos los consumidores del contexto.
    const valor = useMemo(() => ({
        usuario,
        estaAutenticado: Boolean(usuario?.token),
        iniciarSesion,
        cerrarSesion,

        /**
         * Solo controla que se MUESTRA. La autorizacion real la hace la API
         * validando el claim firmado del token: manipular esto en el
         * navegador no concede acceso a nada.
         */
        tienePermiso: (codigo) =>
            usuario?.nombreRol === 'Administrador' ||
            (usuario?.permisos ?? []).includes(codigo),
    }), [usuario, iniciarSesion, cerrarSesion]);

    return <AuthContext.Provider value={valor}>{children}</AuthContext.Provider>;
};

// eslint-disable-next-line react-refresh/only-export-components
export const useAuth = () => {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error('useAuth debe usarse dentro de <AuthProvider>');
    return ctx;
};

export default AuthContext;
