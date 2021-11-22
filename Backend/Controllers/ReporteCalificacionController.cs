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
    public class ReporteCalificacionController : Controller
    {

        LCalificacionColaborador lCalificacion = LCalificacionColaborador.Instancia.LCalificacionColaborador;
        LEvento lEvento = LEvento.Instancia.LEvento;
        LFuncionario lFuncionario = LFuncionario.Instancia.LFuncionario;

        public ActionResult Index()
        {
            ViewBag.Titulo = "Inicio";
            Usuario usuario = (Usuario)Session["Usuario"];
            if (usuario.Funcionario.nTipo == (int)TipoFuncionario.Colaborador)
            {
                EFuncionario colaborador = LFuncionario.Instancia.LFuncionario.
                    ObtenerColaboradorPorId(usuario.UsuarioId);
                List<EFuncionario> Colaborares = new List<EFuncionario>();
                Colaborares.Add(colaborador);
                ViewBag.Colaborares = Colaborares;
            }
            else
            {
                ViewBag.Colaborares = LFuncionario.Instancia.LFuncionario.ObtenerListaColaboradores("0");
            }
            return View();
        }



        [HttpPost]
        public ActionResult Estadistica(string fechaInicio, string fechaFin,long funcionarioId)
        {
            DateTime fechaI = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", null);
            DateTime fechaF = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", null);
            int numeroClientesAtendidos = 0;
            int numeroCalificacionesEnviadas = 0;
            decimal? promedioCalificaciones = 0;

            if (funcionarioId==-1)
            {
                numeroClientesAtendidos = lEvento.ObtenerPorRangoFecha(fechaI, fechaF).Count;
                numeroCalificacionesEnviadas = lCalificacion.ObtenerPorRangoFecha(fechaI, fechaF).Count;
                promedioCalificaciones = lFuncionario.ObtenerPromediosTodasCalificaciones();
            }
            else
            {
                numeroClientesAtendidos = lEvento.ObtenerPorColaboradorPorRangoFecha(funcionarioId, fechaI, fechaF).Count;
                numeroCalificacionesEnviadas = lCalificacion.ObtenerPorColaboradorPorRangoFecha(funcionarioId, fechaI, fechaF).Count;
                promedioCalificaciones = numeroCalificacionesEnviadas == 0 ?
                    0 : lFuncionario.ObtenerCalificacionColaborador(funcionarioId);
            }
            
            var data = new {
                numeroClientesAtendidos = numeroClientesAtendidos,
                numeroCalificacionesEnviadas = numeroCalificacionesEnviadas,
                promedioCalificaciones = promedioCalificaciones
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult ListarCalifiacionesDatatable(string fechaInicio, string fechaFin, long funcionarioId)
        {
            DateTime fechaI = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", null);
            DateTime fechaF = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", null);
            List<ECalifiacionColaboradorDataTable> listado = new List<ECalifiacionColaboradorDataTable>();
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
            listado = lCalificacion.ObtenerListadoPaginado(funcionarioId, fechaI, fechaF,
                draw, start, length, sortColumn, sortColumnDir, searchValue, pageSize, skip, out recordsTotal);

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listado });
        }

        

    }
}