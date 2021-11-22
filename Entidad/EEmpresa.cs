using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class EEmpresa
    {
        public long EmpresaId { get; set; }
        public string Nombre { get; set; }
        public string Url { get; set; }
        
        public string Logo { get; set; }
        public string Base64Logo { get; set; }

        public string UrlLogo { get; set; }
        public int Estado { get; set; }
     
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long UsuarioCreador { get; set; }
        public long UsuarioModificador { get; set; }
        public ICollection<Area> Areas { get; set; }


        public EEmpresa()
        {
            this.Base64Logo = "";
        }
        public EEmpresa(Empresa Empresa)
        {
            this.EmpresaId = Empresa.nEmpresaId;
            this.Nombre = Empresa.sNombre;
            this.Url = Empresa.sUrl;
            this.Logo = Empresa.sLogo;
            this.Estado = Empresa.nEstado;
            this.FechaCreacion = Empresa.dFechaCreacion;
            this.FechaModificacion = Empresa.dFechaModificacion;
            this.UsuarioCreador = Empresa.nUsuarioCreador;
            this.UsuarioModificador = Empresa.nUsuarioModificador;
        }
    }
}

