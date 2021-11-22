using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class EArea
    {
        public long AreaId { get; set; }
        public string Nombre { get; set; }
        public int Estado { get; set; }
        public Empresa Empresa { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long UsuarioCreador { get; set; }
        public long UsuarioModificador { get; set; }


        public EArea()
        {

        }
        public EArea(Area Area)
        {
            this.AreaId = Area.nAreaId;
            this.Nombre = Area.sNombre;
            this.Estado = Area.nEstado;
            this.Empresa = Area.Empresa;
            this.FechaCreacion = Area.dFechaCreacion;
            this.FechaModificacion = Area.dFechaModificacion;
            this.UsuarioCreador = Area.nUsuarioCreador;
            this.UsuarioModificador = Area.nUsuarioModificador;
        }
    }
}
