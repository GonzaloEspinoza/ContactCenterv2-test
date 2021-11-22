using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad.Enums
{
    public enum EstadoDeEvento : long
    {
        SinAsignacion = 1,
        Asignado = 2,
        NoResuelto = 3,
        Atendido = 4,
        Reasignado = 5,

    }
}
