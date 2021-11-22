using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class EPais
    {
        public long PaisId { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public int Estado { get; set; }
        public string Bandera { get; set; }
        public string Base64Bandera { get; set; }
        public string UrlBandera { get; set; }
        public ICollection<Ciudad> Ciudades { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int DiferenciaHorario { get; set; }
        public string SiglaMoneda { get; set; }
        public string Moneda { get; set; }
        public string SiglaPais { get; set; }

       

        public EPais()
        {
            this.Base64Bandera = "";
        }
        public EPais(Pais Pais)
        {
            this.PaisId = Pais.nPaisId;
            this.Nombre = Pais.sNombre;
            this.Codigo = Pais.sCodigo;
            this.Estado = Pais.nEstado;
            this.Bandera = Pais.sBandera;
            this.Ciudades = Pais.Ciudad;
            this.FechaCreacion = Pais.dFechaCreacion;
            this.FechaModificacion = Pais.dFechaModificacion;
            this.SiglaPais = Pais.sSiglaPais;
            this.Moneda = Pais.sMoneda;
            this.SiglaMoneda = Pais.sSiglaMoneda;
            this.DiferenciaHorario = Pais.nDiferenciaHorario;
        }
    }
}
