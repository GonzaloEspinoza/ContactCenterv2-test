using System;
using System.Collections.Generic;
using System.Linq;
using DataP;
using System.Text;
using System.Threading.Tasks;
using Entidad.Enums;
using Entidad;

namespace Logica
{
    public class LBaseCC<E> : LBaseBase<dbFexpoCruzEntities, E>
    {   
        protected override dbFexpoCruzEntities GetEsquema()
        {
            return new dbFexpoCruzEntities();
        }

        private static LBaseCC<E> instancia;
        private LPermiso lPermiso;
        private LUsuarioBackEnd lUsuarioBackEnd;
        private LBitacora lBitacora;
        private LPerfil lPerfil;
        private LFuncionario lFuncionario;
        private LCiudad lCiudad;
        private LPais lPais;
        private LArea lArea;
        private LEmpresa lEmpresa;
        private LTipoAtencion lTipoAtencion;
        private LHorarioColaborador LhorarioColaborador;
        private LDia lDia;
        private LCliente lCliente;
        private LEvento lEvento;
        private LMensaje lMensaje;
        private LHilo lHilo;
        private LNotificacion lNotificacion;
        private LNotificacionFuncionario lNotificacionFuncionario;
        private LEstadoEstadoEvento lEstadoEstadoEvento;
        private LPreguntaFrecuente lPreguntaFrecuente;
        private LCategoriaPreguntaFrecuente lCategoriaPreguntaFrecuente;
        private LCalificacionColaborador lCalificacionColaborador;


