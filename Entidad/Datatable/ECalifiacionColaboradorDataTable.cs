using DataP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{
    public class ECalifiacionColaboradorDataTable
    {
        public long EventoId { get; set; }
        public decimal? Calificacion { get; set; }
        public string Funcionario { get; set; }
        public string Comentario { get; set; }
        
        public ECalifiacionColaboradorDataTable()
        {

        }

    }

   
}
