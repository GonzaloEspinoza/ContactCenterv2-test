using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ECalificacionColaborador
    {
        public long CalificacionColaboradorId { get; set; }
        public string Comentario { get; set; }
        public decimal Puntuacion { get; set; }
        public Funcionario Funcionario { get; set; }
        public Cliente Cliente { get; set; }
        public Evento Evento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }


        public ECalificacionColaborador()
        {

        }
        public ECalificacionColaborador(CalificacionColaborador calificacion)
        {
            this.CalificacionColaboradorId = calificacion.nCalificacionColaboradorId;
            this.Comentario = calificacion.sComentario;
            this.Puntuacion = calificacion.nPuntuacion;
            this.Funcionario = calificacion.Funcionario;
            this.Cliente = calificacion.Cliente;
            this.Evento = calificacion.Evento;
            this.FechaCreacion = calificacion.dFechaCreacion;
            this.FechaModificacion = calificacion.dFechaModificacion;
        }
    }
}