        public static LBaseCC<E> Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new LBaseCC<E>();
                }
                return instancia;
            }
        }
        public LCalificacionColaborador LCalificacionColaborador
        {
            get
            {
                if (lCalificacionColaborador == null)
                {
                    lCalificacionColaborador = new LCalificacionColaborador();
                }
                return lCalificacionColaborador;
            }
        }
        public LEvento LEvento
        {
            get
            {
                if (lEvento == null)
                {
                    lEvento = new LEvento();
                }
                return lEvento;
            }
        }
        public LCategoriaPreguntaFrecuente LCategoriaPreguntaFrecuente
        {
            get
            {
                if (lCategoriaPreguntaFrecuente == null)
                {
                    lCategoriaPreguntaFrecuente = new LCategoriaPreguntaFrecuente();
                }
                return lCategoriaPreguntaFrecuente;
            }
           
        }

        public LPreguntaFrecuente LPreguntaFrecuente
        {
            get
            {
                if (lPreguntaFrecuente == null)
                {
                    lPreguntaFrecuente = new LPreguntaFrecuente();
                }
                return lPreguntaFrecuente;
            }

        }
        public LEstadoEstadoEvento LEstadoEstadoEvento
        {
            get
            {
                if (lEstadoEstadoEvento == null)
                {
                    lEstadoEstadoEvento = new LEstadoEstadoEvento();
                }
                return lEstadoEstadoEvento;
            }
        }


        public LNotificacionFuncionario LNotificacionFuncionario
        {
            get
            {
                if (lNotificacionFuncionario == null)
                {
                    lNotificacionFuncionario = new LNotificacionFuncionario();
                }
                return lNotificacionFuncionario;
            }
        }


        public LNotificacion LNotificacion
        {
            get
            {
                if (lNotificacion == null)
                {
                    lNotificacion = new LNotificacion();
                }
                return lNotificacion;
            }
        }

        public LHilo LHilo
        {
            get
            {
                if (lHilo == null)
                {
                    lHilo = new LHilo();
                }
                return lHilo;
            }
        }
        public LCliente LCliente
        {
            get
            {
                if (lCliente == null)
                {
                    lCliente = new LCliente();
                }
                return lCliente;
            }
        }
        public LMensaje LMensaje
        {
            get
            {
                if (lMensaje == null)
                {
                    lMensaje = new LMensaje();
                }
                return lMensaje;
            }
        }
     
        public LCiudad LCiudad
        {
            get
            {
                if (lCiudad == null)
                {
                    lCiudad = new LCiudad();
                }
                return lCiudad;
            }
        }

        public LDia LDia
        {
            get
            {
                if (lDia == null)
                {
                    lDia = new LDia();
                }
                return lDia;
            }

        }
        public LHorarioColaborador LHorarioColaborador
        {
            get
            {
                if (LhorarioColaborador == null)
                {
                    LhorarioColaborador = new LHorarioColaborador();
                }
                return LhorarioColaborador;
            }

        }

        public LTipoAtencion LTipoAtencion
        {
            get
            {
                if (lTipoAtencion== null)
                {
                    lTipoAtencion = new LTipoAtencion();
                }
                return lTipoAtencion;
            }

        }
        public LEmpresa LEmpresa
        {
            get
            {
                if (lEmpresa == null)
                {
                    lEmpresa = new LEmpresa();
                }
                return lEmpresa;
            }

        }
        public LArea LArea
        {
            get
            {
                if (lArea == null)
                {
                    lArea = new LArea();
                }
                return lArea;
            }

        }
        public LPais LPais
        {
            get
            {
                if (lPais == null)
                { 
                    lPais = new LPais();
                }
                return lPais;
            }

        }
      
        public LBitacora LBitacora
        {
            get
            {
                if (lBitacora == null)
                {
                    lBitacora = new LBitacora();
                }
                return lBitacora;
            }
        }
        public LUsuarioBackEnd LUsuarioBackEnd
        {
            get
            {
                if (lUsuarioBackEnd == null)
                {
                    lUsuarioBackEnd = new LUsuarioBackEnd();
                }
                return lUsuarioBackEnd;
            }
        }
        public LFuncionario LFuncionario
        {
            get
            {
                if (lFuncionario == null)
                {
                    lFuncionario = new LFuncionario();
                }
                return lFuncionario;
            }
        }
     
        public LPermiso LPermiso
        {
            get
            {
                if (lPermiso == null)
                {
                    lPermiso = new LPermiso();

                }
                return lPermiso;
            }
        }

      
        public LPerfil LPerfil
        {
            get
            {
                if (lPerfil == null)
                {
                    lPerfil = new LPerfil();
                }
                return lPerfil;
            }
        }
      
    
        public static void RegistrarEnBitacora(dbFexpoCruzEntities esquema, long? UsuarioId, long? IdImplicado, AccionBitacora Accion, string Transaccion, TipoBitacora Tipo, string Detalle)
        {
            Bitacora Bitacora = new Bitacora();
            Bitacora.Accion = Accion.ToString();
            Bitacora.Transaccion = Transaccion;
            Bitacora.Detalle = Detalle;
            Bitacora.FechaHora = EUtils.Now;
            Bitacora.UsuarioId = UsuarioId;
            Bitacora.IdImplicado = IdImplicado;
            Bitacora.Tipo = (int)Tipo;
            esquema.Bitacora.Add(Bitacora);
            esquema.SaveChanges();
        }
        public static void RegistrarExitoEnBitacora(dbFexpoCruzEntities esquema, long? UsuarioId, long? IdImplicado, AccionBitacora Accion, string Transaccion, string Detalle)
        {
            RegistrarEnBitacora(esquema, UsuarioId, IdImplicado, Accion, Transaccion, TipoBitacora.Exito, Detalle);
        }

        public static void RegistrarErrorEnBitacora(dbFexpoCruzEntities esquema, long? UsuarioId, long? IdImplicado, AccionBitacora Accion, string Transaccion, string Detalle)
        {
            RegistrarEnBitacora(esquema, UsuarioId, IdImplicado, Accion, Transaccion, TipoBitacora.Error, Detalle);
        }

        public static void RegistrarErrorEnBitacora(dbFexpoCruzEntities esquema, long? UsuarioId, long? IdImplicado, AccionBitacora Accion, string Transaccion, string Detalle, Exception Exception)
        {
            RegistrarEnBitacora(esquema, UsuarioId, IdImplicado, Accion, Transaccion, TipoBitacora.Error, Detalle + " " + Exception.Message + " " + (Exception.InnerException != null ? Exception.InnerException.Message : ""));
        }

        public void RegistrarEnBitacora(dbFexpoCruzEntities esquema, long? UsuarioId, long? IdImplicado, AccionBitacora Accion, TipoBitacora Tipo, string Detalle)
        {
            RegistrarEnBitacora(esquema, UsuarioId, IdImplicado, Accion, typeof(E).Name, Tipo, Detalle);
        }

        public void RegistrarExitoEnBitacora(dbFexpoCruzEntities esquema, long? UsuarioId, long? IdImplicado, AccionBitacora Accion, string Detalle)
        {
            RegistrarExitoEnBitacora(esquema, UsuarioId, IdImplicado, Accion, typeof(E).Name, Detalle);
        }

        public void RegistrarErrorEnBitacora(dbFexpoCruzEntities esquema, long? UsuarioId, long? IdImplicado, AccionBitacora Accion, string Detalle)
        {
            RegistrarErrorEnBitacora(esquema, UsuarioId, IdImplicado, Accion, typeof(E).Name, Detalle);
        }

        public void RegistrarErrorEnBitacora(dbFexpoCruzEntities esquema, long? UsuarioId, long? IdImplicado, AccionBitacora Accion, Exception Exception)
        {
            RegistrarErrorEnBitacora(esquema, UsuarioId, IdImplicado, Accion, typeof(E).Name, Exception);
        }

        public void RegistrarErrorEnBitacora(dbFexpoCruzEntities esquema, long? UsuarioId, long? IdImplicado, AccionBitacora Accion, string Detalle, Exception Exception)
        {
            RegistrarErrorEnBitacora(esquema, UsuarioId, IdImplicado, Accion, typeof(E).Name, Detalle, Exception);
        }
    }
}
