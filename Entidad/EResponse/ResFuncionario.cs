using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;

namespace Entidad
{
    public class ResFuncionario
    {
        public long FuncionarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Ci { get; set; }
        public string FotoPefil { get; set; }

        public ResFuncionario()
        {

        }

        public ResFuncionario(Funcionario f)
        {
            this.FuncionarioId = f.nFuncionarioId;
            this.Nombre = f.sNombre;
            this.Apellido = f.sApellido;
            this.Ci = f.sCi;
            this.FotoPefil = f.sFotoPefil;
        }

    }
}
