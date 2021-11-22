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
    
    public partial class Cliente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cliente()
        {
            this.CalificacionColaborador = new HashSet<CalificacionColaborador>();
            this.Evento = new HashSet<Evento>();
            this.Mensaje = new HashSet<Mensaje>();
        }
    
        public long nClienteId { get; set; }
        public string sCodigoCliente { get; set; }
        public string sNombres { get; set; }
        public string sApellidos { get; set; }
        public string sUrlFoto { get; set; }
        public string sTokenNotificacionFirebase { get; set; }
        public string sTipoSistemaOperativo { get; set; }
        public int nEstado { get; set; }
        public long nEmpresaId { get; set; }
        public System.DateTime dFechaCreacion { get; set; }
        public System.DateTime dFechaModificacion { get; set; }
        public string sCodigoPais { get; set; }
        public string sNumeroCelular { get; set; }
        public string sCorreo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CalificacionColaborador> CalificacionColaborador { get; set; }
        public virtual Empresa Empresa { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Evento> Evento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mensaje> Mensaje { get; set; }
    }
}
