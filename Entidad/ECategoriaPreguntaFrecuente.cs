using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ECategoriaPreguntaFrecuente
    {
        public long CategoriaPreguntaFrecuenteId { get; set; }
        public string Nombre { get; set; }
        public int Estado { get; set; }
        public Empresa Empresa { get; set; }

        public ICollection<PreguntaFrecuente> PreguntaFrecuente { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }




        public ECategoriaPreguntaFrecuente()
        {
           
        }
        public ECategoriaPreguntaFrecuente(CategoriaPreguntaFrecuente CategoriaPreguntaFrecuente)
        {
            this.CategoriaPreguntaFrecuenteId = CategoriaPreguntaFrecuente.nCategoriaPreguntaFrecuenteId;
            this.Nombre = CategoriaPreguntaFrecuente.sNombre;
            this.Estado = CategoriaPreguntaFrecuente.nEstado;
            this.Empresa = CategoriaPreguntaFrecuente.Empresa;
            this.PreguntaFrecuente = CategoriaPreguntaFrecuente.PreguntaFrecuente;
            this.FechaCreacion = CategoriaPreguntaFrecuente.dFechaCreacion;
            this.FechaModificacion =CategoriaPreguntaFrecuente.dFechaModificacion;
       
        }
    }
}
