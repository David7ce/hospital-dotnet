using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace AppHospital
{
    /// <summary>
    /// Lógica de interacción para Hospitalizacion.xaml
    /// </summary>
    public partial class Hospitalizacion : Window
    {
        private SqlConnection miConexionSql;

        // Variable de eleccion, entre Paciente y Doctor
        string eleccion = "";

        // Constructor sin parámetro de entrada
        public Hospitalizacion()
        {
            InitializeComponent();

            // Establecer la ubicación de inicio de la ventana en el centro de la pantalla
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            InicializarConexion();
        }

        // Constructor que recibe una conexión SQL
        public Hospitalizacion(SqlConnection miConexionSql)
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
                // Manejar cualquier excepción que ocurra durante la conexión o consulta a la base de datos
                MessageBox.Show("Error al conectar a la base de datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Métodos CRUD
        private void Registrar()
        {
            try
            {
                string idPaciente = txtIdPaciente.Text;
                string idDoctor = txtIdDoctor.Text;
                string habitacion = txtHabitacion.Text;
                string cama = txtCama.Text;
                string fechaIngreso = dpFechaIngreso.Text;
                string fechaAlta = dpFechaAlta.Text;

                string consulta = "INSERT INTO Hospitalizacion (ID_paciente, ID_DoctorResponsable, Habitacion, Cama, Fecha_ingreso, Fecha_alta) " +
                                  $"VALUES ('{idPaciente}', '{idDoctor}', '{habitacion}', '{cama}', '{fechaIngreso}','{fechaAlta}')";

                SqlCommand miComandoSql = new SqlCommand(consulta, miConexionSql);
                miConexionSql.Open();
                miComandoSql.ExecuteNonQuery();
                miConexionSql.Close();
                MessageBox.Show("Hospitalización insertado correctamente!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar la hospitalización: " + ex.Message);
            }
        }

        private void MostrarPaciente()
        {
            eleccion = "Paciente";

            string consulta = "SELECT ID, CONCAT(Nombre, ' ' ,Apellido1, ' ' ,Apellido2)"
                            + " AS infoPaciente FROM Paciente";

            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(consulta, miConexionSql);

            using (miAdaptadorSql)
            {
                DataTable pacientesTabla = new DataTable();
                miAdaptadorSql.Fill(pacientesTabla);
                lstboxPacienteDoctor.DisplayMemberPath = "infoPaciente";
                lstboxPacienteDoctor.SelectedValuePath = "ID";
                lstboxPacienteDoctor.ItemsSource = pacientesTabla.DefaultView;
            }
        }

        private void MostrarDoctor()
        {
            eleccion = "Doctor";

            string consulta = "SELECT ID, CONCAT(Nombre, ' ',Apellido1, ' ',Apellido2)"
                            + " AS infoDoctor FROM Doctor";

            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(consulta, miConexionSql);

            using (miAdaptadorSql)
            {
                DataTable doctoresTabla = new DataTable();
                miAdaptadorSql.Fill(doctoresTabla);
                lstboxPacienteDoctor.DisplayMemberPath = "infoDoctor";
                lstboxPacienteDoctor.SelectedValuePath = "ID";
                lstboxPacienteDoctor.ItemsSource = doctoresTabla.DefaultView;
            }
        }

        // Muestra las hospitalizaciones de pacientes y doctores
        private void Mostrar()
        {
            if (lstboxPacienteDoctor.SelectedIndex != -1)
            {
                string consulta = "";

                if (eleccion == "Paciente")
                {
                    consulta = "SELECT ID, CONCAT(Habitacion, ' ', Cama, ' ', Fecha_Ingreso, ' ', Fecha_Alta) "
                    + "AS infoHospitalizacion FROM Hospitalizacion WHERE ID_Paciente = @id_mostrado;";
                }
                else if (eleccion == "Doctor")
                {
                    consulta = "SELECT ID, CONCAT(Habitacion, ' ', Cama, ' ', Fecha_Ingreso, ' ', Fecha_Alta) "
                    + "AS infoHospitalizacion FROM Hospitalizacion WHERE ID_DoctorResponsable = @id_mostrado;";
                }

                SqlCommand sqlComando = new SqlCommand(consulta, miConexionSql);
                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(sqlComando);

                using (miAdaptadorSql)
                {
                    sqlComando.Parameters.AddWithValue("@id_mostrado", lstboxPacienteDoctor.SelectedValue);

                    DataTable hospitalizacionTabla = new DataTable();
                    miAdaptadorSql.Fill(hospitalizacionTabla);

                    lstboxHospitalizacion.DisplayMemberPath = "infoHospitalizacion";
                    lstboxHospitalizacion.SelectedValuePath = "ID";
                    lstboxHospitalizacion.ItemsSource = hospitalizacionTabla.DefaultView;
                }
            }
        }

        private void Eliminar()
        {
            // MessageBox.Show(lstboxHospitalizacion.SelectedValue.ToString());

            string consulta = "DELETE FROM Hospitalizacion WHERE ID=@id_borrado";

            SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

            miConexionSql.Open();
            miSqlCommand.Parameters.AddWithValue("@id_borrado", lstboxHospitalizacion.SelectedValue);
            miSqlCommand.ExecuteNonQuery();
            miConexionSql.Close();

            Mostrar();
            MessageBox.Show("Hospitalización eliminada!");
        }

        private void RellenarDatos()
        {
            string consulta = "SELECT ID_Paciente, ID_DoctorResponsable, Habitacion, Cama, Fecha_Ingreso, Fecha_alta " +
                "FROM Hospitalizacion WHERE ID=@idRelleno;";

            SqlCommand comandoSQL = new SqlCommand(consulta, miConexionSql);

            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(comandoSQL);

            using (miAdaptadorSql)
            {
                comandoSQL.Parameters.AddWithValue("@idRelleno", lstboxHospitalizacion.SelectedValue);
                DataTable hospTabla = new DataTable();
                miAdaptadorSql.Fill(hospTabla);

                txtIdPaciente.Text = hospTabla.Rows[0]["ID_Paciente"].ToString();
                txtIdDoctor.Text = hospTabla.Rows[0]["ID_DoctorResponsable"].ToString();
                txtHabitacion.Text = hospTabla.Rows[0]["Habitacion"].ToString();
                txtCama.Text = hospTabla.Rows[0]["Cama"].ToString();
                dpFechaIngreso.SelectedDate = Convert.ToDateTime(hospTabla.Rows[0]["Fecha_Ingreso"]);
                dpFechaAlta.SelectedDate = Convert.ToDateTime(hospTabla.Rows[0]["Fecha_Ingreso"]);
            }
        }

        private void Modificar()
        {
            try
            {
                string idDoctor = txtIdPaciente.Text;
                string habitacion = txtHabitacion.Text;
                string cama = txtCama.Text;
                string fechaIngreso = dpFechaIngreso.Text;
                string fechaAlta = dpFechaAlta.Text;

                string consulta = $"UPDATE Hospitalizacion SET ID_DoctorResponsable = '{idDoctor}', Habitacion = '{habitacion}', " +
                               $"Cama = '{cama}', Fecha_Ingreso = '{fechaIngreso}', Fecha_alta = '{fechaAlta}' WHERE ID = @idHosp;";

                miConexionSql.Open();
                using (SqlCommand command = new SqlCommand(consulta, miConexionSql))
                {

                    command.Parameters.AddWithValue("@idHosp", lstboxHospitalizacion.SelectedValue);
                    command.ExecuteNonQuery();
                }
                miConexionSql.Close();

                MessageBox.Show("Hospitalización modificada.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar la hospitalización: " + ex.Message);
            }
        }

        // Métodos de botones
        private void btnRegistrar(object sender, RoutedEventArgs e)
        {
            Registrar();
        }

        // Métodos para botones
        private void btnModificar(object sender, RoutedEventArgs e)
        {
            Modificar();
        }

        private void btnEliminar(object sender, RoutedEventArgs e)
        {
            Eliminar();
        }

        private void btnMostrarPaciente(object sender, RoutedEventArgs e)
        {
            MostrarPaciente();
        }

        private void btnMostrarDoctor(object sender, RoutedEventArgs e)
        {
            MostrarDoctor();
        }

        private void btnVolver(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow(miConexionSql);
            main.Show();
            this.Close();
        }

        // Métodos SelectionChanged para listbox
        private void lstboxPacienteDoctor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mostrar();
        }

        private void lstboxHospitalizacion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RellenarDatos();
        }
    }
}
