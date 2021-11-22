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
    public class LCategoriaPreguntaFrecuente : LBaseCC<CategoriaPreguntaFrecuente>

    {
        public ECategoriaPreguntaFrecuente Convert(CategoriaPreguntaFrecuente CP)
        {
            return new ECategoriaPreguntaFrecuente(CP);
        }
        public List<ECategoriaPreguntaFrecuente> Convert(List<CategoriaPreguntaFrecuente> L)
        {
            List<ECategoriaPreguntaFrecuente> LC = new List<ECategoriaPreguntaFrecuente>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<ECategoriaPreguntaFrecuente> Obtener()
        {
            var Entidad = new List<ECategoriaPreguntaFrecuente>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.CategoriaPreguntaFrecuente
                                 where x.nEstado != (int)Estado.Eliminado
                                 select x).ToList();
                Entidad = Convert(resultado);
                return Entidad;
            }
        }
        public ECategoriaPreguntaFrecuente ObtenerCategoria(long nCategoriaPreguntaFrecuenteId)
        {
            using (var esquema = GetEsquema())
            {
                CategoriaPreguntaFrecuente entidad = (from x in esquema.CategoriaPreguntaFrecuente
                                                      where x.nCategoriaPreguntaFrecuenteId == nCategoriaPreguntaFrecuenteId
                                                      select x).FirstOrDefault();

                return Convert(entidad);
            }
            
        }

        private CategoriaPreguntaFrecuente ObtenerCategoria(dbFexpoCruzEntities esquema, long? nCategoriaPreguntaFrecuenteId)
        {
            CategoriaPreguntaFrecuente entidad = (from x in esquema.CategoriaPreguntaFrecuente
                                                  where x.nCategoriaPreguntaFrecuenteId == nCategoriaPreguntaFrecuenteId
                                                  select x).FirstOrDefault();
            return entidad;
        }
        public List<ECategoriaPreguntaFrecuente> ObtenerLista(string estado)
        {
            using (var esquema = GetEsquema())
            {
                // todos
                var entidades = (from u in esquema.CategoriaPreguntaFrecuente
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


        public CategoriaPreguntaFrecuente Nuevo(CategoriaPreguntaFrecuente Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    Entidad.dFechaCreacion = EUtils.Now;
                    Entidad.dFechaModificacion = EUtils.Now;
                    Entidad.nUsuarioCreador = UsuarioId;
                    Entidad.nUsuarioModificador = UsuarioId;
                    esquema.CategoriaPreguntaFrecuente.Add(Entidad);
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



        public CategoriaPreguntaFrecuente Modificar(CategoriaPreguntaFrecuente Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    CategoriaPreguntaFrecuente EntidadLocal = ObtenerCategoria(esquema, Entidad.nCategoriaPreguntaFrecuenteId);

                    EntidadLocal.sNombre = Entidad.sNombre;
                    EntidadLocal.nEstado = Entidad.nEstado;
                    EntidadLocal.dFechaModificacion = EUtils.Now;
                    EntidadLocal.nUsuarioCreador = UsuarioId;
                    EntidadLocal.nUsuarioModificador = UsuarioId;
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




        public void CambiarEstado(long id, Estado estado, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                CategoriaPreguntaFrecuente EntidadEx = ObtenerCategoria(esquema, id);
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
                    Exception Ex = new Exception("No existe PreguntaFrecuente con id " + id);
                    RegistrarErrorEnBitacora(esquema, UsuarioId, id, AccionBitacora.Obteniendo, "PreguntaFrecuente cambio de estado", Ex);
                    throw Ex;
                }
            }


        }
        public string EntidadDetalleBitacora(CategoriaPreguntaFrecuente Entidad)
        {
            return Entidad.nCategoriaPreguntaFrecuenteId + "|" + Entidad.sNombre ;
        }

        public void Validar(CategoriaPreguntaFrecuente Entidad)
        {
            if (string.IsNullOrEmpty(Entidad.sNombre.ToString()))
            {
                throw new BussinessException("la pregunta es obligatoria.");
            }
            if (string.IsNullOrEmpty(Entidad.nCategoriaPreguntaFrecuenteId.ToString()))
            {
                throw new BussinessException("La categoria de pregunta es obligatorio.");
            }
        }


        private void ValidarCambioEstado(CategoriaPreguntaFrecuente Entidad, Estado estado, dbFexpoCruzEntities esquema)
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

        private string DeshabilitarValidar(CategoriaPreguntaFrecuente Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string HabilitarValidar(CategoriaPreguntaFrecuente Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string EliminarValidar(CategoriaPreguntaFrecuente Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }



    }
}
