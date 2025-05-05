using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PryMaciaConexionBD1._2
{
    internal class ClsProducto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int CategoriaId { get; set; }

        public ClsProducto(string Nombre, string Descripcion, decimal Precio, int Stock, int CategoriaId)
        {
            this.Nombre = Nombre;
            this.Descripcion = Descripcion;
            this.Precio = Precio;
            this.Stock = Stock;
            this.CategoriaId = CategoriaId;
        }
    }
}
