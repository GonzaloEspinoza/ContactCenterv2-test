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
    public class LHorarioColaborador : LBaseCC<HorarioColaborador>
    {


        public EHorarioColaborador Convert(HorarioColaborador CP)
        {
            return new EHorarioColaborador(CP);
        }
        public List<EHorarioColaborador> Convert(List<HorarioColaborador> L)
        {
            List<EHorarioColaborador> LC = new List<EHorarioColaborador>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<EHorarioColaborador> Obtener()
        {
            var Entidad = new List<EHorarioColaborador>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.HorarioColaborador
                                 where x.nEstado != (int)Estado.Eliminado
                                 select x).ToList();
                Entidad = Convert(resultado);
                
                return Entidad;
            }
        }


        public EHorarioColaborador Obtener(long id)
        {
            using (var esquema = GetEsquema())
            {
                HorarioColaborador entidad = ObtenerHorarioColaborador(esquema, id);
                if (entidad == null)
                {
                    throw new BussinessException("No se puede encontrar la HorarioColaborador seleccionada.");
                }
                var resultado = Convert(entidad);
                return resultado;
            }
        }

        private HorarioColaborador ObtenerHorarioColaborador(dbFexpoCruzEntities esquema, long? HorarioColaboradorId)
        {
            HorarioColaborador entidad = (from x in esquema.HorarioColaborador
                                 where x.nHorarioFuncionarioId == HorarioColaboradorId && x.nEstado != (int)Estado.Eliminado
                                 select x).FirstOrDefault();
            return entidad;
        }
        public List<EHorarioColaborador> ObtenerLista(string estado,long FuncionarioId)
        {
            using (var esquema = GetEsquema())
            {
                // todos
                var entidades = (from u in esquema.HorarioColaborador
                                 where u.nEstado != (int)Estado.Eliminado && u.nFuncionarioId== FuncionarioId
                                 select u);
                if (estado == "0") // deshabilitado
                {
                    entidades = (from u in entidades
                                 where u.nEstado == (int)Estado.Desabilitado && u.nFuncionarioId == FuncionarioId
                                 select u);
                }
                else if (estado == "1") // habilitados
                {
                    entidades = (from u in entidades
                                 where u.nEstado == (int)Estado.Habilitado && u.nFuncionarioId == FuncionarioId
                                 select u);
                }
                var resultado = Convert(entidades.ToList());
                return resultado;
            }
        }

        public HorarioColaborador Nuevo(HorarioColaborador Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    List<HorarioColaborador> existe = (from x in esquema.HorarioColaborador
                                                        where x.nEstado != (int)Estado.Eliminado &&
                                               x.nDiaId == Entidad.nDiaId && x.nFuncionarioId == Entidad.nFuncionarioId
                                               select x).ToList();
                    validarExistencia(existe, Entidad);
                    Entidad.dFechaCreacion = EUtils.Now;
                    Entidad.dFechaModificacion = EUtils.Now;
                    Entidad.nUsuarioCreador = UsuarioId;
                    Entidad.nUsuarioModificador = UsuarioId;
                    Entidad.nEstado = (int)Estado.Habilitado;
                    esquema.HorarioColaborador.Add(Entidad);
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


     


        public void CambiarEstado(long id, Estado estado, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                HorarioColaborador EntidadEx = ObtenerHorarioColaborador(esquema, id);
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
                    Exception Ex = new Exception("No existe HorarioColaborador con id " + id);
                    RegistrarErrorEnBitacora(esquema, UsuarioId, id, AccionBitacora.Obteniendo, "HorarioColaborador cambio de estado", Ex);
                    throw Ex;
                }
            }

        }
        public string EntidadDetalleBitacora(HorarioColaborador Entidad)
        {
            return Entidad.nHorarioFuncionarioId + "|" + Entidad.nFuncionarioId + "|" + Entidad.nEstado;
        }

        public void validarExistencia(List<HorarioColaborador> Existe, HorarioColaborador Entidad)
        {
            if (Existe != null)
            {
                if (Existe.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();
                    if (Entidad.tHoraInicio >= Entidad.tHoraFin)
                    {
                        builder.Append("La hora inicio no puede ser mayor o igual a la hora fin");
                        builder.Append(".");
                        throw new BussinessException(builder.ToString());
                    }
                    foreach (HorarioColaborador horario in Existe)
                    {
                        if (horario.tHoraInicio <= Entidad.tHoraInicio && Entidad.tHoraInicio <= horario.tHoraFin)
                        {
                            builder.Append("Las horas coinciden con otro horario ya registrado el mismo dia");
                            builder.Append(".");
                            throw new BussinessException(builder.ToString());
                        }
                        if (horario.tHoraInicio <= Entidad.tHoraFin && Entidad.tHoraFin <= horario.tHoraFin)
                        {
                            builder.Append("Las horas coinciden con otro horario ya registrado el mismo dia");
                            builder.Append(".");
                            throw new BussinessException(builder.ToString());
                        }
                    }
                }
            }
        }


        private void ValidarCambioEstado(HorarioColaborador Entidad, Estado estado, dbFexpoCruzEntities esquema)
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

        private string DeshabilitarValidar(HorarioColaborador Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string HabilitarValidar(HorarioColaborador Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string EliminarValidar(HorarioColaborador Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }

       


    }
}
