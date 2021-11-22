using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ResPreguntaFrecuente
    {
        public long PreguntaFrecuenteId { get; set; }
        public ResTipoAtencion TipoAtencion { get; set; }
        public ResCategoriaPreguntaFrecuente CategoriaPreguntaFrecuente { get; set; }
        public string Pregunta { get; set; }

        public ResPreguntaFrecuente()
        {

        }

        public ResPreguntaFrecuente(PreguntaFrecuente preguntaFrecuente)
        {
            this.PreguntaFrecuenteId = preguntaFrecuente.nPreguntaFrecuenteId;
            this.TipoAtencion = new ResTipoAtencion(preguntaFrecuente.TipoAtencion);
            this.CategoriaPreguntaFrecuente = new ResCategoriaPreguntaFrecuente(preguntaFrecuente.CategoriaPreguntaFrecuente);
            this.Pregunta = preguntaFrecuente.sPregunta;
        }

    }
}
