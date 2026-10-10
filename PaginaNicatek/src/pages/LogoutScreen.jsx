import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const LogoutScreen = () => {
  const navigate = useNavigate();
  const { cerrarSesion } = useAuth();
  const [progreso, setProgreso] = useState(0);

  // ── Leer el nombre ANTES de limpiar sessionStorage ────────────────────────
  const [nombre] = useState(() => sessionStorage.getItem('nombreUsuario') || 'Usuario');

  useEffect(() => {
    // Se usa cerrarSesion() y no sessionStorage.clear() a secas: hay que
    // vaciar tambien el estado de AuthContext. Si no, el contexto seguiria
    // creyendo que hay sesion y <SoloInvitados> rebotaria /login de vuelta
    // a /overview, dejando al usuario dando vueltas sin poder salir.
    cerrarSesion();

    const intervalo = setInterval(() => {
      setProgreso(prev => {
        if (prev >= 100) { clearInterval(intervalo); return 100; }
        return prev + 4;
      });
    }, 60);

    const timer = setTimeout(() => {
      navigate('/login');
    }, 1600);

    return () => { clearTimeout(timer); clearInterval(intervalo); };
  }, [navigate, cerrarSesion]);

  return (
    <>
      <style>{`
        @import url('https://fonts.googleapis.com/css2?family=Syne:wght@700;800&family=DM+Sans:wght@300;400&display=swap');

        .logout-root {
          min-height: 100vh;
          display: flex;
          align-items: center;
          justify-content: center;
          background: #080e14;
          font-family: 'DM Sans', sans-serif;
          position: relative;
          overflow: hidden;
        }

        .logout-grid {
          position: absolute;
          inset: 0;
          background-image:
            linear-gradient(rgba(0,198,255,0.03) 1px, transparent 1px),
            linear-gradient(90deg, rgba(0,198,255,0.03) 1px, transparent 1px);
          background-size: 48px 48px;
          pointer-events: none;
        }

        .logout-orb {
          position: absolute;
          width: 600px;
          height: 600px;
          border-radius: 50%;
          background: radial-gradient(circle, rgba(0,198,255,0.07) 0%, transparent 70%);
          top: 50%;
          left: 50%;
          transform: translate(-50%, -50%);
          pointer-events: none;
        }

        .logout-card {
          position: relative;
          z-index: 1;
          text-align: center;
          padding: 48px 40px;
          background: rgba(255,255,255,0.03);
          border: 1px solid rgba(255,255,255,0.07);
          border-radius: 20px;
          backdrop-filter: blur(12px);
          width: 360px;
          animation: logout-in 0.5s cubic-bezier(0.16,1,0.3,1) both;
        }

        @keyframes logout-in {
          from { opacity: 0; transform: translateY(16px) scale(0.98); }
          to   { opacity: 1; transform: translateY(0) scale(1); }
        }

        .logout-icon-wrap {
          width: 64px;
          height: 64px;
          border-radius: 50%;
          background: rgba(0,198,255,0.08);
          border: 1px solid rgba(0,198,255,0.2);
          display: flex;
          align-items: center;
          justify-content: center;
          margin: 0 auto 24px;
          animation: logout-pulse 1.6s ease-in-out infinite;
        }

        @keyframes logout-pulse {
          0%, 100% { box-shadow: 0 0 0 0 rgba(0,198,255,0.2); }
          50%       { box-shadow: 0 0 0 10px rgba(0,198,255,0); }
        }

        .logout-eyebrow {
          font-size: 11px;
          font-weight: 500;
          color: #00c6ff;
          letter-spacing: 0.12em;
          text-transform: uppercase;
          margin-bottom: 10px;
        }

        .logout-title {
          font-family: 'Syne', sans-serif;
          font-size: 22px;
          font-weight: 700;
          color: #f0f4f8;
          margin-bottom: 8px;
          letter-spacing: -0.01em;
        }

        .logout-title span {
          background: linear-gradient(135deg, #00c6ff, #0072ff);
          -webkit-background-clip: text;
          -webkit-text-fill-color: transparent;
          background-clip: text;
        }

        .logout-desc {
          font-size: 13px;
          color: #4a5f72;
          font-weight: 300;
          margin-bottom: 32px;
          line-height: 1.6;
        }

        .logout-progress-track {
          width: 100%;
          height: 3px;
          background: rgba(255,255,255,0.06);
          border-radius: 100px;
          overflow: hidden;
        }

        .logout-progress-fill {
          height: 100%;
          border-radius: 100px;
          background: linear-gradient(90deg, #00c6ff, #0072ff);
          transition: width 0.06s linear;
          box-shadow: 0 0 8px rgba(0,198,255,0.5);
        }

        .logout-footer {
          margin-top: 20px;
          font-size: 11px;
          color: #2d4255;
          letter-spacing: 0.04em;
        }
      `}</style>

      <div className="logout-root">
        <div className="logout-grid" />
        <div className="logout-orb" />

        <div className="logout-card">
          <div className="logout-icon-wrap">
            <svg width="26" height="26" fill="none" stroke="#00c6ff" viewBox="0 0 24 24" strokeWidth={1.8}>
              <path strokeLinecap="round" strokeLinejoin="round"
                d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
            </svg>
          </div>

          <p className="logout-eyebrow">Sesión finalizada</p>
          <h2 className="logout-title">Hasta pronto, <span>{nombre}</span></h2>
          <p className="logout-desc">
            Tu sesión ha sido cerrada de forma segura.<br />
            Redirigiendo al inicio de sesión...
          </p>

          <div className="logout-progress-track">
            <div className="logout-progress-fill" style={{ width: `${progreso}%` }} />
          </div>

          <p className="logout-footer">NicaTek · Sistema de Gestión Empresarial</p>
        </div>
      </div>
    </>
  );
};

export default LogoutScreen;