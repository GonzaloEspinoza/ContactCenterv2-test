using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ResEvento
    {
        public long EventoId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public ResEstadoEvento EstadoEvento { get; set; }
        public ResTipoAtencion TipoAtencion { get; set; }

        public ResEvento()
        {

        }

        public ResEvento(Evento evento)
        {
            this.EventoId = evento.nEventoId;
            this.FechaCreacion = evento.dFechaCreacion;
            this.EstadoEvento = new ResEstadoEvento(evento.EstadoEvento);
            this.TipoAtencion = new ResTipoAtencion(evento.TipoAtencion);
        }

    }
}
