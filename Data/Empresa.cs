
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
    
public partial class Empresa
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Empresa()
    {

        this.Area = new HashSet<Area>();

        this.Cliente = new HashSet<Cliente>();

        this.CategoriaPreguntaFrecuente = new HashSet<CategoriaPreguntaFrecuente>();

    }


    public long nEmpresaId { get; set; }

    public string sNombre { get; set; }

    public string sUrl { get; set; }

    public string sLogo { get; set; }

    public int nEstado { get; set; }

    public System.DateTime dFechaCreacion { get; set; }

    public System.DateTime dFechaModificacion { get; set; }

    public long nUsuarioCreador { get; set; }

    public long nUsuarioModificador { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Area> Area { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Cliente> Cliente { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<CategoriaPreguntaFrecuente> CategoriaPreguntaFrecuente { get; set; }

}

}
