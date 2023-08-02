using AppHospital.Ventanas;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace AppHospital
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
         * Ventana Doctor:
         *    - [x] Mostrar Doctor
         *    - [x] Agregar Doctor
         *    - [x] Eliminar Doctor
         *    - [x] Modificar Doctor
         *      
         *  Ventana Paciente:
         *    - [x] Mostrar Paciente
         *    - [x] Agregar Paciente
         *    - [x] Eliminar Paciente
         *    - [x] Modificar Paciente
         *      
         * Ventana Consulta:
         *    - [x] Mostrar Paciente/Doctor
         *    - [x] Seleccionar Paciente/Doctor y Mostrar Consulta 
         *    - [x] Agregar Consulta
         *    - [x] Eliminar Consulta
         *    - [x] Modificar Consulta
         * 
         * Ventana Tratamiento:
         *    - [x] Mostrar Paciente/Doctor
         *    - [x] Seleccionar Paciente/Doctor y Mostrar Tratamiento
         *    - [x] Agregar Tratamiento
         *    - [x] Eliminar Tratamiento
         *    - [x] Modificar Tratamiento
         * 
         * Ventana Hospitalizacion:
         *    - [x] Mostrar Paciente/Doctor
         *    - [x] Seleccionar Paciente/Doctor y Mostrar Hospitalizacion
         *    - [x] Agregar Hospitalizacion
         *    - [x] Eliminar Hospitalizacion
         *    - [x] Modificar Hospitalizacion
         *    
         * Ventana Enfermeria:
         *    - [x] Mostrar Supervisor(Doctor)
         *    - [ ] Seleccionar Supervisor(Doctor) y Mostrar Enfermero/a
         *    - [x] Agregar Enfermero/a
         *    - [x] Eliminar Enfermero/a
         *    - [x] Modificar Enfermero/a
         */


        private SqlConnection miConexionSql;

        public MainWindow()
        {
            InitializeComponent();

            // Establecer la ubicación de inicio de la ventana en el centro de la pantalla
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // this.Width = 650;
            // this.Height = 1000;

            InicializarConexion();
        }

        // Constructor que recibe una conexión SQL
        public MainWindow(SqlConnection miConexionSql)
        {
            InitializeComponent();
            this.miConexionSql = miConexionSql;
        }

        private void InicializarConexion()
        {
            try
            {
                ConexionSQL conexionSQL = new ConexionSQL();
                string connectionString = conexionSQL.crearConexion();
                miConexionSql = new SqlConnection(connectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar a la base de datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDoctor_Click(object sender, RoutedEventArgs e)
        {
            Doctor doctor = new Doctor(miConexionSql);
            doctor.Show();
            this.Close();
        }

        private void btnPaciente_Click(object sender, RoutedEventArgs e)
        {
            Paciente paciente = new Paciente(miConexionSql);
            paciente.Show();
            this.Close();
        }

        private void btnConsulta_Click(object sender, RoutedEventArgs e)
        {
            Consulta consulta = new Consulta(miConexionSql);
            consulta.Show();
            this.Close();
        }

        private void btnTratamiento_Click(object sender, RoutedEventArgs e)
        {
            Tratamiento tratamiento = new Tratamiento(miConexionSql);
            tratamiento.Show();
            this.Close();
        }

        private void btnHospitalizacion_Click(object sender, RoutedEventArgs e)
        {
            Hospitalizacion hospitalizacion = new Hospitalizacion(miConexionSql);
            hospitalizacion.Show();
            this.Close();
        }

        private void btnEnfermeria_Click(object sender, RoutedEventArgs e)
        {
            Enfermeria enfermeria = new Enfermeria(miConexionSql);
            enfermeria.Show();
            this.Close();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        // Métodos de selección
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
