using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ResHilo
    {
        public long EventoHiloId { get; set; }
        public ResFuncionario Funcionario { get; set; }
        public ResEvento Evento { get; set; }
        public ResHilo()
        {
        }
        public ResHilo(Hilo hilo)
        {
            this.EventoHiloId = hilo.EventoHilo.FirstOrDefault().nEventoHiloId;
            this.Funcionario = hilo.Funcionario!=null ? new ResFuncionario(hilo.Funcionario) : null;
            this.Evento = new ResEvento(hilo.EventoHilo.FirstOrDefault().Evento);
        }
    }
}
