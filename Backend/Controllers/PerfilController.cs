using Backend.Models;
using DataP;
using Entidad;
using Entidad.Enums;
using Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Backend.Controllers
{
    public class PerfilController : Controller
    {
        private LPerfil LPerfil = LPerfil.Instancia.LPerfil;

        public ActionResult Index()
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, "Perfil");
            ViewBag.PermisosH = Hijos;
            try
            {
                ViewBag.Toasts = Session["Toasts"];
                Session["Toasts"] = null;
                Usuario usuario = (Usuario)Session["Usuario"];
                return View(LPerfil.ObtenerPerfilesListar(usuario.TipoUsuario));
            }
            catch (Exception ex)
            {
                ViewBag.Titulo = "Inicio";
                ViewBag.Mensaje = ex.Message;
                ViewBag.Controlador = "Inicio";
                return PartialView("NotFound404");
            }
        }

        public ActionResult Perfil(int id)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, "Perfil");
            ViewBag.PermisosH = Hijos;
            Usuario usuario = (Usuario)Session["Usuario"];
            List<EPermiso> Permisos = new List<EPermiso>();
            if (id > 0)
            {
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
                {
                    ViewBag.Titulo = "Perfiles";
                    ViewBag.Mensaje = "Usted no puede editar Perfiles. Contacte al administrador.";
                    ViewBag.Controlador = "Perfil";
                    return PartialView("NotFound404");
                }
                ViewBag.Accion = "ModificarPerfil";
                ViewBag.Title = "Editar perfil";
                List<EPermiso> PermisosDisponibles = new List<EPermiso>();
                EPerfil per = null;
                try
                {
                    per = LPerfil.ObtenerPerfil(id, usuario.TipoUsuario);
                    PermisosDisponibles = LPerfil.ObtenerPermisosSeleccionables(usuario.TipoUsuario);
                }
                catch (BussinessException ex)
                {
                    ViewBag.Titulo = "Perfiles";
                    ViewBag.Mensaje = ex.Message;
                    ViewBag.Controlador = "Perfil";
                    return PartialView("NotFound404");
                }
                catch (Exception ex)
                {
                    ViewBag.Titulo = "Perfiles";
                    ViewBag.Mensaje = "Error, Contacte al administrador.";
                    ViewBag.Controlador = "Perfil";
                    return PartialView("NotFound404");
                }

                if (per == null)
                {
                    ViewBag.Titulo = "Perfiles";
                    ViewBag.Mensaje = "No es posible encontrar el perfil seleccionado.";
                    ViewBag.Controlador = "Perfil";
                    return PartialView("NotFound404");
                }

                Permisos = new List<EPermiso>();
                foreach (var p in PermisosDisponibles)
                {
                    actualizarActivo(p, per.ListaPermiso);
                    Permisos.Add(p);
                }
                ViewBag.Permisos = Permisos;
                return View("FormularioPerfil", per);
            }
            else
            {
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Crear))
                {
                    ViewBag.Titulo = "Perfiles";
                    ViewBag.Mensaje = "Usted no puede dar de alta Perfiles. Contacte al administrador.";
                    ViewBag.Controlador = "Perfil";
                    return PartialView("NotFound404");
                }
                ViewBag.Accion = "NuevoPerfil";
                ViewBag.Title = "Nuevo perfil";
                try
                {
                    Permisos = LPerfil.ObtenerPermisosSeleccionables(usuario.TipoUsuario);
                }
                catch (BussinessException ex)
                {
                    ViewBag.Titulo = "Perfiles";
                    ViewBag.Mensaje = ex.Message;
                    ViewBag.Controlador = "Perfil";
                    return PartialView("NotFound404");
                }
                catch (Exception ex)
                {
                    ViewBag.Titulo = "Perfiles";
                    ViewBag.Mensaje = "Error, Contacte al administrador.";
                    ViewBag.Controlador = "Perfil";
                    return PartialView("NotFound404");
                }
                ViewBag.Permisos = Permisos;
                return View("FormularioPerfil", new EPerfil());
            }
        }

        public bool actualizarActivo(EPermiso p, List<EPermiso> ListPermisosPerfil)
        {
            if (ListPermisosPerfil.Exists(x => x.PermisoId == p.PermisoId))
            {
                p.Activado = true;
            }
            else
            {
                foreach (var item in ListPermisosPerfil)
                {
                    if (actualizarActivo(p, item.PermisosHijos))
                    {
                        break;
                    }
                }
            }
            foreach (var item in p.PermisosHijos)
            {
                actualizarActivo(item, ListPermisosPerfil);
            }
            return p.Activado;
        }

        [HttpPost]
        public JavaScriptResult NuevoPerfil(string nomPer, string descPer, string PermisosPerfiles, int tipo, bool habilitadoPer)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, "Perfil");
            ViewBag.PermisosH = Hijos;
            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Crear))
            {
                return JavaScript("MostrarMensaje('Usted no puede dar de alta Perfiles. Contactese con el administrador.');");
            }
            if (string.IsNullOrEmpty(nomPer))
            {
                return JavaScript("MostrarMensaje('Ingrese un nombre de perfil válido.');");
            }
            if (string.IsNullOrEmpty(descPer))
            {
                return JavaScript("MostrarMensaje('Ingrese una descripción válida.');");
            }
            if (PermisosPerfiles.Length < 1)
            {
                return JavaScript("MostrarMensaje('Debe seleccionar al menos una función disponible.');");
            }

            Perfil perfil = new Perfil();
            perfil.Nombre = nomPer;
            perfil.Decripcion = descPer;
            perfil.Tipo = tipo;
            perfil.Estado = habilitadoPer ? (int)Estado.Habilitado : (int)Estado.Desabilitado;
            perfil.Seleccionable = true;
            perfil.Editable = true;
            string[] permisos = PermisosPerfiles.Split(',');
            perfil.Permiso.Clear();
            foreach (var p in permisos)
            {
                Permiso oP = new Permiso();
                oP.PermisoId = int.Parse(p);

                perfil.Permiso.Add(oP);
            }
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                LPerfil.NuevoPerfil(perfil, usuario.UsuarioId);
                Session["Toasts"] = new List<string>() { "Nuevo perfil,Los datos del nuevo perfil se guardaron exitosamente." };
                return JavaScript("redireccionar('" + Url.Action("Index", "Perfil") + "');");
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
        public JavaScriptResult ModificarPerfil(int idPer, string nomPer, string descPer, string PermisosPerfiles, int tipo, bool habilitadoPer)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, "Perfil");
            ViewBag.PermisosH = Hijos;
            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
            {
                return JavaScript("MostrarMensaje('Usted no puede editar Perfiles. Contactese con el administrador.');");
            }
            if (string.IsNullOrEmpty(nomPer))
            {
                return JavaScript("MostrarMensaje('Ingrese un nombre de perfil válido.');");
            }
            if (string.IsNullOrEmpty(descPer))
            {
                return JavaScript("MostrarMensaje('Ingrese una descripción válida.');");
            }
            if (PermisosPerfiles.Length < 1)
            {
                return JavaScript("MostrarMensaje('Debe seleccionar al menos una función disponible.');");
            }

            Perfil perfil = new Perfil();
            perfil.PerfilId = idPer;
            perfil.Tipo = tipo;
            perfil.Nombre = nomPer;
            perfil.Decripcion = descPer;
            perfil.Estado = habilitadoPer ? (int)Estado.Habilitado : (int)Estado.Desabilitado;

            string[] permisos = PermisosPerfiles.Split(',');
            perfil.Permiso.Clear();
            foreach (var p in permisos)
            {
                Permiso oP = new Permiso();
                oP.PermisoId = int.Parse(p);
                perfil.Permiso.Add(oP);
            }

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                LPerfil.ModificarPerfil(perfil, usuario.UsuarioId);
                Session["Toasts"] = new List<string>() { "Actualización de perfil,Los datos del perfil se guardaron exitosamente." };
                return JavaScript("redireccionar('" + Url.Action("Index", "Perfil") + "');");
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
        public ActionResult DeshabilitarPerfil(string idPerfil)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, "Perfil");
            ViewBag.PermisosH = Hijos;
            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
            {
                return new HttpStatusCodeResult(400, "Usted no puede editar Perfiles. Contactese con el administrador.");
            }
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                EPerfil entidad = LPerfil.Deshabilitar(Convert.ToInt32(idPerfil), usuario.UsuarioId);
                return PartialView("_ListadoPerfiles", LPerfil.ObtenerPerfilesListar(usuario.TipoUsuario));
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


        [HttpPost]
        public ActionResult EliminarPerfil(string idPerfil)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, "Perfil");
            ViewBag.PermisosH = Hijos;
            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Eliminar))
            {
                return new HttpStatusCodeResult(400, "Usted no puede eliminar Perfiles. Contactese con el administrador.");
            }
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                EPerfil entidad = LPerfil.Eliminar(Convert.ToInt32(idPerfil), usuario.UsuarioId);
                return PartialView("_ListadoPerfiles", LPerfil.ObtenerPerfilesListar(usuario.TipoUsuario));
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

        [HttpPost]
        public ActionResult HabilitarPerfil(string idPerfil)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, "Perfil");
            ViewBag.PermisosH = Hijos;
            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
            {
                return new HttpStatusCodeResult(400, "MostrarMensaje('Usted no puede editar Perfiles. Contactese con el administrador.');");
            }
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                LPerfil.Habilitar(Convert.ToInt32(idPerfil), usuario.UsuarioId);
                return PartialView("_ListadoPerfiles", LPerfil.ObtenerPerfilesListar(usuario.TipoUsuario));
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