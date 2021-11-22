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
    public class LPreguntaFrecuente : LBaseCC<PreguntaFrecuente>
    {


        public EPreguntaFrecuente Convert(PreguntaFrecuente CP)
        {
            return new EPreguntaFrecuente(CP);
        }
        public List<EPreguntaFrecuente> Convert(List<PreguntaFrecuente> L)
        {
            List<EPreguntaFrecuente> LC = new List<EPreguntaFrecuente>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<EPreguntaFrecuente> Obtener()
        {
            var Entidad = new List<EPreguntaFrecuente>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.PreguntaFrecuente
                                 select x).ToList();
                Entidad = Convert(resultado);
                return Entidad;
            }
        }

      




        public EPreguntaFrecuente Obtener(long id)
        {
            using (var esquema = GetEsquema())
            {
                PreguntaFrecuente entidad = ObtenerPreguntaFrecuente(esquema, id);
                if (entidad == null)
                {
                    throw new BussinessException("No se puede encontrar la PreguntaFrecuente seleccionada.");
                }
                var resultado = Convert(entidad);
                return resultado;
            }
        }

        private PreguntaFrecuente ObtenerPreguntaFrecuente(dbFexpoCruzEntities esquema, long? PreguntaFrecuenteId)
        {
            PreguntaFrecuente entidad = (from x in esquema.PreguntaFrecuente
                                 where x.nPreguntaFrecuenteId == PreguntaFrecuenteId 
                                 select x).FirstOrDefault();
            return entidad;
        }
        public List<EPreguntaFrecuente> ObtenerLista(long categoria,string estado)
        {
            using (var esquema = GetEsquema())
            {
                // todos
                var entidades = (from u in esquema.PreguntaFrecuente
                                 where u.nEstado != (int)Estado.Eliminado && u.nCategoriaPreguntaFrecuenteId== categoria
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

        

        public void CambiarEstado(long id, Estado estado, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                PreguntaFrecuente EntidadEx = ObtenerPreguntaFrecuente(esquema, id);
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
                    Exception Ex = new Exception("No existe PreguntaFrecuente con id " + id);
                    RegistrarErrorEnBitacora(esquema, UsuarioId, id, AccionBitacora.Obteniendo, "PreguntaFrecuente cambio de estado", Ex);
                    throw Ex;
                }
            }

        }
        public string EntidadDetalleBitacora(PreguntaFrecuente Entidad)
        {
            return Entidad.nPreguntaFrecuenteId + "|" + Entidad.nPreguntaFrecuenteId + "|" + Entidad.nTipoAtencionId;
        }

        public void Validar(PreguntaFrecuente Entidad)
        {
            if (string.IsNullOrEmpty(Entidad.sPregunta.ToString()))
            {
                throw new BussinessException("la pregunta es obligatoria.");
            }
            if (string.IsNullOrEmpty(Entidad.nCategoriaPreguntaFrecuenteId.ToString()))
            {
                throw new BussinessException("La categoria de pregunta es obligatorio.");
            }
        }


        private void ValidarCambioEstado(PreguntaFrecuente Entidad, Estado estado, dbFexpoCruzEntities esquema)
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

        private string DeshabilitarValidar(PreguntaFrecuente Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string HabilitarValidar(PreguntaFrecuente Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string EliminarValidar(PreguntaFrecuente Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }

        public PreguntaFrecuente Nuevo(PreguntaFrecuente Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    Entidad.dFechaCreacion = EUtils.Now;
                    Entidad.dFechaModificacion = EUtils.Now;
                  
                    esquema.PreguntaFrecuente.Add(Entidad);
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

        public PreguntaFrecuente Modificar(PreguntaFrecuente Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    PreguntaFrecuente EntidadLocal = ObtenerPreguntaFrecuente(esquema, Entidad.nPreguntaFrecuenteId);

                    EntidadLocal.sPregunta = Entidad.sPregunta;
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


       


        #region api

        public ResPreguntaFrecuente ConvertResponse(PreguntaFrecuente CP)
        {
            return new ResPreguntaFrecuente(CP);
        }
        public List<ResPreguntaFrecuente> ConvertResponse(List<PreguntaFrecuente> L)
        {
            List<ResPreguntaFrecuente> LC = new List<ResPreguntaFrecuente>();
            L.ForEach(x => LC.Add(ConvertResponse(x)));
            return LC;
        }

        public List<ResPreguntaFrecuente> ListarPorEmpresa(ReqListarPreguntaFrecuente parametro)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    var preguntasFrecuentes = (from x in esquema.PreguntaFrecuente
                                               join c in esquema.CategoriaPreguntaFrecuente on x.nCategoriaPreguntaFrecuenteId equals
                                               c.nCategoriaPreguntaFrecuenteId
                                               where x.TipoAtencion.Area.nEmpresaId == parametro.EmpresaId && x.nEstado != (int)Estado.Eliminado && c.nEstado != (int)Estado.Eliminado
                                               select x).ToList();
                    var preguntasHabilitadas = (from x in preguntasFrecuentes
                                                where x.nEstado == (int)Estado.Habilitado
                                               select x).ToList();
                   
                    return ConvertResponse(preguntasHabilitadas);
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
