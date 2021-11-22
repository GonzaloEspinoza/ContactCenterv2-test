using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ResMensajeV2
    {
        public string Mensaje { get; set; }
        public string Archivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public ResFuncionario Funcionario { get; set; }
        public ResCliente Cliente { get; set; }
        public ResEvento Evento { get; set; }

        public ResMensajeV2()
        {
        }
        public ResMensajeV2(Mensaje mensaje)
        {
            this.Mensaje = mensaje.sTexto;
            this.Archivo = EUtils.URLImagen(mensaje.sArchivo);
            this.FechaCreacion = mensaje.dFechaCreacion;
            this.Funcionario = mensaje.Funcionario==null?new ResFuncionario(): new ResFuncionario(mensaje.Funcionario);
            this.Cliente = mensaje.Cliente == null ? new ResCliente() : new ResCliente(mensaje.Cliente);
            this.Evento = mensaje.EventoHilo.Evento == null ? new ResEvento() : new ResEvento(mensaje.EventoHilo.Evento);
        }
    }
}
