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
    public class AreaController : Controller
    {

        private LArea lLogica = LArea.Instancia.LArea;
        private LEmpresa lEmpresa =LEmpresa.Instancia.LEmpresa;
        private string Controller = "Area";
        private string Titulo = "Area";
        private string TituloLowerCase = "Area";
        private string TituloSingleLower = "Area";
        private string TituloSingleUpper = "Area";
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


        public ActionResult Area(long id)
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
                EArea entidad = null;
                try
                {
                    entidad = lLogica.Obtener(id);
                    var empresas = lEmpresa.Obtener();
                 
                    ViewBag.Empresas = empresas;
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

                return View("FormularioArea", entidad);
            }
            else
            {
                ViewBag.Empresas = lEmpresa.Obtener();
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Crear))
                {
                    ViewBag.Titulo = Titulo;
                    ViewBag.Mensaje = "Usted no puede dar de alta " + TituloLowerCase + ". Contacte al administrador.";
                    ViewBag.Controlador = Controller;
                    return PartialView("NotFound404");
                }
                ViewBag.Accion = "Nuevo";
                ViewBag.Title = "Nuevo " + Titulo;
                return View("FormularioArea", new EArea());
            }
        }

        [HttpPost]
        public ActionResult Listar(string estado)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            List<EArea> Entidades = new List<EArea>();
            try
            {
                Usuario usuarioL = (Usuario)Session["Usuario"];
                Entidades = lLogica.ObtenerLista(estado);
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            return PartialView("_ListadoArea", Entidades);
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
            EArea entidad = null;
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
        public JavaScriptResult Nuevo(string nombre, long empresaId, bool habilitado)
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
                Area Area = new Area()
                {
                    sNombre = nombre,
                    nEmpresaId=empresaId,
                    nEstado = habilitado ? (int)Estado.Habilitado : (int)Estado.Desabilitado
                };
               
                lLogica.Nuevo(Area, usuario.UsuarioId);
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
        public JavaScriptResult Modificar(long id, string nombre,long empresaId, bool habilitado)
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
                Area Entidad = new Area()
                {
                    nAreaId = id,
                    sNombre = nombre,
                    nEmpresaId=empresaId,
                    nEstado = habilitado ? (int)Estado.Habilitado : (int)Estado.Desabilitado
                };
                
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


