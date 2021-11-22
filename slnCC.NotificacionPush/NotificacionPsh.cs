using NotificacionPush.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificacionPush
{
    public class NotificacionPsh
    {
        public static string CertAppleURL { get { return ConfigurationManager.AppSettings.Get("CertAppleURL"); } }
        public static string CertApplePass { get { return ConfigurationManager.AppSettings.Get("CertApplePass"); } }
        public static bool ApplePushProd { get { return bool.Parse(ConfigurationManager.AppSettings.Get("ApplePushProduccion")); } }
        public static string GCMKeyCliente { get { return ConfigurationManager.AppSettings.Get("GCMKeyCliente"); } }
        public static string GCMKeyEmpresa { get { return ConfigurationManager.AppSettings.Get("GCMKeyEmpresa"); } }
        public static string GCMKeyConductor { get { return ConfigurationManager.AppSettings.Get("GCMKeyConductor"); } }

        public static string AGHKeyIdCliente { get { return ConfigurationManager.AppSettings.Get("AGHKeyIdCliente"); } }
        public static string AGHKeyIdConductor { get { return ConfigurationManager.AppSettings.Get("AGHKeyIdConductor"); } }
        public static string AGHKeyIdComercio { get { return ConfigurationManager.AppSettings.Get("AGHKeyIdComercio"); } }
        public static string AGHKeySecretCliente { get { return ConfigurationManager.AppSettings.Get("AGHKeySecretCliente"); } }
        public static string AGHKeySecretConductor { get { return ConfigurationManager.AppSettings.Get("AGHKeySecretConductor"); } }
        public static string AGHKeySecretComercio { get { return ConfigurationManager.AppSettings.Get("AGHKeySecretComercio"); } }


        public static int CantidadPorHilo { get { return int.Parse(ConfigurationManager.AppSettings.Get("CantidadPorHilo")); } }
        public static int SegundosReintento { get { return int.Parse(ConfigurationManager.AppSettings.Get("SegundosReintento")); } }
        public static int CantidadIntentos { get { return int.Parse(ConfigurationManager.AppSettings.Get("CantidadIntentos")); } }


        public static Token PrepararTocken(string idNotificion, string mensajeAlerta, string tituloAlerta, string tipo, string idContenido, string pushId, string tipoSO, int Badge, int tipoApp)
        {
            Dispositivo dispositivo = Dispositivo.Android;
            switch (tipoSO.Trim().ToUpper())
            {
                case "IOS":
                    dispositivo = Dispositivo.Apple;
                    break;
                case "ANDROID":
                    dispositivo = Dispositivo.Android;
                    break;
                case "HUAWEI":
                    dispositivo = Dispositivo.Huawei;
                    break;
                default:
                    break;
            }
            return new Token(pushId, dispositivo, mensajeAlerta, tituloAlerta , tipo, idContenido, idNotificion, Badge, (Aplicacion)tipoApp);
        }

        public static void EnviarNotificacion(string Tipo, Notificacion pushIdNotif)
        {
            EnviarNotificacion("", Tipo, pushIdNotif);
        }

        public static void EnviarNotificacion(string Mensaje, string Tipo, Notificacion pushIdNotif)
        {
            EnviarNotificacion("",Mensaje, Tipo, new List<Notificacion>() { pushIdNotif });
        }

        public static void EnviarNotificacion(string Tipo, List<Notificacion> pushIdsNotifsIds)
        {
            EnviarNotificacion("", "", Tipo, pushIdsNotifsIds);
        }

        public static void EnviarNotificacion(string Titulo, string Mensaje, string Tipo, List<Notificacion> pushIdsNotifsIds)
        {
            try
            {
                if (pushIdsNotifsIds.Count > 0)
                {
                    List<Token> tokens = new List<Token>();
                    string notificacionId = DateTime.Now.ToString("yyyyMMddHHmmssss");
                    foreach (var item in pushIdsNotifsIds)
                    {
                        tokens.Add(PrepararTocken(notificacionId, Mensaje, Titulo, Tipo, item.NotificacionId.ToString(), item.PushId, item.TipoSO, item.Badge, item.TipoAplicacion));
                    }

                    NotificacionEnviador oNoti = new NotificacionEnviador(new Alerta(Mensaje),
                           tokens, CantidadPorHilo, SegundosReintento, CantidadIntentos, CertAppleURL, CertApplePass, GCMKeyCliente, GCMKeyEmpresa, GCMKeyConductor, AGHKeyIdCliente, AGHKeySecretCliente, AGHKeyIdConductor, AGHKeySecretConductor, AGHKeyIdComercio, AGHKeySecretComercio , ApplePushProd);
                    oNoti.Enviar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void EnviarNotificacion(string Titulo, string Mensaje, string url, string Tipo, List<Notificacion> pushIdsNotifsIds)
        {
            try
            {
                if (pushIdsNotifsIds.Count > 0)
                {
                    List<Token> tokens = new List<Token>();
                    string notificacionId = DateTime.Now.ToString("yyyyMMddHHmmssss");
                    foreach (var item in pushIdsNotifsIds)
                    {
                        tokens.Add(PrepararTocken(notificacionId, Mensaje, Titulo, Tipo, item.NotificacionId.ToString(), item.PushId, item.TipoSO, item.Badge, item.TipoAplicacion));
                    }

                    NotificacionEnviador oNoti = new NotificacionEnviador(new Alerta(Mensaje, Titulo, url),
                           tokens, CantidadPorHilo, SegundosReintento, CantidadIntentos, CertAppleURL, CertApplePass, GCMKeyCliente, GCMKeyEmpresa, GCMKeyConductor, AGHKeyIdCliente, AGHKeySecretCliente, AGHKeyIdConductor, AGHKeySecretConductor, AGHKeyIdComercio, AGHKeySecretComercio, ApplePushProd);
                    oNoti.Enviar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void EnviarSimpleNotification(string titulo, string message, List<KeyValuePair<string, string>> destinos, int Badge, int TipoAplicacion)
        {
            try
            {
                List<Token> tokens = new List<Token>();
                string idandroid = DateTime.Now.ToString();

                foreach (var item in destinos)
                {
                    tokens.Add(PrepararTocken(idandroid, message, titulo, "1", "1", item.Value, item.Key, Badge, TipoAplicacion));
                }

                NotificacionEnviador oNoti = new NotificacionEnviador(new Alerta(message),
                       tokens, CantidadPorHilo, SegundosReintento, CantidadIntentos, CertAppleURL, CertApplePass, GCMKeyCliente, GCMKeyEmpresa, GCMKeyConductor, AGHKeyIdCliente, AGHKeySecretCliente, AGHKeyIdConductor, AGHKeySecretConductor, AGHKeyIdComercio, AGHKeySecretComercio, ApplePushProd);
                oNoti.Enviar();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
