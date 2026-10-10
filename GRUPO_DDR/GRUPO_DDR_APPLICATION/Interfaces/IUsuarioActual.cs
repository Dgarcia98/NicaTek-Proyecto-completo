namespace GRUPO_DDR_APPLICATION.Interfaces
{
    /// <summary>
    /// Identidad del usuario que hace la peticion, leida del JWT.
    ///
    /// Existe porque el frontend enviaba id_Creador / id_Modificador en el
    /// cuerpo de cada request, con un `|| 1` de respaldo: si fallaba, la
    /// accion se atribuia al usuario 1 (el administrador). Ademas se podia
    /// falsificar cambiando un numero en el JSON, lo que hacia inservible
    /// la auditoria de Fecha_Creacion / Id_Creador.
    ///
    /// A partir de aqui el id sale del token firmado y los DTOs ya no
    /// deciden quien hizo que.
    /// </summary>
    public interface IUsuarioActual
    {
        /// <summary>Id_Usuario del token. Null si la peticion es anonima.</summary>
        int? IdUsuario { get; }

        /// <summary>Id_Usuario del token; lanza si no hay sesion valida.</summary>
        int IdUsuarioRequerido { get; }

        string? NombreUsuario { get; }

        string? NombreRol { get; }

        bool EstaAutenticado { get; }

        bool TienePermiso(string codigo);
    }
}
