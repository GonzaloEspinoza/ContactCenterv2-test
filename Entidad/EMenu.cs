using DataP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{
    public class EMenu : Permiso
    {
        public EMenu(Permiso entidad)
            : base()
        {
            HijosMenu = new List<EMenu>();
            Descripcion = entidad.Descripcion;
            PermisoId = entidad.PermisoId;
            Nombre = entidad.Nombre;
            PermisoPadreId = entidad.PermisoPadreId;
            Pantalla = entidad.Pantalla;
            Orden = entidad.Orden;
            Menu = entidad.Menu;
            IconoMenu = entidad.IconoMenu;
            Seleccionable = entidad.Seleccionable;
            foreach (var item in entidad.Hijos)
            {
                HijosMenu.Add(new EMenu(item));
            }
        }

        public EMenu()
            : base()
        {
            HijosMenu = new List<EMenu>();
        }
        public bool Activado { get; set; }
        public bool InPerfil { get; set; }

        public List<EMenu> HijosMenu { get; set; }

        public bool Selected(string action)
        {
            bool sw = !string.IsNullOrWhiteSpace(Pantalla) && action.Equals(Pantalla.Split('/')[0].ToUpper());
            if (sw)
            {
                return true;
            }
            foreach (var menu in HijosMenu)
            {
                if (menu.Menu)
                {
                    sw = menu.Selected(action) || sw;
                }
            }
            return sw;
        }



    }
}