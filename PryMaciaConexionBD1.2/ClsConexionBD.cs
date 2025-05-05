using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PryMaciaConexionBD1._2
{
    internal class ClsConexionBD
    {
        //cadena de conexion
        string cadenaConexion = "Server=localhost\\SQLEXPRESS;Database=Comercio;Trusted_Connection=True;";
        //Server=localhost\SQLEXPRESS;Database=Ventas2;Trusted_Connection=True;


        SqlConnection coneccionBaseDatos;


        SqlCommand comandoBaseDatos;

        public string nombreBaseDeDatos;


        public void ConectarBD()
        {
            try
            {
                coneccionBaseDatos = new SqlConnection(cadenaConexion);

                nombreBaseDeDatos = coneccionBaseDatos.Database;

                coneccionBaseDatos.Open();

                MessageBox.Show("Bienvenido a " + nombreBaseDeDatos);
            }
            catch (Exception error)
            {
                MessageBox.Show("Tiene un errorcito - " + error.Message);
            }

        }

        public DataTable ObtenerContactos()
        {
            DataTable tablaContactos = new DataTable();

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    string consulta = "SELECT * FROM Contactos";

                    using (SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion))
                    {
                        adaptador.Fill(tablaContactos);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener contactos: " + ex.Message);
            }

            return tablaContactos;
        }
        public void Agregar(ClsProducto producto)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    string query = "INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, CategoriaId) VALUES (@nombre, @descripcion, @precio, @stock, @categoriaId)";

                    SqlCommand comando = new SqlCommand(query, conexion);
                    SqlDataAdapter adaptador = new SqlDataAdapter(comando);

                    comando.Parameters.AddWithValue("@nombre", producto.Nombre);
                    comando.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                    comando.Parameters.AddWithValue("@precio", producto.Precio);
                    comando.Parameters.AddWithValue("@stock", producto.Stock);
                    comando.Parameters.AddWithValue("@categoriaId", producto.CategoriaId);
                    comando.ExecuteNonQuery();

                    MessageBox.Show("Producto agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception error)
            {
                MessageBox.Show("Error al agregar producto: " + error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Cargarcategorias(ComboBox Combo)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    string query = "SELECT Id, Nombre FROM Categorias";

                    SqlCommand comando = new SqlCommand(query, conexion);
                    SqlDataAdapter adaptador = new SqlDataAdapter(comando);

                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    Combo.DataSource = tabla;
                    Combo.DisplayMember = "Nombre";
                    Combo.ValueMember = "Id";
                }

            }
            catch (Exception error)
            {
                MessageBox.Show("Error al cargar categorías: " + error.Message);
            }
        }
        public void ListarBD(DataGridView Grilla)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    string query = "SELECT * FROM Productos";

                    SqlCommand comando = new SqlCommand(query, conexion);
                    SqlDataAdapter adaptador = new SqlDataAdapter(comando);

                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);
                    Grilla.DataSource = tabla;
                }

            }
            catch (Exception error)
            {
                MessageBox.Show("No se pudieron cargar los productos correctamente. Revise su conexión o intente más tarde.", "Error de carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public void EliminarProducto(int codigo)
        {

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    string query = "DELETE FROM Productos WHERE Codigo = @Codigo";

                    SqlCommand comando = new SqlCommand(query, conexion);
                    comando.Parameters.AddWithValue("@Codigo", codigo);
                    comando.ExecuteNonQuery();

                    MessageBox.Show("Producto eliminado correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar producto: " + ex.Message);
            }

        }

        public void ModificarProducto(int codigo, string nombre, string descripcion, decimal precio, int stock, int categoriaId)

        {
            string query = "UPDATE Productos SET Nombre = @Nombre, Descripcion = @Descripcion, Precio = @Precio, Stock = @Stock, CategoriaId = @CategoriaId WHERE Codigo = @Codigo";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Nombre", nombre);
                comando.Parameters.AddWithValue("@Descripcion", descripcion);
                comando.Parameters.AddWithValue("@Precio", precio);
                comando.Parameters.AddWithValue("@Stock", stock);
                comando.Parameters.AddWithValue("@CategoriaId", categoriaId);
                comando.Parameters.AddWithValue("@Codigo", codigo); // 👈 ESTA LÍNEA ES CLAVE

                try
                {
                    conexion.Open();
                    comando.ExecuteNonQuery();
                    MessageBox.Show("Producto actualizado correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar el producto: " + ex.Message);
                }
            }
        }
        public bool ProductoExiste(string nombre)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT COUNT(*) FROM Productos WHERE Nombre = @nombre";

                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@nombre", nombre);

                int count = (int)comando.ExecuteScalar();
                return count > 0;
            }
        }

    }
}

