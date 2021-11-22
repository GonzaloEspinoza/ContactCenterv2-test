using DataP;
using Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Backend.Models
{
    public class Util
    {
        public static List<Permiso> DevolverPermisoMenu(HttpSessionStateBase Session, string Controller)
        {
            Usuario oUsuario = SessionToUsuario<Usuario>(Session["Usuario"]);
            LPermiso NPermiso = LPermiso.Instancia.LPermiso;
            return NPermiso.DevolverPermisoMenu(oUsuario, Controller);
        }

        public static T SessionToUsuario<T>(object sesion) where T : Usuario
        {
            T oUsuario = null;
            if (sesion != null)
            {
                try
                {
                    oUsuario = (T)sesion;
                }
                catch
                {
                    oUsuario = null;
                }
            }
            return oUsuario;

        }
    }
}