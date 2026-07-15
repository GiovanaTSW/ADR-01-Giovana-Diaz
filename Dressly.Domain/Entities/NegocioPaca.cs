using System;
using System.Collections.Generic;
using System.Text;

namespace Dressly.Domain.Entities
{
    public class NegocioPaca
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string CategoriaPrenda { get; set; } = string.Empty;
        public string Coordenadas { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }
}
