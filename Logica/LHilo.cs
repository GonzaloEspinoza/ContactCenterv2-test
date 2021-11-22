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
    public class LHilo : LBaseCC<Hilo>
    {


        public EHilo Convert(Hilo CP)
        {
            return new EHilo(CP);
        }
        public List<EHilo> Convert(List<Hilo> L)
        {
            List<EHilo> LC = new List<EHilo>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<EHilo> Obtener()
        {
            var Entidad = new List<EHilo>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.Hilo
                                 select x).ToList();
                Entidad = Convert(resultado);
                return Entidad;
            }
        }

        public int porCiudad(int id) {

           
            using (var esquema = GetEsquema())
            {

                int resultado =
                  (from a in esquema.Hilo
                   join b in esquema.Funcionario on a.nFuncionarioId equals b.nFuncionarioId
                   join c in esquema.Ciudad on b.nCiudadId equals c.nCiudadId
                   join d in esquema.EventoHilo on a.nHiloId equals d.nHiloId

                   where c.nCiudadId == id
                   select a).Count();
              
                
                return resultado;
            }
        }

        public int porFuncionario(int id)
        {


            using (var esquema = GetEsquema())
            {

                int resultado =
                  (from a in esquema.Hilo
                   where a.nFuncionarioId == id
                   select a).Count();


                return resultado;
            }
        }
        public String ListaCiudad()
        {

            
            using (var esquema = GetEsquema())
            {

                
                string strDatos;
                strDatos = "[['Ciudades','Tickets'],";
                var resultado = (from x in esquema.Ciudad
                                 select x).ToList();
                foreach (var item  in resultado){
                    int cantidad = porCiudad(unchecked((int)item.nCiudadId));
                    string nombre = item.sNombre;
                    strDatos = strDatos + "[";
                    strDatos = strDatos + "'" + nombre + "'" + "," + cantidad;
                    strDatos = strDatos + "],";
                }
                strDatos = strDatos.Remove(strDatos.Length - 1);
                strDatos = strDatos + "]";
                char oldChar='\'';
                char newChar = '"';
                string strNew= strDatos.Replace(oldChar, newChar);

                return strNew;
            }
        }

        public String ListaTiempos()
        {


            
                string strDatos;
                strDatos = "[['Tiempo','Tickets'],";
                string[] tiempo= { "Hoy día","Este mes","Este año" };
        
                int i = 1;
                while (i<=3)
                {
                    int cantidad = porTiempo(i);
                    string nombre = tiempo[i-1];
                    strDatos = strDatos + "[";
                    strDatos = strDatos + "'" + nombre + "'" + "," + cantidad;
                    strDatos = strDatos + "],";
                i++;
                }
                strDatos = strDatos.Remove(strDatos.Length - 1);
                strDatos = strDatos + "]";
                char oldChar = '\'';
                char newChar = '"';
                string strNew = strDatos.Replace(oldChar, newChar);

                return strNew;
            
        }


        public int porTiempo(int id)
        {
            int resultado=0;
            DateTime fecha = System.DateTime.Today;
            using (var esquema = GetEsquema())
            {
                switch (id)
                {
                    case 1:
                       
                        resultado =
                (from a in esquema.EventoHilo

                 where a.dFechaCreacion.Day == fecha.Day
                 select a).Count();
                  
                        return resultado;
                    case 2:
                        resultado =
              (from a in esquema.EventoHilo

               where a.dFechaCreacion.Month == fecha.Month
               select a).Count();
                        return resultado;
                      
                    case 3:
                        resultado =
              (from a in esquema.EventoHilo

               where a.dFechaCreacion.Year == fecha.Year
               select a).Count();
                        return resultado;
                    default:
                        return resultado;

                }
              


               
            }
        }
        public float Promedio()
        {
            using (var esquema = GetEsquema())
            {
     
                int resultado = esquema.Database.SqlQuery<int>("SELECT SUM(DATEDIFF(MINUTE, Hilo.dFechaCreacion, EventoHilo.dFechaCreacion)) from EventoHilo, Hilo where  EventoHilo.nHiloId = Hilo.nHiloId").FirstOrDefault();
                int cantidad = esquema.Database.SqlQuery<int>("SELECT COUNT(*) from EventoHilo, Hilo where  EventoHilo.nHiloId = Hilo.nHiloId").FirstOrDefault();
                float promedio =(float) resultado / cantidad;
                return promedio;
            }           
        }
        public int Total()
        {
            using (var esquema = GetEsquema())
            {

               
                int cantidad = esquema.Database.SqlQuery<int>("SELECT COUNT(*) from EventoHilo").FirstOrDefault();
               
                return cantidad;
            }
        }
        public EHilo Obtener(long id)
        {
            using (var esquema = GetEsquema())
            {
                Hilo entidad = ObtenerHilo(esquema, id);
                if (entidad == null)
                {
                    throw new BussinessException("No se puede encontrar la Hilo seleccionada.");
                }
                var resultado = Convert(entidad);
                return resultado;
            }
        }

        private Hilo ObtenerHilo(dbFexpoCruzEntities esquema, long? HiloId)
        {
            Hilo entidad = (from x in esquema.Hilo
                                 where x.nHiloId == HiloId 
                                 select x).FirstOrDefault();
            return entidad;
        }

      
        public List<EHilo> ObtenerLista()
        {
            using (var esquema = GetEsquema())
            {
                var entidades = (from u in esquema.Hilo
                                 where  u.nEstado == (int)Estado.Habilitado
                                 orderby u.dFechaCreacion descending
                                 select u).ToList();
                var resultado = Convert(entidades);
                return resultado;
            }
        }

        public List<EHilo> ObtenerListaPorColaborador(long funcionarioId)
        {
            using (var esquema = GetEsquema())
            {
                var entidades = (from u in esquema.Hilo
                                 where (u.nFuncionarioId == funcionarioId || u.nFuncionarioId==null) && u.nEstado==(int)Estado.Habilitado
                                 orderby u.dFechaCreacion descending
                                 select u).ToList();
                var resultado = Convert(entidades);
                return resultado;
            }
        }

        public List<EHilo> ObtenerListaPorColaboradorBusqueda(long funcionarioId,string busqueda)
        {
            using (var esquema = GetEsquema())
            {
                var entidades = (from u in esquema.Hilo
                                 join eh in esquema.EventoHilo on u.nHiloId equals eh.nHiloId
                                 join e in esquema.Evento on eh.nEventoId equals e.nEventoId
                                 where u.nFuncionarioId == funcionarioId && u.nEstado==(int)Estado.Habilitado
                                 && (e.Cliente.sNombres.Contains(busqueda) || e.Cliente.sApellidos.Contains(busqueda))
                                 orderby u.dFechaCreacion descending
                                 select u).ToList();
                var resultado = Convert(entidades);
                return resultado;
            }
        }

        public List<EHilo> ObtenerListaBusqueda(string busqueda)
        {
            using (var esquema = GetEsquema())
            {
                var entidades = (from u in esquema.Hilo
                                 join eh in esquema.EventoHilo on u.nHiloId equals eh.nHiloId
                                 join e in esquema.Evento on eh.nEventoId equals e.nEventoId
                                 where u.nEstado == (int)Estado.Habilitado
                                 && (e.Cliente.sNombres.Contains(busqueda) || e.Cliente.sApellidos.Contains(busqueda))
                                 orderby u.dFechaCreacion descending
                                 select u).ToList();
                var resultado = Convert(entidades);
                return resultado;
            }
        }

        public EHilo ObtenerHiloPorEventoId(long eventoId)
        {
            using (var esquema = GetEsquema())
            {
                var entidad = (from u in esquema.Hilo
                                 join eh in esquema.EventoHilo on u.nHiloId equals eh.nHiloId
                                 join e in esquema.Evento on eh.nEventoId equals e.nEventoId
                                 where e.nEventoId == eventoId
                                 orderby u.dFechaCreacion descending
                                 select u).FirstOrDefault();
                if (entidad==null)
                {
                    return null;
                }
                var resultado = Convert(entidad);
                return resultado;
            }
        }


        public int ObtenerNumeroPorColaborador(long funcionarioId, int skip, int take)
        {
            using (var esquema = GetEsquema())
            {
                var entidades = (from u in esquema.Hilo
                                 where u.nFuncionarioId == funcionarioId
                                 orderby u.dFechaCreacion descending
                                 select u).ToList();
              
                return entidades.Count;
            }

        }
        public Hilo Nuevo(Hilo Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    Entidad.dFechaCreacion = EUtils.Now;
                    Entidad.dFechaModificacion = EUtils.Now;
                    esquema.Hilo.Add(Entidad);
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


        public Hilo Modificar(Hilo Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    Hilo EntidadLocal = ObtenerHilo(esquema, Entidad.nHiloId);

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
                Hilo EntidadEx = ObtenerHilo(esquema, id);
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
                    Exception Ex = new Exception("No existe Hilo con id " + id);
                    RegistrarErrorEnBitacora(esquema, UsuarioId, id, AccionBitacora.Obteniendo, "Hilo cambio de estado", Ex);
                    throw Ex;
                }
            }

        }
        public string EntidadDetalleBitacora(Hilo Entidad)
        {
            return Entidad.nHiloId + "|" + Entidad.nHiloId + "|" + Entidad.nFuncionarioId;
        }

        public void Validar(Hilo Entidad)
        {
            if (string.IsNullOrEmpty(Entidad.nFuncionarioId.ToString()))
            {
                throw new BussinessException("El colaborador es obligatorio.");
            }
           
        }


        private void ValidarCambioEstado(Hilo Entidad, Estado estado, dbFexpoCruzEntities esquema)
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

        private string DeshabilitarValidar(Hilo Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string HabilitarValidar(Hilo Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string EliminarValidar(Hilo Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }



        #region api
        public ResHilo ConvertResponse(Hilo CP)
        {
            return new ResHilo(CP);
        }
        public List<ResHilo> ConvertResponse(List<Hilo> L)
        {
            List<ResHilo> LC = new List<ResHilo>();
            L.ForEach(x => LC.Add(ConvertResponse(x)));
            return LC;
        }
        public List<ResHilo> ListarPorCliente(string CodigoCliente)
        {

            using (var esquema = GetEsquema())
            {
                try
                {
                    Cliente clienteExistente = (from x in esquema.Cliente
                                                where x.sCodigoCliente == CodigoCliente
                                                select x).FirstOrDefault();
                    if (clienteExistente == null)
                    {
                        return new List<ResHilo>();
                    }
                    List<Hilo> hilos = (from x in esquema.Hilo
                                        join eh in esquema.EventoHilo on x.nHiloId equals eh.nHiloId
                                        join e in esquema.Evento on eh.nEventoId equals e.nEventoId
                                        join c in esquema.Cliente on e.nClienteId equals c.nClienteId
                                        where c.sCodigoCliente == CodigoCliente && x.nActual == (int)Estado.Habilitado
                                        orderby x.dFechaCreacion descending
                                            select x).ToList();
                    return ConvertResponse(hilos);
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

        public List<ResHilo> ListarPorClienteNoAtendido(string CodigoCliente)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Cliente clienteExistente = (from x in esquema.Cliente
                                                where x.sCodigoCliente == CodigoCliente
                                                select x).FirstOrDefault();
                    if (clienteExistente == null)
                    {
                        return new List<ResHilo>();
                    }
                    List<Hilo> hilos = (from x in esquema.Hilo
                                        join eh in esquema.EventoHilo on x.nHiloId equals eh.nHiloId
                                        join e in esquema.Evento on eh.nEventoId equals e.nEventoId
                                        join c in esquema.Cliente on e.nClienteId equals c.nClienteId
                                        where c.sCodigoCliente == CodigoCliente 
                                        && (e.nEstadoEventoId == (int)EstadoDeEvento.Asignado 
                                        || e.nEstadoEventoId == (int)EstadoDeEvento.Reasignado) && x.nActual==(int)Estado.Habilitado
                                        orderby x.dFechaCreacion descending
                                        select x).ToList();
                    return ConvertResponse(hilos);
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
        public ResHilo ObtenerPorEventoHiloId(long eventoHiloId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                  Hilo hilo = (from x in esquema.Hilo
                                        join eh in esquema.EventoHilo on x.nHiloId equals eh.nHiloId
                                        join e in esquema.Evento on eh.nEventoId equals e.nEventoId
                                        join c in esquema.Cliente on e.nClienteId equals c.nClienteId
                                        where eh.nEventoHiloId==eventoHiloId
                                        orderby x.dFechaCreacion descending
                                        select x).FirstOrDefault();
                    return ConvertResponse(hilo);
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
