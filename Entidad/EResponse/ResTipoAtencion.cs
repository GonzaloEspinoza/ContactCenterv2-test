using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ResTipoAtencion
    {
        public long TipoAtencionId { get; set; }
        public string Nombre { get; set; }
        public int Estado { get; set; }
        public ResArea Area { get; set; }
        public string Color { get; set; }
        public ResTipoAtencion()
        {
        }
        public ResTipoAtencion(TipoAtencion TipoAtencion)
        {
            this.TipoAtencionId = TipoAtencion.nTipoAtencionId;
            this.Nombre = TipoAtencion.sNombre;
            this.Estado = TipoAtencion.nEstado;
            this.Area = new ResArea() {
                AreaId = TipoAtencion.Area.nAreaId,
                Nombre = TipoAtencion.Area.sNombre,
                EmpresaId = TipoAtencion.Area.Empresa.nEmpresaId,
                EmpresaNombre = TipoAtencion.Area.Empresa.sNombre
            };
            this.Color = TipoAtencion.sColor;
        }
    }
}
