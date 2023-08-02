using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace AppHospital
{
    /// <summary>
    /// Lógica de interacción para Paciente.xaml
    /// </summary>
    public partial class Paciente : Window
    {
        private SqlConnection miConexionSql;  // nivel de acceso a private para encapsulación

        int idPacienteSelect;

        public Paciente()
        {
            InitializeComponent();

            // Establecer la ubicación de inicio de la ventana en el centro de la pantalla
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //this.Width = 650;
            //this.Height = 1000;

            InicializarConexion();
        }

        // Constructor que recibe una conexión SQL
        public Paciente(SqlConnection miConexionSql)
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

        private void Mostrar()
        {
            string consulta = "SELECT ID, Nombre FROM Paciente";

            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(consulta, miConexionSql);

            using (miAdaptadorSql)
            {
                DataTable pacientesTabla = new DataTable();
                miAdaptadorSql.Fill(pacientesTabla);
                lstPacientes.DisplayMemberPath = "Nombre";
                lstPacientes.SelectedValuePath = "ID";
                lstPacientes.ItemsSource = pacientesTabla.DefaultView;
            }
        }

        private void Registrar()
        {
            try
            {
                string nombre = txtNombre.Text;
                string apellido1 = txtPrimerApellido.Text;
                string apellido2 = txtSegundoApellido.Text;
                string fechaNacimiento = dpFechaNamcimiento.Text;
                string direccion = txtDireccion.Text;
                string telefono = txtTelefono.Text;
                string email = txtEmail.Text;

                string consulta = "INSERT INTO Paciente (Nombre, Apellido1, Apellido2, Fecha_Nacimiento, Direccion, Telefono, Email) " +
                                  $"VALUES ('{nombre}', '{apellido1}', '{apellido2}', '{fechaNacimiento}'," +
                                  $" '{direccion}', '{telefono}', '{email}');";

                SqlCommand miComandoSql = new SqlCommand(consulta, miConexionSql);
                miConexionSql.Open();
                miComandoSql.ExecuteNonQuery();
                miConexionSql.Close();

                Mostrar();
                MessageBox.Show("Paciente insertado correctamente!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar el paciente: " + ex.Message);
            }
        }

        private void Eliminar()
        {
            if (lstPacientes.SelectedIndex != -1)
            {
                try
                {
                    // cambiar por ID=@idPaciente
                    string consulta = "DELETE FROM Paciente WHERE ID=4";

                    SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);
                    miConexionSql.Open();

                    //miSqlCommand.Parameters.AddWithValue("@paciente", lstPacientes.SelectedValue);
                    miSqlCommand.ExecuteNonQuery();

                    miConexionSql.Close();
                    MessageBox.Show("Paciente eliminado!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("No has seleccionado un paciente para borrar.");
            }
        }

        // Función que rellena los campos para después actualizar un nuevo dato
        // La sobrecarga sobre ya que idPacienteSelect = lstPacientes.SelectedValue
        public void RellenarDatos()
        {
            string consulta = "SELECT Nombre, Apellido1, Apellido2, Fecha_Nacimiento, Direccion, Telefono, Email FROM Paciente " +
                "WHERE ID=@idPaciente;";

            SqlCommand comandoSQL = new SqlCommand(consulta, miConexionSql);

            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(comandoSQL);

            using (miAdaptadorSql)
            {
                comandoSQL.Parameters.AddWithValue("@idPaciente", lstPacientes.SelectedValue);
                DataTable pacientesTabla = new DataTable();
                miAdaptadorSql.Fill(pacientesTabla);

                txtNombre.Text = pacientesTabla.Rows[0]["Nombre"].ToString();
                txtPrimerApellido.Text = pacientesTabla.Rows[0]["Apellido1"].ToString();
                txtSegundoApellido.Text = pacientesTabla.Rows[0]["Apellido2"].ToString();
                // DateTime fechaNacimiento = Convert.ToDateTime(pacientesTabla.Rows[0]["Fecha_Nacimiento"]);
                dpFechaNamcimiento.SelectedDate = Convert.ToDateTime(pacientesTabla.Rows[0]["Fecha_Nacimiento"]);
                txtDireccion.Text = pacientesTabla.Rows[0]["Direccion"].ToString();
                txtTelefono.Text = pacientesTabla.Rows[0]["Telefono"].ToString();
                txtEmail.Text = pacientesTabla.Rows[0]["Email"].ToString();
            }
        }

        // Función para actualizar o modificar un registro de la tabla
        private void Modificar()
        {
            try
            {
                string nombre = txtNombre.Text;
                string apellido1 = txtPrimerApellido.Text;
                string apellido2 = txtSegundoApellido.Text;
                string fechaNacimiento = dpFechaNamcimiento.Text;
                string direccion = txtDireccion.Text;
                string telefono = txtTelefono.Text;
                string email = txtEmail.Text;

                string query = $"UPDATE Paciente SET Nombre='{nombre}', Apellido1='{apellido1}', Apellido2='{apellido2}', " +
                    $"Fecha_Nacimiento='{fechaNacimiento}', Direccion='{direccion}', Telefono='{telefono}', Email='{email}' WHERE ID = @idPaciente;";

                miConexionSql.Open();
                using (SqlCommand command = new SqlCommand(query, miConexionSql))
                {
                    command.Parameters.AddWithValue("@idPaciente", lstPacientes.SelectedValue);
                    command.ExecuteNonQuery();
                }
                miConexionSql.Close();

                MessageBox.Show("Paciente modificado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar el paciente: " + ex.Message);
            }
        }

        // Métodos para botones
        private void btnRegistrar(object sender, RoutedEventArgs e)
        {
            Registrar();
        }

        private void btnMostrar(object sender, RoutedEventArgs e)
        {
            Mostrar();
        }

        private void btnEliminar(object sender, RoutedEventArgs e)
        {
            Eliminar();
        }

        private void btnModificar(object sender, RoutedEventArgs e)
        {
            // idPacienteSelect = int.Parse(lstPacientes.SelectedValue.ToString());
            Modificar();
        }

        private void btnVolver(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow(miConexionSql);
            main.Show();
            this.Close();
        }

        // Este botón se puede eliminar ya que se añade la función de rellenar campos al clickar sobre el listado
        /*
        private void btnRellenar(object sender, RoutedEventArgs e)
        {
            idPacienteSelect = int.Parse(lstPacientes.SelectedValue.ToString());

            RellenarDatos(idPacienteSelect);
        }
        */

        // Métodos para selección de elementos
        private void lstPacientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // idPacienteSelect = int.Parse(lstPacientes.SelectedValue.ToString());
            RellenarDatos();
        }
    }
}
