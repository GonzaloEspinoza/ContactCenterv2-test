using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataP;
using Entidad;
using Entidad.Enums;
using System.Configuration;
using NotificacionPush;

namespace Logica
{
    public class LNotificacion : LBaseCC<Cliente>
    {

        public void CreateNotificationPushs(dbFexpoCruzEntities esquema, TipoNotificacionFuncionario tipo,
            long contenidoId, string contenido, List<long> usuariosIds = null, string titulo = "")
        {
            var notificaciones = new List<global::NotificacionPush.Entidades.Notificacion>();
            usuariosIds = usuariosIds.Distinct().ToList();

            var dispositivos = (from x in esquema.Cliente
                                where usuariosIds.Contains(x.nClienteId)
                                && x.sTokenNotificacionFirebase != null
                                && x.sTokenNotificacionFirebase != ""
                                && x.sTipoSistemaOperativo != ""
                                && x.sTipoSistemaOperativo != null
                                select new { PushId = x.sTokenNotificacionFirebase, TipoOS = x.sTipoSistemaOperativo, TipoApp = 1 }).ToList();


            foreach (var x in dispositivos)
            {
                global::NotificacionPush.Entidades.Notificacion notipush = new global::NotificacionPush.Entidades.Notificacion(contenidoId, x.TipoOS, x.PushId, 0, x.TipoApp);
                notificaciones.Add(notipush);
            }

            NotificacionPsh.EnviarNotificacion(string.IsNullOrEmpty(titulo) ? ObtenerTituloWithTipoNotificacion(tipo) : titulo, contenido, "", (int)tipo + "", notificaciones);
        }

        public void CreateNotificationWebPush(dbFexpoCruzEntities esquema, TipoNotificacionFuncionario tipo, long contenidoId,
          string contenido, List<long> usuariosIds = null, string titulo = "")
        {
            var notificaciones = new List<global::NotificacionPush.Entidades.Notificacion>();
            usuariosIds = usuariosIds.Distinct().ToList();


            var dispositivos = (from x in esquema.Usuario
                                where usuariosIds.Contains(x.UsuarioId)
                                && x.sLastPushIdWeb != null
                                && x.sLastPushIdWeb != ""
                                select new { PushId = x.sLastPushIdWeb, TipoOS = "WEB", TipoApp = 4 }).ToList();


            foreach (var x in dispositivos)
            {
                global::NotificacionPush.Entidades.Notificacion notipush = new global::NotificacionPush.Entidades.Notificacion(contenidoId, x.TipoOS, x.PushId, 0, x.TipoApp);
                notificaciones.Add(notipush);
            }

            NotificacionPsh.EnviarNotificacion(string.IsNullOrEmpty(titulo) ? ObtenerTituloWithTipoNotificacion(tipo) : titulo, contenido, "", (int)tipo + "", notificaciones);

        }


        private string ObtenerTituloWithTipoNotificacion(TipoNotificacionFuncionario tipo)
        {
            switch (tipo)
            {
                case TipoNotificacionFuncionario.SinAsignacion: return "Evento sin asisgnación";
                case TipoNotificacionFuncionario.Asignado: return "Evento asignado";
                case TipoNotificacionFuncionario.Respondido: return "Evento respondido";
                case TipoNotificacionFuncionario.Atendido: return "Evento Atendido";
                case TipoNotificacionFuncionario.Reasignado: return "Evento Reasignado";
                case TipoNotificacionFuncionario.Mensaje: return "Nuevo mensaje";
                default: return "YAIGO";
            }
        }
    }

}
