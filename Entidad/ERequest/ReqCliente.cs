using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ReqCliente
    {
        public string CodigoCliente { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string UrlFoto { get; set; }
        public string CodigoPais { get; set; }
        public string NumeroCelular { get; set; }
        public string Correo { get; set; }
        public string TokenNotificacionFirebase { get; set; }
        public string TipoSistemaOperativo { get; set; }
        public long EmpresaId { get; set; }

        public ReqCliente()
        {

        }
      
    }
}
