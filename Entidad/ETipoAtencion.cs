using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ETipoAtencion
    {
        public long TipoAtencionID { get; set; }
        public string Nombre { get; set; }
        public int Estado { get; set; }
        public Area Area { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long UsuarioCreador { get; set; }
        public long UsuarioModificador { get; set; }

        public string Color { get; set; }

        public ETipoAtencion()
        {

        }
        public ETipoAtencion(TipoAtencion TipoAtencion)
        {
            this.TipoAtencionID = TipoAtencion.nTipoAtencionId;
            this.Nombre = TipoAtencion.sNombre;
            this.Estado = TipoAtencion.nEstado;
            this.Area = TipoAtencion.Area;
            this.FechaCreacion = TipoAtencion.dFechaCreacion;
            this.FechaModificacion = TipoAtencion.dFechaModificacion;
            this.UsuarioCreador = TipoAtencion.nUsuarioCreador;
            this.UsuarioModificador = TipoAtencion.nUsuarioModificador;
            this.Color = TipoAtencion.sColor;
        }
    }
}
