using Backend.Models;
using DataP;
using Entidad;
using Entidad.Enums;
using Logica;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
namespace BackEnd.Controllers
{
    public class EmpresaController : Controller
    {

        private LEmpresa lLogica = LEmpresa.Instancia.LEmpresa;
        private string Controller = "Empresa";
        private string Titulo = "Empresa";
        private string TituloLowerCase = "Empresa";
        private string TituloSingleLower = "Empresa";
        private string TituloSingleUpper = "Empresa";
        public ActionResult Index()
         {

            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            try
            {
                ViewBag.Toasts = Session["Toasts"];
                Session["Toasts"] = null;
                return View(lLogica.Obtener());
            }
            catch (Exception ex)
            {
                ViewBag.Titulo = "Inicio";
                ViewBag.Mensaje = ex.Message;
                ViewBag.Controlador = "Inicio";
                return PartialView("NotFound404");
            }
        }


        public ActionResult Empresa(long id)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            Usuario usuario = (Usuario)Session["Usuario"];

            if (id > 0)
            {
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
                {
                    ViewBag.Titulo = Titulo;
                    ViewBag.Mensaje = "Usted no puede editar " + TituloLowerCase + ". Contacte al administrador.";
                    ViewBag.Controlador = Controller;
                    return PartialView("NotFound404");
                }
                ViewBag.Accion = "Modificar";
                ViewBag.Title = "Editar " + Titulo;
                EEmpresa entidad = null;
                try
                {
                    entidad = lLogica.Obtener(id);
                }
                catch (BussinessException ex)
                {
                    ViewBag.Titulo = Titulo;
                    ViewBag.Mensaje = ex.Message;
                    ViewBag.Controlador = Controller;
                    return PartialView("NotFound404");
                }
                catch (Exception ex)
                {
                    ViewBag.Titulo = Titulo;
                    ViewBag.Mensaje = "Error, Contacte al administrador.";
                    ViewBag.Controlador = Controller;
                    return PartialView("NotFound404");
                }

                return View("FormularioEmpresa", entidad);
            }
            else
            {
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Crear))
                {
                    ViewBag.Titulo = Titulo;
                    ViewBag.Mensaje = "Usted no puede dar de alta " + TituloLowerCase + ". Contacte al administrador.";
                    ViewBag.Controlador = Controller;
                    return PartialView("NotFound404");
                }
                ViewBag.Accion = "Nuevo";
                ViewBag.Title = "Nuevo " + Titulo;
                return View("FormularioEmpresa", new EEmpresa());
            }
        }

        [HttpPost]
        public ActionResult Listar(string estado)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            List<EEmpresa> Entidades = new List<EEmpresa>();
            try
            {
                Usuario usuarioL = (Usuario)Session["Usuario"];
                Entidades = lLogica.ObtenerLista(estado);
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            return PartialView("_ListadoEmpresa", Entidades);
        }

        public ActionResult detalle(long id)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            Usuario usuario = (Usuario)Session["Usuario"];

            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Listar))
            {
                ViewBag.Titulo = Titulo;
                ViewBag.Mensaje = "Usted no puede listar " + TituloLowerCase + ". Contacte al administrador.";
                ViewBag.Controlador = Controller;
                return PartialView("NotFound404");
            }
            ViewBag.Accion = "";
            ViewBag.Title = "Detalle " + Titulo;
            EEmpresa entidad = null;
            try
            {
                entidad = lLogica.Obtener(id);
            }
            catch (BussinessException ex)
            {
                ViewBag.Titulo = Titulo;
                ViewBag.Mensaje = ex.Message;
                ViewBag.Controlador = Controller;
                return PartialView("NotFound404");
            }
            catch (Exception ex)
            {
                ViewBag.Titulo = Titulo;
                ViewBag.Mensaje = "Error, Contacte al administrador.";
                ViewBag.Controlador = Controller;
                return PartialView("NotFound404");
            }

            return View("Detalle", entidad);

        }

        [HttpPost]
        [ValidateInput(false)]
        public JavaScriptResult Nuevo(string nombre, string url,string logo, bool habilitado)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Crear))
            {
                return JavaScript("MostrarMensaje('Usted no puede dar de alta " + TituloLowerCase + ". Contactese con el administrador.');");
            }
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa Empresa = new Empresa()
                {
                    sNombre = nombre,
                    sUrl = url,
                
                    nEstado = habilitado ? (int)Estado.Habilitado : (int)Estado.Desabilitado
                };
                if (!string.IsNullOrWhiteSpace(logo))
                {
                    Empresa.sLogo = logo.Substring(logo.IndexOf(",") + 1);
                }


                lLogica.Nuevo(Empresa, usuario.UsuarioId);
                Session["Toasts"] = new List<string>() { "Nuevo " + TituloSingleUpper + ", los datos del nuevo " + TituloSingleLower + " se guardaron exitosamente." };
                return JavaScript("redireccionar('" + Url.Action("Index", Controller) + "');");
            }
            catch (BussinessException ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('Hubo un problema, contacte al administrador.');");
            }
        }




        [HttpPost]
        [ValidateInput(false)]
        public JavaScriptResult Modificar(long id, string nombre,string url, string logo, bool habilitado)
        {

            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
            {
                return JavaScript("MostrarMensaje('Usted no puede editar " + Titulo + ". Contactese con el administrador.');");
            }

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Empresa Entidad = new Empresa()
                {
                    nEmpresaId = id,
                    sNombre = nombre,
                    sUrl = url,
                    nEstado = habilitado ? (int)Estado.Habilitado : (int)Estado.Desabilitado
                };
                if (!string.IsNullOrWhiteSpace(logo))
                {
                    Entidad.sLogo = logo.Substring(logo.IndexOf(",") + 1);
                }
                lLogica.Modificar(Entidad, usuario.UsuarioId);

                Session["Toasts"] = new List<string>() { "Actualización de " + TituloSingleUpper + ", los datos de la " + TituloSingleLower + " se guardaron exitosamente." };
                return JavaScript("redireccionar('" + Url.Action("Index", Controller) + "');");
            }
            catch (BussinessException ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('Hubo un problema, contacte al administrador.');");
            }
        }

        [HttpPost]
        public ActionResult CambiarEstado(long id, Estado estado)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            if (estado == Estado.Eliminado)
            {
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Eliminar))
                {
                    return new HttpStatusCodeResult(400, "Usted no puede eliminar " + Titulo + ". Contactese con el administrador.");
                }
            }
            else
            {
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
                {
                    return new HttpStatusCodeResult(400, "Usted no puede editar " + Titulo + ". Contactese con el administrador.");
                }
            }

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                lLogica.CambiarEstado(id, estado, usuario.UsuarioId);
                return PartialView("_ListadoEmpresa", lLogica.Obtener());

            }
            catch (BussinessException ex)
            {
                return new HttpStatusCodeResult(400, ex.Message);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, "Hubo un problema, contacte al administrador.");
            }
        }
    }

}



