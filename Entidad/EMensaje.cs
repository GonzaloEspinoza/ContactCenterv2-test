using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;


namespace Entidad
{
    public class EMensaje
    {
        public long MensajeId { get; set; }
        public string Texto { get; set; }
        public string Archivo { get; set; }
        public int Estado { get; set; }
        public EventoHilo EventoHilo { get; set; }
        public Cliente Cliente { get; set; }
        public Funcionario Funcionario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long UsuarioCreador { get; set; }
        public long UsuarioModificador { get; set; }


        public EMensaje()
        {

        }
        public EMensaje(Mensaje Mensaje)
        {
            this.MensajeId = Mensaje.nMensajeId;
            this.Texto = Mensaje.sTexto;
            this.Archivo = Mensaje.sArchivo;
            this.Estado = Mensaje.nEstado;
            this.EventoHilo = Mensaje.EventoHilo;
            this.Cliente = Mensaje.Cliente;
            this.Funcionario = Mensaje.Funcionario;
            this.FechaCreacion = Mensaje.dFechaCreacion;
            this.FechaModificacion = Mensaje.dFechaModificacion;
            
        }
    }
}
