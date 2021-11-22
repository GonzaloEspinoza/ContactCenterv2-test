using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using PushSharp.Core;
using PushSharp.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificacionPush.Entidades
{
    public class Alerta
    {
        public string MensajeAlerta { get; set; }
        public string TituloAlerta { get; set; }
        public string Sonido { get; set; }
        public string UrlImg { get; set; }
        public Alerta()
        {
            MensajeAlerta = "";
            TituloAlerta = "";
            Sonido = "sound.caf";
            UrlImg = "";
        }
        public Alerta(string mensaje)
        {
            MensajeAlerta = mensaje;
            TituloAlerta = "";
            Sonido = "sound.caf";
            UrlImg = "";
        }
        public Alerta(string mensaje, string titulo)
        {
            MensajeAlerta = mensaje;
            Sonido = "sound.caf";
            TituloAlerta = titulo;
            UrlImg = "";
        }

        public Alerta(string mensaje, string titulo, string urlImg)
        {
            MensajeAlerta = mensaje;
            Sonido = "sound.caf";
            TituloAlerta = titulo;
            UrlImg = urlImg;
        }

        public INotification Notificacion(Token token)
        {
            if (token.TipoDispositivo == Dispositivo.Apple)
            {
                MensajeAlerta = MensajeAlerta.Replace("<strong>", "");
                MensajeAlerta = MensajeAlerta.Replace("</strong>", " ");
           //     string json = JObject.Parse("{\"aps\":{\"badge\":" + token.Badge + ", \"tipo\":\"" + token.Tipo + "\", \"idContenido\":" + token.IdContenido + ", \"alert\":{ \"body\": \"" + MensajeAlerta + "\", \"title\": \"" + TituloAlerta + "\"} , \"sound\":\"" + Sonido + "\" }}").ToString();
                return new ApnsNotification()
                {
                    DeviceToken = token.TokenID,
                    Payload = JObject.Parse("{\"aps\":{\"badge\":" + token.Badge + ", \"tipo\":\"" + token.Tipo + "\", \"idContenido\":" + token.IdContenido + ", \"alert\":{ \"body\": \"" + MensajeAlerta + "\", \"title\": \"" + TituloAlerta + "\"} , \"sound\":\"" + Sonido + "\" }}")

                    //Payload = JObject.Parse("{\"aps\":{\"badge\":" + token.Badge + ", \"tipo\":\"" + token.Tipo + "\", \"idContenido\":" + token.IdContenido + ", \"alert\": { \"body\": \"" + MensajeAlerta + "\", \"title\": \"" + TituloAlerta + "\"} , \"sound\":\"" + Sonido + "\" }}");
                };
            }
            else if (token.TipoDispositivo == Dispositivo.Android || token.TipoDispositivo == Dispositivo.Huawei)
            {
                //return new GcmNotification()
                //{
                //    RegistrationIds = new List<string> {
                //token.TokenID},
                //    Data = JObject.Parse(JsonAndroid(token))
                //};

                var notif = new GcmNotification()
                {
                    RegistrationIds = new List<string> {
                token.TokenID},
                    Data = JObject.Parse(JsonAndroid(token))
                };

                if (token.TipoAplicacion == Aplicacion.Conductor
                    || token.TipoAplicacion == Aplicacion.Encargado)
                {
                    notif.Priority = GcmNotificationPriority.High;
                }
                else
                {
                    notif.Priority = GcmNotificationPriority.Normal;
                }

                return notif;


            }
            return null;
        }

        
        public string JsonAndroid(Token tkn)
        {
            //https://stackoverflow.com/questions/1056121/how-to-create-json-string-in-c-sharp
            // Juan disuclpa que te moleste con esto, pero por si acaso tienes posibilidades de prestarme hasta este miercoles proximo 1000 bs,  es que nacio mi hija este fin de semana y fue de emergencia y hubo complicaciones asi que gaste mas de lo que pensaba. a Dios gracias hoy todo bien. el miercoles ya tendria para devolverte, claro si es que se puede xD
            string json = @"{""title"":""" + "Alguien" + @""",""text"":""" + MensajeAlerta + @""",""badge"":" + tkn.Badge + @",""sound"":""" + Sonido + @""",""tipo"":""" + tkn.Tipo + @""",""idContenido"":""" + tkn.IdContenido + @""",""idNotificacion"":""" + tkn.IdNotificacion + @"""}";
            //string json1 = @"{""ulrimg"":""" + UrlImg + @""",""title"":""" + TituloAlerta + @""",""alert"":""" + MensajeAlerta + @""",""badge"":" + tkn.Badge + @",""sound"":""" + Sonido + @""",""tipo"":""" + tkn.Tipo + @""",""idContenido"":""" + tkn.IdContenido + @""",""idNotificacion"":""" + tkn.IdNotificacion + @"""}";
            string json1 = @"{""ulrimg"":""" + UrlImg + @""",""title"":""" + TituloAlerta + @""",""alert"":""" + MensajeAlerta + @""",""badge"":" + tkn.Badge + @",""sound"":""" + Sonido + @""",""tipo"":""" + tkn.Tipo + @""",""idContenido"":""" + tkn.IdContenido + @""",""idNotificacion"":""" + tkn.IdNotificacion + @"""}";

            //var my_jsondata = new
            //{
            //    priority = 10,//high
            //    data = new {
            //        ulrimg = UrlImg,
            //        title = TituloAlerta,
            //        alert = MensajeAlerta,
            //        badge = tkn.Badge,
            //        sound = Sonido,
            //        tipo = tkn.Tipo,
            //        idContenido = tkn.IdContenido,
            //        idNotificacion = tkn.IdNotificacion
            //    }
            //};
            var my_jsondata = new
            {
                ulrimg = UrlImg,
                title = TituloAlerta,
                alert = MensajeAlerta,
                badge = tkn.Badge,
                sound = Sonido,
                tipo = tkn.Tipo,
                idContenido = tkn.IdContenido,
                idNotificacion = tkn.IdNotificacion
            };


            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(my_jsondata);

           // return json1;
            return json_data;

           // return @"{""alert"":""" + MensajeAlerta + @""",""badge"":"+tkn.Badge+@",""sound"":""" + Sonido + @""",""tipo"":""" + tkn.Tipo + @""",""idContenido"":""" + tkn.IdContenido + @""",""idNotificacion"":""" + tkn.IdNotificacion + @"""}";
        }
    }
}
