using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace AppHospital
{
    /// <summary>
    /// Lógica de interacción para Tratamiento.xaml
    /// </summary>
    public partial class Tratamiento : Window
    {
        string eleccion = "";

        private SqlConnection miConexionSql;

        public Tratamiento()
        {
            InitializeComponent();

            //this.Width = 650;
            //this.Height = 1000;

            InicializarConexion();
        }

        // Constructor que recibe una conexión SQL
        public Tratamiento(SqlConnection miConexionSql)
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

        // Métodos CRUD
        private void Registrar()
        {
            try
            {
                string idPaciente = txtIdPaciente.Text;
                string idDoctor = txtIdDoctor.Text;
                string habitacion = txtMedicamento.Text;
                string dosis = txtDosis.Text;
                string duracion = txtDuracion.Text;

                string consulta = "INSERT INTO Tratamiento (ID_paciente, ID_Doctor, Medicamento, Dosis, Duracion) " +
                                  $"VALUES ('{idPaciente}', '{idDoctor}', '{habitacion}', '{dosis}', '{duracion}')";

                SqlCommand miComandoSql = new SqlCommand(consulta, miConexionSql);
                miConexionSql.Open();
                miComandoSql.ExecuteNonQuery();
                miConexionSql.Close();
                MessageBox.Show("Tratamiento insertado correctamente!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar el doctor: " + ex.Message);
            }
        }

        private void MostrarPaciente()
        {
            eleccion = "Paciente";

            string consulta = "SELECT Id, CONCAT(ID, ' - ' ,Nombre, ' ' ,Apellido1, ' ' ,Apellido2)"
                            + " AS infoPaciente FROM Paciente";

            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(consulta, miConexionSql);

            using (miAdaptadorSql)
            {
                DataTable pacientesTabla = new DataTable();
                miAdaptadorSql.Fill(pacientesTabla);
                lstboxPacienteDoctor.DisplayMemberPath = "infoPaciente";
                lstboxPacienteDoctor.SelectedValuePath = "Id";
                lstboxPacienteDoctor.ItemsSource = pacientesTabla.DefaultView;
            }
        }

        private void MostrarDoctor()
        {
            eleccion = "Doctor";

            string consulta = "SELECT ID, CONCAT(ID, ' - ' ,Nombre, ' ' ,Apellido1, ' ' ,Apellido2)"
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

        // Muestra los tratamientos dados para pacientes y asociados a doctores
        private void Mostrar()
        {
            if (lstboxPacienteDoctor.SelectedIndex != -1)
            {
                string consulta = "";

                if (eleccion == "Paciente")
                {
                    consulta = "SELECT ID, CONCAT(Medicamento, ' ', Dosis, ' ', Duracion) "
                    + "AS infoTratamiento FROM Tratamiento WHERE id_paciente = @id_tratamiento;";
                }
                else if (eleccion == "Doctor")
                {
                    consulta = "SELECT ID, CONCAT(Medicamento, ' ', Dosis, ' ', Duracion) "
                    + "AS infoTratamiento FROM Tratamiento WHERE id_doctor = @id_tratamiento;";
                }

                SqlCommand sqlComando = new SqlCommand(consulta, miConexionSql);
                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(sqlComando);

                using (miAdaptadorSql)
                {
                    sqlComando.Parameters.AddWithValue("@id_tratamiento", lstboxPacienteDoctor.SelectedValue);

                    DataTable tratamientosTabla = new DataTable();
                    miAdaptadorSql.Fill(tratamientosTabla);

                    lstboxTratamiento.DisplayMemberPath = "infoTratamiento";
                    lstboxTratamiento.SelectedValuePath = "ID";
                    lstboxTratamiento.ItemsSource = tratamientosTabla.DefaultView;
                }
            }
        }

        private void Eliminar()
        {
            if (lstboxTratamiento.SelectedIndex != -1)
            {
                try
                {
                    string consulta = "DELETE FROM Tratamiento WHERE ID=@id_tratamiento";

                    SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);
                    miConexionSql.Open();

                    miSqlCommand.Parameters.AddWithValue("@id_tratamiento", lstboxTratamiento.SelectedValue);

                    miSqlCommand.ExecuteNonQuery();

                    miConexionSql.Close();

                    // Prara refrescar la pantalla de los mostrados
                    Mostrar();

                    MessageBox.Show("Tratamiento eliminado.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("No has seleccionado una consulta para borrar.");
            }
        }

        // Método para rellenar los TextBox y el DatePicker
        public void RellenarDatos()
        {
            string consulta = "SELECT ID_Paciente, ID_Doctor, Medicamento, Dosis, Duracion FROM Tratamiento " +
                "WHERE ID = @idTratamiento;";

            SqlCommand comandoSQL = new SqlCommand(consulta, miConexionSql);
            SqlDataAdapter adaptadorSQL = new SqlDataAdapter(comandoSQL);

            using (adaptadorSQL)
            {
                comandoSQL.Parameters.AddWithValue("@idTratamiento", lstboxTratamiento.SelectedValue);
                DataTable tratamientosTabla = new DataTable();
                adaptadorSQL.Fill(tratamientosTabla);

                txtIdPaciente.Text = tratamientosTabla.Rows[0]["ID_Paciente"].ToString();
                txtIdDoctor.Text = tratamientosTabla.Rows[0]["ID_Doctor"].ToString();
                txtMedicamento.Text = tratamientosTabla.Rows[0]["Medicamento"].ToString();
                txtDosis.Text = tratamientosTabla.Rows[0]["Dosis"].ToString();
                txtDuracion.Text = tratamientosTabla.Rows[0]["Duracion"].ToString();
            }
        }

        private void Modificar()
        {
            try
            {
                string idPaciente = txtIdPaciente.Text;
                string idDoctor = txtIdDoctor.Text;
                string medicamento = txtMedicamento.Text;
                string dosis = txtDosis.Text;
                string duracion = txtDuracion.Text;

                string consulta = $"UPDATE Tratamiento SET ID_Paciente='{idPaciente}', ID_Doctor='{idDoctor}', Medicamento='{medicamento}', " +
                    $"Dosis='{dosis}', Duracion='{duracion}' WHERE ID = @idTratamiento;";

                miConexionSql.Open();
                using (SqlCommand command = new SqlCommand(consulta, miConexionSql))
                {
                    command.Parameters.AddWithValue("@idTratamiento", lstboxTratamiento.SelectedValue);
                    command.ExecuteNonQuery();
                }
                miConexionSql.Close();

                MessageBox.Show("Tratamiento modificado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar el tratamiento: " + ex.Message);
            }
        }

        // Métodos para botones
        private void btnRegistrar(object sender, RoutedEventArgs e)
        {
            Registrar();
        }

        private void btnMostrarPaciente(object sender, RoutedEventArgs e)
        {
            MostrarPaciente();
        }

        private void btnModificar(object sender, RoutedEventArgs e)
        {
            Modificar();
        }

        private void btnMostrarDoctor(object sender, RoutedEventArgs e)
        {
            MostrarDoctor();
        }

        private void btnEliminarListbox(object sender, RoutedEventArgs e)
        {
            Eliminar();
        }

        private void btnVolver(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow(miConexionSql);
            main.Show();
            this.Close();
        }

        // Métodos de selección de cajas de listados
        private void lstboxPacienteDoctor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mostrar();
        }

        private void lstboxTratamiento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RellenarDatos();
        }
    }
}
