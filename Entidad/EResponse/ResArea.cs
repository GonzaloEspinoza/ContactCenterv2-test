using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ResArea
    {
        public long AreaId { get; set; }
        public string Nombre { get; set; }
        public long EmpresaId { get; set; }
        public string EmpresaNombre { get; set; }

        public ResArea()
        {

        }

        public ResArea(Area area)
        {
            this.AreaId = area.nAreaId;
            this.Nombre = area.sNombre;
            this.EmpresaId = area.Empresa.nEmpresaId;
            this.EmpresaNombre = area.Empresa.sNombre;
        }

    }
}
