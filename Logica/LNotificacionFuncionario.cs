using DataP;
using Entidad;
using Entidad.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica
{
    public class LNotificacionFuncionario : LBaseCC<NotificacionFuncionario> 
    {
        public ENotificacionFuncionario Convert(NotificacionFuncionario C)
        {
            return new ENotificacionFuncionario(C);
        }
        public List<ENotificacionFuncionario> Convert(List<NotificacionFuncionario> L)
        {
            List<ENotificacionFuncionario> LC = new List<ENotificacionFuncionario>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<ENotificacionFuncionario> Obtener()
        {
            using (var esquema = GetEsquema())
            {
                List<NotificacionFuncionario> resultado = (from x in esquema.NotificacionFuncionario
                                                        where x.nEstado != (int)Estado.Eliminado
                                                orderby x.dFechaCreacion descending
                                                select x).ToList();
                return Convert(resultado);
            }
        }
        private NotificacionFuncionario ObtenerNotificacion(dbFexpoCruzEntities esquema, long NotificacionId)
        {
            NotificacionFuncionario entidad = (from x in esquema.NotificacionFuncionario
                                            where x.nNotificacionFuncionarioId == NotificacionId && x.nEstado != (int)Estado.Eliminado
                                    select x).FirstOrDefault();
            return entidad;
        }
        public ENotificacionFuncionario Obtener(long id)
        {
            using (var esquema = GetEsquema())
            {
                NotificacionFuncionario entidad = ObtenerNotificacion(esquema, id);
                if (entidad == null)
                {
                    throw new BussinessException("No se puede encontrar la categoria seleccionada.");
                }
                var resultado = Convert(entidad);
                return resultado;
            }
        }

        public void MarcarNotificacionLeida(long idNotificacion)
        {
            using (var esquema = GetEsquema())
            {
                NotificacionFuncionario entidad = ObtenerNotificacion(esquema, idNotificacion);
                entidad.nLeido = 1;
                esquema.SaveChanges();
            }
        }

        public void EliminarNotificaciones(long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                List<NotificacionFuncionario> entidad = (from x in esquema.NotificacionFuncionario
                                                      where x.nFuncionarioId == UsuarioId
                                                      && x.nEstado==(int)Estado.Habilitado
                                                      select x).ToList();

                entidad.ForEach(x=>x.nEstado = (int)Estado.Eliminado);
                esquema.SaveChanges();
            }
        }

        public List<ENotificacionFuncionario> ObtenerLista(long UsuarioId)
        {
            List<ENotificacionFuncionario> resultado = new List<ENotificacionFuncionario>();
            using (var esquema = GetEsquema())
            {
                // todos
                var entidades = (from u in esquema.NotificacionFuncionario
                                 where u.nEstado != (int)Estado.Eliminado && u.nLeido==0 && u.nFuncionarioId== UsuarioId
                                 orderby u.dFechaCreacion descending
                                 select u);
                foreach(NotificacionFuncionario item in entidades)
                {
                    EHilo hilo = LHilo.Instancia.LHilo.ObtenerHiloPorEventoId(item.nIdImplicado);

                    if (hilo!=null)
                    {
                        var start = DateTime.Now;
                        var oldDate = item.dFechaCreacion;
                        double minutos = (start - oldDate).TotalMinutes;
                        double horas = (start - oldDate).TotalHours;
                        double dias = (start - oldDate).TotalDays;


                        string tiempo = "Hace ";
                        if (minutos <= 60)
                        {
                            tiempo = tiempo + Math.Ceiling(minutos) + " minuto(s)";
                        }
                        else if (horas <= 24)
                        {
                            tiempo = tiempo + Math.Ceiling(horas) + " hora(s)";
                        }
                        else
                        {
                            tiempo = tiempo + Math.Ceiling(dias) + " dia(s)";
                        }

                        ENotificacionFuncionario Entidad = new ENotificacionFuncionario();
                        Entidad.NotificacionFuncionarioId = item.nNotificacionFuncionarioId;
                        Entidad.FuncionarioId = item.nFuncionarioId;
                        Entidad.Leido = item.nLeido;
                        Entidad.Url = item.sUrl;
                        Entidad.IdImplicado = item.nIdImplicado;
                        Entidad.NombreCliente = hilo.Cliente.sNombres + " " + hilo.Cliente.sApellidos;
                        Entidad.ImagenCliente = hilo.Cliente.sUrlFoto;
                        Entidad.Tiempo = tiempo;
                        Entidad.Descripcion = item.sDescripcion;

                        resultado.Add(Entidad);
                    }
                   
                }
                
                return resultado;
            }
        }


        
    }
}
