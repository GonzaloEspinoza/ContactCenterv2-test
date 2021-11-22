using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ResCategoriaPreguntaFrecuente
    {
        public long CategoriaPreguntaFrecuentenId { get; set; }
        public string Nombre { get; set; }
        
        public ResCategoriaPreguntaFrecuente()
        {
        }
        public ResCategoriaPreguntaFrecuente(CategoriaPreguntaFrecuente categoria)
        {
            this.CategoriaPreguntaFrecuentenId = categoria.nCategoriaPreguntaFrecuenteId;
            this.Nombre = categoria.sNombre;
           
        }
    }
}
