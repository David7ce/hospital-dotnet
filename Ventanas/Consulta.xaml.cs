using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace AppHospital
{
    /// <summary>
    /// Lógica de interacción para Consulta.xaml
    /// </summary>
    public partial class Consulta : Window
    {
        private SqlConnection miConexionSql;

        string eleccion = "";

        public Consulta()
        {
            InitializeComponent();

            // Establecer la ubicación de inicio de la ventana en el centro de la pantalla
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //this.width = 650;
            //this.height = 1000;

            InicializarConexion();
        }

        // Constructor que recibe una conexión SQL
        public Consulta(SqlConnection miConexionSql)
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

        private void Registrar()
        {
            try
            {
                string idPaciente = txtIdPaciente.Text;
                string idDoctor = txtIdDoctor.Text;
                string fechaConsulta = dpFechaConsulta.Text;
                string diagnostico = txtDiagnostico.Text;

                string consulta = "INSERT INTO Consulta (ID_Paciente, ID_Doctor, Fecha_consulta, Diagnostico) " +
                                  $"VALUES ('{idPaciente}', '{idDoctor}', '{fechaConsulta}', '{diagnostico}');";

                SqlCommand miComandoSql = new SqlCommand(consulta, miConexionSql);
                miConexionSql.Open();
                miComandoSql.ExecuteNonQuery();
                miConexionSql.Close();
                MessageBox.Show("Consulta insertado correctamente!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar la consulta: " + ex.Message);
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

            string consulta = "SELECT Id, CONCAT(ID, ' - ' ,Nombre, ' ' ,Apellido1, ' ' ,Apellido2)"
                            + " AS infoDoctor FROM Doctor";

            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(consulta, miConexionSql);

            using (miAdaptadorSql)
            {
                DataTable doctoresTabla = new DataTable();
                miAdaptadorSql.Fill(doctoresTabla);
                lstboxPacienteDoctor.DisplayMemberPath = "infoDoctor";
                lstboxPacienteDoctor.SelectedValuePath = "Id";
                lstboxPacienteDoctor.ItemsSource = doctoresTabla.DefaultView;
            }
        }

        // Muestra las consultas de pacientes y asociados a doctores
        private void Mostrar()
        {
            if (lstboxPacienteDoctor.SelectedIndex != -1)
            {
                string consulta = "";

                // El condicional selecciona el ID_Paciente o ID_Doctor, hay que tener bien el select para poder eliminar
                if (eleccion == "Paciente")
                {
                    consulta = "SELECT C.ID, CONCAT(D.Nombre, ' - ', C.Fecha_Consulta, ' - ', C.Diagnostico) AS infoConsulta FROM Consulta C "
                    + "INNER JOIN Doctor D ON D.ID = C.ID_Doctor "
                    + "WHERE ID_Paciente = @id_consulta;";
                }
                else if (eleccion == "Doctor")
                {
                    consulta = "SELECT C.ID, CONCAT(D.Nombre, ' - ', C.Fecha_Consulta, ' - ', C.Diagnostico) AS infoConsulta FROM Consulta C "
                    + "INNER JOIN Doctor D ON D.ID = C.ID_Doctor "
                    + "WHERE ID_Doctor = @id_consulta;";
                }

                SqlCommand sqlComando = new SqlCommand(consulta, miConexionSql);
                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(sqlComando);

                using (miAdaptadorSql)
                {
                    sqlComando.Parameters.AddWithValue("@id_consulta", lstboxPacienteDoctor.SelectedValue);

                    DataTable consultasTabla = new DataTable();
                    miAdaptadorSql.Fill(consultasTabla);

                    lstboxConsulta.DisplayMemberPath = "infoConsulta";
                    lstboxConsulta.SelectedValuePath = "ID";
                    lstboxConsulta.ItemsSource = consultasTabla.DefaultView;
                }
            }
        }

        private void Eliminar()
        {
            if (lstboxConsulta.SelectedIndex != -1)
            {
                try
                {
                    // int idForzado = 3;
                    string consulta = "DELETE FROM Consulta WHERE ID=@id_consulta";

                    SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);
                    miConexionSql.Open();

                    miSqlCommand.Parameters.AddWithValue("@id_consulta", lstboxConsulta.SelectedValue);

                    miSqlCommand.ExecuteNonQuery();

                    miConexionSql.Close();

                    Mostrar();

                    MessageBox.Show("Consulta eliminada.");
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

        // TODO: Rellenar Datos
        public void RellenarDatos()
        {
            string consulta = "SELECT ID_Paciente, ID_Doctor, Fecha_Consulta, Diagnostico " +
                              "FROM Consulta WHERE ID=@idRelleno;";

            SqlCommand comandoSQL = new SqlCommand(consulta, miConexionSql);

            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(comandoSQL);

            using (miAdaptadorSql)
            {
                comandoSQL.Parameters.AddWithValue("@idRelleno", lstboxConsulta.SelectedValue);
                DataTable consultaTabla = new DataTable();
                miAdaptadorSql.Fill(consultaTabla);

                txtIdPaciente.Text = consultaTabla.Rows[0]["ID_Paciente"].ToString();
                txtIdDoctor.Text = consultaTabla.Rows[0]["ID_Doctor"].ToString();
                dpFechaConsulta.SelectedDate = Convert.ToDateTime(consultaTabla.Rows[0]["Fecha_Consulta"]);
                txtDiagnostico.Text = consultaTabla.Rows[0]["Diagnostico"].ToString();
            }
        }


        private void Modificar()
        {
            try
            {
                string idPaciente = txtIdPaciente.Text;
                string idDoctor = txtIdDoctor.Text;
                string fechaConsulta = dpFechaConsulta.Text;
                string diagnostico = txtDiagnostico.Text;

                string consulta = $"UPDATE Consulta SET ID_Paciente = '{idPaciente}', ID_Doctor = '{idDoctor}', Fecha_Consulta = '{fechaConsulta}', " +
                               $"Diagnostico = '{diagnostico}' WHERE ID = @idConsulta;";

                miConexionSql.Open();
                using (SqlCommand command = new SqlCommand(consulta, miConexionSql))
                {

                    command.Parameters.AddWithValue("@idConsulta", lstboxConsulta.SelectedValue);
                    command.ExecuteNonQuery();
                }
                miConexionSql.Close();

                MessageBox.Show("Consulta modificada.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar la consulta: " + ex.Message);
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

        private void btnMostrarDoctor(object sender, RoutedEventArgs e)
        {
            MostrarDoctor();
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


        // Métodos para selección de listados
        private void lstboxPacienteDoctor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mostrar();
        }

        private void lstboxConsulta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RellenarDatos();
        }
    }
}
