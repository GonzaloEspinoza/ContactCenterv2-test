using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ECiudad
    {
        public long CiudadId { get; set; }
        public string Nombre { get; set; }
        public int Estado { get; set; }
        public string Bandera { get; set; }
        public string Base64Bandera { get; set; }

        public string UrlBandera { get; set; }
        public Pais Pais { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }


        public ECiudad()
        {
            this.Base64Bandera = "";
        }
        public ECiudad(Ciudad Ciudad)
        {
            this.CiudadId = Ciudad.nCiudadId;
            this.Nombre = Ciudad.sNombre;
            this.Estado = Ciudad.nEstado;
            this.Bandera = Ciudad.sBandera;
            this.Pais = Ciudad.Pais;
            this.FechaCreacion = Ciudad.dFechaCreacion;
            this.FechaModificacion = Ciudad.dFechaModificacion;
        }
    }
}
