
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Data
{

using System;
    using System.Collections.Generic;
    
public partial class HorarioColaborador
{

    public long nHorarioFuncionarioId { get; set; }

    public System.TimeSpan tHoraInicio { get; set; }

    public System.TimeSpan tHoraFin { get; set; }

    public int nEstado { get; set; }

    public long nFuncionarioId { get; set; }

    public long nDiaId { get; set; }

    public System.DateTime dFechaCreacion { get; set; }

    public System.DateTime dFechaModificacion { get; set; }

    public long nUsuarioCreador { get; set; }

    public long nUsuarioModificador { get; set; }



    public virtual Dia Dia { get; set; }

    public virtual Funcionario Funcionario { get; set; }

}

}
