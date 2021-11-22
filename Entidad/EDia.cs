using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class EDia
    {
        public long DiaId { get; set; }
        public string Nombre { get; set; }


        public EDia()
        {

        }
        public EDia(Dia dia)
        {
            this.DiaId = dia.nDiaId;
            this.Nombre = dia.sNombre;
        }
    }
}
