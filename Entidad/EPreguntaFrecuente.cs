using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class EPreguntaFrecuente
    {
        public long PreguntaFrecuenteId { get; set; }
        public TipoAtencion TipoAtencion { get; set; }
        public CategoriaPreguntaFrecuente CategoriaPreguntaFrecuente { get; set; }
        public string Pregunta { get; set; }

        public int Estado { get; set; }
        public Empresa Empresa { get; set; }


        public EPreguntaFrecuente()
        {
        }
        public EPreguntaFrecuente(PreguntaFrecuente preguntaFrecuente)
        {
            this.PreguntaFrecuenteId = preguntaFrecuente.nPreguntaFrecuenteId;
            this.TipoAtencion = preguntaFrecuente.TipoAtencion;
            this.CategoriaPreguntaFrecuente = preguntaFrecuente.CategoriaPreguntaFrecuente;
            this.Pregunta = preguntaFrecuente.sPregunta;
            this.Empresa = preguntaFrecuente.TipoAtencion.Area.Empresa;
            this.Estado = preguntaFrecuente.TipoAtencion.nEstado;
        }
    }
}
