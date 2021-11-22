using DataP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Entidad
{
    public class ENotificacionFuncionario
    {
        public long NotificacionFuncionarioId { get; set; }
        public long FuncionarioId { get; set; }
        public string Url { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Tiempo { get; set; }

        public int Leido { get; set; }
        public long IdImplicado { get; set; }


        public string NombreCliente { get; set; }
        public string ImagenCliente { get; set; }
        public ENotificacionFuncionario()
        {
        }
        public ENotificacionFuncionario(NotificacionFuncionario NotificacionFuncionario)
        {
            this.NotificacionFuncionarioId = NotificacionFuncionario.nNotificacionFuncionarioId;
            this.FuncionarioId = NotificacionFuncionario.nNotificacionFuncionarioId;
            this.Url = NotificacionFuncionario.sUrl;
            this.Titulo = NotificacionFuncionario.sTitulo;
            this.Descripcion = NotificacionFuncionario.sDescripcion;
            this.Estado = NotificacionFuncionario.nEstado;
            this.FechaCreacion = NotificacionFuncionario.dFechaCreacion;
            this.Leido = NotificacionFuncionario.nLeido;
            this.IdImplicado = NotificacionFuncionario.nIdImplicado;
        }

    }
}
