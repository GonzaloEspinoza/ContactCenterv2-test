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
    
    public partial class Pais
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pais()
        {
            this.Ciudad = new HashSet<Ciudad>();
        }
    
        public long nPaisId { get; set; }
        public string sNombre { get; set; }
        public string sCodigo { get; set; }
        public string sBandera { get; set; }
        public int nEstado { get; set; }
        public System.DateTime dFechaCreacion { get; set; }
        public System.DateTime dFechaModificacion { get; set; }
        public long nUsuarioCreador { get; set; }
        public long nUsuarioModificador { get; set; }
        public string sSiglaMoneda { get; set; }
        public string sMoneda { get; set; }
        public string sSiglaPais { get; set; }
        public int nDiferenciaHorario { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ciudad> Ciudad { get; set; }
    }
}
