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
    public class LArea : LBaseCC<Area>
    {


        public EArea Convert(Area CP)
        {
            return new EArea(CP);
        }
        public List<EArea> Convert(List<Area> L)
        {
            List<EArea> LC = new List<EArea>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<EArea> Obtener()
        {
            var Entidad = new List<EArea>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.Area
                                 where x.nEstado != (int)Estado.Eliminado
                                 select x).ToList();
                foreach (var item in resultado)
                {
                    Entidad.Add(new EArea()
                    {
                        AreaId= item.nAreaId,
                        Nombre = item.sNombre,
                        Estado = item.nEstado,
                        Empresa=item.Empresa
                    });
                }
                return Entidad;
            }
        }


        public EArea Obtener(long id)
        {
            using (var esquema = GetEsquema())
            {
                Area entidad = ObtenerArea(esquema, id);
                if (entidad == null)
                {
                    throw new BussinessException("No se puede encontrar el Area seleccionada.");
                }
                var resultado = Convert(entidad);
              
                return resultado;
            }
        }

        private Area ObtenerArea(dbFexpoCruzEntities esquema, long? AreaId)
        {
            Area entidad = (from x in esquema.Area
                              where x.nAreaId == AreaId && x.nEstado != (int)Estado.Eliminado
                              select x).FirstOrDefault();
            return entidad;
        }
        public List<EArea> ObtenerLista(string estado)
        {
            using (var esquema = GetEsquema())
            {
                // todos
                var entidades = (from u in esquema.Area
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

        public Area Nuevo(Area Entidad, long UsuarioId)
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
                    
                    esquema.Area.Add(Entidad);
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


        public Area Modificar(Area Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    Area EntidadLocal = ObtenerArea(esquema, Entidad.nAreaId);

                    EntidadLocal.sNombre = Entidad.sNombre;
                    EntidadLocal.nEmpresaId = Entidad.nEmpresaId;
                    EntidadLocal.nEstado = Entidad.nEstado;
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

        public string EntidadDetalleBitacora(Area Entidad)
        {
            return Entidad.nAreaId + "|" + Entidad.sNombre + "|" + Entidad.nEstado;
        }

        public void Validar(Area Entidad)
        {
            if (string.IsNullOrEmpty(Entidad.sNombre))
            {
                throw new BussinessException("El nombre es obligatorio.");
            }
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

        public string NombrEArea(long? id)
        {
            using (var esquema = GetEsquema())
            {
                Area entidad = ObtenerArea(esquema, id);
                if (entidad == null)
                {
                    return "";
                }
                var resultado = Convert(entidad);
                return resultado.Nombre;
            }
        }



        public List<EArea> Areaes()
        {
            try
            {
                var Entidad = new List<EArea>();
                using (var esquema = GetEsquema())
                {
                    var resultado = (from x in esquema.Area
                                     where x.nEstado != (int)Estado.Eliminado
                                     select x).ToList();
                    foreach (var item in resultado)
                    {
                        Entidad.Add(new EArea()
                        {
                            AreaId = item.nAreaId,
                            Nombre = item.sNombre
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
                Area EntidadEx = ObtenerArea(esquema, id);
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
                    Exception Ex = new Exception("No existe la area con id " + id);
                    RegistrarErrorEnBitacora(esquema, UsuarioId, id, AccionBitacora.Obteniendo, "Categoria Servicios cambio de estado", Ex);
                    throw Ex;
                }
            }
        }




      
        #region api
        public ResArea ConvertResponse(Area CP)
        {
            return new ResArea(CP);
        }
        public List<ResArea> ConvertResponse(List<Area> L)
        {
            List<ResArea> LC = new List<ResArea>();
            L.ForEach(x => LC.Add(ConvertResponse(x)));
            return LC;
        }
        public List<ResArea> ObtenerListaPorIdEmpresa(Estado estado,long empresaId)
        {
            using (var esquema = GetEsquema())
            {
                var entidades = (from u in esquema.Area
                                 where u.nEstado == (int)estado && u.nEmpresaId==empresaId
                                 select u);
                var resultado = ConvertResponse(entidades.ToList());
                return resultado;
            }
        }
        #endregion


    }
}
