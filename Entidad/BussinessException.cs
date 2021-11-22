using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{
    public class BussinessException : Exception
    {
        public int Codigo { get; set; }

        public BussinessException(int Codigo, string Mensaje) : base(Mensaje)
        {
            this.Codigo = Codigo;
        }

        public BussinessException(string Mensaje)
            : base(Mensaje)
        {
            this.Codigo = 1;
        }
    }
}
