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
    public class LPais : LBaseCC<Pais>
    {


        public EPais Convert(Pais CP)
        {
            return new EPais(CP);
        }
        public List<EPais> Convert(List<Pais> L)
        {
            List<EPais> LC = new List<EPais>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<EPais> Obtener()
        {
            var Entidad = new List<EPais>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.Pais
                                 where x.nEstado != (int)Estado.Eliminado
                                 select x).ToList();
                Entidad = Convert(resultado);
                foreach (var item in Entidad)
                {
                    item.UrlBandera = EUtils.URLImagen(item.Bandera); ;
                    item.Base64Bandera = EUtils.ObtenerImagenBase64(item.Bandera);
                }
                return Entidad;
            }
        }

        public List<EPais> ObtenerHabilitados()
        {
            var Entidad = new List<EPais>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.Pais
                                 where x.nEstado == (int)Estado.Habilitado
                                 select x).ToList();
                Entidad = Convert(resultado);
                foreach (var item in Entidad)
                {
                    item.UrlBandera = EUtils.URLImagen(item.Bandera); ;
                    item.Base64Bandera = EUtils.ObtenerImagenBase64(item.Bandera);
                }
      
                return Entidad;
            }
        }

        public EPais Obtener(long id)
        {
            using (var esquema = GetEsquema())
            {
                Pais entidad = ObtenerPais(esquema, id);
                if (entidad == null)
                {
                    throw new BussinessException("No se puede encontrar la Pais seleccionada.");
                }
                var resultado = Convert(entidad);
                resultado.UrlBandera = EUtils.URLImagen(resultado.Bandera); ;
                resultado.Base64Bandera = EUtils.ObtenerImagenBase64(resultado.Bandera);
                return resultado;
            }
        }

        private Pais ObtenerPais(dbFexpoCruzEntities esquema, long? PaisId)
        {
            Pais entidad = (from x in esquema.Pais
                                 where x.nPaisId == PaisId && x.nEstado != (int)Estado.Eliminado
                                 select x).FirstOrDefault();
            return entidad;
        }
        public List<EPais> ObtenerLista(string estado)
        {
            using (var esquema = GetEsquema())
            {
                // todos
                var entidades = (from u in esquema.Pais
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

        public Pais Nuevo(Pais Entidad, long UsuarioId)
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
                        
                        Entidad.sBandera = EUtils.InsertarImagen("Paises", Entidad.sBandera);
                    }
                    esquema.Pais.Add(Entidad);
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


        public Pais Modificar(Pais Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    Pais EntidadLocal = ObtenerPais(esquema, Entidad.nPaisId);

                    EntidadLocal.sNombre = Entidad.sNombre;
                    EntidadLocal.sCodigo = Entidad.sCodigo;
                    EntidadLocal.nEstado = Entidad.nEstado;
                    EntidadLocal.nDiferenciaHorario = Entidad.nDiferenciaHorario;
                    EntidadLocal.sSiglaMoneda = Entidad.sSiglaMoneda;
                    EntidadLocal.sMoneda = Entidad.sMoneda;
                    EntidadLocal.sSiglaPais = Entidad.sSiglaPais;
                    EntidadLocal.dFechaModificacion = EUtils.Now;
                    EntidadLocal.nUsuarioModificador = UsuarioId;
                    if (Entidad.nEstado != EntidadLocal.nEstado)
                    {
                        ValidarCambioEstado(EntidadLocal, (Estado)Entidad.nEstado, esquema);
                    }
                    EntidadLocal.nEstado = Entidad.nEstado;
                    if (!string.IsNullOrWhiteSpace(Entidad.sBandera))
                    {
                        EntidadLocal.sBandera = EUtils.InsertarImagen("Paises", Entidad.sBandera);
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
                Pais EntidadEx = ObtenerPais(esquema, id);
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
                    Exception Ex = new Exception("No existe el pais con id " + id);
                    RegistrarErrorEnBitacora(esquema, UsuarioId, id, AccionBitacora.Obteniendo, "Pais cambio de estado", Ex);
                    throw Ex;
                }
            }
        }

        public string EntidadDetalleBitacora(Pais Entidad)
        {
            return Entidad.nPaisId + "|" + Entidad.sNombre + "|" + Entidad.nEstado;
        }

        public void Validar(Pais Entidad)
        {
            if (string.IsNullOrEmpty(Entidad.sNombre))
            {
                throw new BussinessException("El nombre es obligatorio.");
            }
            if (string.IsNullOrEmpty(Entidad.sCodigo))
            {
                throw new BussinessException("El codigo es obligatorio.");
            }
            if (string.IsNullOrEmpty(Entidad.sSiglaPais))
            {
                throw new BussinessException("La sigla del pais es obligatorio.");
            }
            if (string.IsNullOrEmpty(Entidad.sMoneda))
            {
                throw new BussinessException("La moneda es obligatorio.");
            }
            if (string.IsNullOrEmpty(Entidad.sSiglaMoneda))
            {
                throw new BussinessException("La sigla de la moneda es obligatorio.");
            }
            if (string.IsNullOrEmpty(Entidad.nDiferenciaHorario.ToString()))
            {
                throw new BussinessException("La diferencia de horario es obligatorio.");
            }
        }


        private void ValidarCambioEstado(Pais Entidad, Estado estado, dbFexpoCruzEntities esquema)
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

        private string DeshabilitarValidar(Pais Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string HabilitarValidar(Pais Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string EliminarValidar(Pais Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }

        public string NombrEPais(long? id)
        {
            using (var esquema = GetEsquema())
            {
                Pais entidad = ObtenerPais(esquema, id);
                if (entidad == null)
                {
                    return "";
                }
                var resultado = Convert(entidad);
                return resultado.Nombre;
            }
        }

       

    }
}
