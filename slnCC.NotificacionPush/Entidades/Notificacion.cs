using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificacionPush.Entidades
{
    public class Notificacion
    {
        public string PushId { get; set; }
        public string TipoSO { get; set; }
        public int TipoAplicacion { get; set; }
        public int Badge { get; set; }

        public long NotificacionId { get; set; }

        public Notificacion(long NotificacionId, string TipoSO, string PushId, int Badge, int TipoAplicacion)
        {
            this.NotificacionId = NotificacionId;
            this.TipoSO = TipoSO;
            this.PushId = PushId;
            this.Badge = Badge;
            this.TipoAplicacion = TipoAplicacion;
        }
    }
}
