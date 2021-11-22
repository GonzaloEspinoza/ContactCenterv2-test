using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class EEventoHilo
    {
        public long EventoHiloId { get; set; }
        public Evento Evento { get; set; }
        public Funcionario Funcionario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long UsuarioCreador { get; set; }
        public long UsuarioModificador { get; set; }

        public Cliente Cliente { get; set; }

        public EEventoHilo()
        {

        }
        public EEventoHilo(EventoHilo EventoHilo)
        {
            this.EventoHiloId = EventoHilo.nEventoHiloId;
            this.Evento = EventoHilo.Evento;
            this.FechaCreacion = EventoHilo.dFechaCreacion;
            this.FechaModificacion = EventoHilo.dFechaModificacion;
            this.Cliente = EventoHilo.Evento.Cliente;
        }
    }
}
