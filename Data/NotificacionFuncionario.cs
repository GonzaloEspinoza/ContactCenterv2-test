
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
    
public partial class NotificacionFuncionario
{

    public long nNotificacionFuncionarioId { get; set; }

    public long nFuncionarioId { get; set; }

    public string sTitulo { get; set; }

    public string sDescripcion { get; set; }

    public string sUrl { get; set; }

    public int nEstado { get; set; }

    public System.DateTime dFechaCreacion { get; set; }

    public int nLeido { get; set; }

    public int nTipoNotificacion { get; set; }

    public long nIdImplicado { get; set; }



    public virtual Funcionario Funcionario { get; set; }

}

}
