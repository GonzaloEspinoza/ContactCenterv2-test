//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataP
{
    using System;
    using System.Collections.Generic;
    
    public partial class CategoriaPreguntaFrecuente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CategoriaPreguntaFrecuente()
        {
            this.PreguntaFrecuente = new HashSet<PreguntaFrecuente>();
        }
    
        public long nCategoriaPreguntaFrecuenteId { get; set; }
        public string sNombre { get; set; }
        public long nEmpresaId { get; set; }
        public long nUsuarioCreador { get; set; }
        public long nUsuarioModificador { get; set; }
        public System.DateTime dFechaCreacion { get; set; }
        public System.DateTime dFechaModificacion { get; set; }
        public int nEstado { get; set; }
    
        public virtual Empresa Empresa { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PreguntaFrecuente> PreguntaFrecuente { get; set; }
    }
}
