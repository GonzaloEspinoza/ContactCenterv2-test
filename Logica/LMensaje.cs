using DataP;
using Entidad;
using Entidad.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Logica
{
    public class LMensaje : LBaseCC<Mensaje>
    {


        public EMensaje Convert(Mensaje CP)
        {
            return new EMensaje(CP);
        }
        public List<EMensaje> Convert(List<Mensaje> L)
        {
            List<EMensaje> LC = new List<EMensaje>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<EMensaje> Obtener(long EventoHiloId)
        {
            var Entidad = new List<EMensaje>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.Mensaje
                                 where x.nEstado != (int)Estado.Eliminado
                                 && x.nEventoHiloId == EventoHiloId
                                 select x).ToList();
                foreach (var item in resultado)
                {
                    Entidad.Add(new EMensaje()
                    {
                        MensajeId = item.nMensajeId,
                        Texto = item.sTexto,
                        Archivo=item.sArchivo,
                        Estado = item.nEstado,
                        
                    });
                }
                return Entidad;
            }
        }


        public EMensaje ObtenerDetalle(long id)
        {
            using (var esquema = GetEsquema())
            {
                Mensaje entidad = ObtenerMensaje(esquema, id);
                if (entidad == null)
                {
                    throw new BussinessException("No se puede encontrar el Mensaje seleccionada.");
                }
                var resultado = Convert(entidad);

                return resultado;
            }
        }

        public List<EHilo> ObtenerMensajesPorEventoHilo(long idEvento)
        {
            using (var esquema = GetEsquema())
            {
                List<EHilo> mensajes = (from x in esquema.EventoHilo
                                        join e in esquema.Evento on x.nEventoId equals e.nEventoId
                                where e.nEventoId == idEvento
                                orderby x.nEventoHiloId ascending
                                        select new EHilo() {
                                    Mensajes = x.Mensaje,
                                    Cliente = x.Evento.Cliente,
                                    Evento = x.Evento
                                }).ToList();
                List<Mensaje> listadoMensajesAntiguos = new List<Mensaje>();

                for (int i = 0;i< mensajes.Count;i++)
                {
                    foreach (var mensaje in mensajes[i].Mensajes)
                    {
                        mensaje.sArchivo = EUtils.URLImagen(mensaje.sArchivo);
                        if (i!=0)
                        {
                            mensajes[0].Mensajes.Add(mensaje);
                        }
                    }
                }
                mensajes[0].Mensajes = mensajes[0].Mensajes.OrderBy(x => x.nMensajeId).ToList();
                return mensajes;
            }
        }


        


        private Mensaje ObtenerMensaje(dbFexpoCruzEntities esquema, long? MensajeId)
        {
            Mensaje entidad = (from x in esquema.Mensaje
                            where x.nMensajeId == MensajeId && x.nEstado != (int)Estado.Eliminado
                            select x).FirstOrDefault();
            return entidad;
        }
        public List<EMensaje> ObtenerLista(string estado)
        {
            using (var esquema = GetEsquema())
            {
                // todos
                var entidades = (from u in esquema.Mensaje
                                 where u.nEstado != (int)Estado.Eliminado
                                 select u);
                if (estado == "0") // deshabilitado
                {
                    entidades = (from u in entidades
                                 where u.nEstado == (int)Estado.Desabilitado
                                 select u);
                }
                else if (estado == "1") // habilitados
                {
                    entidades = (from u in entidades
                                 where u.nEstado == (int)Estado.Habilitado
                                 select u);
                }
                var resultado = Convert(entidades.ToList());

                return resultado;
            }
        }

        public Mensaje Nuevo(Mensaje Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    Entidad.dFechaCreacion = EUtils.Now;
                    Entidad.dFechaModificacion = EUtils.Now;
                    esquema.Mensaje.Add(Entidad);
                    esquema.SaveChanges();

                    List<long> clienteIds = new List<long>();
                    Cliente cliente = (from x in esquema.Cliente
                                       join e in esquema.Evento on x.nClienteId equals e.nClienteId
                                       join eh in esquema.EventoHilo on e.nEventoId equals eh.nEventoId
                            where eh.nEventoHiloId == Entidad.nEventoHiloId
                                orderby x.dFechaCreacion descending
                            select x).FirstOrDefault();
                    clienteIds.Add(cliente.nClienteId);
                    LNotificacion.Instancia.LNotificacion.CreateNotificationPushs(esquema, TipoNotificacionFuncionario.Mensaje,
                        Entidad.nEventoHiloId,
                          Entidad.sTexto, clienteIds, "Contact center, nuevo mensaje");


                    RegistrarExitoEnBitacora(esquema, UsuarioId, null, AccionBitacora.Creacion, EntidadDetalleBitacora(Entidad));
                    return Entidad;
                }
                catch (BussinessException ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Creacion, EntidadDetalleBitacora(Entidad), ex);
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Creacion, EntidadDetalleBitacora(Entidad), ex);
                    throw ex;
                }
            }
        }
        public Mensaje NuevoMensajeImagen(Mensaje Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    ValidarImagen(Entidad);
                    Entidad.dFechaCreacion = EUtils.Now;
                    Entidad.dFechaModificacion = EUtils.Now;
                    Entidad.sArchivo = EUtils.InsertarImagen("Mensajes", Entidad.sArchivo);

                    esquema.Mensaje.Add(Entidad);
                    esquema.SaveChanges();

                    List<long> clienteIds = new List<long>();
                    Cliente cliente = (from x in esquema.Cliente
                                       join e in esquema.Evento on x.nClienteId equals e.nClienteId
                                       join eh in esquema.EventoHilo on e.nEventoId equals eh.nEventoId
                                       where eh.nEventoHiloId == Entidad.nEventoHiloId
                                       orderby x.dFechaCreacion descending
                                       select x).FirstOrDefault();
                    clienteIds.Add(cliente.nClienteId);
                    LNotificacion.Instancia.LNotificacion.CreateNotificationPushs(esquema, TipoNotificacionFuncionario.Mensaje,
                        Entidad.nEventoHiloId,
                          "Imagen", clienteIds, "Contact center, nuevo mensaje");

                    RegistrarExitoEnBitacora(esquema, UsuarioId, null, AccionBitacora.Creacion, EntidadDetalleBitacora(Entidad));
                    return Entidad;
                }
                catch (BussinessException ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Creacion, EntidadDetalleBitacora(Entidad), ex);
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Creacion, EntidadDetalleBitacora(Entidad), ex);
                    throw ex;
                }
            }
        }


        public Mensaje Modificar(Mensaje Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    Mensaje EntidadLocal = ObtenerMensaje(esquema, Entidad.nMensajeId);

                    EntidadLocal.sTexto = Entidad.sTexto;
                    EntidadLocal.nEventoHiloId = Entidad.nEventoHiloId;
                    EntidadLocal.sArchivo = Entidad.sArchivo;
                    EntidadLocal.nEstado = Entidad.nEstado;
                    EntidadLocal.dFechaModificacion = EUtils.Now;
                  
                    if (Entidad.nEstado != EntidadLocal.nEstado)
                    {
                        ValidarCambioEstado(EntidadLocal, (Estado)Entidad.nEstado, esquema);
                    }
                    EntidadLocal.nEstado = Entidad.nEstado;

                    esquema.SaveChanges();

                    RegistrarExitoEnBitacora(esquema, UsuarioId, null, AccionBitacora.Modificacion, EntidadDetalleBitacora(EntidadLocal));
                    return EntidadLocal;
                }
                catch (BussinessException ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Modificacion, EntidadDetalleBitacora(Entidad), ex);
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Modificacion, EntidadDetalleBitacora(Entidad), ex);
                    throw ex;
                }
            }
        }

        public string EntidadDetalleBitacora(Mensaje Entidad)
        {
            return Entidad.nMensajeId + "|" + Entidad.sTexto + "|" + Entidad.nEstado;
        }

        public void Validar(Mensaje Entidad)
        {
            if (string.IsNullOrEmpty(Entidad.sTexto))
            {
                throw new BussinessException("El texto es obligatorio.");
            }
        }

        public void ValidarImagen(Mensaje Entidad)
        {
            if (string.IsNullOrEmpty(Entidad.sArchivo))
            {
                throw new BussinessException("La imagen es obligatoria.");
            }
        }


        

        private void ValidarCambioEstado(Mensaje Entidad, Estado estado, dbFexpoCruzEntities esquema)
        {
            switch (estado)
            {
                case Estado.Desabilitado:
                    DeshabilitarValidar(Entidad, esquema);
                    break;
                case Estado.Habilitado:
                    HabilitarValidar(Entidad, esquema);
                    break;
                case Estado.Eliminado:
                    EliminarValidar(Entidad, esquema);
                    break;
            }
        }

        private string DeshabilitarValidar(Mensaje Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string HabilitarValidar(Mensaje Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string EliminarValidar(Mensaje Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }

        public string NombrEMensaje(long? id)
        {
            using (var esquema = GetEsquema())
            {
                Mensaje entidad = ObtenerMensaje(esquema, id);
                if (entidad == null)
                {
                    return "";
                }
                var resultado = Convert(entidad);
                return resultado.Texto;
            }
        }



        public List<EMensaje> Mensajees()
        {
            try
            {
                var Entidad = new List<EMensaje>();
                using (var esquema = GetEsquema())
                {
                    var resultado = (from x in esquema.Mensaje
                                     where x.nEstado != (int)Estado.Eliminado
                                     select x).ToList();
                    foreach (var item in resultado)
                    {
                        Entidad.Add(new EMensaje()
                        {
                            MensajeId = item.nMensajeId,
                            Texto = item.sTexto
                        });
                    }
                    return Entidad;
                }
            }
            catch (BussinessException ex)
            {
                throw new BussinessException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void CambiarEstado(long id, Estado estado, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                Mensaje EntidadEx = ObtenerMensaje(esquema, id);
                if (EntidadEx != null)
                {
                    try
                    {
                        ValidarCambioEstado(EntidadEx, estado, esquema);
                        EntidadEx.nEstado = (int)estado;
                        EntidadEx.dFechaModificacion = EUtils.Now;
                        esquema.SaveChanges();
                        RegistrarExitoEnBitacora(esquema, UsuarioId, id, estado == Estado.Habilitado ? AccionBitacora.Habilitacion : estado == Estado.Desabilitado ? AccionBitacora.Habilitacion : AccionBitacora.Eliminacion, EntidadDetalleBitacora(EntidadEx));
                    }
                    catch (BussinessException ex)
                    {
                        RegistrarErrorEnBitacora(esquema, UsuarioId, id, estado == Estado.Habilitado ? AccionBitacora.Habilitacion : estado == Estado.Desabilitado ? AccionBitacora.Habilitacion : AccionBitacora.Eliminacion, EntidadDetalleBitacora(EntidadEx), ex); throw new BussinessException(ex.Message);
                    }
                }
                else
                {
                    Exception Ex = new Exception("No existe la Mensaje con id " + id);
                    RegistrarErrorEnBitacora(esquema, UsuarioId, id, AccionBitacora.Obteniendo, "Categoria Servicios cambio de estado", Ex);
                    throw Ex;
                }
            }
        }


        #region api
        
        public List<ResMensaje> ObtenerMensajesPorEventoHiloCliente(long idEventoHilo)
        {
            using (var esquema = GetEsquema())
            {

                var evento = (from x in esquema.Evento
                              join eh in esquema.EventoHilo on x.nEventoId equals eh.nEventoId
                              where eh.nEventoHiloId == idEventoHilo
                              select x).FirstOrDefault();
                if (evento==null)
                {
                    throw new BussinessException("El evento no existe.");
                }
                var mensajes = (from x in esquema.Mensaje
                                join eh in esquema.EventoHilo on x.nEventoHiloId equals eh.nEventoHiloId
                                join e in esquema.Evento on eh.nEventoId equals e.nEventoId
                                where e.nEventoId == evento.nEventoId
                                orderby x.nMensajeId ascending
                                select x).ToList();
                List<ResMensaje> LC = new List<ResMensaje>();
                mensajes.ForEach(x => LC.Add(new ResMensaje(x)));
                return LC;
            }
        }

        
        public List<ResMensajeV2> ObtenerMensajesPorEventoHiloClienteV2(long idEventoHilo)
        {
            using (var esquema = GetEsquema())
            {

                var evento = (from x in esquema.Evento
                              join eh in esquema.EventoHilo on x.nEventoId equals eh.nEventoId
                              where eh.nEventoHiloId == idEventoHilo
                              select x).FirstOrDefault();
                if (evento == null)
                {
                    throw new BussinessException("El evento no existe.");
                }
                var mensajes = (from x in esquema.Mensaje
                                join eh in esquema.EventoHilo on x.nEventoHiloId equals eh.nEventoHiloId
                                join e in esquema.Evento on eh.nEventoId equals e.nEventoId
                                where e.nEventoId == evento.nEventoId
                                orderby x.nMensajeId ascending
                                select x).ToList();

                List<ResMensajeV2> LC = new List<ResMensajeV2>();
                mensajes.ForEach(x => LC.Add(new ResMensajeV2(x)));
                return LC;
            }
        }

        public bool RegistrarMensaje(long eventoHiloId,string texto, List<string> archivos)
        {
            using (var esquema = GetEsquema())
            {

                Cliente clienteExistente = (from x in esquema.Cliente
                                            join e in esquema.Evento on x.nClienteId equals e.nClienteId
                                            join eh in esquema.EventoHilo on e.nEventoId equals eh.nEventoId
                                            where eh.nEventoHiloId == eventoHiloId
                                            select x).FirstOrDefault();
                if (clienteExistente == null)
                {
                    throw new BussinessException("No se encontro el hilo");
                }

                Evento eventoActual = (from x in esquema.Evento
                                            join eh in esquema.EventoHilo on x.nEventoId equals eh.nEventoId
                                            where eh.nEventoHiloId == eventoHiloId
                                            select x).FirstOrDefault();
                if (eventoActual == null)
                {
                    throw new BussinessException("El evento no fue encontrado");
                }
                if (eventoActual.nEstadoEventoId == (int)EstadoDeEvento.Atendido || eventoActual.nEstadoEventoId == (int)EstadoDeEvento.NoResuelto)
                {
                    throw new BussinessException("No puede enviar mensajes, el chat ya fue atendido.");
                }

                if (archivos.Count>0)
                {
                    foreach (string archivo in archivos)
                    {
                        Mensaje mensajeArchivo = new Mensaje();
                        mensajeArchivo.sTexto = "";
                        mensajeArchivo.sArchivo = archivo;
                        mensajeArchivo.nEventoHiloId = eventoHiloId;
                        mensajeArchivo.nEstado = 1;
                        mensajeArchivo.nClienteId = clienteExistente.nClienteId;
                        mensajeArchivo.dFechaCreacion = EUtils.Now;
                        mensajeArchivo.dFechaModificacion = EUtils.Now;
                        esquema.Mensaje.Add(mensajeArchivo);
                    }
                }
                if (texto != "")
                {
                    Mensaje mensajeTexto = new Mensaje();
                    mensajeTexto.sTexto = texto;
                    mensajeTexto.sArchivo = "";
                    mensajeTexto.nEventoHiloId = eventoHiloId;
                    mensajeTexto.nEstado = 1;
                    mensajeTexto.nClienteId = clienteExistente.nClienteId;
                    mensajeTexto.dFechaCreacion = EUtils.Now;
                    mensajeTexto.dFechaModificacion = EUtils.Now;
                    esquema.Mensaje.Add(mensajeTexto);
                }
                esquema.SaveChanges();
                return true;
            }
        }

        #endregion



    }
}
