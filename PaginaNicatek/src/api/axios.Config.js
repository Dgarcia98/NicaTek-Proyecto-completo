import axios from 'axios';

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
    },
});

/**
 * Callback que AuthContext registra para limpiar la sesion cuando la API
 * responde 401. El interceptor vive fuera del arbol de React, por eso no
 * puede usar hooks directamente.
 */
let onSesionExpirada = null;
export const setOnSesionExpirada = (fn) => { onSesionExpirada = fn; };

api.interceptors.request.use(
    (config) => {
        const token = sessionStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

api.interceptors.response.use(
    (response) => response,
    (error) => {
        // Sin respuesta del servidor: caida de red o API apagada.
        if (error.request && !error.response) {
            return Promise.reject(
                new Error('No se pudo conectar con el servidor. Verifica tu conexion.')
            );
        }

        const status = error.response?.status;

        // 401: el token vencio o fue revocado (por ejemplo al rotar la
        // clave de firma). Antes esto dejaba al usuario atrapado: cada
        // pantalla fallaba con un toast y no habia forma de volver al
        // login sin borrar el sessionStorage a mano.
        if (status === 401) {
            const enLogin = window.location.pathname.startsWith('/login');
            if (!enLogin) {
                onSesionExpirada?.();
                // replace y no href: no deja la pagina vencida en el historial.
                window.location.replace('/login?expirada=1');
            }
        }

        // 403: hay sesion valida pero al rol le falta el permiso. No se
        // cierra la sesion; se deja que la pantalla muestre el mensaje.
        if (status === 403 && !error.response.data?.mensaje) {
            error.response.data = {
                ...error.response.data,
                mensaje: 'No tiene permiso para realizar esta accion.',
            };
        }

        return Promise.reject(error);
    }
);

/**
 * Extrae el mensaje legible de un error de axios.
 * La API responde { exito, mensaje, trazaId }; los formatos viejos usaban
 * `msj`, y ModelState devuelve { errors: { Campo: [...] } }.
 */
export const mensajeDeError = (error, porDefecto = 'Ocurrio un error inesperado.') => {
    const d = error?.response?.data;

    if (typeof d === 'string' && d.trim()) return d;

    if (d && typeof d === 'object') {
        if (d.mensaje) return d.mensaje;
        if (d.msj) return d.msj;
        if (d.message) return d.message;
        if (d.title) return d.title;

        // Errores de validacion del modelo
        if (d.errors && typeof d.errors === 'object') {
            const primero = Object.values(d.errors).flat()[0];
            if (primero) return primero;
        }
    }

    return error?.message || porDefecto;
};

export default api;
