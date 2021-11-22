using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ReqEnviarMensaje
    {
        public long EventoHiloId { get; set; }

        public string Texto { get; set; }

        public string Archivo { get; set; }

        public ReqEnviarMensaje()
        {

        }
      
    }
}
