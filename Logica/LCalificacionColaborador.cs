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
    public class LCalificacionColaborador : LBaseCC<CalificacionColaborador>
    {


        public ECalificacionColaborador Convert(CalificacionColaborador CP)
        {
            return new ECalificacionColaborador(CP);
        }
        public List<ECalificacionColaborador> Convert(List<CalificacionColaborador> L)
        {
            List<ECalificacionColaborador> LC = new List<ECalificacionColaborador>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<ECalificacionColaborador> Obtener()
        {
            var Entidad = new List<ECalificacionColaborador>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.CalificacionColaborador
                                 select x).ToList();
                
                return Convert(resultado);
            }
        }

        public List<ECalificacionColaborador> ObtenerPorRangoFecha(DateTime fi, DateTime ff)
        {
            var Entidad = new List<ECalificacionColaborador>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.CalificacionColaborador
                                 where x.dFechaCreacion >= fi && x.dFechaCreacion <= ff
                                 && x.nCalificado == 1
                                 select x).ToList();

                return Convert(resultado);
            }
        }

        public List<ECalificacionColaborador> ObtenerPorColaboradorPorRangoFecha(long funcionarioId, DateTime fi, DateTime ff)
        {
            var Entidad = new List<ECalificacionColaborador>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.CalificacionColaborador
                                 where x.nFuncionarioId == funcionarioId &&
                                 x.dFechaCreacion >= fi && x.dFechaCreacion <= ff
                                 && x.nCalificado == 1
                                 select x).ToList();

                return Convert(resultado);
            }
        }

        public List<ECalifiacionColaboradorDataTable> ObtenerListadoPaginado(long funcionarioId, DateTime fi, DateTime ff, string draw, string start,
            string length, string sortColumn, string sortColumnDir, string searchValue, int pageSize, int skip, out int recordsTotal)
        {
            List<ECalifiacionColaboradorDataTable> listado = new List<ECalifiacionColaboradorDataTable>();

            using (var esquema = GetEsquema())
            {
                IQueryable<ECalifiacionColaboradorDataTable> query = null;

                if (funcionarioId == -1)
                {
                    query = (from c in esquema.CalificacionColaborador
                             join f in esquema.Funcionario on c.nFuncionarioId equals f.nFuncionarioId
                             join e in esquema.Evento on c.nEventoId equals e.nEventoId
                             where c.nCalificado == 1 &&
                            e.dFechaCreacion >= fi && e.dFechaCreacion <= ff
                             select new ECalifiacionColaboradorDataTable
                             {
                                 EventoId = e.nEventoId,
                                 Comentario = c.sComentario,
                                 Funcionario = f.sNombre +" "+ f.sApellido,
                                 Calificacion = c.nPuntuacion
                             });
                }
                else 
                {
                    query = (from c in esquema.CalificacionColaborador
                             join f in esquema.Funcionario on c.nFuncionarioId equals f.nFuncionarioId
                             join e in esquema.Evento on c.nEventoId equals e.nEventoId
                             where c.nCalificado == 1 && f.nFuncionarioId==funcionarioId &&
                            e.dFechaCreacion >= fi && e.dFechaCreacion <= ff
                             select new ECalifiacionColaboradorDataTable
                             {
                                 EventoId = e.nEventoId,
                                 Comentario = c.sComentario,
                                 Funcionario = f.sNombre + " " + f.sApellido,
                                 Calificacion = c.nPuntuacion
                             });
                }

                //Sorting    
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        default:
                            query = sortColumnDir == "asc" ? query.OrderBy(s => s.EventoId) : 
                                query.OrderByDescending(s => s.EventoId);
                            break;
                    }

                }

                recordsTotal = query.Count();

                listado = query.Skip(skip).Take(pageSize).ToList();
            }
            return listado;
        }

        public ECalificacionColaborador Obtener(long id)
        {
            using (var esquema = GetEsquema())
            {
                CalificacionColaborador entidad = ObtenerCalificacion(esquema, id);
                if (entidad == null)
                {
                    throw new BussinessException("No se puede encontrar el Area seleccionada.");
                }
                var resultado = Convert(entidad);
              
                return resultado;
            }
        }

        private CalificacionColaborador ObtenerCalificacion(dbFexpoCruzEntities esquema, long? CalificacionColaboradorId)
        {
            CalificacionColaborador entidad = (from x in esquema.CalificacionColaborador
                              where x.nCalificacionColaboradorId == CalificacionColaboradorId 
                              select x).FirstOrDefault();
            return entidad;
        }
        public List<ECalificacionColaborador> ObtenerLista(string estado)
        {
            using (var esquema = GetEsquema())
            {
                // todos
                var entidades = (from u in esquema.CalificacionColaborador
                                 select u);
                
                var resultado = Convert(entidades.ToList());
               
                return resultado;
            }
        }

        public CalificacionColaborador Nuevo(CalificacionColaborador Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    Entidad.dFechaCreacion = EUtils.Now;
                    Entidad.dFechaModificacion = EUtils.Now;
                    
                    esquema.CalificacionColaborador.Add(Entidad);
                    esquema.SaveChanges();

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


        public CalificacionColaborador Modificar(CalificacionColaborador Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    CalificacionColaborador EntidadLocal = ObtenerCalificacion(esquema, Entidad.nCalificacionColaboradorId);

                    EntidadLocal.sComentario = Entidad.sComentario;
                    EntidadLocal.nPuntuacion = Entidad.nPuntuacion;
                    EntidadLocal.nFuncionarioId = Entidad.nFuncionarioId;
                    EntidadLocal.nClienteId = Entidad.nClienteId;
                    EntidadLocal.nEventoId = Entidad.nEventoId;
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

        public string EntidadDetalleBitacora(CalificacionColaborador Entidad)
        {
            return Entidad.nCalificacionColaboradorId + "|" + Entidad.sComentario + "|" + Entidad.nPuntuacion;
        }

        public void Validar(CalificacionColaborador Entidad)
        {
           
        }


        private void ValidarCambioEstado(Area Entidad, Estado estado, dbFexpoCruzEntities esquema)
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

        private string DeshabilitarValidar(Area Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string HabilitarValidar(Area Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string EliminarValidar(Area Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }






        #region api
        public ResCalificacionColaborador ConvertResponse(CalificacionColaborador CP)
        {
            return new ResCalificacionColaborador(CP);
        }
        public List<ResCalificacionColaborador> ConvertResponse(List<CalificacionColaborador> L)
        {
            List<ResCalificacionColaborador> LC = new List<ResCalificacionColaborador>();
            L.ForEach(x => LC.Add(ConvertResponse(x)));
            return LC;
        }
        public void registrarCalificacionColaborador(ReqCalificacionColaborador form)
        {
            using (var esquema = GetEsquema())
            {
                var entidad = (from c in esquema.CalificacionColaborador
                               where c.nEventoId == form.EventoId
                               select c).FirstOrDefault();
                if (entidad == null)
                {
                    throw new BussinessException("Evento no encontrado.");
                }

                var colaborador = (from eh in esquema.EventoHilo
                                   join h in esquema.Hilo on eh.nHiloId equals h.nHiloId
                                   join f in esquema.Funcionario on h.nFuncionarioId equals f.nFuncionarioId
                                   where eh.nEventoId == form.EventoId
                                   orderby eh.nEventoHiloId descending
                                   select f).FirstOrDefault();

                entidad.nPuntuacion = form.Puntuacion;
                entidad.sComentario = form.Comentario!=null? form.Comentario:"";
                entidad.nCalificado = 1;


                var calificaciones = (from c in esquema.CalificacionColaborador
                                      where c.nFuncionarioId == colaborador.nFuncionarioId
                                      select c).ToList();
                int puntajeTotal = 0;
                foreach (var item in calificaciones)
                {
                    puntajeTotal = item.nPuntuacion + puntajeTotal;
                }
                colaborador.nPuntaje = calificaciones.Count == 0 ? 0 : puntajeTotal / calificaciones.Count;

                esquema.SaveChanges();

            }
        }

        public List<ResCalificacionColaborador> listarCalificacionColaborador(ReqListarCalificaciones form)
        {
            using (var esquema = GetEsquema())
            {

                var cliente = (from c in esquema.Cliente
                               where c.sCodigoCliente == form.CodigoCliente
                               select c).FirstOrDefault();
                if (cliente == null)
                {
                    return new List<ResCalificacionColaborador>();
                }
                List<CalificacionColaborador> entidad = (from c in esquema.CalificacionColaborador
                               where c.nClienteId == cliente.nClienteId && c.nCalificado==0
                               select c).ToList();
                return ConvertResponse(entidad);

            }
        }



        #endregion


    }
}
