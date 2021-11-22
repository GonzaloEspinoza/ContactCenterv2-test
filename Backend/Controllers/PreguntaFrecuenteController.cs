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
    public class PreguntaFrecuenteController : Controller
    {
        private LPreguntaFrecuente lLogica = LPreguntaFrecuente.Instancia.LPreguntaFrecuente;
        private LCategoriaPreguntaFrecuente lCategoria = LCategoriaPreguntaFrecuente.Instancia.LCategoriaPreguntaFrecuente;
        private LTipoAtencion lTipoAtencion = LTipoAtencion.Instancia.LTipoAtencion;
        private string Controller = "PreguntaFrecuente";
        private string Titulo = "Preguntas Frecuentes";
        public ActionResult Index()
        {

            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            try
            {
                ViewBag.Titulo = "Categoria pregunta frecuente";
                ViewBag.Toasts = Session["Toasts"];
                Session["Toasts"] = null;
               
                return View(lCategoria.Obtener());
            }
            catch (Exception ex)
            {
                ViewBag.Titulo = Titulo;
                ViewBag.Mensaje = ex.Message;
                ViewBag.Controlador = Controller;
                return PartialView("NotFound404");
            }
        }
        [HttpPost]
        public ActionResult ListarCategoria(string estado)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            List<ECategoriaPreguntaFrecuente> Entidades = new List<ECategoriaPreguntaFrecuente>();
            try
            {
                Usuario usuarioL = (Usuario)Session["Usuario"];
                Entidades = lCategoria.ObtenerLista(estado);
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            return PartialView("_ListadoCategoriaPreguntaFrecuente", Entidades);
        }


        public ActionResult categoria(long id)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            Usuario usuario = (Usuario)Session["Usuario"];
            ViewBag.Empresas = LEmpresa.Instancia.LEmpresa.ObtenerLista("1");
            if (id > 0)
            {
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
                {
                    ViewBag.Titulo = Titulo;
                    ViewBag.Mensaje = "Usted no puede editar " + Titulo + ". Contacte al administrador.";
                    ViewBag.Controlador = Controller;
                    return PartialView("NotFound404");
                }
                ViewBag.Accion = "modificarcategoria";
                ViewBag.Titulo = "Editar categoria";
                ECategoriaPreguntaFrecuente entidad = null;
                try
                {
                    entidad = lCategoria.ObtenerCategoria(id);
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
                return View("FormularioCategoriaPreguntaFrecuente", entidad);
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
                ViewBag.Accion = "nuevacategoria";
                ViewBag.Titulo = "Nueva categoria";
                return View("FormularioCategoriaPreguntaFrecuente", new ECategoriaPreguntaFrecuente());
            }
        }


        [HttpPost]
        [ValidateInput(false)]
        public JavaScriptResult nuevacategoria(string nombre,long empresaId,  bool habilitado)
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
                CategoriaPreguntaFrecuente categoria = new CategoriaPreguntaFrecuente()
                {
                    sNombre = nombre,
                    nEmpresaId = empresaId,
                    nEstado = habilitado ? (int)Estado.Habilitado : (int)Estado.Desabilitado
                };

                lCategoria.Nuevo(categoria, usuario.UsuarioId);
                Session["Toasts"] = new List<string>() { "Nueva categoria, los datos de la nueva categoria se guardaron exitosamente." };
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
        public JavaScriptResult modificarcategoria(long id, string nombre, long empresaId, bool habilitado)
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
                CategoriaPreguntaFrecuente Entidad = new CategoriaPreguntaFrecuente()
                {
                    nCategoriaPreguntaFrecuenteId = id,
                    sNombre = nombre,
                    nEmpresaId = empresaId,
                    nEstado = habilitado ? (int)Estado.Habilitado : (int)Estado.Desabilitado
                };

                lCategoria.Modificar(Entidad, usuario.UsuarioId);

                Session["Toasts"] = new List<string>() { "Actualización de categoria, los datos de la categoria se guardaron exitosamente." };
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
        public ActionResult detallecategoria(long id)
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
            ViewBag.Titulo = "Detalle categoria";
            ECategoriaPreguntaFrecuente entidad = null;
            try
            {
                entidad = lCategoria.ObtenerCategoria(id);
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

            return View("DetalleCategoria", entidad);

        }


        [HttpPost]
        public ActionResult CambiarEstadoCategoria(long id, Estado estado)
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
                lCategoria.CambiarEstado(id, estado, usuario.UsuarioId);
                return PartialView("_ListadoCategoriaPreguntaFrecuente", lCategoria.Obtener());

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



        public ActionResult preguntas(long categoriaId)
        {

            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            try
            {
                ViewBag.Titulo = "Preguntas Frecuentes";
                ViewBag.Toasts = Session["Toasts"];
                Session["Toasts"] = null;
                ViewBag.categoriaId = categoriaId;
                return View(lLogica.ObtenerLista(categoriaId, "-1"));
            }
            catch (Exception ex)
            {
                ViewBag.Titulo = Titulo;
                ViewBag.Mensaje = ex.Message;
                ViewBag.Controlador = Controller;
                return PartialView("NotFound404");
            }
        }

        [HttpPost]
        public ActionResult ListarPreguntas(long categoriaId, string estado)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            List<EPreguntaFrecuente> Entidades = new List<EPreguntaFrecuente>();
            try
            {
                Usuario usuarioL = (Usuario)Session["Usuario"];
                Entidades = lLogica.ObtenerLista(categoriaId, estado);
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            return PartialView("_ListadoPreguntaFrecuente", Entidades);
        }

        public ActionResult preguntafrecuente(long id)
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
                ViewBag.Titulo = "Editar pregunta frecuente";
                EPreguntaFrecuente entidad = null;
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
                ViewBag.Categorias = lCategoria.Obtener();
                ViewBag.TipoAtencion = lTipoAtencion.Obtener();
                return View("FormularioPreguntaFrecuente", entidad);
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
                ViewBag.Titulo = "Nueva pregunta frecuente";
                ViewBag.Categorias = lCategoria.Obtener();
                ViewBag.TipoAtencion = lTipoAtencion.Obtener();
                return View("FormularioPreguntaFrecuente", new EPreguntaFrecuente());
            }
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
            ViewBag.Titulo = "Detalle pregunta frecuente";
            EPreguntaFrecuente entidad = null;
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
        public JavaScriptResult Nuevo(string pregunta, long idcategoria,long idatencion, bool habilitado)
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
                PreguntaFrecuente PreguntaFrecuente = new PreguntaFrecuente()
                {
                    sPregunta = pregunta,
                    nCategoriaPreguntaFrecuenteId = idcategoria,
                    nTipoAtencionId=idatencion,
                    nEstado = habilitado ? (int)Estado.Habilitado : (int)Estado.Desabilitado
                };
               
                lLogica.Nuevo(PreguntaFrecuente, usuario.UsuarioId);
                Session["Toasts"] = new List<string>() { "Nueva pregunta frecuente, los datos de la nueva pregunta frecuente se guardaron exitosamente." };
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
        public JavaScriptResult Modificar(long id, string pregunta,  long idcategoria, long idatencion, bool habilitado)
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
                PreguntaFrecuente Entidad = new PreguntaFrecuente()
                {
                    nPreguntaFrecuenteId = id,
                    sPregunta = pregunta,
                    nCategoriaPreguntaFrecuenteId = idcategoria,
                    nTipoAtencionId=idatencion,
                    nEstado = habilitado ? (int)Estado.Habilitado : (int)Estado.Desabilitado
                };
              
                lLogica.Modificar(Entidad, usuario.UsuarioId);

                Session["Toasts"] = new List<string>() { "Actualización de la pregunta frecuente, los datos de la pregunta frecuente se guardaron exitosamente." };
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
                return PartialView("_ListadoPreguntaFrecuente", lLogica.Obtener());

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