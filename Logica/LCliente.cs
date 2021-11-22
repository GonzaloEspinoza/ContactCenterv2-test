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
    public class LCliente : LBaseCC<Cliente>
    {
        public ECliente Convert(Cliente CP)
        {
            return new ECliente(CP);
        }
        public List<ECliente> Convert(List<Cliente> L)
        {
            List<ECliente> LC = new List<ECliente>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<ECliente> Obtener()
        {
            var Entidad = new List<ECliente>();
            using (var esquema = GetEsquema())
            {
                //var resultado0 = (from x in esquema.Cliente
                //                 where x.nEstado != (int)Estado.Eliminado
                //                 select x).ToList();

                var resultado = esquema.Cliente.Include("Empresa").Where(x => x.nEstado != (int)Estado.Eliminado).ToList();
                Entidad = Convert(resultado);
                return Entidad;
            }
        }


        public ECliente Obtener(long id)
        {
            using (var esquema = GetEsquema())
            {
                Cliente entidad = ObtenerCliente(esquema, id);
                if (entidad == null)
                {
                    throw new BussinessException("No se puede encontrar la Cliente seleccionada.");
                }
                var resultado = Convert(entidad);
                return resultado;
            }
        }

        private Cliente ObtenerCliente(dbFexpoCruzEntities esquema, long? ClienteId)
        {
            Cliente entidad = (from x in esquema.Cliente
                                 where x.nClienteId == ClienteId && x.nEstado != (int)Estado.Eliminado
                                 select x).FirstOrDefault();
            return entidad;
        }

        public Cliente ObtenerClientePorCodigo(dbFexpoCruzEntities esquema, string CodigoCliente)
        {
            Cliente entidad = (from x in esquema.Cliente
                               where x.sCodigoCliente == CodigoCliente && x.nEstado != (int)Estado.Eliminado
                               select x).FirstOrDefault();
            return entidad;
        }

        public Cliente ObtenerClientePorCodigoDistinto(dbFexpoCruzEntities esquema, string CodigoCliente, long ClienteId)
        {
            Cliente entidad = (from x in esquema.Cliente
                               where x.sCodigoCliente == CodigoCliente && x.nClienteId != ClienteId && x.nEstado != (int)Estado.Eliminado
                               select x).FirstOrDefault();
            return entidad;
        }
        public List<ECliente> ObtenerLista(string estado)
        {
            using (var esquema = GetEsquema())
            {
                // todos
                var entidades = (from u in esquema.Cliente
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

        

        public List<EClienteDataTable> ObtenerListadoPaginado(int estado, string draw, string start, 
            string length, string sortColumn, string sortColumnDir, string searchValue, int pageSize, int skip, out int recordsTotal)
        {
            List<EClienteDataTable> listado = new List<EClienteDataTable>();

            using (var esquema = GetEsquema())
            {
                IQueryable<EClienteDataTable> query = null;

                if (estado == -1 || estado == 2)
                {
                    query = (from d in esquema.Cliente
                             where d.nEstado != (int)Estado.Eliminado && (d.sNombres.Contains(searchValue) || 
                             d.nClienteId.ToString().Contains(searchValue) 
                             || d.sNumeroCelular.Contains(searchValue) 
                             || d.sApellidos.Contains(searchValue) 
                             || d.sCorreo.Contains(searchValue) 
                             || d.Empresa.sNombre.Contains(searchValue) )
                             select new EClienteDataTable
                             {
                                 ClienteId = d.nClienteId,
                                 Nombres = d.sNombres,
                                 Apellidos = d.sApellidos,
                                 EmpresaNombre = d.Empresa.sNombre,
                                 Estado = d.nEstado 

                             });
                }
                else if(estado==0)
                {
                    query = (from d in esquema.Cliente
                             where d.nEstado == (int)Estado.Desabilitado && (d.sNombres.Contains(searchValue) ||
                             d.nClienteId.ToString().Contains(searchValue)
                             || d.sNumeroCelular.Contains(searchValue)
                             || d.sApellidos.Contains(searchValue)
                             || d.sCorreo.Contains(searchValue)
                             || d.Empresa.sNombre.Contains(searchValue)) 
                             select new EClienteDataTable
                             {
                                 ClienteId = d.nClienteId,
                                 Nombres = d.sNombres,
                                 Apellidos = d.sApellidos,
                                 EmpresaNombre = d.Empresa.sNombre,
                                 Estado = d.nEstado

                             });
                }else if(estado==1)
                {
                    query = (from d in esquema.Cliente
                             where d.nEstado == (int)Estado.Habilitado && (d.sNombres.Contains(searchValue) ||
                             d.nClienteId.ToString().Contains(searchValue)
                             || d.sNumeroCelular.Contains(searchValue)
                             || d.sApellidos.Contains(searchValue)
                             || d.sCorreo.Contains(searchValue)
                             || d.Empresa.sNombre.Contains(searchValue))
                             select new EClienteDataTable
                             {
                                 ClienteId = d.nClienteId,
                                 Nombres = d.sNombres,
                                 Apellidos = d.sApellidos,
                                 EmpresaNombre = d.Empresa.sNombre,
                                 Estado = d.nEstado

                             });
                }


                //Sorting    
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumn)
                    {
                        case "ClienteId":
                            query = sortColumnDir == "asc" ? query.OrderBy(s => s.ClienteId) : query.OrderByDescending(s => s.ClienteId);
                            break;
                        case "Nombres":
                            query = sortColumnDir == "asc" ? query.OrderBy(s => s.Nombres) : query.OrderByDescending(s => s.Nombres);
                            break;
                        case "Apellidos":
                            query = sortColumnDir == "asc" ? query.OrderBy(s => s.Apellidos) : query.OrderByDescending(s => s.Apellidos);
                            break;
                        default:
                            query = sortColumnDir == "asc" ? query.OrderBy(s => s.ClienteId) : query.OrderByDescending(s => s.ClienteId);
                            break;
                    }

                }

                recordsTotal = query.Count();

                listado = query.Skip(skip).Take(pageSize).ToList();
            }
            return listado;
        }

        public List<ECliente> ObtenerTodosClientes()
        {
            using (var esquema = GetEsquema())
            {
                // todos
                var entidades = (from u in esquema.Cliente
                                 where u.nEstado != (int)Estado.Eliminado
                                 select u);
                var resultado = Convert(entidades.ToList());
                return resultado;
            }
        }

        public List<ECliente> ObtenerClientesEmpresa(long funcionarioId)
        {
            using (var esquema = GetEsquema())
            {
                var funcionario = (from u in esquema.Funcionario
                                   where u.nFuncionarioId == funcionarioId
                                   && u.nEstado != (int)Estado.Eliminado && u.nTipo ==
                                   (int)TipoFuncionario.Colaborador
                                   select u).FirstOrDefault();
                var entidades = (from u in esquema.Cliente
                                 where u.nEstado != (int)Estado.Eliminado && u.Empresa.nEmpresaId== funcionario.Area.nEmpresaId
                                 select u);
                var resultado = Convert(entidades.ToList());
                return resultado;
            }
        }

        

        public Cliente Nuevo(Cliente Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    if (ObtenerClientePorCodigo(esquema,Entidad.sCodigoCliente)!=null)
                    {
                        throw new BussinessException("El codigo del cliente ya existe.");
                    }

                    Entidad.dFechaCreacion = EUtils.Now;
                    Entidad.dFechaModificacion = EUtils.Now;
                    esquema.Cliente.Add(Entidad);
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


        public Cliente Modificar(Cliente Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    if (ObtenerClientePorCodigoDistinto(esquema, Entidad.sCodigoCliente, Entidad.nClienteId) != null)
                    {
                        throw new BussinessException("El codigo del cliente ya existe.");
                    }

                    Cliente EntidadLocal = ObtenerCliente(esquema, Entidad.nClienteId);

                    EntidadLocal.sNombres = Entidad.sNombres;
                    EntidadLocal.sApellidos = Entidad.sApellidos;
                    EntidadLocal.sCodigoCliente = Entidad.sCodigoCliente;
                    EntidadLocal.sUrlFoto = Entidad.sUrlFoto;
                    EntidadLocal.sCodigoPais = Entidad.sCodigoPais;
                    EntidadLocal.sNumeroCelular = Entidad.sNumeroCelular;
                    EntidadLocal.sCorreo = Entidad.sCorreo;
                    EntidadLocal.sTokenNotificacionFirebase = Entidad.sTokenNotificacionFirebase;
                    EntidadLocal.sTipoSistemaOperativo = Entidad.sTipoSistemaOperativo;
                    EntidadLocal.nEmpresaId = Entidad.nEmpresaId;
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


        public void CambiarEstado(long id, Estado estado, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                Cliente EntidadEx = ObtenerCliente(esquema, id);
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
                    Exception Ex = new Exception("No existe Cliente con id " + id);
                    RegistrarErrorEnBitacora(esquema, UsuarioId, id, AccionBitacora.Obteniendo, "Cliente cambio de estado", Ex);
                    throw Ex;
                }
            }

        }
        public string EntidadDetalleBitacora(Cliente Entidad)
        {
            return Entidad.nClienteId + "|" + Entidad.sNombres + "|" + Entidad.nEstado;
        }

        public void Validar(Cliente Entidad)
        {
          
            if (string.IsNullOrEmpty(Entidad.sNombres))
            {
                throw new BussinessException("El nombre es obligatorio.");
            }
            if (string.IsNullOrEmpty(Entidad.sApellidos))
            {
                throw new BussinessException("El apellido es obligatorio.");
            }
            if (string.IsNullOrEmpty(Entidad.sTipoSistemaOperativo))
            {
                throw new BussinessException("El tipo del sistema operativo es obligatorio.");
            }
            if (string.IsNullOrEmpty(Entidad.sTokenNotificacionFirebase))
            {
                throw new BussinessException("El token de notificacion firebase es obligatorio.");
            }
            if (string.IsNullOrEmpty(Entidad.nEmpresaId.ToString()))
            {
                throw new BussinessException("La empresa es obligatorio.");
            }
            if (string.IsNullOrEmpty(Entidad.sCodigoCliente))
            {
                throw new BussinessException("El codigo del cliente es obligatorio.");
            }
        }


        private void ValidarCambioEstado(Cliente Entidad, Estado estado, dbFexpoCruzEntities esquema)
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

        private string DeshabilitarValidar(Cliente Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string HabilitarValidar(Cliente Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }
        private string EliminarValidar(Cliente Entidad, dbFexpoCruzEntities esquema)
        {
            StringBuilder builder = new StringBuilder();
            return builder.ToString();
        }


        #region api
        
        #endregion



    }
}
