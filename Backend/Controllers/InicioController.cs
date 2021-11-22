using Backend.Models;
using DataP;
using Entidad;
using Entidad.Enums;
using Logica;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Backend.Controllers
{
    public class InicioController : Controller
    {
        /*private LSolicitud lSolicitud = LSolicitud.Instancia.LSolicitud;
        private LUsuarioCliente lUsuarioCliente = LUsuarioCliente.Instancia.LUsuarioCliente;
        */
        private LNotificacionFuncionario lNotificacionFuncionario = LNotificacionFuncionario.Instancia.LNotificacionFuncionario;
        private LUsuarioBackEnd lUsuarioBackEnd = LUsuarioBackEnd.Instancia.LUsuarioBackEnd;

        // GET: Inicio
        public ActionResult Index()
        {
            ViewBag.Titulo = "Inicio";
            return View();
        }
        [HttpPost]
        public ActionResult ActualizarEstadoMenu(string estado)
        {
            Session["menuAbierto"] = estado == "True" ? false : true;
            var data = new
            {
                codigo = 0
            };
            return Json(data);
        }
        

        [HttpPost]
        public ActionResult Estadistica(string fechaInicio, string fechaFin)
        {
            DateTime fechaI = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", null);
            DateTime fechaF = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", null);
            /*double TotalVentas = lSolicitud.getTotalVentasEntreFechas(fechaI, fechaF);
            double TotalComisionVentas = lSolicitud.getTotalComisionVentasEntreFechas(fechaI, fechaF);
            int NumeroVentas = lSolicitud.getNumeroVentasEntreFechas(fechaI, fechaF);
            int ClientesNuevos = lUsuarioCliente.getNumeroClientesNuevosEntreFechas(fechaI, fechaF);
            List<EREstadistica> servicioVendidos = lSolicitud.serviciosVendidosEntreFechas(fechaI, fechaF);
            List<EREstadistica> ventas = lSolicitud.numeroVentasEntreFechas(fechaI, fechaF);
            List<EREstadistica> totalNeroVentas = lSolicitud.montoTotalVentasEntreFechas(fechaI, fechaF);
            List<EREstadistica> productoVendidos = lSolicitud.productoVendidosEntreFechas(fechaI, fechaF);*/
            var data = new {
                totalVentas = 0,
                totalComision = 0,
                numeroVentas = 0,
                totalClientesNuevos = 0/*,
                data1= new List<EREstadistica>(),
                data2 = new List<EREstadistica>(),
                data3 = new List<EREstadistica>(),
                data4 = new List<EREstadistica>()*/
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult RegistroFirebaseWeb(string token)
        {
            try
            {
                Usuario oUsuario = Util.SessionToUsuario<Usuario>(Session["Usuario"]);
                /*bool permiso = lUsuarioBackEnd.verificarPermisoSolicitud(oUsuario.UsuarioId);
                if (permiso)
                {*/
                    LUsuarioBackEnd lUsuarioBackEnd = LUsuarioBackEnd.Instancia.LUsuarioBackEnd;
                    lUsuarioBackEnd.RegistrarTokenWeb(oUsuario.UsuarioId, token);
                //}
                var data = new
                {
                    success = true,
                    message = "Correcto",
                    permiso = true
                };
                return Json(data);
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
        public ActionResult ObtenerNotificaciones()
        {
            try
            {
                Usuario oUsuario = Util.SessionToUsuario<Usuario>(Session["Usuario"]);
                List<ENotificacionFuncionario> notificaciones = lNotificacionFuncionario.ObtenerLista(oUsuario.UsuarioId);
                return PartialView("_ListadoNotificaciones", notificaciones);
            }
            catch (BussinessException ex)
            {
                return new HttpStatusCodeResult(400, "Hubo un problema, contacte al administrador.");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, "Hubo un problema, contacte al administrador.");
            }

        }



        [HttpPost]
        public ActionResult MarcarNotificacionLeida(long idNotificacion)
        {
            try
            {
               lNotificacionFuncionario.MarcarNotificacionLeida(idNotificacion);
                var data = new
                {
                    sucess = true,
                    message = "Correcto"
                };
                return Json(data);
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
        public ActionResult EliminarNotificaciones()
        {
            try
            {
                Usuario oUsuario = Util.SessionToUsuario<Usuario>(Session["Usuario"]);
                lNotificacionFuncionario.EliminarNotificaciones(oUsuario.UsuarioId);
                var data = new
                {
                    sucess = true,
                    message = "Correcto"
                };
                return Json(data);
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