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
    public class ReporteEventoController : Controller
    {
        private LCiudad lCiudad = LCiudad.Instancia.LCiudad;
        private LEstadoEstadoEvento lEstadoEvento = LEstadoEstadoEvento.Instancia.LEstadoEstadoEvento;
        private LHilo lHilo = LHilo.Instancia.LHilo;
        private LFuncionario lFuncionario = LFuncionario.Instancia.LFuncionario;

        // GET: Indicador
        public ActionResult Index()
        {
            ViewBag.Ciudad = lCiudad.Obtener();
            ViewBag.Estado = lEstadoEvento.Obtener();
            ViewBag.Funcionario = lFuncionario.ObtenerListaTodosFuncionarios();
            return View();
        }

        public int getxCiudad(string id_ciudad) {
            int id = Int16.Parse(id_ciudad);
            int cantidad = lHilo.porCiudad(id);
            return cantidad;
        }

        public int getxEstado(string id_estado)
        {
            int id = Int16.Parse(id_estado);
            int cantidad = lEstadoEvento.porEstado(id);
            return cantidad;
        }

        public int getxFuncionario(string id_funcionario)
        {
            int id = Int16.Parse(id_funcionario);
            int cantidad = lHilo.porFuncionario(id);
            return cantidad;
        }
        public int getxTiempo(string id_tiempo)
        {
            int id = Int16.Parse(id_tiempo);
            int cantidad = lHilo.porTiempo(id);
            return cantidad;
        }


        public float getPromedio()
        {
            float cantidad = lHilo.Promedio();
            return cantidad;
        }

        public int getTotal()
        {
            int cantidad = lHilo.Total();
            return cantidad;
        }
        [HttpPost]
        public string ListaCiudad()
        {
           
            return lHilo.ListaCiudad();
        }


        [HttpPost]
        public string ListaEstados()
        {

            return lEstadoEvento.ListaEstados();
        }


        [HttpPost]
        public string ListaTiempos()
        {

            return lHilo.ListaTiempos();
        }

    }
   
}