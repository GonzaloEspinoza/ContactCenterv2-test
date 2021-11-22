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
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Configuration;

namespace Service.Controllers
{
    public class eventoController : ApiController
    {

        [Route("api/evento/registrar")]
        [HttpPost]
        public IHttpActionResult registrar(ReqRegistrarEvento form)
        {
            try
            {
                ResHilo resultado = LEvento.Instancia.LEvento.Registrar(form);
                return Ok(RespuestaApi<ResHilo>.createRespuestaSuccess(resultado));
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

        [Route("api/evento/listarPorCliente")]
        [HttpPost]
        public IHttpActionResult listarPorCliente(ReqListarEventoPorCliente form)
        {
            try
            {
                List<ResHilo> resultado = LHilo.Instancia.LHilo.ListarPorCliente(form.CodigoCliente);
                return Ok(RespuestaApi<List<ResHilo>>.createRespuestaSuccess(resultado));
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

        [Route("api/evento/listarMensajes")]
        [HttpPost]
        public IHttpActionResult listarMensajes(ReqListarMensajesPorEventoHilo form)
        {
            List<ResMensaje> mensajes = new List<ResMensaje>();
            try
            {
                mensajes = LMensaje.Instancia.LMensaje.ObtenerMensajesPorEventoHiloCliente(form.EventoHiloId);
                return Ok(RespuestaApi<List<ResMensaje>>.createRespuestaSuccess(mensajes));
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

        [Route("api/evento/listarMensajesV2")]
        [HttpPost]
        public IHttpActionResult listarMensajesV2(ReqListarMensajesPorEventoHilo form)
        {
            List<ResMensajeV2> mensajes = new List<ResMensajeV2>();
            try
            {
                mensajes = LMensaje.Instancia.LMensaje.ObtenerMensajesPorEventoHiloClienteV2(form.EventoHiloId);
                return Ok(RespuestaApi<List<ResMensajeV2>>.createRespuestaSuccess(mensajes));
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

        [Route("api/evento/enviarMensaje")]
        [HttpPost]
        public async Task<IHttpActionResult> enviarMensaje()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                string root = System.Web.HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);

                await Request.Content.ReadAsMultipartAsync(provider);
                string texto = "";
                List<string> archivos = new List<string>();
                long eventoHiloId = 0;

                foreach (var key in provider.FormData.AllKeys)
                {
                    texto = provider.FormData.GetValues("Texto").SingleOrDefault();
                    eventoHiloId = long.Parse(provider.FormData.GetValues("EventoHiloId").SingleOrDefault());
                }
                int n = 0 ;
                foreach (MultipartFileData fileData in provider.FileData)
                {
                    string fileName = fileData.Headers.ContentDisposition.FileName;
                    string extension = Path.GetExtension(fileData.Headers.ContentDisposition.FileName.TrimStart('\"').TrimEnd('\"'));
                    if (extension!="")
                    {
                        archivos.Add(EUtils.InsertarImagenMultipart("Mensajes", fileData.LocalFileName, fileName, extension,n));
                    }
                    n++;
                }
                LMensaje.Instancia.LMensaje.RegistrarMensaje(eventoHiloId, texto, archivos);
                return Ok(RespuestaApi<string>.createRespuestaSuccess("Mensaje enviado correctamente"));
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


        [Route("api/evento/numeroEventoDisponibles")]
        [HttpPost]
        public IHttpActionResult numeroEventoDisponibles(ReqNumeroEventoDisponibles form)
        {
            try
            {
                List<ResHilo> resultado = LHilo.Instancia.LHilo.ListarPorClienteNoAtendido(form.CodigoCliente);
                return Ok(RespuestaApi<List<ResHilo>>.createRespuestaSuccess(resultado));
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