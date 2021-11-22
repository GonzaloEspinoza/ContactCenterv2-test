
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
    
public partial class Permiso
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Permiso()
    {

        this.Hijos = new HashSet<Permiso>();

        this.Usuario = new HashSet<Usuario>();

        this.Perfil = new HashSet<Perfil>();

    }


    public long PermisoId { get; set; }

    public string Nombre { get; set; }

    public int Tipo { get; set; }

    public string Descripcion { get; set; }

    public Nullable<long> PermisoPadreId { get; set; }

    public string Pantalla { get; set; }

    public int Orden { get; set; }

    public bool Menu { get; set; }

    public bool Seleccionable { get; set; }

    public string IconoMenu { get; set; }

    public int Accion { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Permiso> Hijos { get; set; }

    public virtual Permiso Padre { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Usuario> Usuario { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Perfil> Perfil { get; set; }

}

}
