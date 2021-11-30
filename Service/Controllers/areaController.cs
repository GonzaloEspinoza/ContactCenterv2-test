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

    //[AllowAnonymous]
    public class areaController : ApiController
    {
        [Route("api/area/listar")]
        //[HttpGet]
        //public IHttpActionResult Get()
        //{
        //    string[] data = new string[] { "carlos", "marco", "freddy" };
        //    return Ok(data);
        //}

        [HttpPost]
        public IHttpActionResult listar(ReqListarArea form)
        {
            try
            {
                List<ResArea> resultado = LArea.Instancia.LArea.ObtenerListaPorIdEmpresa(Estado.Habilitado, form.EmpresaId);
                return Ok(RespuestaApi<List<ResArea>>.createRespuestaSuccess(resultado));
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