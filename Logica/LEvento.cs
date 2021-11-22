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
    public class LEvento : LBaseCC<Evento>
    {


        public EEvento Convert(Evento CP)
        {
            return new EEvento(CP);
        }
        public List<EEvento> Convert(List<Evento> L)
        {
            List<EEvento> LC = new List<EEvento>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<EEvento> Obtener()
        {
            var Entidad = new List<EEvento>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.Evento
                                 select x).ToList();
                Entidad = Convert(resultado);
                return Entidad;
            }
        }

        public List<EEvento> ObtenerPorRangoFecha(DateTime fi, DateTime ff)
        {
            var Entidad = new List<EEvento>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.Evento
                                 where x.dFechaCreacion >= fi && x.dFechaCreacion <= ff
                                 select x).ToList();
                Entidad = Convert(resultado);
                return Entidad;
            }
        }
        public List<EEvento> ObtenerPorColaboradorPorRangoFecha(long funcionarioId, DateTime fi, DateTime ff)
        {
            var Entidad = new List<EEvento>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.Evento
                                 join eh in esquema.EventoHilo on x.nEventoId equals eh.nEventoId
                                 join h in esquema.Hilo on eh.nHiloId equals h.nHiloId
                                 where h.nFuncionarioId == funcionarioId &&
                                 x.dFechaCreacion >= fi && x.dFechaCreacion <= ff
                                 select x).ToList();
                Entidad = Convert(resultado);
                return Entidad;
            }
        }


        public EEvento Obtener(long id)
        {
            using (var esquema = GetEsquema())
            {
                Evento entidad = ObtenerEvento(esquema, id);
                if (entidad == null)
                {
                    throw new BussinessException("No se puede encontrar la Evento seleccionada.");
                }
                var resultado = Convert(entidad);
                return resultado;
            }
        }

        private Evento ObtenerEvento(dbFexpoCruzEntities esquema, long? EventoId)
        {
            Evento entidad = (from x in esquema.Evento
                                 where x.nEventoId == EventoId 
                                 select x).FirstOrDefault();
            return entidad;
        }
        public List<EEvento> ObtenerLista(long estadoEvento)
        {
            using (var esquema = GetEsquema())
            {
                var entidades = (from u in esquema.Evento
                                 where u.nEstadoEventoId == estadoEvento
                                 select u);
                var resultado = Convert(entidades.ToList());
                return resultado;
            }
        }

        public Evento Nuevo(long clienteId, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Funcionario funcionario = LFuncionario.Instancia.LFuncionario.ObtenerFuncionario(UsuarioId);
                    if (funcionario == null)
                    {
                        throw new BussinessException("No hay colaboradores disponibles");
                    }
                    long TipoAtencionId = 1;
                    var empresa = (from x in esquema.TipoAtencion
                                   where x.nTipoAtencionId == TipoAtencionId
                                   select x.Area.Empresa).FirstOrDefault();
                    if (empresa == null)
                    {
                        throw new BussinessException("No hay empresas disponibles");
                    }
                    Cliente clienteExistente = (from x in esquema.Cliente
                                                where x.nClienteId == clienteId && x.nEstado != (int)Estado.Eliminado
                                                select x).FirstOrDefault();
                    

                    Evento evento = new Evento();
                    evento.nClienteId = clienteExistente.nClienteId;
                    evento.nEstadoEventoId = (long)Entidad.Enums.EstadoDeEvento.Asignado;
                    evento.nTipoAtencionId = TipoAtencionId;
                    evento.dFechaCreacion = EUtils.Now;
                    evento.dFechaModificacion = EUtils.Now;
                    esquema.Evento.Add(evento);
                    esquema.SaveChanges();

                    HistorialEstadoEvento historialEstadoEvento = new HistorialEstadoEvento();
                    historialEstadoEvento.nEventoId = evento.nEventoId;
                    historialEstadoEvento.nEstadoEventoId = (long)Entidad.Enums.EstadoDeEvento.Asignado;
                    historialEstadoEvento.sDescripcion = "Evento nro. " + evento.nEventoId;
                    historialEstadoEvento.dFechaCreacion = EUtils.Now;
                    historialEstadoEvento.dFechaModificacion = EUtils.Now;
                    esquema.HistorialEstadoEvento.Add(historialEstadoEvento);
                    esquema.SaveChanges();


                    Hilo hilo = new Hilo();
                    hilo.nFuncionarioId = funcionario.nFuncionarioId;
                    hilo.sCodigo = EUtils.GenerateRandomCode();
                    hilo.nEstado = (int)Estado.Habilitado;
                    hilo.dFechaCreacion = EUtils.Now;
                    hilo.dFechaModificacion = EUtils.Now;
                    esquema.Hilo.Add(hilo);
                    esquema.SaveChanges();

                    EventoHilo eventoHilo = new EventoHilo();
                    eventoHilo.nEventoId = evento.nEventoId;
                    eventoHilo.nHiloId = hilo.nHiloId;
                    eventoHilo.dFechaCreacion = EUtils.Now;
                    eventoHilo.dFechaModificacion = EUtils.Now;
                    esquema.EventoHilo.Add(eventoHilo);
                    esquema.SaveChanges();

                    List<long> tokenAdmin = LUsuarioBackEnd.Instancia.LUsuarioBackEnd.
                        RegistrarNotificacionAdministrador(funcionario.nFuncionarioId, "Nuevo evento", "Nuevo evento",
                        (int)evento.nEventoId, TipoNotificacionFuncionario.Asignado);
                    /*LNotificacion.Instancia.LNotificacion.CreateNotificationWebPush(esquema, TipoNotificacionFuncionario.Asignado, evento.nEventoId,
                                "Nuevo evento", tokenAdmin, "Nuevo evento");*/
                    return evento;
                }
                catch (BussinessException ex)
                {
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public Evento reasignarEvento(long UsuarioId, long funcionarioId, long idEventoHilo)
        {
            using (var esquema = GetEsquema())
            {
                
                try
                {
                    Funcionario funcionario = LFuncionario.Instancia.LFuncionario.ObtenerColaborador(funcionarioId);
                    if (funcionario == null)
                    {
                        throw new BussinessException("No hay colaboradores disponibles");
                    }


                    var eventoHiloAnterior = (from x in esquema.EventoHilo
                                  where x.nEventoHiloId == idEventoHilo
                                  select x).FirstOrDefault();
                    Evento evento = (from x in esquema.Evento
                                     where x.nEventoId == eventoHiloAnterior.nEventoId
                                     select x).FirstOrDefault();
                    if (evento == null)
                    {
                        throw new BussinessException("No hay evento disponible");
                    }
                    evento.nEstadoEventoId = (long)Entidad.Enums.EstadoDeEvento.Reasignado;
                    esquema.SaveChanges();

                    Hilo hiloEvento = (from x in esquema.Hilo
                                      where x.nHiloId == eventoHiloAnterior.nHiloId
                                      select x).FirstOrDefault();
                    hiloEvento.nEstado = (int)Estado.Desabilitado;
                    hiloEvento.nActual = (int)Estado.Desabilitado;
                    esquema.SaveChanges();

                    HistorialEstadoEvento historialEstadoEvento = new HistorialEstadoEvento();
                    historialEstadoEvento.nEventoId = evento.nEventoId;
                    historialEstadoEvento.nEstadoEventoId = (long)Entidad.Enums.EstadoDeEvento.Reasignado;
                    historialEstadoEvento.sDescripcion = "Evento nro. " + evento.nEventoId;
                    historialEstadoEvento.dFechaCreacion = EUtils.Now;
                    historialEstadoEvento.dFechaModificacion = EUtils.Now;
                    esquema.HistorialEstadoEvento.Add(historialEstadoEvento);
                    esquema.SaveChanges();


                    Hilo hilo = new Hilo();
                    hilo.nFuncionarioId = funcionario.nFuncionarioId;
                    hilo.sCodigo = EUtils.GenerateRandomCode();
                    hilo.nEstado = (int)Estado.Habilitado;
                    hilo.nActual = (int)Estado.Habilitado;
                    hilo.dFechaCreacion = EUtils.Now;
                    hilo.dFechaModificacion = EUtils.Now;
                    esquema.Hilo.Add(hilo);
                    esquema.SaveChanges();

                    EventoHilo eventoHilo = new EventoHilo();
                    eventoHilo.nEventoId = evento.nEventoId;
                    eventoHilo.nHiloId = hilo.nHiloId;
                    eventoHilo.dFechaCreacion = EUtils.Now;
                    eventoHilo.dFechaModificacion = EUtils.Now;
                    esquema.EventoHilo.Add(eventoHilo);
                    esquema.SaveChanges();

                    List<long> tokenAdmin = LUsuarioBackEnd.Instancia.LUsuarioBackEnd.
                        RegistrarNotificacionAdministrador(funcionario.nFuncionarioId, "Reasignacion evento", "Reasignacion evento",
                        (int)evento.nEventoId, TipoNotificacionFuncionario.Asignado);
                    /*LNotificacion.Instancia.LNotificacion.CreateNotificationWebPush(esquema, TipoNotificacionFuncionario.Reasignado, evento.nEventoId,
                                "Reasignacion evento", tokenAdmin, "Reasignacion evento");*/
                    return evento;
                }
                catch (BussinessException ex)
                {
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public Evento marcarEventoAtendido(long idEventoHilo)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    

                    var eventoHiloAnterior = (from x in esquema.EventoHilo
                                              where x.nEventoHiloId == idEventoHilo
                                              select x).FirstOrDefault();
                    Evento evento = (from x in esquema.Evento
                                     where x.nEventoId == eventoHiloAnterior.nEventoId
                                     select x).FirstOrDefault();
                    if (evento == null)
                    {
                        throw new BussinessException("No hay evento disponible");
                    }
                    evento.nEstadoEventoId = (long)Entidad.Enums.EstadoDeEvento.Atendido;
                    esquema.SaveChanges();

                    Hilo hiloEvento = (from x in esquema.Hilo
                                       where x.nHiloId == eventoHiloAnterior.nHiloId
                                       select x).FirstOrDefault();
                    hiloEvento.nEstado = (int)Estado.Desabilitado;
                    esquema.SaveChanges();

                    HistorialEstadoEvento historialEstadoEvento = new HistorialEstadoEvento();
                    historialEstadoEvento.nEventoId = evento.nEventoId;
                    historialEstadoEvento.nEstadoEventoId = (long)Entidad.Enums.EstadoDeEvento.Atendido;
                    historialEstadoEvento.sDescripcion = "Evento nro. " + evento.nEventoId;
                    historialEstadoEvento.dFechaCreacion = EUtils.Now;
                    historialEstadoEvento.dFechaModificacion = EUtils.Now;
                    esquema.HistorialEstadoEvento.Add(historialEstadoEvento);
                    esquema.SaveChanges();

                    CalificacionColaborador calificacion = new CalificacionColaborador();
                    calificacion.nPuntuacion = 0;
                    calificacion.sComentario = "";
                    calificacion.nCalificado = 0;
                    calificacion.nEventoId = evento.nEventoId;
                    calificacion.nFuncionarioId = (long)hiloEvento.nFuncionarioId;
                    calificacion.nClienteId = evento.nClienteId;
                    calificacion.dFechaCreacion = EUtils.Now;
                    calificacion.dFechaModificacion = EUtils.Now;
                    esquema.CalificacionColaborador.Add(calificacion);
                    esquema.SaveChanges();

                    return evento;
                }
                catch (BussinessException ex)
                {
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public Evento marcarEventoNoResuelto(long idEventoHilo)
        {
            using (var esquema = GetEsquema())
            {

                try
                {


                    var eventoHiloAnterior = (from x in esquema.EventoHilo
                                              where x.nEventoHiloId == idEventoHilo
                                              select x).FirstOrDefault();
                    Evento evento = (from x in esquema.Evento
                                     where x.nEventoId == eventoHiloAnterior.nEventoId
                                     select x).FirstOrDefault();
                    if (evento == null)
                    {
                        throw new BussinessException("No hay evento disponible");
                    }
                    evento.nEstadoEventoId = (long)Entidad.Enums.EstadoDeEvento.NoResuelto;
                    esquema.SaveChanges();

                    Hilo hiloEvento = (from x in esquema.Hilo
                                       where x.nHiloId == eventoHiloAnterior.nHiloId
                                       select x).FirstOrDefault();
                    hiloEvento.nEstado = (int)Estado.Desabilitado;
                    esquema.SaveChanges();

                    HistorialEstadoEvento historialEstadoEvento = new HistorialEstadoEvento();
                    historialEstadoEvento.nEventoId = evento.nEventoId;
                    historialEstadoEvento.nEstadoEventoId = (long)Entidad.Enums.EstadoDeEvento.NoResuelto;
                    historialEstadoEvento.sDescripcion = "Evento nro. " + evento.nEventoId;
                    historialEstadoEvento.dFechaCreacion = EUtils.Now;
                    historialEstadoEvento.dFechaModificacion = EUtils.Now;
                    esquema.HistorialEstadoEvento.Add(historialEstadoEvento);
                    esquema.SaveChanges();

                    CalificacionColaborador calificacion = new CalificacionColaborador();
                    calificacion.nPuntuacion = 0;
                    calificacion.sComentario = "";
                    calificacion.nCalificado = 0;
                    calificacion.nEventoId = evento.nEventoId;
                    calificacion.nFuncionarioId = (long)hiloEvento.nFuncionarioId;
                    calificacion.nClienteId = evento.nClienteId;
                    calificacion.dFechaCreacion = EUtils.Now;
                    calificacion.dFechaModificacion = EUtils.Now;
                    esquema.CalificacionColaborador.Add(calificacion);
                    esquema.SaveChanges();

                    return evento;
                }
                catch (BussinessException ex)
                {
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public Evento tomarEvento(long idEventoHilo,long idFuncionario)
        {
            using (var esquema = GetEsquema())
            {

                try
                {


                    var eventoHiloAnterior = (from x in esquema.EventoHilo
                                              where x.nEventoHiloId == idEventoHilo
                                              select x).FirstOrDefault();
                    Evento evento = (from x in esquema.Evento
                                     where x.nEventoId == eventoHiloAnterior.nEventoId
                                     select x).FirstOrDefault();
                    if (evento == null)
                    {
                        throw new BussinessException("No hay evento disponible");
                    }
                    evento.nEstadoEventoId = (long)Entidad.Enums.EstadoDeEvento.Asignado;
                    esquema.SaveChanges();

                    Hilo hiloEvento = (from x in esquema.Hilo
                                       where x.nHiloId == eventoHiloAnterior.nHiloId
                                       select x).FirstOrDefault();
                    hiloEvento.nFuncionarioId = idFuncionario;
                    esquema.SaveChanges();

                    HistorialEstadoEvento historialEstadoEvento = new HistorialEstadoEvento();
                    historialEstadoEvento.nEventoId = evento.nEventoId;
                    historialEstadoEvento.nEstadoEventoId = (long)Entidad.Enums.EstadoDeEvento.Asignado;
                    historialEstadoEvento.sDescripcion = "Evento nro. " + evento.nEventoId;
                    historialEstadoEvento.dFechaCreacion = EUtils.Now;
                    historialEstadoEvento.dFechaModificacion = EUtils.Now;
                    esquema.HistorialEstadoEvento.Add(historialEstadoEvento);
                    esquema.SaveChanges();


                    return evento;
                }
                catch (BussinessException ex)
                {
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        
        public Evento Modificar(Evento Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    Evento EntidadLocal = ObtenerEvento(esquema, Entidad.nEventoId);

                    EntidadLocal.dFechaModificacion = EUtils.Now;
                    
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


        public void CambiarEstado(long id, Estado estado, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                Evento EntidadEx = ObtenerEvento(esquema, id);
                if (EntidadEx != null)
                {
                    try
                    {
                        ValidarCambioEstado(EntidadEx, estado, esquema);
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
                    Exception Ex = new Exception("No existe Evento con id " + id);
                    RegistrarErrorEnBitacora(esquema, UsuarioId, id, AccionBitacora.Obteniendo, "Evento cambio de estado", Ex);
                    throw Ex;
                }
            }

        }
        public string EntidadDetalleBitacora(Evento Entidad)
        {
            return Entidad.nEventoId + "|" + Entidad.nEventoId + "|" + Entidad.nClienteId;
        }

        public void Validar(Evento Entidad)
        {
            if (string.IsNullOrEmpty(Entidad.nClienteId.ToString()))
            {
                throw new BussinessException("El cliente es obligatorio.");
            }
           
        }


        private void ValidarCambioEstado(Evento Entidad, Estado estado, dbFexpoCruzEntities esquema)
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

        private string DeshabilitarValidar(Evento Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string HabilitarValidar(Evento Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string EliminarValidar(Evento Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }



        #region api
       
        public ResHilo Registrar(ReqRegistrarEvento parametro)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                   
                    Funcionario funcionario = LFuncionario.Instancia.LFuncionario.ObtenerColaboradorColaPorCiudad(parametro.CiudadId);
                    if (funcionario == null)
                    {
                        throw new BussinessException("Por el momento no contamos con un agente disponible");
                    }
                    var empresa = (from x in esquema.TipoAtencion
                                                    where x.nTipoAtencionId == parametro.TipoAtencionId
                                            select x.Area.Empresa).FirstOrDefault();
                    if (empresa == null)
                    {
                        throw new BussinessException("No hay empresas disponibles");
                    }
                    Cliente clienteExistente = (from x in esquema.Cliente
                                       where x.sCodigoCliente == parametro.Cliente.CodigoCliente && x.nEstado != (int)Estado.Eliminado
                                       select x).FirstOrDefault();
                    Cliente clienteNuevo = new Cliente();
                    if (clienteExistente!=null)
                    {
                        clienteExistente.sNombres = parametro.Cliente.Nombres;
                        clienteExistente.sApellidos = parametro.Cliente.Apellidos;
                        clienteExistente.sCodigoCliente = parametro.Cliente.CodigoCliente;
                        clienteExistente.sUrlFoto = parametro.Cliente.UrlFoto;
                        clienteExistente.sCodigoPais = parametro.Cliente.CodigoPais;
                        clienteExistente.sNumeroCelular = parametro.Cliente.NumeroCelular;
                        clienteExistente.sCorreo = parametro.Cliente.Correo;
                        clienteExistente.sTokenNotificacionFirebase = parametro.Cliente.TokenNotificacionFirebase;
                        clienteExistente.sTipoSistemaOperativo = parametro.Cliente.TipoSistemaOperativo;
                        clienteExistente.nEmpresaId = empresa.nEmpresaId;
                        clienteExistente.nEstado = (int)Estado.Habilitado;
                        clienteExistente.dFechaModificacion = EUtils.Now;
                        //esquema.SaveChanges();
                    }
                    else
                    {
                        clienteNuevo.sNombres = parametro.Cliente.Nombres;
                        clienteNuevo.sApellidos = parametro.Cliente.Apellidos;
                        clienteNuevo.sCodigoCliente = parametro.Cliente.CodigoCliente;
                        clienteNuevo.sUrlFoto = parametro.Cliente.UrlFoto;
                        clienteNuevo.sCodigoPais = parametro.Cliente.CodigoPais;
                        clienteNuevo.sNumeroCelular = parametro.Cliente.NumeroCelular;
                        clienteNuevo.sCorreo = parametro.Cliente.Correo;
                        clienteNuevo.sTokenNotificacionFirebase = parametro.Cliente.TokenNotificacionFirebase;
                        clienteNuevo.sTipoSistemaOperativo = parametro.Cliente.TipoSistemaOperativo;
                        clienteNuevo.nEmpresaId = empresa.nEmpresaId;
                        clienteNuevo.nEstado = (int)Estado.Habilitado;
                        clienteNuevo.dFechaCreacion = EUtils.Now;
                        clienteNuevo.dFechaModificacion = EUtils.Now;
                        esquema.Cliente.Add(clienteNuevo);
                        //esquema.SaveChanges();
                    }

                    Evento evento = new Evento();
                    evento.nClienteId = clienteExistente != null ? clienteExistente.nClienteId : clienteNuevo.nClienteId;
                    evento.nEstadoEventoId = parametro.CiudadId != 0 ?
                        (long)Entidad.Enums.EstadoDeEvento.Asignado : (long)Entidad.Enums.EstadoDeEvento.SinAsignacion;
                    evento.nTipoAtencionId = parametro.TipoAtencionId;
                    evento.dFechaCreacion = EUtils.Now;
                    evento.dFechaModificacion = EUtils.Now;
                    esquema.Evento.Add(evento);
                    //esquema.SaveChanges();

                    HistorialEstadoEvento historialEstadoEvento = new HistorialEstadoEvento();
                    historialEstadoEvento.nEventoId = evento.nEventoId;
                    historialEstadoEvento.nEstadoEventoId = parametro.CiudadId != 0 ?
                        (long)Entidad.Enums.EstadoDeEvento.Asignado : (long)Entidad.Enums.EstadoDeEvento.SinAsignacion;
                    historialEstadoEvento.sDescripcion = "Evento nro. " + evento.nEventoId;
                    historialEstadoEvento.dFechaCreacion = EUtils.Now;
                    historialEstadoEvento.dFechaModificacion = EUtils.Now;
                    esquema.HistorialEstadoEvento.Add(historialEstadoEvento);
                    //esquema.SaveChanges();

                    
                    Hilo hilo = new Hilo();
                    if (parametro.CiudadId != 0)
                    {
                        hilo.nFuncionarioId = funcionario.nFuncionarioId;
                    }
                    hilo.sCodigo = EUtils.GenerateRandomCode();
                    hilo.nEstado = (int)Estado.Habilitado;
                    hilo.nActual = (int)Estado.Habilitado;
                    hilo.dFechaCreacion = EUtils.Now;
                    hilo.dFechaModificacion = EUtils.Now;
                    esquema.Hilo.Add(hilo);
                    //esquema.SaveChanges();

                    EventoHilo eventoHilo = new EventoHilo();
                    eventoHilo.nEventoId = evento.nEventoId;
                    eventoHilo.nHiloId = hilo.nHiloId;
                    eventoHilo.dFechaCreacion = EUtils.Now;
                    eventoHilo.dFechaModificacion = EUtils.Now;
                    esquema.EventoHilo.Add(eventoHilo);
                    esquema.SaveChanges();

                    if (parametro.CiudadId != 0)
                    {
                        List<long> usuariosIs = LUsuarioBackEnd.Instancia.LUsuarioBackEnd.
                       RegistrarNotificacionAdministrador(funcionario.nFuncionarioId, "Nuevo evento", "Nuevo evento",
                       (int)evento.nEventoId, TipoNotificacionFuncionario.Asignado);
                        /*LNotificacion.Instancia.LNotificacion.CreateNotificationWebPush(esquema, TipoNotificacionFuncionario.Asignado, evento.nEventoId,
                                    "Nuevo evento", usuariosIs, "Nuevo evento");*/
                    }
                   
                    ResHilo eventoHiloCreado = LHilo.Instancia.LHilo.ObtenerPorEventoHiloId(eventoHilo.nEventoHiloId);

                    return eventoHiloCreado;
                }
                catch (BussinessException ex)
                {
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        
        #endregion



    }
}
