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
    public class PaisController : Controller
    {
        private LPais lLogica = LPais.Instancia.LPais;
        private string Controller = "Pais";
        private string Titulo = "Pais";
        public ActionResult Index()
        {

            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            try
            {
                ViewBag.Titulo = "Paises";
                ViewBag.Toasts = Session["Toasts"];
                Session["Toasts"] = null;
                return View(lLogica.Obtener());
            }
            catch (Exception ex)
            {
                ViewBag.Titulo = Titulo;
                ViewBag.Mensaje = ex.Message;
                ViewBag.Controlador = Controller;
                return PartialView("NotFound404");
            }
        }


        public ActionResult Pais(long id)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            Usuario usuario = (Usuario)Session["Usuario"];

            if (id > 0)
            {
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
                {
                    ViewBag.Titulo = Titulo;
                    ViewBag.Mensaje = "Usted no puede editar " + Titulo + ". Contacte al administrador.";
                    ViewBag.Controlador = Controller;
                    return PartialView("NotFound404");
                }
                ViewBag.Accion = "Modificar";
                ViewBag.Titulo = "Editar " + Titulo;
                EPais entidad = new EPais();
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

                return View("FormularioPais", entidad);
            }
            else
            {
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Crear))
                {
                    ViewBag.Titulo = Titulo;
                    ViewBag.Mensaje = "Usted no puede dar de alta " + Titulo + ". Contacte al administrador.";
                    ViewBag.Controlador = Controller;
                    return PartialView("NotFound404");
                }
                ViewBag.Accion = "Nuevo";
                ViewBag.Titulo = "Nuevo " + Titulo;
                return View("FormularioPais", new EPais());
            }
        }

        [HttpPost]
        public ActionResult Listar(string estado)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            List<EPais> Entidades = new List<EPais>();
            try
            {
                Usuario usuarioL = (Usuario)Session["Usuario"];
                Entidades = lLogica.ObtenerLista(estado);
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            return PartialView("_ListadoPais", Entidades);
        }

        public ActionResult detalle(long id)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            Usuario usuario = (Usuario)Session["Usuario"];

            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Listar))
            {
                ViewBag.Titulo = Titulo;
                ViewBag.Mensaje = "Usted no puede listar " + Titulo + ". Contacte al administrador.";
                ViewBag.Controlador = Controller;
                return PartialView("NotFound404");
            }
            ViewBag.Accion = "";
            ViewBag.Titulo = "Detalle de " + Titulo;
            EPais entidad = null;
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
        public JavaScriptResult Nuevo(string nombre, string bandera, string codigo, bool habilitado,int diferenciaHorario, string siglaMoneda
            , string moneda, string siglaPais)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Crear))
            {
                return JavaScript("MostrarMensaje('Usted no puede dar de alta " + Titulo + ". Contactese con el administrador.');");
            }
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Pais Pais = new Pais()
                {
                    sNombre = nombre,
                    sCodigo = codigo,
                    nEstado = habilitado ? (int)Estado.Habilitado : (int)Estado.Desabilitado,
                    nDiferenciaHorario = diferenciaHorario,
                    sSiglaMoneda = siglaMoneda,
                    sMoneda = moneda,
                    sSiglaPais = siglaPais
            };
                if (!string.IsNullOrWhiteSpace(bandera))
                {
                    Pais.sBandera = bandera.Substring(bandera.IndexOf(",") + 1);
                }
                lLogica.Nuevo(Pais, usuario.UsuarioId);
                Session["Toasts"] = new List<string>() { "Nuevo " + Titulo + ", los datos del nuevo " + Titulo + " se guardaron exitosamente." };
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
        public JavaScriptResult Modificar(long id, string nombre, string codigo, string bandera, bool habilitado, int diferenciaHorario, string siglaMoneda
            , string moneda, string siglaPais)
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
                Pais Entidad = new Pais()
                {
                    nPaisId = id,
                    sNombre = nombre,
                    sCodigo = codigo,
                    nEstado = habilitado ? (int)Estado.Habilitado : (int)Estado.Desabilitado,
                    nDiferenciaHorario = diferenciaHorario,
                    sSiglaMoneda = siglaMoneda,
                    sMoneda = moneda,
                    sSiglaPais = siglaPais
                };
                if (!string.IsNullOrWhiteSpace(bandera))
                {
                    Entidad.sBandera = bandera.Substring(bandera.IndexOf(",") + 1);
                }
                lLogica.Modificar(Entidad, usuario.UsuarioId);

                Session["Toasts"] = new List<string>() { "Actualización de " + Titulo + ", los datos del " + Titulo + " se guardaron exitosamente." };
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
                return PartialView("_ListadoPais", lLogica.Obtener());

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
