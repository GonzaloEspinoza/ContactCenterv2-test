using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using System.Text;
using System.Configuration;
using System.IO;

namespace Service.App_Start
{
    public class LogMetadata
    {
        public string RequestContentType { get; set; }
        public string RequestUri { get; set; }
        public string RequestMethod { get; set; }
        public string RequestBody { get; set; }
        public DateTime? RequestTimestamp { get; set; }
        public string ResponseContentType { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public DateTime? ResponseTimestamp { get; set; }


    }

    public class CustomLogHandler : DelegatingHandler
    {
        public static bool LogServiceWrite
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("LogServiceWrite") == "1";
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var logMetadata = BuildRequestMetadata(request);
            var response = await base.SendAsync(request, cancellationToken);
            logMetadata = BuildResponseMetadata(logMetadata, response);
            await SendToLog(logMetadata);
            return response;
        }
        public static LogMetadata BuildRequestLog(HttpRequestMessage request)
        {
            var log = new CustomLogHandler().BuildRequestMetadata(request);
            return log;
        }
        private LogMetadata BuildRequestMetadata(HttpRequestMessage request)
        {
            string jsonContent = "";
            try
            {
                jsonContent = request.Content.ReadAsStringAsync().Result;
            }
            catch (Exception)
            {
            }
            LogMetadata log = new LogMetadata
            {
                RequestMethod = request.Method.Method,
                RequestTimestamp = DateTime.Now,
                RequestUri = request.RequestUri.ToString(),
                RequestBody = jsonContent
        };
            return log;
        }

        private LogMetadata BuildResponseMetadata(LogMetadata logMetadata, HttpResponseMessage response)
        {
            logMetadata.ResponseStatusCode = response.StatusCode;
            logMetadata.ResponseTimestamp = DateTime.Now;
            logMetadata.ResponseContentType = response.Content.Headers.ContentType.MediaType;
            return logMetadata;
        }

        private async Task<bool> SendToLog(LogMetadata logMetadata)
        {
            // TODO: Write code here to store the logMetadata instance to a pre-configured log store...
            if (LogServiceWrite)
            {
                try
                {
                    // Escribimos en el log de servicios web
                    StringBuilder sb = new StringBuilder();

                    var filePath = ConfigurationManager.AppSettings["PathLog"];
                    string titulo = "--------------------------------------------   " + DateTime.Now.ToString() + "   --------------------------------------------";
                    sb.AppendLine(titulo);


                    string nombreMetodo = "Método: " + logMetadata.RequestMethod;
                    sb.AppendLine(nombreMetodo);
                    string nombreUri = "Url: " + logMetadata.RequestUri;
                    sb.AppendLine(nombreUri);
                    string body = "Body: " + logMetadata.RequestBody;
                    sb.AppendLine(body);

                    //if (pParametros != null && pParametros.Length > 0)
                    //{
                    //    string parametros = "Parámetros:      " + pParametros[0];
                    //    sb.AppendLine(parametros);
                    //    for (int i = 1; i < pParametros.Length - 1; i++)
                    //    {
                    //        parametros = "                 " + pParametros[i];
                    //        sb.AppendLine(parametros);
                    //    }
                    //}
                    //else
                    //{
                    //    string parametros = "Parámetros:      Sin parámetros";
                    //    sb.AppendLine(parametros);
                    //}

                    var fecha = DateTime.Now.ToString("yyyy-MM-dd-" + "LOGCONSUMOS");

                    File.AppendAllText(filePath + fecha + ".txt", sb.ToString());

                    sb.Clear();
                }
                catch (Exception)
                {

                }
               
            }

            return true;
        }


    }



}