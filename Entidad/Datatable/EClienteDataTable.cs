using DataP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{
    public class EClienteDataTable 
    {
        public long ClienteId { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string EmpresaNombre { get; set; }
        public int Estado { get; set; }

        
        public EClienteDataTable()
        {

        }

    }

   
}
