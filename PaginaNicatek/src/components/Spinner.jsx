const Spinner = () => {  // Componente Spinner simple
  return (
    <div className="flex justify-center items-center">    
      <div 
        className="w-10 h-10 border-4 border-t-4 border-blue-500 border-opacity-50 rounded-full animate-spin"
        style={{ borderTopColor: 'transparent' }}
      ></div>
    </div>
  );
};

export default Spinner;
