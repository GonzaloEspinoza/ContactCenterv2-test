using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class EEvento
    {
        public long EventoId { get; set; }
        public EstadoEvento EstadoEvento { get; set; }
        public Cliente Cliente { get; set; }
        public TipoAtencion TipoAtencion { get; set; }
        public ICollection<HistorialEstadoEvento> HistorialEstadoEvento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }


        public EEvento()
        {
        }
        public EEvento(Evento evento)
        {
            this.EventoId = evento.nEventoId;
            this.EstadoEvento = evento.EstadoEvento;
            this.Cliente = evento.Cliente;
            this.TipoAtencion = evento.TipoAtencion;
            this.HistorialEstadoEvento = evento.HistorialEstadoEvento;
            this.FechaModificacion = evento.dFechaCreacion;
            this.FechaModificacion = evento.dFechaModificacion;
        }
    }
}
