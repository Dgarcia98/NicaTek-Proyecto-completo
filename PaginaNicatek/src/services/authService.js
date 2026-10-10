import api, { mensajeDeError } from '../api/axios.Config';

/**
 * Autentica contra la API y devuelve los datos de sesion normalizados.
 *
 * Ya NO escribe en sessionStorage: de eso se encarga AuthContext, que es
 * quien conoce el estado de la app. Tampoco consulta los permisos por
 * separado, porque ahora vienen dentro de la respuesta del login (y,
 * firmados, dentro del propio token).
 */
export const loginUsuario = async ({ usuario, contrasena }) => {
    try {
        const { data } = await api.post('/Usuario/Login', {
            nombre_Usuario: usuario,
            contrasena,
        });

        if (!data?.token) {
            return { ok: false, mensaje: 'El servidor no devolvio un token de seguridad.' };
        }

        return {
            ok: true,
            datos: {
                token: data.token,
                nombreUsuario: data.nombre_Usuario || usuario,
                idUsuario: data.id_Usuario ?? null,
                idRol: data.id_Rol ?? null,
                nombreRol: data.nombre_Rol || '',
                idPersona: data.id_Persona ?? '',
                permisos: Array.isArray(data.permisos) ? data.permisos : [],
            },
        };
    } catch (error) {
        return { ok: false, mensaje: mensajeDeError(error, 'Error al intentar iniciar sesion') };
    }
};
