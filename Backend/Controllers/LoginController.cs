using DataP;
using Entidad;
using Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Backend.Controllers
{

    [AllowAnonymous]
    public class LoginController : Controller
    {
        // GET: Login
        private LUsuarioBackEnd LUsuarioBackEnd = LBaseCC<Usuario>.Instancia.LUsuarioBackEnd;

        // GET: Login
        public ActionResult Index()
        {
            ViewBag.Titulo = "Inicio sesión";
            return View();
        }

        public ActionResult Bloquear()
        {
            if (Session["Usuario"] == null)
            {
                return View("Index");
            }
            else
            {
                Session["Bloqueado"] = true;
            }
            return View("page_user_lock");
        }

        [HttpPost]
        public JavaScriptResult IniciarSesion(string usr, string pass)
        {
            try
            {
#if DEBUG

                //Session["Usuario"] = LUsuarioBackEnd.IniciarSesion("admin", "Alguien", true) ;
                //return JavaScript("redireccionar('" + Url.Action("Index", "Inicio") + "');");
#endif
                if (Session["menuAbierto"] == null)
                {
                    Session["menuAbierto"] = true;
                }
               
                
                Usuario us = LUsuarioBackEnd.IniciarSesion(usr, EUtils.MD5Hash(pass), true);
                if (us == null)
                {
                    throw new Exception("Nombre de <b>Usuario</b> o <b>Contraseña</b> incorrectos");
                }
                Session["Bloqueado"] = false;
                if (us.PrimeraVez == true)
                {
                    Session["UsuarioTemp"] = us;
                    return JavaScript("PopupCambiarContrasena();");
                }
                else
                {
                    Session["Usuario"] = us;
                    Session["UsuarioToken"] = Guid.NewGuid().ToString();
                    return JavaScript("redireccionar('" + Url.Action("Index", "Inicio") + "');");
                }
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message.Replace("'", "");
                ViewBag.Mensaje = mensaje;
                return JavaScript("MostrarMensaje('" + mensaje + "');");
            }
        }



        public ActionResult CerrarSesion()
        {
            Session.RemoveAll();
            return RedirectToAction("Index", "Login");
        }

        public JavaScriptResult MantenerEnSesion(long param1)
        {
            try
            {
                Usuario usuarioLogueado = (Usuario)Session["Usuario"];
                if (usuarioLogueado == null || usuarioLogueado.UsuarioId != param1)
                {
                    var Result = LUsuarioBackEnd.IniciarSesion(param1);
                    string usuariotoken = (string)Session["UsuarioToken"];
                    if (string.IsNullOrEmpty(usuariotoken))
                    {
                        Session["UsuarioToken"] = Guid.NewGuid().ToString();
                    }
                    Session["Usuario"] = Result;
                }
                return JavaScript("");
            }
            catch (Exception ex)
            {
                Session.RemoveAll();
                if (Request.IsAjaxRequest())
                {
                    Session["Error"] = "Sus permisos han sido modificados, inicie sesión nuevamente";
                }
                return JavaScript("redireccionar('" + Url.Action("Index", "Login") + "');");
            }
        }

        public JavaScriptResult CambiarContrasena(string newPass1, string newPass2)
        {
            try
            {
                if (Session["UsuarioTemp"] != null)
                {
                    if (newPass1 != string.Empty && newPass2 != string.Empty)
                    {
                        Usuario oUsuario = (Usuario)Session["UsuarioTemp"];

                        if (newPass1.Equals(newPass2))
                        {
                            bool formatoCorrecto = Regex.IsMatch(newPass1, @"(?=^\S{6,20}$)(^(?=\S*[a-zA-Z])(?=\S*\d).*$)");

                            if (formatoCorrecto)
                            {

                                Usuario us = LUsuarioBackEnd.CambiarContrasena(oUsuario.UsuarioId, oUsuario.Password, EUtils.MD5Hash(newPass1));
                                if (us == null)
                                {
                                    throw new Exception("Nombre de <b>Usuario</b> o <b>Contraseña</b> incorrectos.");
                                }
                                Session.Remove("UsuarioTemp");
                                Session["Usuario"] = us;
                                Session["UsuarioToken"] = Guid.NewGuid().ToString();
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
                        throw new Exception("Por favor, ingresar todos los campos.");
                    }
                }
                else
                {
                    throw new Exception("Debe iniciar sesión para cambiar la contraseña.");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
                return JavaScript("MostrarMensajeCambiarPass('" + ex.Message + "');");
            }
        }
    }
}