using PushSharp.Apple;
using PushSharp.Google;
using NotificacionPush.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AGConnectAdmin;
using AGConnectAdmin.Messaging;
using NUnit.Framework;

namespace NotificacionPush
{
    public class Enviador
    {
        public List<Token> Tokens { get; set; }
        public List<string> TokensSended { get; set; }
        public Alerta alerta { get; set; }
        public string CertAppleURL { get; set; }
        public string CertApplePass { get; set; }
        public string GCMKeyCliente { get; set; }
        public string GCMKeyEmpresa { get; set; }
        public string GCMKeyConductor { get; set; }
        public int SegundosReintento { get; set; }
        public int CantidadIntentos { get; set; }
        public bool ApplePushProduccion { get; set; }

        public string AGHClientIdCliente { get; set; }
        public string AGHClientSecretCliente { get; set; }
        public string AGHClientIdConductor { get; set; }
        public string AGHClientSecretConductor { get; set; }
        public string AGHClientIdComercio { get; set; }
        public string AGHClientSecretComercio { get; set; }

        private GcmServiceBroker gcmPush = null;
        private ApnsServiceBroker apnsPush = null;
        private AGConnectApp appOptionHuaweiConductor = null;
        private AGConnectApp appOptionHuaweiCliente = null;
        private AGConnectApp appOptionHuaweiComercio = null;

        private int ContadorEnviados = 0;
        private int ContadorNoEnviados = 0;
        private int ContadorIntentos = 0;

