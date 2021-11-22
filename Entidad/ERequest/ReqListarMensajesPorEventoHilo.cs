using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ReqListarMensajesPorEventoHilo
    {
      
        public long EventoHiloId { get; set; }
        public int Pagina { get; set; }
        public ReqListarMensajesPorEventoHilo()
        {

        }
      
    }
}
