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
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BackEnd.Controllers
{
    public class EventoController : Controller
    {
        private LEvento lLogica = LEvento.Instancia.LEvento;
        private LHilo lHilo = LHilo.Instancia.LHilo;
        private LMensaje lMensaje = LMensaje.Instancia.LMensaje;
        private LFuncionario lFuncionario = LFuncionario.Instancia.LFuncionario;
        private LCliente lCliente = LCliente.Instancia.LCliente;
        private LEstadoEstadoEvento lEstadoEvento = LEstadoEstadoEvento.Instancia.LEstadoEstadoEvento;
        private string Controller = "Evento";

        // GET: Evento
        public ActionResult Index()
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            try
            {
                ViewBag.Toasts = Session["Toasts"];
                Session["Toasts"] = null;
                Usuario usuario = (Usuario)Session["Usuario"];
                if (usuario.Funcionario.nTipo==(int)TipoFuncionario.Colaborador)
                {
                    ViewBag.clientes = lCliente.ObtenerClientesEmpresa(usuario.UsuarioId);
                    ViewBag.colaboradores = lFuncionario.ObtenerListaColaboradoresDeLaMismaEmpresa(usuario.UsuarioId);
                    return View(lHilo.ObtenerListaPorColaborador(usuario.UsuarioId));
                }else
                {
                    ViewBag.clientes = lCliente.ObtenerTodosClientes();
                    ViewBag.colaboradores = lFuncionario.ObtenerListaTodosFuncionarios();
                    return View(lHilo.ObtenerLista());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Titulo = "Inicio";
                ViewBag.Mensaje = ex.Message;
                ViewBag.Controlador = "Inicio";
                return PartialView("NotFound404");
            }
        }


        [HttpPost]
        public ActionResult ListarMensajesPorBusqueda(string busqueda)
        {
            List<EHilo> mensajes = new List<EHilo>();
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];

                if (usuario.Funcionario.nTipo == (int)TipoFuncionario.Colaborador)
                {
                    mensajes = lHilo.ObtenerListaPorColaboradorBusqueda(usuario.UsuarioId, busqueda);
                }
                else
                {
                    mensajes = lHilo.ObtenerListaBusqueda(busqueda);
                }

                
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            return PartialView("_ListadoEventos", mensajes);
        }


        [HttpPost]
        public ActionResult ListarMensajes(long idEvento)
        {
            List<EHilo> mensajes = new List<EHilo>();
            try
            {
                Usuario usuarioL = (Usuario)Session["Usuario"];
                mensajes = lMensaje.ObtenerMensajesPorEventoHilo(idEvento);
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            return PartialView("_ListadoMensajes", mensajes);
        }

        [HttpPost]
        public ActionResult enviarMensajeTexto(string texto, long idEventoHilo)
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Mensaje mensaje = new Mensaje()
                {
                    sTexto = texto,
                    sArchivo = "",
                    nEstado = 1,
                    nFuncionarioId = usuario.UsuarioId,
                    nEventoHiloId = idEventoHilo
                };

                Mensaje mensajeNuevo = lMensaje.Nuevo(mensaje, usuario.UsuarioId);

                return Json(new
                {
                    success = true,
                    message = "Mensaje enviado correctamente",
                    data = mensajeNuevo,
                    fechaCreacion = mensajeNuevo.dFechaCreacion.ToString("dd/mm/yyyy hh:mm")
                });
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

        [HttpPost]
        public ActionResult reasignarEvento(long funcionarioId, long idEventoHilo)
        {
           
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                lLogica.reasignarEvento(usuario.UsuarioId, funcionarioId, idEventoHilo);
                return Json(new { success = true, message = "Evento reasignado correctamente" });
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

        [HttpPost]
        public ActionResult marcarEventoAtendido(long idEventoHilo)
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                lLogica.marcarEventoAtendido(idEventoHilo);
                return Json(new { success = true, message = "Evento reasignado correctamente" });
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

        [HttpPost]
        public ActionResult marcarEventoNoResuelto(long idEventoHilo)
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                lLogica.marcarEventoNoResuelto(idEventoHilo);
                return Json(new { success = true, message = "Evento reasignado correctamente" });
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


        [HttpPost]
        public ActionResult tomarEvento(long idEventoHilo)
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                lLogica.tomarEvento(idEventoHilo, usuario.UsuarioId);
                return Json(new { success = true, message = "Evento reasignado correctamente" });
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
        


        [HttpPost]
        public ActionResult nuevoEvento(long clienteNuevoEventoId, long funcionarioNuevoEventoId)
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                lLogica.Nuevo(clienteNuevoEventoId, funcionarioNuevoEventoId);
                return Json(new { success = true, message = "Evento reasignado correctamente" });
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

        

        [HttpPost]
        public ActionResult enviarMensajeImagen(string archivo, long idEventoHilo)
        {

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Mensaje mensaje = new Mensaje()
                {
                    sTexto = "",
                    sArchivo = archivo.Substring(archivo.IndexOf(",") + 1),
                    nEstado = 1,
                    nFuncionarioId = usuario.UsuarioId,
                    nEventoHiloId = idEventoHilo
                };

                Mensaje mensajeNuevo = lMensaje.NuevoMensajeImagen(mensaje, usuario.UsuarioId);
                mensajeNuevo.sArchivo = EUtils.URLImagen(mensajeNuevo.sArchivo);
                return Json(new
                {
                    success = true,
                    message = "Mensaje enviado correctamente",
                    data = mensajeNuevo,
                    fechaCreacion = mensajeNuevo.dFechaCreacion.ToString("dd/mm/yyyy hh:mm")
                });
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