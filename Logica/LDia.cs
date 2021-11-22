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
    public class LDia : LBaseCC<Dia>
    {


        public EDia Convert(Dia CP)
        {
            return new EDia(CP);
        }
        public List<EDia> Convert(List<Dia> L)
        {
            List<EDia> LC = new List<EDia>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<EDia> Obtener()
        {
            var Entidad = new List<EDia>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.Dia
                                 select x).ToList();
                Entidad = Convert(resultado);
                
                return Entidad;
            }
        }


        public EDia Obtener(long id)
        {
            using (var esquema = GetEsquema())
            {
                Dia entidad = ObtenerDia(esquema, id);
                if (entidad == null)
                {
                    throw new BussinessException("No se puede encontrar la Dia seleccionada.");
                }
                var resultado = Convert(entidad);
                return resultado;
            }
        }

        private Dia ObtenerDia(dbFexpoCruzEntities esquema, long? DiaId)
        {
            Dia entidad = (from x in esquema.Dia
                                 where x.nDiaId == DiaId
                                 select x).FirstOrDefault();
            return entidad;
        }
        public List<EDia> ObtenerLista()
        {
            using (var esquema = GetEsquema())
            {
                // todos
                var entidades = (from u in esquema.Dia
                                 select u);
                var resultado = Convert(entidades.ToList());
                return resultado;
            }
        }

        public Dia Nuevo(Dia Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    esquema.Dia.Add(Entidad);
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


        public Dia Modificar(Dia Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    Dia EntidadLocal = ObtenerDia(esquema, Entidad.nDiaId);

                    EntidadLocal.sNombre = Entidad.sNombre;

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
                Dia EntidadEx = ObtenerDia(esquema, id);
                if (EntidadEx != null)
                {
                    try
                    {
                        ValidarCambioEstado(EntidadEx, estado, esquema);
                     
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
                    Exception Ex = new Exception("No existe Dia con id " + id);
                    RegistrarErrorEnBitacora(esquema, UsuarioId, id, AccionBitacora.Obteniendo, "Dia cambio de estado", Ex);
                    throw Ex;
                }
            }

        }
        public string EntidadDetalleBitacora(Dia Entidad)
        {
            return Entidad.nDiaId + "|" + Entidad.sNombre;
        }

        public void Validar(Dia Entidad)
        {
            if (string.IsNullOrEmpty(Entidad.sNombre.ToString()))
            {
                throw new BussinessException("El nombre es obligatorio.");
            }
           
        }


        private void ValidarCambioEstado(Dia Entidad, Estado estado, dbFexpoCruzEntities esquema)
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

        private string DeshabilitarValidar(Dia Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string HabilitarValidar(Dia Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string EliminarValidar(Dia Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }

       


    }
}
