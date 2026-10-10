# Configuración y seguridad — instalación local

Este sistema corre **solo en local**: SQL Server en `DESKTOP-S4PP812` y la
API en `http://localhost:5114`. No hay nada en Azure.

## No hay que configurar nada para arrancar

```powershell
cd GRUPO_DDR\GRUPO_DDR_API
dotnet run
```

O F5 en Visual Studio con cualquier perfil (`http`, `https`, `IIS Express`).
Funciona con o sin `ASPNETCORE_ENVIRONMENT`.

### Cadena de conexión

Está en `appsettings.json`. Usa **autenticación integrada de Windows**: no
lleva usuario ni contraseña, por eso puede versionarse sin riesgo.

```json
"CadenaSQL": "Data Source=DESKTOP-S4PP812;Initial Catalog=Nicatek;Integrated Security=True;..."
```

Si tu instancia se llama distinto, cambiá el `Data Source` (por ejemplo
`.\SQLEXPRESS` o `localhost`).

### Clave de firma del JWT

Esta sí es un secreto, así que **no** está en el repositorio. La API la
resuelve sola, en este orden:

1. Configuración (variable de entorno `Jwt__Key`, o `dotnet user-secrets`)
2. Archivo `GRUPO_DDR_API/.jwt-dev-key`
3. Si no existe, **genera una de 512 bits y la guarda ahí**

El archivo está en `.gitignore`. No hay que hacer nada manualmente.

Para regenerarla, borrá `.jwt-dev-key` y reiniciá la API. Eso cierra todas
las sesiones abiertas, que es justo lo que se busca al rotar una clave.

> La clave anterior (`EstaClaveDebeSerMuyLarga...`) estaba escrita en
> `appsettings.json` y versionada, así que se considera comprometida. Ya no
> se usa: al primer arranque se generó una nueva.

### Ajustes de `appsettings.json`

| Clave | Local | En un servidor real |
|---|---|---|
| `Seguridad:ForzarHttps` | `false` | `true` |
| `Seguridad:PublicarSwagger` | `true` | `false` |
| `Cors:AllowedOrigins` | `localhost:5173`, `localhost:4173` | el dominio real |

`ForzarHttps` está apagado a propósito: el frontend llama a
`http://localhost:5114/api`. Con la redirección activa, arrancar con el
perfil `https` de Visual Studio hacía que cada llamada respondiera 307 hacia
`https://localhost:7162`; el navegador la seguía a otro origen y CORS la
rechazaba. Se veía exactamente como "la API no funciona".

---

## Qué cambió en seguridad

| Antes | Ahora |
|---|---|
| Ningún controlador tenía `[Authorize]`: la API entera era pública | Filtro global de autorización; solo `Usuario/Login` es anónimo |
| Los permisos vivían en `sessionStorage` y la UI solo atenuaba botones | Los permisos son claims firmados en el JWT y se validan en el servidor con `[RequierePermiso]` |
| `id_Creador` llegaba en el body con respaldo `|| 1` (el admin) | Sale del claim `NameIdentifier` del token |
| Precio y total los enviaba el navegador | El precio se lee de `Tbl_Producto`; el total se suma en el servidor |
| `AllowAnyOrigin()` | Lista blanca desde `Cors:AllowedOrigins` |
| Swagger publicado en la raíz de producción | Solo en desarrollo, bajo `/swagger` |
| `UseHttpsRedirection` comentado | Activo, más HSTS en producción |
| `ex.Message` devuelto al cliente en ~200 `catch` | Middleware central: detalle al log, mensaje genérico + `trazaId` al cliente |
| Login distinguía "usuario no existe" de "contraseña mala" | Mensaje único y verificación de hash en tiempo constante |
| `Encoding.ASCII` sobre la clave JWT (descarta bytes >127) | `Encoding.UTF8` |

---

## Lo que queda pendiente

Al ser una instalación local, varios de estos pesan mucho menos que si la
API estuviera expuesta a internet. Por orden de importancia:

1. **Permisos granulares en los 28 controladores restantes.** Hoy exigen
   sesión válida (ya no son públicos), pero no un permiso específico. Los
   sensibles — `Usuario`, `Venta`, `Compra`, `Dashboard` — ya lo tienen.
   Para el resto, agregar `[RequierePermiso("INV_READ")]` y compañía.
   Este es el único que importa de verdad en local, porque separa lo que
   puede hacer un empleado de lo que puede hacer un administrador.
2. **Los 7 códigos de permiso son gruesos.** `USR_MANAGE` cubre leer, crear,
   editar y eliminar usuarios. Conviene separarlos, pero hay que insertar
   los códigos nuevos en `Cls_Permisos` **y** asignarlos en
   `Tbl_Roles_Permisos` antes de usarlos en el código, o el rol se queda
   sin acceso.
3. **Sin refresh token.** El token dura 8 horas y no se puede revocar: un
   usuario desactivado sigue entrando hasta que expire.
4. **Token en `sessionStorage`.** Accesible desde JavaScript. En local, con
   la API solo escuchando en la misma máquina, el riesgo es bajo. Si esto
   se publica alguna vez, pasar a cookie `HttpOnly` + `Secure` + `SameSite`.
5. **Rate limiting en el login.** Relevante solo si la API se expone.
   `AddRateLimiter` de .NET 9 lo resuelve en pocas líneas.

### Si algún día se publica en un servidor

1. `Seguridad:ForzarHttps` → `true`, `Seguridad:PublicarSwagger` → `false`
2. `Cors:AllowedOrigins` → el dominio real, no `localhost`
3. `Jwt__Key` → variable de entorno con una clave nueva (no el archivo)
4. Cadena de conexión → variable de entorno `ConnectionStrings__CadenaSQL`,
   con un usuario de SQL de permisos mínimos (no `sa` ni un administrador)

> Nota: el rol `Administrador` pasa cualquier verificación de permiso
> (`PermisoHandler.RolAdministrador`). Es una red de seguridad para que una
> tabla `Tbl_Roles_Permisos` incompleta no deje al administrador fuera de su
> propio sistema.
