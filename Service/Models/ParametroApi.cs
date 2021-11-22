using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Service.Models
{
    public class ParametroApi<T>
    {
        public T DatoG { get; set; }
        public string DatoStr { get; set; }
        public string Imagen { get; set; }
        public long UserId { get; set; }
        public string Lat { get; set; }
        public string Respuesta { get; set; }
        public string PersonasConfirmadas { get; set; }
        public string Lng { get; set; }
        public string Motivo { get; set; }
        public string Observaciones { get; set; }
        public string NuevaDireccion { get; set; }
        public long Cant { get; set; }
        public long OrdenInicio { get; set; }
        public long OrdenFin { get; set; }

    }
}