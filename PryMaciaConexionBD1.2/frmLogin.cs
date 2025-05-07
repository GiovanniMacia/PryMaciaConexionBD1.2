using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PryMaciaConexionBD1._2
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            txtUsuario.Font =new Font("Georgia", 12, FontStyle.Regular);
            txtContraseña.Font = new Font("Georgia", 12, FontStyle.Regular);
        }

        private void chkContraseña_CheckedChanged(object sender, EventArgs e)
        {
            if (chkContraseña.Checked)
            {
                // Si el checkbox está marcado, se muestra la contraseña tal como se escribe (sin ocultar con asteriscos)
                txtContraseña.PasswordChar = '\0';  // Esto elimina el carácter de contraseña (muestra el texto tal como se escribe)
            }
            else
            {
                // Si el checkbox no está marcado, se oculta la contraseña con asteriscos
                txtContraseña.PasswordChar = '*';  // Esto pone asteriscos como caracteres de la contraseña
            }
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string username = txtUsuario.Text.Trim();
            string password = txtContraseña.Text;

            // Verificar campos vacíos
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, completá ambos campos.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Cadena de conexión
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=Comercio;Trusted_Connection=True;";
            string query = "SELECT COUNT(*) FROM Usuarios WHERE Usuario = @usuario AND Clave = @clave";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@usuario", username);
                    cmd.Parameters.AddWithValue("@clave", password);

                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Inicio de sesión exitoso.");
                        frmPrincipal productos = new frmPrincipal();
                        productos.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Usuario o contraseña incorrectos.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar con la base de datos: " + ex.Message);
                }
            }
        }
    }
}
