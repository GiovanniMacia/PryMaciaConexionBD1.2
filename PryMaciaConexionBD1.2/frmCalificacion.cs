using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PryMaciaConexionBD1._2
{
    public partial class Calificacion : Form
    {
        public int CalificacionSeleccionada { get; private set; }
        public Calificacion()
        {
            InitializeComponent();
            // Crear botones de calificación del 1 al 5
            for (int i = 1; i <= 5; i++)
            {
                Button btn = new Button();
                btn.Text = i.ToString();
                btn.Size = new Size(40, 40);
                btn.Location = new Point(30 + (i - 1) * 50, 50);

                // Asocia el evento click al manejador btn_Click
                btn.Click += new EventHandler(btn_Click);

                this.Controls.Add(btn);
            }
            
    } 


        private void frmCalificacion_Load(object sender, EventArgs e)
        {

        }
        private void btn_Click(object sender, EventArgs e)
        {
            Button boton = sender as Button;
            if (boton != null)
            {
                // Guarda la calificación seleccionada
                CalificacionSeleccionada = int.Parse(boton.Text);

                MessageBox.Show($"Gracias por calificarnos con {CalificacionSeleccionada} estrella(s) 🌟", "¡Gracias!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK; // ← Muy importante para que ShowDialog() devuelva OK
                this.Close(); // Cierra el formulario
            }
    }
}
}



