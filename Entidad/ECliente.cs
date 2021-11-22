using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ECliente
    {
        public long ClienteId { get; set; }
        public string CodigoCliente { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string UrlFoto { get; set; }
        public string CodigoPais { get; set; }
        public string NumeroCelular { get; set; }
        public string Correo { get; set; }

        public string TokenNotificacionFirebase { get; set; }
        public string TipoSistemaOperativo { get; set; }
        public int Estado { get; set; }
        public Empresa Empresa { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }


        public ECliente()
        {

        }
        public ECliente(Cliente cliente)
        {
            this.ClienteId = cliente.nClienteId;
            this.CodigoCliente = cliente.sCodigoCliente;
            this.Nombres = cliente.sNombres;
            this.Apellidos = cliente.sApellidos;
            this.UrlFoto = cliente.sUrlFoto;
            this.CodigoPais = cliente.sCodigoPais;
            this.NumeroCelular = cliente.sNumeroCelular;
            this.Correo = cliente.sCorreo;
            this.TokenNotificacionFirebase = cliente.sTokenNotificacionFirebase;
            this.TipoSistemaOperativo = cliente.sTipoSistemaOperativo;
            this.Estado = cliente.nEstado;
            this.Empresa = cliente.Empresa;
            this.FechaCreacion = cliente.dFechaCreacion;
            this.FechaModificacion = cliente.dFechaModificacion;
        }
    }
}
