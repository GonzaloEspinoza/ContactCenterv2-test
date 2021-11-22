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
    public class preguntaFrecuenteController : ApiController
    {
        [Route("api/preguntaFrecuente/listar")]
        [HttpPost]
        public IHttpActionResult listar(ReqListarPreguntaFrecuente form)
        {
            try
            {
                List<ResPreguntaFrecuente> resultado = LPreguntaFrecuente.Instancia.LPreguntaFrecuente.ListarPorEmpresa(form);
                return Ok(RespuestaApi<List<ResPreguntaFrecuente>>.createRespuestaSuccess(resultado));
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