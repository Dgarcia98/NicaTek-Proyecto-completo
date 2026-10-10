import { useState, useEffect } from 'react';
import { useNavigate, useLocation, useSearchParams } from 'react-router-dom';
import { loginUsuario } from '../services/authService';
import { useAuth } from '../context/AuthContext';

const Login = () => {
  const [usuario, setUsuario] = useState('');
  const [contrasena, setContrasena] = useState('');
  const [error, setError] = useState('');
  const [mostrarContrasena, setMostrarContrasena] = useState(false);
  const [cargando, setCargando] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const [params] = useSearchParams();
  const { iniciarSesion } = useAuth();

  // El interceptor de axios redirige aqui con ?expirada=1 cuando la API
  // devuelve 401, para que el usuario entienda por que salio de la sesion
  // en vez de encontrarse el login sin explicacion.
  useEffect(() => {
    if (params.get('expirada')) {
      setError('Tu sesión expiró. Vuelve a iniciar sesión.');
    }
  }, [params]);

  const handleLogin = async (e) => {
    e.preventDefault();
    if (!usuario || !contrasena) { setError('Ingresa usuario y contraseña'); return; }
    setCargando(true);
    setError('');

    const resultado = await loginUsuario({ usuario, contrasena });

    if (resultado.ok) {
      // Antes aqui iba navigate() + window.location.reload(): App.jsx leia
      // el token una sola vez al montar, asi que recargar era la unica
      // forma de que el router se enterara. Con la sesion en contexto,
      // esto re-renderiza solo y sin el parpadeo blanco de la recarga.
      iniciarSesion(resultado.datos);
      const destino = location.state?.desde || '/overview';
      navigate(destino, { replace: true });
    } else {
      setError(resultado.mensaje);
      setCargando(false);
    }
  };

  useEffect(() => { window.scrollTo(0, 0); }, []);

  return (
    <>
      <style>{`
        @import url('https://fonts.googleapis.com/css2?family=Syne:wght@400;600;700;800&family=DM+Sans:wght@300;400;500&display=swap');

        * { box-sizing: border-box; margin: 0; padding: 0; }

        .login-root {
          min-height: 100vh;
          display: flex;
          background: #080e14;
          font-family: 'DM Sans', sans-serif;
          overflow: hidden;
          position: relative;
        }

        /* Panel izquierdo decorativo */
        .login-panel-left {
          flex: 1;
          display: none;
          flex-direction: column;
          justify-content: center;
          padding: 60px;
          position: relative;
          overflow: hidden;
        }

        @media (min-width: 1024px) {
          .login-panel-left { display: flex; }
        }

        .login-panel-left::before {
          content: '';
          position: absolute;
          inset: 0;
          background: linear-gradient(135deg, #0a1628 0%, #0d1f3c 50%, #071320 100%);
        }

        /* Cuadrícula geométrica de fondo */
        .login-grid {
          position: absolute;
          inset: 0;
          background-image:
            linear-gradient(rgba(0, 198, 255, 0.04) 1px, transparent 1px),
            linear-gradient(90deg, rgba(0, 198, 255, 0.04) 1px, transparent 1px);
          background-size: 48px 48px;
        }

        /* Orbes de luz */
        .login-orb-1 {
          position: absolute;
          width: 500px;
          height: 500px;
          border-radius: 50%;
          background: radial-gradient(circle, rgba(0, 198, 255, 0.12) 0%, transparent 70%);
          top: -100px;
          left: -100px;
          pointer-events: none;
        }

        .login-orb-2 {
          position: absolute;
          width: 300px;
          height: 300px;
          border-radius: 50%;
          background: radial-gradient(circle, rgba(0, 114, 255, 0.1) 0%, transparent 70%);
          bottom: 80px;
          right: -60px;
          pointer-events: none;
        }

        /* Líneas decorativas */
        .login-lines {
          position: absolute;
          bottom: 0;
          left: 0;
          right: 0;
          height: 200px;
          overflow: hidden;
          opacity: 0.15;
        }

        .login-lines::before {
          content: '';
          position: absolute;
          bottom: -80px;
          left: 50%;
          transform: translateX(-50%);
          width: 600px;
          height: 600px;
          border: 1px solid #00c6ff;
          border-radius: 50%;
        }

        .login-lines::after {
          content: '';
          position: absolute;
          bottom: -120px;
          left: 50%;
          transform: translateX(-50%);
          width: 800px;
          height: 800px;
          border: 1px solid #00c6ff;
          border-radius: 50%;
        }

        .login-brand {
          position: relative;
          z-index: 1;
        }

        .login-brand-badge {
          display: inline-flex;
          align-items: center;
          gap: 8px;
          background: rgba(0, 198, 255, 0.1);
          border: 1px solid rgba(0, 198, 255, 0.25);
          border-radius: 100px;
          padding: 6px 14px;
          margin-bottom: 32px;
        }

        .login-brand-badge-dot {
          width: 6px;
          height: 6px;
          border-radius: 50%;
          background: #00c6ff;
          animation: login-pulse 2s ease-in-out infinite;
        }

        @keyframes login-pulse {
          0%, 100% { opacity: 1; transform: scale(1); }
          50% { opacity: 0.5; transform: scale(0.8); }
        }

        .login-brand-badge-text {
          font-size: 12px;
          font-weight: 500;
          color: #00c6ff;
          letter-spacing: 0.08em;
          text-transform: uppercase;
        }

        .login-title {
          font-family: 'Syne', sans-serif;
          font-size: clamp(36px, 4vw, 52px);
          font-weight: 800;
          color: #f0f4f8;
          line-height: 1.1;
          margin-bottom: 20px;
          letter-spacing: -0.02em;
        }

        .login-title span {
          background: linear-gradient(135deg, #00c6ff, #0072ff);
          -webkit-background-clip: text;
          -webkit-text-fill-color: transparent;
          background-clip: text;
        }

        .login-subtitle {
          font-size: 15px;
          color: #6b7f94;
          line-height: 1.7;
          max-width: 380px;
          font-weight: 300;
        }

        /* Stats */
        .login-stats {
          display: flex;
          gap: 32px;
          margin-top: 48px;
          position: relative;
          z-index: 1;
        }

        .login-stat {
          display: flex;
          flex-direction: column;
          gap: 4px;
        }

        .login-stat-num {
          font-family: 'Syne', sans-serif;
          font-size: 28px;
          font-weight: 700;
          color: #f0f4f8;
        }

        .login-stat-label {
          font-size: 12px;
          color: #4a5f72;
          text-transform: uppercase;
          letter-spacing: 0.06em;
        }

        .login-stat-divider {
          width: 1px;
          background: rgba(255,255,255,0.08);
          align-self: stretch;
        }

        /* Panel derecho — formulario */
        .login-panel-right {
          width: 100%;
          max-width: 480px;
          margin: 0 auto;
          display: flex;
          align-items: center;
          justify-content: center;
          padding: 32px 24px;
          position: relative;
          z-index: 1;
        }

        @media (min-width: 1024px) {
          .login-panel-right {
            width: 480px;
            margin: 0;
            border-left: 1px solid rgba(255,255,255,0.05);
          }
        }

        .login-form-wrap {
          width: 100%;
          max-width: 380px;
          animation: login-slide-up 0.5s cubic-bezier(0.16, 1, 0.3, 1) both;
        }

        @keyframes login-slide-up {
          from { opacity: 0; transform: translateY(24px); }
          to { opacity: 1; transform: translateY(0); }
        }

        .login-form-header {
          margin-bottom: 36px;
        }

        .login-form-eyebrow {
          font-size: 11px;
          font-weight: 500;
          color: #00c6ff;
          letter-spacing: 0.12em;
          text-transform: uppercase;
          margin-bottom: 10px;
        }

        .login-form-title {
          font-family: 'Syne', sans-serif;
          font-size: 28px;
          font-weight: 700;
          color: #f0f4f8;
          margin-bottom: 8px;
          letter-spacing: -0.01em;
        }

        .login-form-desc {
          font-size: 14px;
          color: #4a5f72;
          font-weight: 300;
        }

        /* Campo */
        .login-field {
          margin-bottom: 16px;
        }

        .login-field label {
          display: block;
          font-size: 12px;
          font-weight: 500;
          color: #6b7f94;
          letter-spacing: 0.06em;
          text-transform: uppercase;
          margin-bottom: 8px;
        }

        .login-field-wrap {
          position: relative;
        }

        .login-field-icon {
          position: absolute;
          left: 14px;
          top: 50%;
          transform: translateY(-50%);
          color: #4a5f72;
          transition: color 0.2s;
          pointer-events: none;
          display: flex;
        }

        .login-field-wrap:focus-within .login-field-icon {
          color: #00c6ff;
        }

        .login-input {
          width: 100%;
          background: rgba(255,255,255,0.04);
          border: 1px solid rgba(255,255,255,0.08);
          border-radius: 10px;
          padding: 13px 14px 13px 42px;
          color: #f0f4f8;
          font-size: 14px;
          font-family: 'DM Sans', sans-serif;
          transition: all 0.2s;
          outline: none;
        }

        .login-input::placeholder { color: #374d61; }

        .login-input:focus {
          border-color: rgba(0, 198, 255, 0.4);
          background: rgba(0, 198, 255, 0.04);
          box-shadow: 0 0 0 3px rgba(0, 198, 255, 0.08);
        }

        .login-input-pr { padding-right: 44px; }

        .login-eye-btn {
          position: absolute;
          right: 12px;
          top: 50%;
          transform: translateY(-50%);
          background: none;
          border: none;
          color: #4a5f72;
          cursor: pointer;
          padding: 4px;
          display: flex;
          align-items: center;
          transition: color 0.2s;
        }

        .login-eye-btn:hover { color: #00c6ff; }

        /* Botón submit */
        .login-btn {
          width: 100%;
          padding: 14px;
          border-radius: 10px;
          border: none;
          font-family: 'Syne', sans-serif;
          font-size: 15px;
          font-weight: 600;
          letter-spacing: 0.02em;
          cursor: pointer;
          transition: all 0.2s;
          margin-top: 8px;
          position: relative;
          overflow: hidden;
        }

        .login-btn-active {
          background: linear-gradient(135deg, #00c6ff 0%, #0072ff 100%);
          color: #fff;
          box-shadow: 0 4px 24px rgba(0, 114, 255, 0.3);
        }

        .login-btn-active:hover {
          transform: translateY(-1px);
          box-shadow: 0 8px 32px rgba(0, 114, 255, 0.4);
        }

        .login-btn-active:active { transform: translateY(0); }

        .login-btn-disabled {
          background: rgba(255,255,255,0.06);
          color: #374d61;
          cursor: not-allowed;
        }

        .login-btn-loading {
          display: flex;
          align-items: center;
          justify-content: center;
          gap: 8px;
        }

        .login-spinner {
          width: 16px;
          height: 16px;
          border: 2px solid rgba(255,255,255,0.3);
          border-top-color: #fff;
          border-radius: 50%;
          animation: login-spin 0.7s linear infinite;
        }

        @keyframes login-spin { to { transform: rotate(360deg); } }

        /* Error */
        .login-error-box {
          display: flex;
          align-items: center;
          gap: 10px;
          background: rgba(255, 59, 59, 0.08);
          border: 1px solid rgba(255, 59, 59, 0.2);
          border-radius: 10px;
          padding: 12px 14px;
          margin-top: 16px;
          animation: login-slide-up 0.3s ease;
        }

        .login-error-dot {
          width: 6px;
          height: 6px;
          border-radius: 50%;
          background: #ff4d4d;
          flex-shrink: 0;
        }

        .login-error-text {
          font-size: 13px;
          color: #ff7070;
          font-weight: 400;
        }

        /* Fondo móvil */
        .login-bg-mobile {
          position: fixed;
          inset: 0;
          background: radial-gradient(ellipse at 20% 20%, rgba(0,198,255,0.06) 0%, transparent 50%),
                      radial-gradient(ellipse at 80% 80%, rgba(0,114,255,0.05) 0%, transparent 50%);
          pointer-events: none;
          z-index: 0;
        }

        .login-divider {
          display: flex;
          align-items: center;
          gap: 12px;
          margin: 24px 0;
        }

        .login-divider-line {
          flex: 1;
          height: 1px;
          background: rgba(255,255,255,0.06);
        }

        .login-divider-text {
          font-size: 11px;
          color: #2d4255;
          text-transform: uppercase;
          letter-spacing: 0.1em;
        }

        .login-footer {
          text-align: center;
          margin-top: 28px;
          font-size: 12px;
          color: #2d4255;
        }
      `}</style>

      <div className="login-root">
        <div className="login-bg-mobile" />

        {/* Panel izquierdo */}
        <div className="login-panel-left">
          <div className="login-grid" />
          <div className="login-orb-1" />
          <div className="login-orb-2" />
          <div className="login-lines" />

          <div className="login-brand">
            <div className="login-brand-badge">
              <div className="login-brand-badge-dot" />
              <span className="login-brand-badge-text">Sistema activo</span>
            </div>

            <h1 className="login-title">
              Gestión<br />
              <span>Inteligente</span><br />
              de Inventario
            </h1>

            <p className="login-subtitle">
              Plataforma integral para el control de compras, ventas, bodega y recursos humanos de tu empresa.
            </p>

            <div className="login-stats">

              <div className="login-stat">
                <span className="login-stat-num">24/7</span>
                <span className="login-stat-label">Disponible</span>
              </div>
              <div className="login-stat-divider" />
              <div className="login-stat">
                <span className="login-stat-num">100%</span>
                <span className="login-stat-label">Seguro</span>
              </div>
            </div>
          </div>
        </div>

        {/* Panel derecho — formulario */}
        <div className="login-panel-right">
          <div className="login-form-wrap">
            <div className="login-form-header">
              <p className="login-form-eyebrow">Acceso al sistema</p>
              <h2 className="login-form-title">Bienvenido</h2>
              <p className="login-form-desc">Ingresa tus credenciales para continuar</p>
            </div>

            <form onSubmit={handleLogin}>
              {/* Usuario */}
              <div className="login-field">
                <label htmlFor="login-usuario">Usuario</label>
                <div className="login-field-wrap">
                  <span className="login-field-icon" aria-hidden="true">
                    <svg width="16" height="16" fill="none" stroke="currentColor" viewBox="0 0 24 24" strokeWidth={1.8}>
                      <path strokeLinecap="round" strokeLinejoin="round" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                    </svg>
                  </span>
                  <input
                    id="login-usuario"
                    name="username"
                    type="text"
                    value={usuario}
                    onChange={e => setUsuario(e.target.value)}
                    placeholder="Nombre de usuario"
                    className="login-input"
                    autoComplete="username"
                    required
                  />
                </div>
              </div>

              {/* Contraseña */}
              <div className="login-field">
                <label htmlFor="login-contrasena">Contraseña</label>
                <div className="login-field-wrap">
                  <span className="login-field-icon" aria-hidden="true">
                    <svg width="16" height="16" fill="none" stroke="currentColor" viewBox="0 0 24 24" strokeWidth={1.8}>
                      <path strokeLinecap="round" strokeLinejoin="round" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                    </svg>
                  </span>
                  <input
                    id="login-contrasena"
                    name="password"
                    type={mostrarContrasena ? 'text' : 'password'}
                    value={contrasena}
                    onChange={e => setContrasena(e.target.value)}
                    placeholder="••••••••"
                    className="login-input login-input-pr"
                    autoComplete="current-password"
                    required
                  />
                  <button
                    type="button"
                    onClick={() => setMostrarContrasena(!mostrarContrasena)}
                    className="login-eye-btn"
                    aria-label={mostrarContrasena ? 'Ocultar contraseña' : 'Mostrar contraseña'}
                  >
                    {mostrarContrasena ? (
                      <svg width="16" height="16" fill="none" stroke="currentColor" viewBox="0 0 24 24" strokeWidth={1.8}>
                        <path strokeLinecap="round" strokeLinejoin="round" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
                      </svg>
                    ) : (
                      <svg width="16" height="16" fill="none" stroke="currentColor" viewBox="0 0 24 24" strokeWidth={1.8}>
                        <path strokeLinecap="round" strokeLinejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                        <path strokeLinecap="round" strokeLinejoin="round" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                      </svg>
                    )}
                  </button>
                </div>
              </div>

              {/* Botón */}
              <button
                type="submit"
                disabled={cargando || !usuario || !contrasena}
                className={`login-btn ${cargando || !usuario || !contrasena ? 'login-btn-disabled' : 'login-btn-active'}`}>
                {cargando ? (
                  <span className="login-btn-loading">
                    <span className="login-spinner" />
                    Verificando...
                  </span>
                ) : 'Iniciar sesión'}
              </button>

              {/* Error */}
              {error && (
                <div className="login-error-box">
                  <div className="login-error-dot" />
                  <span className="login-error-text">{error}</span>
                </div>
              )}
            </form>

            <div className="login-divider">
              <div className="login-divider-line" />
              <span className="login-divider-text">NicaTek</span>
              <div className="login-divider-line" />
            </div>

            <p className="login-footer">
              Sistema de Gestión Empresarial &copy; {new Date().getFullYear()}
            </p>
          </div>
        </div>
      </div>
    </>
  );
};

export default Login;