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
    public class LTipoAtencion : LBaseCC<TipoAtencion>
    {


        public ETipoAtencion Convert(TipoAtencion CP)
        {
            return new ETipoAtencion(CP);
        }
        public List<ETipoAtencion> Convert(List<TipoAtencion> L)
        {
            List<ETipoAtencion> LC = new List<ETipoAtencion>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<ETipoAtencion> Obtener()
        {
            var Entidad = new List<ETipoAtencion>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.TipoAtencion
                                 where x.nEstado != (int)Estado.Eliminado
                                 select x).ToList();
                foreach (var item in resultado)
                {
                    Entidad.Add(new ETipoAtencion()
                    {
                        TipoAtencionID= item.nTipoAtencionId,
                        Nombre = item.sNombre,
                        Estado = item.nEstado,
                        Area=item.Area,
                        Color=item.sColor
                    });
                }
                return Entidad;
            }
        }

        public ETipoAtencion Obtener(long id)
        {
            using (var esquema = GetEsquema())
            {
                TipoAtencion entidad = ObtenerTipoAtencion(esquema, id);
                if (entidad == null)
                {
                    throw new BussinessException("No se puede encontrar el TipoAtencion seleccionada.");
                }
                var resultado = Convert(entidad);
              
                return resultado;
            }
        }

        private TipoAtencion ObtenerTipoAtencion(dbFexpoCruzEntities esquema, long? TipoAtencionId)
        {
            TipoAtencion entidad = (from x in esquema.TipoAtencion
                              where x.nTipoAtencionId == TipoAtencionId && x.nEstado != (int)Estado.Eliminado
                              select x).FirstOrDefault();
            return entidad;
        }
        public List<ETipoAtencion> ObtenerLista(string estado)
        {
            using (var esquema = GetEsquema())
            {
                // todos
                var entidades = (from u in esquema.TipoAtencion
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

        public TipoAtencion Nuevo(TipoAtencion Entidad, long UsuarioId)
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
                 
                    esquema.TipoAtencion.Add(Entidad);
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


        public TipoAtencion Modificar(TipoAtencion Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    TipoAtencion EntidadLocal = ObtenerTipoAtencion(esquema, Entidad.nTipoAtencionId);

                    EntidadLocal.sNombre = Entidad.sNombre;
                    EntidadLocal.nAreaId = Entidad.nAreaId;
                    EntidadLocal.nEstado = Entidad.nEstado;
                    EntidadLocal.sColor = Entidad.sColor;
                    EntidadLocal.dFechaModificacion = EUtils.Now;
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

        public string EntidadDetalleBitacora(TipoAtencion Entidad)
        {
            return Entidad.nTipoAtencionId + "|" + Entidad.sNombre + "|" + Entidad.nEstado;
        }

        public void Validar(TipoAtencion Entidad)
        {
            if (string.IsNullOrEmpty(Entidad.sNombre))
            {
                throw new BussinessException("El nombre es obligatorio.");
            }
        }


        private void ValidarCambioEstado(TipoAtencion Entidad, Estado estado, dbFexpoCruzEntities esquema)
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

        private string DeshabilitarValidar(TipoAtencion Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string HabilitarValidar(TipoAtencion Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string EliminarValidar(TipoAtencion Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }

        public string NombrETipoAtencion(long? id)
        {
            using (var esquema = GetEsquema())
            {
                TipoAtencion entidad = ObtenerTipoAtencion(esquema, id);
                if (entidad == null)
                {
                    return "";
                }
                var resultado = Convert(entidad);
                return resultado.Nombre;
            }
        }
       
        public void CambiarEstado(long id, Estado estado, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                TipoAtencion EntidadEx = ObtenerTipoAtencion(esquema, id);
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
                    Exception Ex = new Exception("No existe el Tipo de Atencion con id " + id);
                    RegistrarErrorEnBitacora(esquema, UsuarioId, id, AccionBitacora.Obteniendo, "Categoria Servicios cambio de estado", Ex);
                    throw Ex;
                }
            }
        }



        #region api
        public ResTipoAtencion ConvertResponse(TipoAtencion CP)
        {
            return new ResTipoAtencion(CP);
        }
        public List<ResTipoAtencion> ConvertResponse(List<TipoAtencion> L)
        {
            List<ResTipoAtencion> LC = new List<ResTipoAtencion>();
            L.ForEach(x => LC.Add(ConvertResponse(x)));
            return LC;
        }
        public List<ResTipoAtencion> ObtenerListaPorAreaId(Estado estado, long areaId)
        {
            using (var esquema = GetEsquema())
            {
                var entidades = (from u in esquema.TipoAtencion
                                 where u.nEstado == (int)estado && u.nAreaId== areaId
                                 select u);
                var resultado = ConvertResponse(entidades.ToList());
                return resultado;
            }
        }
        #endregion

    }
}
