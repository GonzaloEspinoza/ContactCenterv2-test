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
    public class calificacionColaboradorController : ApiController
    {

        [Route("api/calificacionColaborador/listar")]
        [HttpPost]
        public IHttpActionResult listar(ReqListarCalificaciones form)
        {
            try
            {
                List<ResCalificacionColaborador>  resultado = 
                    LCalificacionColaborador.Instancia.LCalificacionColaborador.listarCalificacionColaborador(form);
                return Ok(RespuestaApi<List<ResCalificacionColaborador>>.
                    createRespuestaSuccess(resultado));
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
        [Route("api/calificacionColaborador/registrar")]
        [HttpPost]
        public IHttpActionResult registrar(ReqCalificacionColaborador form)
        {
            try
            {
                LCalificacionColaborador.Instancia.LCalificacionColaborador.registrarCalificacionColaborador(form);
                return Ok(RespuestaApi<string>.createRespuestaSuccess("Calificacion registrada correctamente"));
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