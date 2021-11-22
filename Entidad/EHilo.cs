using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class EHilo
    {
        public long HiloId { get; set; }
        public string Codigo { get; set; }
        public int Estado { get; set; }
        public Funcionario Funcionario { get; set; }
        public EventoHilo EventoHilo { get; set; }
        public Mensaje Mensaje { get; set; }
        public EstadoEvento EstadoEvento { get; set; }
        public ICollection<Mensaje> Mensajes { get; set; }
        public Cliente Cliente { get; set; }
        public Evento Evento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }


        public EHilo()
        {
        }
        public EHilo(Hilo hilo)
        {
            this.HiloId = hilo.nHiloId;
            this.Codigo = hilo.sCodigo;
            this.Estado = hilo.nEstado;
            this.Funcionario = hilo.Funcionario;
            this.EventoHilo = hilo.EventoHilo.FirstOrDefault();
            this.Mensaje = hilo.EventoHilo.FirstOrDefault().Mensaje.OrderByDescending(m => m.nMensajeId).FirstOrDefault();
            this.Mensajes = hilo.EventoHilo.FirstOrDefault().Mensaje.OrderByDescending(m => m.nMensajeId).ToList();
            this.Cliente = hilo.EventoHilo.FirstOrDefault().Evento.Cliente;
            this.EstadoEvento = hilo.EventoHilo.FirstOrDefault().Evento.EstadoEvento;
            this.Evento = hilo.EventoHilo.FirstOrDefault().Evento;
            this.FechaCreacion = hilo.dFechaCreacion;
            this.FechaModificacion = hilo.dFechaModificacion;
        }


       
    }
}
