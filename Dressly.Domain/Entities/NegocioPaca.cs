using System;
using System.Collections.Generic;
using System.Text;

namespace Dressly.Domain.Entities
{
    public class NegocioPaca
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string CategoriaPrenda { get; set; }
        public string Coordenadas { get; set; }
        public string Telefono { get; set; }
    }
}
