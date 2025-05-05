using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PryMaciaConexionBD1._2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        ClsConexionBD conexion = new ClsConexionBD();
        int? productoSeleccionado = null;
        int pos = -1;
        string datosOriginales = "";


        private void dgvDatos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            conexion.ConectarBD();
            conexion.ListarBD(dgvDatos);
            conexion.Cargarcategorias(cmbCategoria);

        }

        private void MostrarProductos()
        {
            string query = "SELECT * FROM Productos";

            using (SqlConnection conexion = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=Comercio;Trusted_Connection=True;"))
            {
                try
                {
                    SqlDataAdapter adaptador = new SqlDataAdapter(query, conexion);
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);
                    dgvDatos.DataSource = tabla;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar productos: " + ex.Message);
                }
            }
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            cmbCategoria.SelectedIndex = -1;
            productoSeleccionado = null;
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
        string.IsNullOrWhiteSpace(txtDescripcion.Text) ||
        string.IsNullOrWhiteSpace(txtPrecio.Text) ||
        cmbCategoria.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor completá todos los campos antes de modificar el producto.", "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (productoSeleccionado == null)
            {
                MessageBox.Show("Seleccioná un producto antes de intentar modificarlo.", "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show("¿Estás seguro de que deseas modificar este producto?", "Confirmar modificación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes)
                return;

            string nombre = txtNombre.Text;
            string descripcion = txtDescripcion.Text;
            if (!decimal.TryParse(txtPrecio.Text.Replace("$", "").Trim(), out decimal precio))
            {
                MessageBox.Show("Ingresá un precio válido.", "Error de formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int stock = (int)numStock.Value;
            int categoriaId = Convert.ToInt32(cmbCategoria.SelectedValue);

            // Comparar si hubo algún cambio
            string datosActuales = $"{nombre}|{descripcion}|{precio}|{stock}|{categoriaId}";
            if (datosActuales == datosOriginales)
            {
                MessageBox.Show("No se detectaron cambios en el producto.", "Sin cambios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Ejecutar la modificación
            ClsConexionBD conexion = new ClsConexionBD();
            conexion.ModificarProducto((int)productoSeleccionado, nombre, descripcion, precio, stock, categoriaId);

            // Refrescar y limpiar
            conexion.ListarBD(dgvDatos);
            LimpiarCampos();
            productoSeleccionado = null;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (pos >= 0)
            {
                var confirmacion = MessageBox.Show("¿Estás seguro de que deseas eliminar este producto?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmacion != DialogResult.Yes)
                    return;

                // Obtener el código del producto seleccionado (asumo que es la primera celda)
                int codigo = Convert.ToInt32(dgvDatos.Rows[pos].Cells[0].Value);

                // Llamar al método para eliminar de la BD
                conexion.EliminarProducto(codigo);

                // Luego eliminar de la grilla
                dgvDatos.Rows.RemoveAt(pos);
            }
            else
            {
                MessageBox.Show("Seleccioná un producto para eliminar.");
            }
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0) // Asegura que no sea el encabezado
            {
                DataGridViewRow fila = dgvDatos.Rows[e.RowIndex];

                // Guardamos el ID del producto seleccionado
                productoSeleccionado = Convert.ToInt32(fila.Cells["Codigo"].Value);

                // Pasamos los valores a los controles
                txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
                txtDescripcion.Text = fila.Cells["Descripcion"].Value.ToString();
                txtPrecio.Text = fila.Cells["Precio"].Value.ToString();
                numStock.Value = Convert.ToInt32(fila.Cells["Stock"].Value);
                cmbCategoria.SelectedValue = Convert.ToInt32(fila.Cells["CategoriaId"].Value);

                // Guardamos el estado original para comparación
                datosOriginales = $"{txtNombre.Text}|{txtDescripcion.Text}|{txtPrecio.Text}|{numStock.Value}|{cmbCategoria.SelectedValue}";
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            // Validaciones de campos obligatorios
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtDescripcion.Text) ||
                string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                cmbCategoria.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor completá todos los campos antes de agregar el producto.", "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener datos de los controles
            string nombre = txtNombre.Text.Trim();
            string descripcion = txtDescripcion.Text.Trim();
            decimal precio;
            if (!decimal.TryParse(txtPrecio.Text.Replace("$", "").Trim(), out precio))
            {
                MessageBox.Show("Ingresá un precio válido.", "Error de formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int stock = (int)numStock.Value;
            int categoriaId = Convert.ToInt32(cmbCategoria.SelectedValue);

            ClsConexionBD objConexion = new ClsConexionBD();

            // Validación de producto duplicado por nombre
            if (productoSeleccionado == null && objConexion.ProductoExiste(nombre))
            {
                MessageBox.Show("Ya existe un producto con ese nombre.", "Producto duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (productoSeleccionado == null)
            {
                // Agregar nuevo producto
                ClsProducto producto = new ClsProducto(nombre, descripcion, precio, stock, categoriaId);
                objConexion.Agregar(producto);
            }
            else
            {
                // Modificar producto existente
                objConexion.ModificarProducto((int)productoSeleccionado, nombre, descripcion, precio, stock, categoriaId);
                productoSeleccionado = null; // limpiar selección después de modificar
            }

            // Refrescar la tabla y limpiar campos
            objConexion.ListarBD(dgvDatos);
            LimpiarCampos();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string nombreBuscar = txtBuscar.Text.Trim();

            if (string.IsNullOrEmpty(nombreBuscar))
            {
                MessageBox.Show("Por favor ingresa un nombre para buscar.");
                return;
            }

            string query = "SELECT * FROM Productos WHERE Nombre LIKE @nombre";

            using (SqlConnection conexion = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=Comercio;Trusted_Connection=True;"))
            {
                try
                {
                    SqlCommand comando = new SqlCommand(query, conexion);
                    comando.Parameters.AddWithValue("@nombre", "%" + nombreBuscar + "%");

                    SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);
                    dgvDatos.DataSource = tabla;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al buscar productos: " + ex.Message);
                }
            }
        }

        private void btnRestablecer_Click(object sender, EventArgs e)
        {
            conexion.ListarBD(dgvDatos); // Vuelve a llenar la grilla con todos los productos
            txtBuscar.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var confirmacion = MessageBox.Show("¿Querés salir del programa?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion == DialogResult.Yes)
            {
                Calificacion calificacionForm = new Calificacion();
                if (calificacionForm.ShowDialog() == DialogResult.OK)
                {
                    int calificacion = calificacionForm.CalificacionSeleccionada;
                    MessageBox.Show("Gracias por calificar con " + calificacion + " estrella(s).", "Gracias", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                Application.Exit();
            }
        }
    }
}

