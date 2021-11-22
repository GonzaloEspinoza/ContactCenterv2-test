using DataP;
using Entidad;
using Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace BackEnd.Controllers
{
    public class CambioContrasenaController : Controller
    {
        // GET: CambioContrasena
        private LUsuarioBackEnd LUsuarioBackEnd = LUsuarioBackEnd.Instancia.LUsuarioBackEnd;
        public ActionResult Index()
        {
            return View();
        }
        public JavaScriptResult CambiarContrasena(string oldPass, string newPass1, string newPass2)
        {
            try
            {
                if (Session["Usuario"] != null)
                {
                    Usuario oUsuario = (Usuario)Session["Usuario"];
                    oldPass = EUtils.MD5Hash(oldPass);
                    if (oUsuario.Password == oldPass)
                    {
                        if (newPass1.Equals(newPass2))
                        {
                            bool formatoCorrecto = Regex.IsMatch(newPass1, @"(?=^\S{6,15}$)(^(?=\S*[a-zA-Z])(?=\S*\d).*$)");

                            if (formatoCorrecto)
                            {
                                Usuario us = LUsuarioBackEnd.CambiarContrasena(oUsuario.UsuarioId, oUsuario.Password, EUtils.MD5Hash(newPass1));
                                if (us == null)
                                {
                                    throw new Exception("Los datos ingresados son inválidos");
                                }
                                Session["Usuario"] = us;
                                return JavaScript("redireccionar('" + Url.Action("Index", "Inicio") + "');");
                            }
                            else
                            {
                                throw new Exception("El formato de la nueva contraseña no es válido.");
                            }
                        }
                        else
                        {
                            throw new Exception("No existe coincidencia entre las contraseñas.");
                        }
                    }
                    else
                    {
                        throw new Exception("Contraseña antigua inválida.");
                    }
                }
                else
                {
                    throw new Exception("Debe iniciar sesión para cambiar la contraseña.");
                }
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
        }
    }

}