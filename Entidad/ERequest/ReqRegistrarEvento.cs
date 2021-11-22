using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ReqRegistrarEvento
    {
        public long TipoAtencionId { get; set; }
        public long CiudadId { get; set; }
        
        public ReqCliente Cliente { get; set; }
        public ReqRegistrarEvento()
        {
        }
      
    }
}
