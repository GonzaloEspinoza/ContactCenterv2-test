using Entidad;
using Logica;
using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Entidad.Enums;

namespace Service.Controllers
{

    public class ciudadController : ApiController
    {
        [Route("api/ciudad/listar")]

        //[HttpGet]
        //public IHttpActionResult Get()
        //{
        //    string[] data = new string[] { "carlos", "marco", "freddy" };
        //    return Ok(data);
        //}

        [HttpPost]
        public IHttpActionResult listar(ReqListarCiudad form)
        {
            try
            {
                List<ResCiudad> resultado = LCiudad.Instancia.LCiudad.ObtenerCiudadesDisponiblesPorEmpresa(form.EmpresaId);
                return Ok(RespuestaApi<List<ResCiudad>>.createRespuestaSuccess(resultado));
            }
            catch (BussinessException ex)
            {
                return Ok(RespuestaApi<string>.createRespuestaError(ex.Message));
            }
            catch (Exception ex)
            {
                return Ok(RespuestaApi<string>.createRespuestaError(ex.Message));
            }
        }


    }
}