using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ResCalificacionColaborador
    {
        public long EventoId { get; set; }
        public ResFuncionario Colaborador { get; set; }


        public ResCalificacionColaborador()
        {
        }
        public ResCalificacionColaborador(CalificacionColaborador calificacion)
        {
            this.EventoId = calificacion.nEventoId;
            this.Colaborador = new ResFuncionario(calificacion.Funcionario);

        }
    }
}
