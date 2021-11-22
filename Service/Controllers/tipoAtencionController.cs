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
    public class tipoAtencionController : ApiController
    {
        [Route("api/tipoAtencion/listar")]
        [HttpPost]
        public IHttpActionResult listar(ReqListarTipoAtencion form)
        {
            try
            {
                LTipoAtencion lTipoAtencion = LTipoAtencion.Instancia.LTipoAtencion;
                List<ResTipoAtencion> resultado = lTipoAtencion.ObtenerListaPorAreaId(Estado.Habilitado, form.AreaId);
                return Ok(RespuestaApi<List<ResTipoAtencion>>.createRespuestaSuccess(resultado));
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