using System;
using System.Collections.Generic;
using System.Text;

namespace Dressly.Domain.Entities
{
    public class IdentidadKibbeInfo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string TipoEnergia { get; set; } = string.Empty;
        public string DescripcionFisica { get; set; } = string.Empty;
        public string LineasRecomendadas { get; set; } = string.Empty;
        public string LineasNoRecomendadas { get; set; } = string.Empty;
        public string TelasSugeridas { get; set; } = string.Empty;
        public string Evitar {  get; set; } = string.Empty;

    }
}
