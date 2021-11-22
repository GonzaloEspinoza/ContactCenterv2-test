using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class EHorarioColaborador
    {
        public long HorarioFuncionarioId { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int Estado { get; set; }
        public Dia Dia { get; set; }
        public Funcionario Funcionario { get; set; }


        public EHorarioColaborador()
        {

        }
        public EHorarioColaborador(HorarioColaborador horarioColaborador)
        {
            this.HorarioFuncionarioId = horarioColaborador.nHorarioFuncionarioId;
            this.HoraInicio = horarioColaborador.tHoraInicio;
            this.HoraFin = horarioColaborador.tHoraFin;
            this.Estado = horarioColaborador.nEstado;
            this.Dia = horarioColaborador.Dia;
            this.Funcionario = horarioColaborador.Funcionario;
        }
    }
}
