/**
 * Ayudas de PRESENTACION para permisos.
 *
 * IMPORTANTE: esto no es seguridad, es cosmetica.
 *
 * Antes era lo unico que separaba a un empleado de borrar productos: la
 * lista de permisos vivia en sessionStorage y `clsPermiso` se limitaba a
 * poner `pointer-events-none` en el boton. Bastaba con abrir DevTools y
 * escribir sessionStorage.setItem('permisos', '["INV_DELETE"]').
 *
 * Ahora la autorizacion real la aplica la API: cada endpoint valida el
 * claim firmado dentro del JWT ([RequierePermiso] en los controladores).
 * Manipular estos valores en el navegador solo cambia que botones se ven;
 * el servidor sigue respondiendo 403.
 *
 * En componentes nuevos preferi `useAuth().tienePermiso(...)`, que lee del
 * contexto y se re-renderiza al cambiar la sesion. Estas funciones existen
 * para las pantallas que aun importan desde aqui.
 */

const leerPermisos = () => {
    try {
        return JSON.parse(sessionStorage.getItem('permisos') || '[]');
    } catch {
        return [];
    }
};

/**
 * @param {string} codigo Codigo del permiso, ej: 'INV_CREATE'
 * @returns {boolean}
 */
export const tienePermiso = (codigo) => {
    if (sessionStorage.getItem('nombreRol') === 'Administrador') return true;
    return leerPermisos().includes(codigo);
};

/** El rol Administrador es el unico que pasa cualquier verificacion. */
export const esAdmin = () => sessionStorage.getItem('nombreRol') === 'Administrador';

export const getNombreRol = () => sessionStorage.getItem('nombreRol') || '';

/** Clases para atenuar un control sin permiso. */
export const clsPermiso = (codigo) =>
    !tienePermiso(codigo) ? 'opacity-40 cursor-not-allowed pointer-events-none' : '';
