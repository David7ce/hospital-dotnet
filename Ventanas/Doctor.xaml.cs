using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace AppHospital
{
    /// <summary>
    /// Lógica de interacción para Doctor.xaml
    /// </summary>
    public partial class Doctor : Window
    {
        private SqlConnection miConexionSql;  // nivel de acceso a private para encapsulación

        public Doctor()
        {
            InitializeComponent();

            // Establecer la ubicación de inicio de la ventana en el centro de la pantalla
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //this.Width = 650;
            //this.Height = 1000;

            InicializarConexion();
        }

        // Constructor que recibe una conexión SQL
        public Doctor(SqlConnection miConexionSql)
        {
            InitializeComponent();
            this.miConexionSql = miConexionSql;

            MostrarEspecialidad();
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

        private void Mostrar()
        {
            string consulta = "SELECT ID, CONCAT(Nombre, ' ' ,Apellido1, ' ' ,Apellido2) AS infoDoctor FROM Doctor";

            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(consulta, miConexionSql);

            using (miAdaptadorSql)
            {
                DataTable doctoresTabla = new DataTable();
                miAdaptadorSql.Fill(doctoresTabla);
                lstDoctores.DisplayMemberPath = "infoDoctor";
                lstDoctores.SelectedValuePath = "ID";
                lstDoctores.ItemsSource = doctoresTabla.DefaultView;
            }
        }

        private void MostrarEspecialidad()
        {
            try
            {
                string consulta = "SELECT ID, Nombre FROM Especialidad";

                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(consulta, miConexionSql);

                using (miAdaptadorSql)
                {
                    DataTable especialidadTabla = new DataTable();
                    miAdaptadorSql.Fill(especialidadTabla);
                    cmbEspecialidad.DisplayMemberPath = "Nombre";
                    cmbEspecialidad.SelectedValuePath = "ID";
                    cmbEspecialidad.ItemsSource = especialidadTabla.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar Especialidad: " + ex.Message);
            }
        }

        private void Registrar()
        {
            try
            {
                string nombre = txtNombre.Text;
                string apellido1 = txtApellido1.Text;
                string apellido2 = txtApellido2.Text;
                string idEspecialidad = cmbEspecialidad.SelectedValue.ToString();

                string consulta = "INSERT INTO Doctor (Nombre, Apellido1, Apellido2, ID_Especialidad) " +
                                  $"VALUES ('{nombre}', '{apellido1}', '{apellido2}', {idEspecialidad})";

                SqlCommand miComandoSql = new SqlCommand(consulta, miConexionSql);
                miConexionSql.Open();
                miComandoSql.ExecuteNonQuery();
                miConexionSql.Close();

                MessageBox.Show("Doctor insertado correctamente!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar el doctor: " + ex.Message);
            }
        }

        private void Eliminar()
        {
            try
            {
                string consulta = "DELETE FROM Doctor WHERE ID=@id_doctor";

                SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);
                miConexionSql.Open();

                miSqlCommand.Parameters.AddWithValue("@id_doctor", lstDoctores.SelectedValue);
                miSqlCommand.ExecuteNonQuery();

                miConexionSql.Close();

                Mostrar();

                MessageBox.Show("Doctor eliminado!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }

        private void RellenarDatos()
        {
            string consulta = "SELECT Nombre, Apellido1, Apellido2, ID_Especialidad FROM Doctor " +
                "WHERE ID = @idDoctor;";

            SqlCommand comandoSQL = new SqlCommand(consulta, miConexionSql);
            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(comandoSQL);

            using (miAdaptadorSql)
            {
                comandoSQL.Parameters.AddWithValue("@idDoctor", lstDoctores.SelectedValue);

                DataTable doctoresTabla = new DataTable();
                miAdaptadorSql.Fill(doctoresTabla);

                txtNombre.Text = doctoresTabla.Rows[0]["Nombre"].ToString();
                txtApellido1.Text = doctoresTabla.Rows[0]["Apellido1"].ToString();
                txtApellido2.Text = doctoresTabla.Rows[0]["Apellido2"].ToString();
                cmbEspecialidad.SelectedValue = Convert.ToInt32(doctoresTabla.Rows[0]["ID_Especialidad"]);
            }
        }

        private void Modificar()
        {
            try
            {
                string nombre = txtNombre.Text;
                string apellido1 = txtApellido1.Text;
                string apellido2 = txtApellido2.Text;

                string consulta = $"UPDATE Doctor SET Nombre='{nombre}', Apellido1='{apellido1}', Apellido2='{apellido2}' " +
                    $"WHERE ID = @idDoctor;";

                using (SqlCommand command = new SqlCommand(consulta, miConexionSql))
                {
                    miConexionSql.Open();
                    command.Parameters.AddWithValue("@idDoctor", lstDoctores.SelectedValue);
                    command.ExecuteNonQuery();
                    miConexionSql.Close();
                }
                MessageBox.Show("Doctor modificado.");
                Mostrar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar el doctor: " + ex.Message);
            }
        }

        private void btnRegistrar(object sender, RoutedEventArgs e)
        {
            Registrar();
        }

        private void btnMostrar(object sender, RoutedEventArgs e)
        {
            Mostrar();
        }

        private void btnModificar(object sender, RoutedEventArgs e)
        {
            Modificar();
        }

        private void btnEliminar(object sender, RoutedEventArgs e)
        {
            Eliminar();
        }

        private void btnVolver(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow(miConexionSql);
            main.Show();
            this.Close();
        }

        // Métodos para selección de elementos
        private void lstDoctores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RellenarDatos();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
