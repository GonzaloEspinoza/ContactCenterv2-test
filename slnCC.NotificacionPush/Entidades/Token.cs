using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificacionPush.Entidades
{
    public class Token
    {
        public string TokenID { get; set; }
        public Dispositivo TipoDispositivo { get; set; }
        public string Contenido { get; set; }
        public string Titulo { get; set; }
        public string Tipo { get; set; }
        public string IdNotificacion { get; set; }
        public string IdContenido { get; set; }
        public int Badge { get; set; }
        public Aplicacion TipoAplicacion { get; set; }

        public Token(string token, Dispositivo tipoDisp, string MensajeAlerta, string TituloAlerta, string tipo, string idContenido, string idNotif, int Badge, Aplicacion tipoAplic)
        {
            TokenID = token;
            TipoDispositivo = tipoDisp;
            Contenido = MensajeAlerta;
            Titulo = TituloAlerta;
            Tipo = tipo;
            IdContenido = idContenido;
            IdNotificacion = idNotif;
            TipoAplicacion = tipoAplic;
            this.Badge = Badge;
        }
    }
}
