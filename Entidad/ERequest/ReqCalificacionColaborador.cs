using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ReqCalificacionColaborador
    {
        public string Comentario { get; set; }
        public int Puntuacion { get; set; }
        public long EventoId { get; set; }

        public ReqCalificacionColaborador()
        {

        }
      
    }
}
