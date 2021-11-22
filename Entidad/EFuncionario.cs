using DataP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{
    public class EFuncionario : Funcionario
    {
        public string sNombreArea { get; set; }
        public string sNombreCiudad { get; set; }

        
        public string TituloEliminar { get; set; }
        public string TituloDeshabilitar { get; set; }

        public ICollection<HorarioColaborador> Horarios { get; set; }
        public EFuncionario(Funcionario f)
        {
            this.nFuncionarioId = f.nFuncionarioId;
            this.sNombre = f.sNombre;
            this.sApellido = f.sApellido;
            this.sCi = f.sCi;
            this.sFotoPefil = f.sFotoPefil;
            this.nEstado = f.nEstado;
            this.Horarios = f.HorarioColaborador;
            this.sNombreArea = f.Area!=null? f.Area.sNombre:"Sin area";
            this.sNombreCiudad = f.Ciudad != null ? f.Ciudad.sNombre : "Sin ciudad";
        }

        public EFuncionario()
        {

        }
    }
}
