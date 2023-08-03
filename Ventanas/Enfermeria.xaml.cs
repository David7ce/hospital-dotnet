using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace AppHospital.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Enfermeria.xaml
    /// </summary>
    public partial class Enfermeria : Window
    {
        private SqlConnection miConexionSql;

        string eleccion = "";

        public Enfermeria()
        {
            InitializeComponent();

            InicializarConexion();
        }

        // Constructor que recibe la conexión SQL
        public Enfermeria(SqlConnection miConexionSql)
        {
            InitializeComponent();
            this.miConexionSql = miConexionSql;

            MostrarIsla();
            MostrarIDSupervisor();
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
                string nombre = txtNombre.Text;
                string apellido1 = txtApellido1.Text;
                string apellido2 = txtApellido2.Text;
                string nifNie = txtNifNie.Text;
                string telefono = txtTelefono.Text;
                string isla = cmbIsla.SelectedValue.ToString();
                string fechaAlta = dpFechaAlta.Text;
                string idSupervisor = cmbSupervisor.SelectedValue.ToString();

                string consulta = "INSERT INTO Personal_Enfermeria (Nombre, Apellido1, Apellido2, NIF_NIE, Telefono, Isla_Residencia, Fecha_Alta, ID_Supervisor) " +
                                 $"VALUES ('{nombre}', '{apellido1}', '{apellido2}', '{nifNie}', '{telefono}', '{isla}', '{fechaAlta}', '{idSupervisor}');";

                SqlCommand miComandoSql = new SqlCommand(consulta, miConexionSql);
                miConexionSql.Open();
                miComandoSql.ExecuteNonQuery();
                miConexionSql.Close();

                Mostrar();
                MessageBox.Show("Personal de enfermería insertado correctamente!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar el personal de enfermería: " + ex.Message);
            }
        }

        private void MostrarIDSupervisor()
        {
            string consulta = "SELECT ID, CONCAT(Nombre, ' ' ,Apellido1, ' ' ,Apellido2)"
                            + " AS infoDoctor FROM Doctor";

            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(consulta, miConexionSql);

            using (miAdaptadorSql)
            {
                DataTable doctoresTabla = new DataTable();
                miAdaptadorSql.Fill(doctoresTabla);
                cmbSupervisor.DisplayMemberPath = "infoDoctor";
                cmbSupervisor.SelectedValuePath = "ID";
                cmbSupervisor.ItemsSource = doctoresTabla.DefaultView;
            }
        }

        private void MostrarIsla()
        {
            try
            {
                string consulta = "SELECT ID, CONCAT(Nombre, ' ' , Provincia, ' ')"
                                + " AS infoIsla FROM Isla";

                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(consulta, miConexionSql);

                using (miAdaptadorSql)
                {
                    DataTable islaTabla = new DataTable();
                    miAdaptadorSql.Fill(islaTabla);
                    cmbIsla.DisplayMemberPath = "infoIsla";
                    cmbIsla.SelectedValuePath = "ID";
                    cmbIsla.ItemsSource = islaTabla.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar la isla: " + ex.Message);
            }
        }

        private void Mostrar()
        {
            try
            {
                string consulta = "SELECT ID, CONCAT(Nombre, ' ' ,Apellido1, ' ' ,Apellido2) AS infoEnfermeria " +
                    "FROM Personal_Enfermeria";

                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(consulta, miConexionSql);

                using (miAdaptadorSql)
                {
                    // miConexionSql.Open();
                    DataTable enfermeriaTabla = new DataTable();
                    miAdaptadorSql.Fill(enfermeriaTabla);
                    lstboxEnfermeria.DisplayMemberPath = "infoEnfermeria";
                    lstboxEnfermeria.SelectedValuePath = "ID";
                    lstboxEnfermeria.ItemsSource = enfermeriaTabla.DefaultView;
                    // miConexionSql.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar enfermeros/as: " + ex.Message);
            }
        }

        private void Rellenar()
        {
            string consulta = "SELECT Nombre, Apellido1, Apellido2, NIF_NIE, Telefono, Isla_Residencia, Fecha_Alta, ID_Supervisor " +
                "FROM Personal_Enfermeria WHERE ID = @idEnf";

            SqlCommand comandoSQL = new SqlCommand(consulta, miConexionSql);
            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(comandoSQL);

            using (miAdaptadorSql)
            {
                comandoSQL.Parameters.AddWithValue("@idEnf", lstboxEnfermeria.SelectedValue);

                DataTable enfermeriaTabla = new DataTable();
                miAdaptadorSql.Fill(enfermeriaTabla);

                txtNombre.Text = enfermeriaTabla.Rows[0]["Nombre"].ToString();
                txtApellido1.Text = enfermeriaTabla.Rows[0]["Apellido1"].ToString();
                txtApellido2.Text = enfermeriaTabla.Rows[0]["Apellido2"].ToString();
                txtNifNie.Text = enfermeriaTabla.Rows[0]["NIF_NIE"].ToString();
                txtTelefono.Text = enfermeriaTabla.Rows[0]["Telefono"].ToString();
                cmbIsla.SelectedValue = Convert.ToInt32(enfermeriaTabla.Rows[0]["Isla_Residencia"]);
                dpFechaAlta.SelectedDate = Convert.ToDateTime(enfermeriaTabla.Rows[0]["Fecha_Alta"]);
                cmbSupervisor.SelectedValue = Convert.ToInt32(enfermeriaTabla.Rows[0]["ID_Supervisor"]); ;
            }
        }

        private void Eliminar()
        {
            if (lstboxEnfermeria.SelectedIndex != -1)
            {
                try
                {
                    string consulta = "DELETE FROM Personal_Enfermeria WHERE ID=@idEnf";

                    SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

                    miConexionSql.Open();
                    miSqlCommand.Parameters.AddWithValue("@idEnf", lstboxEnfermeria.SelectedValue);
                    miSqlCommand.ExecuteNonQuery();
                    miConexionSql.Close();

                    MessageBox.Show("Enfermero/a eliminada.");
                    Mostrar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("No has seleccionado un enfermero/a para borrar.");
            }
        }

        private void Modificar()
        {
            try
            {
                string nombre = txtNombre.Text;
                string apellido1 = txtApellido1.Text;
                string apellido2 = txtApellido2.Text;
                string nifNie = txtNifNie.Text;
                string telefono = txtTelefono.Text;
                string isla = cmbIsla.SelectedValue.ToString();
                string fechaAlta = dpFechaAlta.Text;
                string idSupervisor = cmbSupervisor.SelectedValue.ToString();

                string consulta = $"UPDATE Personal_Enfermeria SET Nombre='{nombre}', Apellido1='{apellido1}', Apellido2='{apellido2}', " +
                     $"NIF_NIE='{nifNie}', telefono='{telefono}', Isla_Residencia='{isla}', Fecha_Alta='{fechaAlta}', ID_Supervisor='{idSupervisor}' " +
                     $"WHERE ID=@idEnf;";

                SqlCommand sqlComando = new SqlCommand(consulta, miConexionSql);
                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(sqlComando);

                using (miAdaptadorSql)
                {
                    miConexionSql.Open();
                    sqlComando.Parameters.AddWithValue("@idEnf", lstboxEnfermeria.SelectedValue);
                    sqlComando.ExecuteNonQuery();
                    miConexionSql.Close();
                }

                MessageBox.Show("Datos de enfermero/a modificado.");
                Mostrar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar los datos del enfermero/a: " + ex.Message);
            }
        }

        // Métodos para botones
        private void btnMostrar(object sender, RoutedEventArgs e)
        {
            Mostrar();
        }

        private void btnRegistrar(object sender, RoutedEventArgs e)
        {
            Registrar();
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

        // Métodos pata Selección
        private void lstboxEnfermeria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Rellenar();
        }
    }
}
