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

    public class ClienteController : Controller
    {
        private LCliente lLogica = LCliente.Instancia.LCliente;
        private string Controller = "Cliente";
        private string Titulo = "Cliente";
        public ActionResult Index()
        {

            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            try
            {
                ViewBag.Titulo = "Clientes";
                ViewBag.Toasts = Session["Toasts"];
                Session["Toasts"] = null;
                return View();
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
        public ActionResult ListarDatatable(int estado = -1)
        {
            List<EClienteDataTable> listado = new List<EClienteDataTable>();
            string draw = "";
            string start = "";
            string length = "";
            string sortColumn = "";
            string sortColumnDir = "";
            string searchValue = "";
            int pageSize, skip, recordsTotal;

            draw = Request.Form.GetValues("draw").FirstOrDefault();
            start = Request.Form.GetValues("start").FirstOrDefault();
            length = Request.Form.GetValues("length").FirstOrDefault();
            sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            pageSize = length != null ? Convert.ToInt32(length) : 0;
            skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal = 0;
            listado = lLogica.ObtenerListadoPaginado(estado, draw, start, length, sortColumn, sortColumnDir, searchValue, pageSize, skip, out recordsTotal);

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listado });
        }

        public ActionResult cliente(long id)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            Usuario usuario = (Usuario)Session["Usuario"];
            ViewBag.empresas = LEmpresa.Instancia.LEmpresa.ObtenerLista("1");
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
                ECliente entidad = null;
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
                ViewBag.Paises = LPais.Instancia.LPais.ObtenerHabilitados();
                return View("FormularioCliente", entidad);
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
                ViewBag.Paises = LPais.Instancia.LPais.ObtenerHabilitados();
                return View("FormularioCliente", new ECliente());
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
            ViewBag.Titulo = "Detalle " + Titulo;
            ECliente entidad = null;
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
        public JavaScriptResult Nuevo(string codigoCliente, string nombres, string apellidos, string urlFoto,
            string codigoPais, string numeroCelular, string correo, string tokenNotificacionFirebase, string tipoSistemaOperativo,
             long empresaId,bool habilitado)
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
                Cliente Cliente = new Cliente()
                {
                    sCodigoCliente = codigoCliente,
                    sNombres = nombres,
                    sApellidos = apellidos,
                    sUrlFoto = urlFoto,
                    sCodigoPais = codigoPais,
                    sNumeroCelular = numeroCelular,
                    sCorreo = correo,
                    sTokenNotificacionFirebase = tokenNotificacionFirebase,
                    sTipoSistemaOperativo = tipoSistemaOperativo,
                    nEmpresaId = empresaId,
                    nEstado = habilitado ? (int)Estado.Habilitado : (int)Estado.Desabilitado
                };
       
                lLogica.Nuevo(Cliente, usuario.UsuarioId);
                Session["Toasts"] = new List<string>() { "Nueva " + Titulo + ", los datos de la nueva " + Titulo + " se guardaron exitosamente." };
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
        public JavaScriptResult Modificar(long id, string codigoCliente, string nombres, string apellidos, string urlFoto,
            string codigoPais, string numeroCelular, string correo, string tokenNotificacionFirebase, string tipoSistemaOperativo,
             long empresaId, bool habilitado)
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
                Cliente Entidad = new Cliente()
                {
                    nClienteId = id,
                    sCodigoCliente = codigoCliente,
                    sNombres = nombres,
                    sApellidos = apellidos,
                    sUrlFoto = urlFoto,
                    sCodigoPais = codigoPais,
                    sNumeroCelular = numeroCelular,
                    sCorreo = correo,
                    sTokenNotificacionFirebase = tokenNotificacionFirebase,
                    sTipoSistemaOperativo = tipoSistemaOperativo,
                    nEmpresaId = empresaId,
                    nEstado = habilitado ? (int)Estado.Habilitado : (int)Estado.Desabilitado
                };
                
                lLogica.Modificar(Entidad, usuario.UsuarioId);

                Session["Toasts"] = new List<string>() { "Actualización de " + Titulo + ", los datos de la " + Titulo + " se guardaron exitosamente." };
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
                return Json(new { success = true, message = "Estado cambiado correctamente" });

            }
            catch (BussinessException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });

            }
        }



    }
}
