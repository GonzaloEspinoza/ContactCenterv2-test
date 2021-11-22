using DataP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{
    public class EPerfil : Perfil
    {

        public string TituloEliminar { get; set; }
        public string TituloDeshabilitar { get; set; }

        public EPerfil(Perfil entidad)
            : base()
        {
            Decripcion = entidad.Decripcion;
            PerfilId = entidad.PerfilId;
            Nombre = entidad.Nombre;
            Estado = entidad.Estado;
            Tipo = entidad.Tipo;
            Habilitado = entidad.Estado == (int)Enums.Estado.Habilitado;
            ListaPermiso = new List<EPermiso>();
            Seleccionable = entidad.Seleccionable;
            Editable = entidad.Editable;
        }

        public EPerfil()
            : base()
        {
            ListaPermiso = new List<EPermiso>();
        }
        public bool Habilitado { get; set; }

        public bool Selected { get; set; }

        public List<EPermiso> ListaPermiso { get; set; }

        public Perfil toPerfil()
        {
            Perfil entidad = new Perfil();
            entidad.PerfilId = PerfilId;
            entidad.Tipo = Tipo;
            entidad.Nombre = Nombre;
            entidad.Decripcion = Decripcion;
            entidad.Estado = Habilitado ? (int)Enums.Estado.Habilitado : (int)Enums.Estado.Desabilitado;
            foreach (var item in ListaPermiso)
            {
                entidad.Permiso.Add(item);
            }
            return entidad;
        }
    }
}