        public Enviador(
           List<Token> _Tokens,
       Alerta _alerta,
       string _CertAppleURL,
       string _CertApplePass,
       string _GCMKeyCliente,
       string _GCMKeyEmpresa,
       string _GCMKeyConductor,
       int _SegundosReintento,

       int _CantidadIntentos,
      
       string _AGHClientIdCliente,
       string _AGHClientSecretCliente,
       string _AGHClientIdConductor,
       string _AGHClientSecretConductor,
       string _AGHClientIdComercio,
       string _AGHClientSecretComercio,

           bool _ApplePushProduccion = false
           )
        {
            this.Tokens = _Tokens;
            this.alerta = _alerta;
            this.CertAppleURL = _CertAppleURL;
            this.CertApplePass = _CertApplePass;
            this.GCMKeyCliente = _GCMKeyCliente;
            this.GCMKeyEmpresa = _GCMKeyEmpresa;
            this.GCMKeyConductor = _GCMKeyConductor;

            this.SegundosReintento = _SegundosReintento;
            this.CantidadIntentos = _CantidadIntentos;
            this.ApplePushProduccion = _ApplePushProduccion;
            this.TokensSended = new List<string>();

            this.AGHClientIdCliente = _AGHClientIdCliente;
            this.AGHClientIdConductor = _AGHClientIdConductor;
            this.AGHClientIdComercio = _AGHClientIdComercio;
            this.AGHClientSecretCliente = _AGHClientSecretCliente;
            this.AGHClientSecretComercio = _AGHClientSecretComercio;
            this.AGHClientSecretConductor = _AGHClientSecretConductor;

            this.appOptionHuaweiCliente = AGConnectApp.InstanceWithName("appOptionHuaweiCliente");
            if (this.appOptionHuaweiCliente == null)
            {
                this.appOptionHuaweiCliente = AGConnectApp.CreateWhitName(new AppOptions()
                {
                    ClientId = _AGHClientIdCliente,
                    ClientSecret = _AGHClientSecretCliente
                },
           "appOptionHuaweiCliente");
            }

            this.appOptionHuaweiConductor = AGConnectApp.InstanceWithName("appOptionHuaweiConductor");
            if (this.appOptionHuaweiConductor == null)
            {
                this.appOptionHuaweiConductor = AGConnectApp.CreateWhitName(new AppOptions()
                {
                    ClientId = _AGHClientIdConductor,
                    ClientSecret = _AGHClientSecretConductor
                },
                            "appOptionHuaweiConductor");
            }

            this.appOptionHuaweiComercio = AGConnectApp.InstanceWithName("appOptionHuaweiComercio");
            if (this.appOptionHuaweiComercio == null)
            {
                this.appOptionHuaweiComercio = AGConnectApp.CreateWhitName(new AppOptions()
                {
                    ClientId = _AGHClientIdComercio,
                    ClientSecret = _AGHClientSecretComercio
                },
            "appOptionHuaweiComercio");
            }
            


        }
        public void DoWork()
        {

            while (!_shouldStop)
            {
                ContadorIntentos++;
                try
                {

                    byte[] appleCert = null;
                    if (!File.Exists(CertAppleURL))
                    {
                        EscribirEnLog("hubo Error : No existe el Certificado iOS");
                    }
                    else
                    {
                        appleCert = File.ReadAllBytes(CertAppleURL);
                        EscribirEnLog("certificado encontrado:" + "iOS:" + CertAppleURL + ",  Pass:" + CertApplePass + ", GCMKeyCliente: " + GCMKeyCliente + ", GCMKeyEmpresa: " + GCMKeyEmpresa + ", GCMKeyConductor: " + GCMKeyConductor + ", Alerta " + alerta);
                    
                    }
                    
                    foreach (var token in Tokens)
                    {
                        if (!TokensSended.Contains(token.TokenID))
                        {
                            try
                            {
                                string FBSenderAuthToken = "";
                                AGConnectApp agConnectApp = null;

                                switch (token.TipoAplicacion)
                                {
                                    case Aplicacion.Usuario: 
                                        FBSenderAuthToken = GCMKeyCliente;
                                        agConnectApp = appOptionHuaweiCliente;

                                        break;
                                    case Aplicacion.Conductor:
                                        FBSenderAuthToken = GCMKeyConductor;
                                        agConnectApp = appOptionHuaweiConductor;

                                        break;
                                    case Aplicacion.Administrador:
                                        FBSenderAuthToken = GCMKeyCliente;
                                        break;
                                    case Aplicacion.Encargado:
                                        FBSenderAuthToken = GCMKeyEmpresa;
                                        agConnectApp = appOptionHuaweiComercio;
                                        break;
                                    default:
                                        FBSenderAuthToken = GCMKeyCliente;
                                        agConnectApp = appOptionHuaweiCliente;
                                        break;
                                }
                                switch (token.TipoDispositivo)
                                {
                                    case Dispositivo.Apple:
                                        if(appleCert != null)
                                        {
                                            ApnsConfiguration.ApnsServerEnvironment ambiente = ApplePushProduccion ? ApnsConfiguration.ApnsServerEnvironment.Production : ApnsConfiguration.ApnsServerEnvironment.Sandbox;
                                            var configApns = new ApnsConfiguration(ambiente, appleCert, CertApplePass, false);

                                            apnsPush = new ApnsServiceBroker(configApns);
                                            apnsPush.OnNotificationFailed += apnsPush_OnNotificationFailed;
                                            apnsPush.OnNotificationSucceeded += apnsPush_OnNotificationSucceeded;
                                            apnsPush.Start();
                                            apnsPush.QueueNotification((ApnsNotification)alerta.Notificacion(token));
                                            apnsPush.Stop();
                                        }                                        
                                        break;
                                    case Dispositivo.Android:
                                        var configGCM = new GcmConfiguration(FBSenderAuthToken);
                                        
                                        configGCM.OverrideUrl("https://fcm.googleapis.com/fcm/send");
                                        configGCM.GcmUrl = "https://fcm.googleapis.com/fcm/send";
                                        gcmPush = new GcmServiceBroker(configGCM);
                                        
                                        gcmPush.OnNotificationSucceeded += gcmPush_OnNotificationSucceeded;
                                        gcmPush.OnNotificationFailed += gcmPush_OnNotificationFailed;
                                        gcmPush.Start();
                                        gcmPush.QueueNotification((GcmNotification)alerta.Notificacion(token));
                                        
                                        gcmPush.Stop();
                                        break;
                                    case Dispositivo.Huawei:
                                        {
                                           
                                            var body = (GcmNotification)alerta.Notificacion(token);
                                            var msg = new Message()
                                            {
                                                Data = body.Data.ToString(),
                                                //Android = new AndroidConfig()
                                                //{
                                                //    Urgency = UrgencyPriority.HIGH,
                                                //    Notification = new AndroidNotification()
                                                //    {
                                                //        Title = token.Titulo,
                                                //        Body = token.Contenido,
                                                //        ClickAction = ClickAction.OpenApp()
                                                //    },
                                                //},
                                                Token = new List<string>() { token.TokenID }
                                              
                                                

                                            };

                                            

                                             _ = SendAGHuaweiNotification(agConnectApp, msg, token.TokenID);



                                            //AGConnectApp clientApp = AGConnectApp.CreateWhitName(new AppOptions()
                                            //{
                                            //    ClientId = "102348763",
                                            //    ClientSecret = "708178cddaac0740ad013c08db1bda33a29bce5d9d59ba293519c44cedf437f9",
                                            //}, "Client");

                                            //AGConnectApp commerceApp = AGConnectApp.CreateWhitName(new AppOptions()
                                            //{
                                            //    ClientId = "103127659",
                                            //    ClientSecret = "7f3ea311ce8d541705cadd344d72c6835f573338ce583b374eadabaf240d1067",
                                            //}, "Commerce");

                                            //SendDataMessage(clientApp,
                                            //    "ANuu4DQivCQNc-V3mxlCIgpp53hRxHe8mm-V0AIDHxGn6QKuuGtXKV9ppjthUZi3XARgWHivG6Zv0jZ1UrnPBsvbHKDnqGZ3amIk12sIf0qD0Ajp4tX_tmShnH-TGRkZbg");
                                            //SendDataMessage(commerceApp, "AJEAGA8oUqa3NiGVpl_hK8IWMsCC_3SLsDvGfyW3AQSqHI_fHHilXzAaR3zIsYhe0yFl7O-r0ykGtmES6_0ZZJuyArPAwh16fJt1d8WxMVtqs9glX4YYgvbBkgr10XflyQ");

                                        }
                                       
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                EscribirEnLog("hubo Error : " + ex.Message);
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }

                    if (todoCorrecto() || (ContadorIntentos >= CantidadIntentos))
                    {
                        this._shouldStop = true;
                    }
                    else
                    {
                        this.ContadorEnviados = 0;
                        this.ContadorNoEnviados = 0;
                        Thread.Sleep(SegundosReintento * 2000);
                    }
                }
                catch (Exception ex)
                {
                    EscribirEnLog("hubo Error : " + ex.Message);
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void SendDataMessage(AGConnectApp app, String token)
        {
            var response = AGConnectMessaging.GetMessaging(app).SendAsync(new Message()
            {
                Data =
                    "{'title':'Test001666', 'body':'value2', 'invasivo': 'si', 'reqservdes':'d4545','servicioNombre':'d4545','vista':'d4545'}",
                Token = new List<string>() { token }
            });

            Console.WriteLine("Response: " + response.Result);
        }

        public async Task SendAGHuaweiNotification(AGConnectApp agapp, Message msg, string token)
        {
            try
            {
                var requestId = await AGConnectMessaging.GetMessaging(agapp).SendAsync(msg);
                Assert.True(!string.IsNullOrEmpty(requestId));

                TokensSended.Add(token);

                ContadorEnviados++;

            }
            catch (Exception ex)
            {
                ContadorNoEnviados++;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("================================================================================");
                sb.AppendLine("Notificacion: " + msg.Data);
                sb.AppendLine("Error: " + ex.Message);
                EscribirEnLog(sb.ToString());
            }

        }


        void apnsPush_OnNotificationSucceeded(ApnsNotification notification)
        {
            string token = "";
            try
            {
                token = notification.DeviceToken;
                TokensSended.Add(token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            ContadorEnviados++;
        }

        void apnsPush_OnNotificationFailed(ApnsNotification notification, AggregateException exception)
        {
            ContadorNoEnviados++;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("================================================================================");
            sb.AppendLine("Notificacion: " + notification.ToString());
            sb.AppendLine("Error: " + exception.Message);

            EscribirEnLog(sb.ToString());
        }

        void gcmPush_OnNotificationFailed(GcmNotification notification, AggregateException exception)
        {
            ContadorNoEnviados++;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("================================================================================");
            sb.AppendLine("Notificacion: " + notification.ToString());
            sb.AppendLine("Error: " + exception.Message);

            EscribirEnLog(sb.ToString());
        }

        private void gcmPush_OnNotificationSucceeded(GcmNotification notification)
        {
            string token = "";
            try
            {
                token = notification.RegistrationIds.First();
                TokensSended.Add(token);
            }
            catch
            {

            }
            ContadorEnviados++;
        }

        public void EscribirEnLog(string mensaje)
        {
            var logHab = ConfigurationManager.AppSettings["logHabilitado"];
            if (Convert.ToBoolean(logHab))
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("\n" + mensaje);
                    var filePath = ConfigurationManager.AppSettings["fileLog"];
                    if (File.Exists(filePath)) {
                        try
                        {
                            File.AppendAllText(filePath, sb.ToString());
                        }
                        catch { }
                    }                        
                    sb.Clear();
                }
                catch (Exception ex)
                {
                }
            }
        }

        bool todoCorrecto()
        {
            return this.ContadorEnviados == (this.Tokens.Count - this.TokensSended.Count);
        }

        public void RequestStop()
        {
            _shouldStop = true;
        }
        // Volatile is used as hint to the compiler that this data
        // member will be accessed by multiple threads.
        private volatile bool _shouldStop;
        private Task _;
    }
}
