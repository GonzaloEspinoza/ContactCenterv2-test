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
    public class LEmpresa : LBaseCC<Empresa>
    {


        public EEmpresa Convert(Empresa CP)
        {
            return new EEmpresa(CP);
        }
        public List<EEmpresa> Convert(List<Empresa> L)
        {
            List<EEmpresa> LC = new List<EEmpresa>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<EEmpresa> Obtener()
        {
            var Entidad = new List<EEmpresa>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.Empresa
                                 where x.nEstado != (int)Estado.Eliminado
                                 select x).ToList();
                Entidad = Convert(resultado);
                foreach (var item in Entidad)
                {
                    item.UrlLogo = EUtils.URLImagen(item.Logo); ;
                    item.Base64Logo = EUtils.ObtenerImagenBase64(item.Logo);
                }
                return Entidad;
            }
        }


        public EEmpresa Obtener(long id)
        {
            using (var esquema = GetEsquema())
            {
                Empresa entidad = ObtenerEmpresa(esquema, id);
                if (entidad == null)
                {
                    throw new BussinessException("No se puede encontrar la ciudad seleccionada.");
                }
                var resultado = Convert(entidad);
                resultado.UrlLogo = EUtils.URLImagen(resultado.Logo); ;
                resultado.Base64Logo = EUtils.ObtenerImagenBase64(resultado.Logo);
                return resultado;
            }
        }

        private Empresa ObtenerEmpresa(dbFexpoCruzEntities esquema, long? EmpresaId)
        {
            Empresa entidad = (from x in esquema.Empresa
                              where x.nEmpresaId == EmpresaId && x.nEstado != (int)Estado.Eliminado
                              select x).FirstOrDefault();
            return entidad;
        }
        public List<EEmpresa> ObtenerLista(string estado)
        {
            using (var esquema = GetEsquema())
            {
                // todos
                var entidades = (from u in esquema.Empresa
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

        public Empresa Nuevo(Empresa Entidad, long UsuarioId)
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
                    if (!string.IsNullOrWhiteSpace(Entidad.sLogo))
                    {

                        Entidad.sLogo = EUtils.InsertarImagen("Empresas", Entidad.sLogo);
                    }
                    esquema.Empresa.Add(Entidad);
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


        public Empresa Modificar(Empresa Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    Empresa EntidadLocal = ObtenerEmpresa(esquema, Entidad.nEmpresaId);

                    EntidadLocal.sNombre = Entidad.sNombre;
                    EntidadLocal.sUrl = Entidad.sUrl;
                    EntidadLocal.nEstado = Entidad.nEstado;
                    EntidadLocal.dFechaModificacion = EUtils.Now;
                    EntidadLocal.nUsuarioModificador = UsuarioId;
                    if (Entidad.nEstado != EntidadLocal.nEstado)
                    {
                        ValidarCambioEstado(EntidadLocal, (Estado)Entidad.nEstado, esquema);
                    }
                    if (!string.IsNullOrWhiteSpace(Entidad.sLogo))
                    {

                        EntidadLocal.sLogo = EUtils.InsertarImagen("Empresas", Entidad.sLogo);
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

        public string EntidadDetalleBitacora(Empresa Entidad)
        {
            return Entidad.nEmpresaId + "|" + Entidad.sNombre + "|" + Entidad.nEstado;
        }

        public void Validar(Empresa Entidad)
        {
            if (string.IsNullOrEmpty(Entidad.sNombre))
            {
                throw new BussinessException("El nombre es obligatorio.");
            }
        }


        private void ValidarCambioEstado(Empresa Entidad, Estado estado, dbFexpoCruzEntities esquema)
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

        private string DeshabilitarValidar(Empresa Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string HabilitarValidar(Empresa Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string EliminarValidar(Empresa Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }

        public string NombrEEmpresa(long? id)
        {
            using (var esquema = GetEsquema())
            {
                Empresa entidad = ObtenerEmpresa(esquema, id);
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
                Empresa EntidadEx = ObtenerEmpresa(esquema, id);
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
                    Exception Ex = new Exception("No existe la empresa con id " + id);
                    RegistrarErrorEnBitacora(esquema, UsuarioId, id, AccionBitacora.Obteniendo, "Categoria Servicios cambio de estado", Ex);
                    throw Ex;
                }
            }
        }


    }
}
