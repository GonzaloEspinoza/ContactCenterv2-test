using Backend.Models;
using DataP;
using Entidad;
using Logica;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Backend.Controllers
{
    public class PermisoController : Controller
    {

        private LPermiso LPermiso = LPermiso.Instancia.LPermiso;

        public ActionResult Index()
        {
            try
            {
                ViewBag.Toasts = Session["Toasts"];
                Session["Toasts"] = null;
                Usuario usuario = (Usuario)Session["Usuario"];
                return View(LPermiso.ObtenerPermisos(usuario.TipoUsuario));
            }
            catch (Exception ex)
            {
                ViewBag.Titulo = "Permiso";
                ViewBag.Mensaje = ex.Message;
                ViewBag.Controlador = "Inicio";
                return PartialView("NotFound404");
            }
        }

        public ActionResult Jerarquia()
        {
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                return View("FormularioJerarquia", LPermiso.ObtenerJerarquia(usuario.TipoUsuario));
            }
            catch (Exception ex)
            {
                ViewBag.Titulo = "Permiso";
                ViewBag.Mensaje = ex.Message;
                ViewBag.Controlador = "Inicio";
                return PartialView("NotFound404");
            }

        }

        public ActionResult Permiso(long id)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            if (id > 0)
            {
                ViewBag.Accion = "ModificarPermiso";
                ViewBag.Title = "Editar permiso";
                Permiso per = LPermiso.ObtenerPermiso(id, usuario.TipoUsuario);
                if (per == null)
                {
                    ViewBag.Titulo = "Permiso";
                    ViewBag.Mensaje = "No es posible encontrar el permiso seleccionado.";
                    ViewBag.Controlador = "Permiso";
                    return PartialView("NotFound404");
                }
                return View("FormularioPermiso", per);
            }
            else
            {
                ViewBag.Accion = "NuevoPermiso";
                ViewBag.Title = "Nuevo permiso";
                return View("FormularioPermiso", new Permiso());
            }
        }


        [HttpPost]
        public JavaScriptResult GuardarJerarquia(string jsonItems)
        {
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                List<ItemNestable> ItemNestable = JsonConvert.DeserializeObject<List<ItemNestable>>(jsonItems);
                int orden = 1;
                List<Permiso> Permisos = new List<DataP.Permiso>();
                foreach (var item in ItemNestable)
                {
                    Permisos.Add(RR(item, orden, out orden));
                }
                LPermiso.GuardarJerarquia(Permisos, usuario.UsuarioId);
                return JavaScript("redireccionar('" + Url.Action("Index", "Permiso") + "');");
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

        public Permiso RR(ItemNestable ItemNestable, int ordenPadre, out int ordenUltimo)
        {
            Permiso Permiso = new Permiso();
            Permiso.PermisoId = ItemNestable.id;
            Permiso.Orden = ordenPadre;
            ordenUltimo = ordenPadre + 1;
            if (ItemNestable.children != null)
            {
                foreach (var children in ItemNestable.children)
                {
                    Permiso PH = RR(children, ordenUltimo, out ordenUltimo);
                    PH.PermisoPadreId = ItemNestable.id;
                    Permiso.Hijos.Add(PH);
                }
            }
            return Permiso;
        }


        [HttpPost]
        public JavaScriptResult NuevoPermiso(string nom, string desc, string pantalla, int tipo, string icono, bool menu, int accion, bool seleccionable)
        {
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Permiso Permiso = new Permiso()
                {
                    Descripcion = desc,
                    Tipo = tipo,
                    IconoMenu = icono,
                    Nombre = nom,
                    Menu = menu,
                    Accion = accion,
                    Seleccionable = seleccionable,
                    Pantalla = pantalla
                };

                LPermiso.NuevoPermiso(Permiso, usuario.UsuarioId);
                Session["Toasts"] = new List<string>() { "Nuevo permiso,Los datos del nuevo permiso se guardaron exitosamente." };
                return JavaScript("redireccionar('" + Url.Action("Index", "Permiso") + "');");
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

        public JavaScriptResult ModificarPermiso(long id, string nom, string desc, string pantalla, int tipo, string icono, bool menu, int accion, bool seleccionable)
        {
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Permiso Permiso = new Permiso()
                {
                    PermisoId = id,
                    Descripcion = desc,
                    Tipo = tipo,
                    IconoMenu = icono,
                    Nombre = nom,
                    Menu = menu,
                    Accion = accion,
                    Seleccionable = seleccionable,
                    Pantalla = pantalla
                };

                LPermiso.ModificarPermiso(Permiso, usuario.UsuarioId);
                Session["Toasts"] = new List<string>() { "Actualización de permiso,Los datos del permiso se guardaron exitosamente." };
                return JavaScript("redireccionar('" + Url.Action("Index", "Permiso") + "');");
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
        public ActionResult EliminarPermiso(long idPermiso)
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                LPermiso.EliminarPermiso(idPermiso, usuario.UsuarioId);
                return PartialView("_ListadoPermiso", LPermiso.ObtenerPermisos(usuario.TipoUsuario));
            }
            catch (BussinessException ex)
            {
                return new HttpStatusCodeResult(400, ex.Message);
                //return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, "Hubo un problema, contacte al administrador.");
                //return JavaScript("MostrarMensaje('Hubo un problema, contacte al administrador.');");
            }
        }
    }
}