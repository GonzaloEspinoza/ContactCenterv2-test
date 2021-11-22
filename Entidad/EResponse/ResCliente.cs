using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ResCliente
    {
        public long ClienteId { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string UrlFoto { get; set; }

        public ResCliente()
        {

        }

        public ResCliente(Cliente f)
        {
            this.ClienteId = f.nClienteId;
            this.Nombres = f.sNombres;
            this.Apellidos = f.sApellidos;
            this.Correo = f.sCorreo;
            this.UrlFoto = f.sUrlFoto;
        }

    }
}
