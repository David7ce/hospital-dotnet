using System.Configuration;

namespace AppHospital
{
    internal class ConexionSQL
    {
        public ConexionSQL() { }

        public string crearConexion()
        {
            // cambiar la frase de conexión según el equipo, en computadora de casa es con un 1 al final
            return ConfigurationManager.ConnectionStrings["AppHospital.Properties.Settings.HospitalDBConnectionString"].ConnectionString;
        }
    }
}
