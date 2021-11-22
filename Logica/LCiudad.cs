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
    public class LCiudad : LBaseCC<Ciudad>
    {


        public ECiudad Convert(Ciudad CP)
        {
            return new ECiudad(CP);
        }
        public List<ECiudad> Convert(List<Ciudad> L)
        {
            List<ECiudad> LC = new List<ECiudad>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<ECiudad> Obtener()
        {
            var Entidad = new List<ECiudad>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.Ciudad
                                 where x.nEstado != (int)Estado.Eliminado
                                 select x).ToList();
                Entidad = Convert(resultado);
                foreach (var item in Entidad)
                {
                    item.UrlBandera = EUtils.URLImagen(item.Bandera); 
                    item.Base64Bandera = EUtils.ObtenerImagenBase64(item.Bandera);
                }
                return Entidad;
            }
        }


        public ECiudad Obtener(long id)
        {
            using (var esquema = GetEsquema())
            {
                Ciudad entidad = ObtenerCiudad(esquema, id);
                if (entidad == null)
                {
                    throw new BussinessException("No se puede encontrar la ciudad seleccionada.");
                }
                var resultado = Convert(entidad);
                resultado.UrlBandera = EUtils.URLImagen(resultado.Bandera); ;
                resultado.Base64Bandera = EUtils.ObtenerImagenBase64(resultado.Bandera);
                return resultado;
            }
        }

        private Ciudad ObtenerCiudad(dbFexpoCruzEntities esquema, long? CiudadId)
        {
            Ciudad entidad = (from x in esquema.Ciudad
                                 where x.nCiudadId == CiudadId && x.nEstado != (int)Estado.Eliminado
                                 select x).FirstOrDefault();
            return entidad;
        }
        public List<ECiudad> ObtenerLista(string estado)
        {
            using (var esquema = GetEsquema())
            {
                // todos
                var entidades = (from u in esquema.Ciudad
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
                foreach (var item in resultado)
                {
                    item.UrlBandera = EUtils.URLImagen(item.Bandera);
                    item.Base64Bandera = EUtils.ObtenerImagenBase64(item.Bandera);
                }
                return resultado;
            }
        }

        public Ciudad Nuevo(Ciudad Entidad, long UsuarioId)
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
                    if (!string.IsNullOrWhiteSpace(Entidad.sBandera))
                    {
                        
                        Entidad.sBandera = EUtils.InsertarImagen("Ciudades", Entidad.sBandera);
                    }
                    esquema.Ciudad.Add(Entidad);
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


        public Ciudad Modificar(Ciudad Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    Ciudad EntidadLocal = ObtenerCiudad(esquema, Entidad.nCiudadId);

                    EntidadLocal.sNombre = Entidad.sNombre;
                    EntidadLocal.nEstado = Entidad.nEstado;
                    EntidadLocal.dFechaModificacion = EUtils.Now;
                    EntidadLocal.nUsuarioModificador = UsuarioId;
                    if (Entidad.nEstado != EntidadLocal.nEstado)
                    {
                        ValidarCambioEstado(EntidadLocal, (Estado)Entidad.nEstado, esquema);
                    }
                    EntidadLocal.nEstado = Entidad.nEstado;
                    if (!string.IsNullOrWhiteSpace(Entidad.sBandera))
                    {
                        EntidadLocal.sBandera = EUtils.InsertarImagen("Ciudades", Entidad.sBandera);
                    }
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
                Ciudad EntidadEx = ObtenerCiudad(esquema, id);
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
                    Exception Ex = new Exception("No existe ciudad con id " + id);
                    RegistrarErrorEnBitacora(esquema, UsuarioId, id, AccionBitacora.Obteniendo, "Ciudad cambio de estado", Ex);
                    throw Ex;
                }
            }

        }
        public string EntidadDetalleBitacora(Ciudad Entidad)
        {
            return Entidad.nCiudadId + "|" + Entidad.sNombre + "|" + Entidad.nEstado;
        }

        public void Validar(Ciudad Entidad)
        {
            if (string.IsNullOrEmpty(Entidad.sNombre))
            {
                throw new BussinessException("El nombre es obligatorio.");
            }
            if (string.IsNullOrEmpty(Entidad.nPaisId.ToString()))
            {
                throw new BussinessException("El pais es obligatorio.");
            }
        }


        private void ValidarCambioEstado(Ciudad Entidad, Estado estado, dbFexpoCruzEntities esquema)
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

        private string DeshabilitarValidar(Ciudad Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string HabilitarValidar(Ciudad Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string EliminarValidar(Ciudad Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }

        public string NombreCiudad(long? id)
        {
            using (var esquema = GetEsquema())
            {
                Ciudad entidad = ObtenerCiudad(esquema, id);
                if (entidad == null)
                {
                    return "";
                }
                var resultado = Convert(entidad);
                return resultado.Nombre;
            }
        }


        #region api
        public ResCiudad ConvertResponse(Ciudad CP)
        {
            return new ResCiudad(CP);
        }
        public List<ResCiudad> ConvertResponse(List<Ciudad> L)
        {
            List<ResCiudad> LC = new List<ResCiudad>();
            L.ForEach(x => LC.Add(ConvertResponse(x)));
            return LC;
        }
        public List<ResCiudad> ObtenerCiudadesDisponiblesPorEmpresa(long idEmpresa)
        {
            using (var esquema = GetEsquema())
            {
                List<Ciudad> resultado = (from x in esquema.Ciudad
                                join f in esquema.Funcionario on x.nCiudadId equals f.nCiudadId
                                where f.Area.nEmpresaId == idEmpresa
                                 select x).Distinct().ToList();

                return ConvertResponse(resultado);
            }
        }

      
        #endregion





    }
}
