using DataP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{
    public class EPermiso : Permiso
    {
        public EPermiso()
            : base()
        {
            PermisosHijos = new List<EPermiso>();
        }
        public bool Activado { get; set; }
        public bool InPerfil { get; set; }
        public List<EPermiso> PermisosHijos { get; set; }
    }
}
