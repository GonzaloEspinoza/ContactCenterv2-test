using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ResEstadoEvento
    {
        public long EstadoEventoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public ResEstadoEvento()
        {

        }

        public ResEstadoEvento(EstadoEvento estadoEvento)
        {
            this.EstadoEventoId = estadoEvento.nEstadoEventoId;
            this.Nombre = estadoEvento.sNombre;
            this.Descripcion = estadoEvento.sDescripcion;
        }

    }
}
