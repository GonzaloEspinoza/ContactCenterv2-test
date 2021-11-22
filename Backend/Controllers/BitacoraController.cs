using DataP;
using Entidad.Enums;
using Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Backend.Controllers
{
    public class BitacoraController : Controller
    {
        // GET: Bitacora
        private LUsuarioBackEnd LNUsuarioBackEnd = LUsuarioBackEnd.Instancia.LUsuarioBackEnd;
        private LBitacora LBitacora = LBitacora.Instancia.LBitacora;

        public ActionResult Index()
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            ViewBag.Usuarios = LNUsuarioBackEnd.ObtenerUsuarios();
            return View(new List<Bitacora>());
        }

        [HttpPost]
        public ActionResult RecargarListar(string Inicio, string Fin, int Accion, int Tipo, long? UsuarioId)
        {
            List<Bitacora> Bitacoras = new List<Bitacora>();
            try
            {
                DateTime Hoy = DateTime.ParseExact(Inicio, "yyyy-MM-dd'T'HH:mm:ss", null);
                DateTime FinHoy = DateTime.ParseExact(Fin, "yyyy-MM-dd'T'HH:mm:ss", null);
                if (Hoy == FinHoy)
                {
                    Hoy = new DateTime(Hoy.Year, Hoy.Month, Hoy.Day, 0, 0, 0);
                    FinHoy = Hoy.AddDays(1);
                }
                TipoBitacora TipoB = TipoBitacora.Ninguno;
                switch (Tipo)
                {
                    case (int)TipoBitacora.Exito:
                        TipoB = TipoBitacora.Exito;
                        break;
                    case (int)TipoBitacora.Error:
                        TipoB = TipoBitacora.Error;
                        break;
                }

                AccionBitacora AccionB = AccionBitacora.Ninguna;
                switch (Accion)
                {
                    case (int)AccionBitacora.Modificacion:
                        AccionB = AccionBitacora.Modificacion;
                        break;
                    case (int)AccionBitacora.CambioPassword:
                        AccionB = AccionBitacora.CambioPassword;
                        break;
                    case (int)AccionBitacora.Anulacion:
                        AccionB = AccionBitacora.Anulacion;
                        break;
                    case (int)AccionBitacora.Creacion:
                        AccionB = AccionBitacora.Creacion;
                        break;
                    case (int)AccionBitacora.InicioSesion:
                        AccionB = AccionBitacora.InicioSesion;
                        break;
                    case (int)AccionBitacora.Eliminacion:
                        AccionB = AccionBitacora.Eliminacion;
                        break;
                }

                Bitacoras = LBitacora.Consultar(TipoB, AccionB, Hoy, FinHoy, UsuarioId);
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            ViewBag.Usuarios = LNUsuarioBackEnd.ObtenerUsuarios();
            return PartialView("_ListadoLog", Bitacoras);
        }
    }
}