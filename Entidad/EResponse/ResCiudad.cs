using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ResCiudad
    {
        public long CiudadId { get; set; }
        public string Nombre { get; set; }
        public long PaisId { get; set; }
        public string PaisNombre { get; set; }

        public ResCiudad()
        {

        }

        public ResCiudad(Ciudad ciudad)
        {
            this.CiudadId = ciudad.nCiudadId;
            this.Nombre = ciudad.sNombre;
            this.PaisId = ciudad.Pais.nPaisId;
            this.PaisNombre = ciudad.Pais.sNombre;
        }

    }
}
