import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowLeft, Save } from 'lucide-react';
import api from '../../api/axios.Config';

const AgregarCategoria = () => {
    const navigate = useNavigate();
    const [nombre, setNombre] = useState('');
    const [cargando, setCargando] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setCargando(true);

        try {
            // Estructura requerida por tu API según el Swagger
            const nuevaCategoria = {
                id_Categoria: 0,
                nombre: nombre,
                id_Estado: 1, // Asumimos 1 como activo por defecto
                nombre_Estado: "Activo",
                fecha_Creacion: new Date().toISOString(),
                fecha_Modificacion: new Date().toISOString(),
                id_Creador: 1, // ID del usuario actual
                id_Modificador: 1
            };

            await api.post('/Categoria/NuevoCategoria', nuevaCategoria);
            navigate('/categorias');
        } catch (err) {
            console.error("Error al crear categoría:", err);
            alert("Error al guardar la categoría.");
        } finally {
            setCargando(false);
        }
    };

    return (
        <div className="p-6 max-w-2xl mx-auto">
            <button onClick={() => navigate('/categorias')} className="flex items-center text-gray-500 mb-6 hover:text-gray-800">
                <ArrowLeft className="mr-2" /> Volver
            </button>
            
            <h1 className="text-2xl font-bold mb-6">Agregar Nueva Categoría</h1>
            
            <form onSubmit={handleSubmit} className="bg-white p-6 rounded-lg shadow border">
                <div className="mb-4">
                    <label className="block text-sm font-medium mb-2">Nombre de la Categoría</label>
                    <input 
                        type="text" 
                        value={nombre}
                        onChange={(e) => setNombre(e.target.value)}
                        className="w-full p-2 border rounded-lg"
                        required
                    />
                </div>
                <button 
                    type="submit" 
                    disabled={cargando}
                    className="flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
                >
                    <Save className="mr-2" size={20} />
                    {cargando ? 'Guardando...' : 'Guardar Categoría'}
                </button>
            </form>
        </div>
    );
};

export default AgregarCategoria;