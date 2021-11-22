using DataP;
using Entidad;
using Entidad.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica
{
    public class LFuncionario : LBaseCC<Funcionario>
    {

        public EFuncionario Convert(Funcionario A)
        {
            return new EFuncionario(A);
        }

        public List<EFuncionario> Convert(List<Funcionario> L)
        {
            List<EFuncionario> LS = new List<EFuncionario>();
            L.ForEach(x => LS.Add(Convert(x)));
            return LS;

        }

        public string GenerarPasswordByNickName(string NickName)
        {
            string PasswordInicial = NickName;
            if (PasswordInicial.Length > 0)
            {
                PasswordInicial = PasswordInicial.Substring(0, 1).ToUpper() + PasswordInicial.Substring(1, PasswordInicial.Length - 1);
            }
            return PasswordInicial;
        }

        public List<EFuncionario> ObtenerLista(string tipo, int TipoUsuarioL)
        {
            using (var esquema = GetEsquema())
            {
                List<Funcionario> entidades = new List<Funcionario>();
                if (tipo == "0")
                {
                    entidades = (from u in esquema.Funcionario
                                 where u.nEstado == (int)Estado.Habilitado && (u.Usuario == null || u.Usuario.TipoUsuario >= TipoUsuarioL
                                 && u.nTipo == (int)TipoFuncionario.Administrador)
                                 select u).ToList();
                }
                else if (tipo == "1")
                {
                    entidades = (from u in esquema.Funcionario
                                 where u.nEstado == (int)Estado.Desabilitado && (u.Usuario == null || u.Usuario.TipoUsuario >= TipoUsuarioL
                                 && u.nTipo == (int)TipoFuncionario.Administrador)
                                 select u).ToList();
                }
                else
                {
                    entidades = (from u in esquema.Funcionario
                                 where u.nEstado != (int)Estado.Eliminado && (u.Usuario == null || u.Usuario.TipoUsuario >= TipoUsuarioL
                                 && u.nTipo == (int)TipoFuncionario.Administrador)
                                 select u).ToList();
                }
                List<EFuncionario> res = new List<EFuncionario>();
                foreach (var us in entidades)
                {
                    res.Add(new EFuncionario()
                    {
                        sApellido = us.sApellido,
                        sNombre = us.sNombre,
                        nEstado = us.nEstado,
                        nFuncionarioId = us.nFuncionarioId,
                        Usuario = us.Usuario,
                        sCi  = us.sCi,
                        TituloDeshabilitar = "",
                        TituloEliminar = ValidarEliminacion(us, false),
                    });
                }
                return res;
            }
        }

        public List<EFuncionario> ObtenerListaColaboradores(string tipo)
        {
            using (var esquema = GetEsquema())
            {
                List<Funcionario> entidades = new List<Funcionario>();
                if (tipo == "0")
                {
                    entidades = (from u in esquema.Funcionario
                                 where u.nEstado == (int)Estado.Habilitado && 
                                 u.nTipo == (int)TipoFuncionario.Colaborador
                                 select u).ToList();
                }
                else if (tipo == "1")
                {
                    entidades = (from u in esquema.Funcionario
                                 where u.nEstado == (int)Estado.Desabilitado &&
                                 u.nTipo == (int)TipoFuncionario.Colaborador
                                 select u).ToList();
                }
                else
                {
                    entidades = (from u in esquema.Funcionario
                                 where u.nEstado != (int)Estado.Eliminado && 
                                 u.nTipo == (int)TipoFuncionario.Colaborador
                                 select u).ToList();
                }
                List<EFuncionario> res = new List<EFuncionario>();
                foreach (var us in entidades)
                {
                    res.Add(new EFuncionario()
                    {
                        sApellido = us.sApellido,
                        sNombre = us.sNombre,
                        nEstado = us.nEstado,
                        nFuncionarioId = us.nFuncionarioId,
                        Usuario = us.Usuario,
                        sCi = us.sCi,
                        TituloDeshabilitar = "",
                        TituloEliminar = ValidarEliminacion(us, false),
                    });
                }
                return res;
            }
        }

        public EFuncionario ObtenerColaboradorPorId(long funcionarioId)
        {
            using (var esquema = GetEsquema())
            {
                
                 var colaborador = (from u in esquema.Funcionario
                                 where u.nEstado == (int)Estado.Habilitado &&
                                 u.nTipo == (int)TipoFuncionario.Colaborador
                                 && u.nFuncionarioId== funcionarioId
                                    select u).FirstOrDefault();
                
                return Convert(colaborador);
            }
        }

        public List<EFuncionario> ObtenerListaColaboradoresDeLaMismaEmpresa(long funcionarioId)
        {
            using (var esquema = GetEsquema())
            {
                List<Funcionario> entidades = new List<Funcionario>();

                var funcionario = (from u in esquema.Funcionario
                                   where u.nFuncionarioId == funcionarioId
                                   && u.nEstado != (int)Estado.Eliminado && u.nTipo ==
                                   (int)TipoFuncionario.Colaborador select u).FirstOrDefault();
                entidades = (from u in esquema.Funcionario
                                where u.nEstado == (int)Estado.Habilitado && u.Area.nEmpresaId==funcionario.Area.nEmpresaId 
                                && u.nTipo == (int)TipoFuncionario.Colaborador
                                select u).ToList();
                
                List<EFuncionario> res = new List<EFuncionario>();
                foreach (var us in entidades)
                {
                    res.Add(new EFuncionario()
                    {
                        sApellido = us.sApellido,
                        sNombre = us.sNombre,
                        nEstado = us.nEstado,
                        nFuncionarioId = us.nFuncionarioId,
                        Usuario = us.Usuario,
                        sCi = us.sCi,
                        TituloDeshabilitar = "",
                        TituloEliminar = ValidarEliminacion(us, false),
                        sNombreArea = us.Area != null ? us.Area.sNombre : "Sin area",
                        sNombreCiudad = us.Ciudad != null ? us.Ciudad.sNombre : "Sin ciudad"
                    });
                }
                return res;
            }

        }


        public List<EFuncionario> ObtenerListaTodosFuncionarios()
        {
            using (var esquema = GetEsquema())
            {
                List<Funcionario> entidades = new List<Funcionario>();

              
                entidades = (from u in esquema.Funcionario
                             where u.nEstado == (int)Estado.Habilitado && u.nTipo ==
                                   (int)TipoFuncionario.Colaborador
                             select u).ToList();

                List<EFuncionario> res = new List<EFuncionario>();
                foreach (var us in entidades)
                {
                    res.Add(new EFuncionario()
                    {
                        sApellido = us.sApellido,
                        sNombre = us.sNombre,
                        nEstado = us.nEstado,
                        nFuncionarioId = us.nFuncionarioId,
                        Usuario = us.Usuario,
                        sCi = us.sCi,
                        TituloDeshabilitar = "",
                        TituloEliminar = ValidarEliminacion(us, false),
                        sNombreArea = us.Area != null ? us.Area.sNombre : "Sin area",
                        sNombreCiudad = us.Ciudad != null ? us.Ciudad.sNombre : "Sin ciudad"
                    });
                }
                return res;
            }

        }

        private string EntidadToDetalleBitacora(Funcionario Entidad)
        {
            return "Usuario: " + Entidad.nEstado + "|" + Entidad.sNombre + "|" + Entidad.sApellido + "|" + Entidad.sCi + "|"; //Entidad.Cargo;
        }



        public Funcionario ModificarEstado(long Id, Estado Estado, long UsuarioLogueadoId, AccionBitacora AccionBitacora)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    var entidad = (from u in esquema.Funcionario where u.nFuncionarioId == Id && u.nEstado != (int)Estado.Eliminado && u.nTipo == (int)TipoFuncionario.Administrador select u).FirstOrDefault();
                    if (entidad != null)
                    {
                        if (Estado == Estado.Eliminado)
                        {
                            ValidarEliminacion(entidad);
                        }
                        entidad.nEstado = (int)Estado;
                        entidad.Usuario.Estado = (int)Estado;
                        entidad.nUsuarioModificador = UsuarioLogueadoId;
                        esquema.SaveChanges();
                        RegistrarExitoEnBitacora(esquema, UsuarioLogueadoId, Id, AccionBitacora, EntidadToDetalleBitacora(entidad));
                        return entidad;
                    }
                    else
                    {
                        throw new BussinessException("Funcionario con Id " + Id + " no existe.");
                    }
                }
                catch (BussinessException ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioLogueadoId, Id, AccionBitacora, ex);
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioLogueadoId, Id, AccionBitacora, ex);
                    throw ex;
                }
            }

        }

        public string ValidarEliminacion(Funcionario entidad, bool ex = true)
        {
            StringBuilder sb = new StringBuilder();
           
            if (!string.IsNullOrWhiteSpace(sb.ToString()))
            {
                if (ex)
                {
                    throw new BussinessException("No es posible eliminar el empleado \n" + sb.ToString());
                }
                else
                {
                    return "No es posible eliminar el empleado\n" + entidad.sNombre + " " + entidad.sApellido + ", ya que:\n" + sb.ToString();
                }
            }
            else
            {
                return "";
            }
        }

        public Funcionario Habilitar(long Id, long UsuarioLogueadoId)
        {
            Funcionario entidad = ModificarEstado(Id, Estado.Habilitado, UsuarioLogueadoId, AccionBitacora.Modificacion);
            return entidad;
        }
        public Funcionario Deshabilitar(long Id, long UsuarioLogueadoId)
        {
            Funcionario entidad = ModificarEstado(Id, Estado.Desabilitado, UsuarioLogueadoId, AccionBitacora.Modificacion);
            return entidad;
        }
        public Funcionario Eliminar(long Id, long UsuarioLogueadoId)
        {
            Funcionario entidad = ModificarEstado(Id, Estado.Eliminado, UsuarioLogueadoId, AccionBitacora.Eliminacion);
            return entidad;
        }


        public Funcionario Obtener(long Id)
        {
            try
            {
                using (var esquema = GetEsquema())
                {
                    var entidad = (from u in esquema.Funcionario where u.nFuncionarioId == Id && u.nEstado != (int)Estado.Eliminado
                                   && u.nTipo == (int)TipoFuncionario.Administrador select u).FirstOrDefault();
                    if (entidad != null)
                    {
                        if (entidad.Usuario != null)
                        {
                            entidad.Usuario = LUsuarioBackEnd.ObtenerPerfilesPermisos(entidad.Usuario);
                        }
                        else
                        {
                            throw new BussinessException("El funcionario no tiene usuario asignado.");
                        }

                        return entidad;
                    }
                    else
                    {
                        throw new BussinessException("Nombre de <b>Usuario</b> o <b>Contraseña</b> incorrectos");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BussinessException("Error: " + ex.Message);
            }
        }

        public Funcionario ObtenerColaborador(long Id)
        {
            try
            {
                using (var esquema = GetEsquema())
                {
                    var entidad = (from u in esquema.Funcionario where u.nFuncionarioId == Id && u.nEstado != (int)Estado.Eliminado && u.nTipo == (int)TipoFuncionario.Colaborador select u).FirstOrDefault();
                    if (entidad != null)
                    {
                        if (entidad.Usuario != null)
                        {
                            entidad.Usuario = LUsuarioBackEnd.ObtenerPerfilesPermisos(entidad.Usuario);
                        }
                        else
                        {
                            throw new BussinessException("El funcionario no tiene usuario asignado.");
                        }

                        return entidad;
                    }
                    else
                    {
                        throw new BussinessException("Nombre de <b>Usuario</b> o <b>Contraseña</b> incorrectos");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BussinessException("Error: " + ex.Message);
            }
        }


        public decimal? ObtenerCalificacionColaborador(long funcionarioId)
        {
            try
            {
                using (var esquema = GetEsquema())
                {
                    Funcionario funarionario = ObtenerColaborador(funcionarioId);
                    if (funarionario==null)
                    {
                        return 0;
                    }
                    return funarionario.nPuntaje;
                }
            }
            catch (Exception ex)
            {
                throw new BussinessException("Error: " + ex.Message);
            }
        }



        public decimal? ObtenerPromediosTodasCalificaciones()
        {
            try
            {
                using (var esquema = GetEsquema())
                {
                    decimal? totalPuntaje = 0;
                    int n = 0;
                    var entidades = (from u in esquema.Funcionario where 
                                   u.nEstado != (int)Estado.Eliminado && u.nTipo == (int)TipoFuncionario.Colaborador
                                   select u).ToList();
                    foreach (var item in entidades)
                    {
                        if (item.nPuntaje != 0)
                        {
                            totalPuntaje = item.nPuntaje + totalPuntaje;
                            n = n+1;
                        }
                    }
                    return n == 0 || totalPuntaje ==0 ? 0: totalPuntaje / n;
                }
            }
            catch (Exception ex)
            {
                throw new BussinessException("Error: " + ex.Message);
            }
        }
        public Funcionario Obtener(dbFexpoCruzEntities esquema, long Id)
        {
            try
            {
                var entidad = (from u in esquema.Funcionario where u.nFuncionarioId == Id && u.nEstado != (int)Estado.Eliminado select u).FirstOrDefault();
                if (entidad != null)
                {
                    return entidad;
                }
                else
                {
                    throw new BussinessException("No se encuentra el funcionario solicitado.");
                }
            }
            catch (Exception ex)
            {
                throw new BussinessException("Error: " + ex.Message);
            }
        }


        public void CompletarInversamenteR(ICollection<Permiso> permisos, Permiso Permiso)
        {
            if (Permiso != null)
            {
                CompletarInversamenteR(permisos, Permiso.Padre);
                if (!permisos.Any(x => x.PermisoId == Permiso.PermisoId))
                {
                    permisos.Add(Permiso);
                }
            }
        }

        public void CompletarPermisosR(ICollection<Permiso> permisos, Usuario Entidad, Permiso Padre)
        {
            bool existeAlMenosUnHijo = false;
            foreach (var pitem in Padre.Hijos)
            {
                if (Entidad.Permiso.Any(x => x.PermisoId == pitem.PermisoId))
                {
                    existeAlMenosUnHijo = true;
                    break;
                }
            }
            if (!existeAlMenosUnHijo)
            {
                foreach (var pitem in Padre.Hijos)
                {
                    permisos.Add(pitem);
                    CompletarPermisosR(permisos, Entidad, pitem);
                }
            }
        }

        public List<EFuncionario> ObtenerHabilitados(long? id = null)
        {
            using (var esquema = GetEsquema())
            {
                List<Funcionario> resultado = (from x in esquema.Funcionario
                                               where x.nEstado == (int)Estado.Habilitado && x.nTipo == (int)TipoFuncionario.AplicacionMovil || x.nFuncionarioId == id
                                               orderby x.sNombre ascending
                                               select x).ToList();
                List<Funcionario> resultado2 = new List<Funcionario>();
                foreach (var item in resultado)
                {
                    resultado2.Add(new Funcionario()
                    {
                        sNombre= item.sNombre + " " + item.sApellido,
                        sCi = item.sCi,
                        nFuncionarioId = item.nFuncionarioId
                    });

                }

                return Convert(resultado2);
            }
        }


        public EFuncionario ObtenerFuncionario(long id)
        {
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.Funcionario
                                                  where x.nEstado == (int)Estado.Habilitado && x.nFuncionarioId == id
                                                  orderby x.sNombre ascending
                                                  select x).FirstOrDefault();
                return Convert(resultado);
            }
        }

        public Funcionario Nuevo(Funcionario Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad, esquema);

                    ICollection<Permiso> permisos = new HashSet<Permiso>();
                    foreach (var item in Entidad.Usuario.Permiso)
                    {
                        Permiso p = (from c in esquema.Permiso where c.PermisoId == item.PermisoId select c).FirstOrDefault();
                        if (p != null)
                        {
                            CompletarInversamenteR(permisos, p);
                            CompletarPermisosR(permisos, Entidad.Usuario, p);
                        }
                    }
                    Entidad.Usuario.Permiso = permisos;

                    ICollection<Perfil> perfiles = new HashSet<Perfil>();
                    foreach (var item in Entidad.Usuario.Perfil)
                    {
                        Perfil p = (from c in esquema.Perfil where c.PerfilId == item.PerfilId select c).FirstOrDefault();
                        if (p != null)
                        {
                            perfiles.Add(p);
                        }
                    }
                    Entidad.Usuario.Perfil = perfiles;

                    if (!string.IsNullOrWhiteSpace(Entidad.sFotoPefil))
                    {

                        Entidad.sFotoPefil = EUtils.InsertarImagen("Funcionarios", Entidad.sFotoPefil);
                    }

                    Entidad.Usuario.PrimeraVez = true;
                    Entidad.Usuario.Estado = (int)Estado.Habilitado;
                    Entidad.Usuario.FechaCreacion = EUtils.Now;
                    Entidad.Usuario.FechaModificacion = EUtils.Now;
                    Entidad.Usuario.UsuarioCreadorId = UsuarioId;
                    Entidad.Usuario.UsuarioModificadorId = UsuarioId;
                    Entidad.Usuario.AccesoAplicacion = false;

                    Entidad.nEstado = (int)Estado.Habilitado;
                    Entidad.dtFechaCreacion = EUtils.Now;
                    Entidad.dtFechaModificacion = EUtils.Now;
                    Entidad.nUsuarioCreador = UsuarioId;
                    Entidad.nUsuarioModificador = UsuarioId;
                    Entidad.nTipo = (int)TipoFuncionario.Administrador;


                    esquema.Funcionario.Add(Entidad);
                    esquema.SaveChanges();

                    RegistrarExitoEnBitacora(esquema, UsuarioId, Entidad.nFuncionarioId, AccionBitacora.Creacion, EntidadToDetalleBitacora(Entidad));
                    return Entidad;
                }
                catch (BussinessException ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Creacion, EntidadToDetalleBitacora(Entidad), ex);
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Creacion, EntidadToDetalleBitacora(Entidad), ex);
                    throw ex;
                }
            }
        }


        public Funcionario NuevoColaborador(Funcionario Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    ValidarColaborador(Entidad, esquema);

                    ICollection<Permiso> permisos = new HashSet<Permiso>();
                    foreach (var item in Entidad.Usuario.Permiso)
                    {
                        Permiso p = (from c in esquema.Permiso where c.PermisoId == item.PermisoId select c).FirstOrDefault();
                        if (p != null)
                        {
                            CompletarInversamenteR(permisos, p);
                            CompletarPermisosR(permisos, Entidad.Usuario, p);
                        }
                    }
                    Entidad.Usuario.Permiso = permisos;

                    ICollection<Perfil> perfiles = new HashSet<Perfil>();
                    foreach (var item in Entidad.Usuario.Perfil)
                    {
                        Perfil p = (from c in esquema.Perfil where c.PerfilId == item.PerfilId select c).FirstOrDefault();
                        if (p != null)
                        {
                            perfiles.Add(p);
                        }
                    }
                    Entidad.Usuario.Perfil = perfiles;

                    if (!string.IsNullOrWhiteSpace(Entidad.sFotoPefil))
                    {

                        Entidad.sFotoPefil = EUtils.InsertarImagen("Funcionarios", Entidad.sFotoPefil);
                    }

                    Entidad.Usuario.PrimeraVez = true;
                    Entidad.Usuario.Estado = (int)Estado.Habilitado;
                    Entidad.Usuario.FechaCreacion = EUtils.Now;
                    Entidad.Usuario.FechaModificacion = EUtils.Now;
                    Entidad.Usuario.UsuarioCreadorId = UsuarioId;
                    Entidad.Usuario.UsuarioModificadorId = UsuarioId;
                    Entidad.Usuario.AccesoAplicacion = false;

                    Entidad.nEstado = (int)Estado.Habilitado;
                    Entidad.dtFechaCreacion = EUtils.Now;
                    Entidad.dtFechaModificacion = EUtils.Now;
                    Entidad.nUsuarioCreador = UsuarioId;
                    Entidad.nUsuarioModificador = UsuarioId;
                    Entidad.nTipo = (int)TipoFuncionario.Colaborador;
                    Entidad.nPuntaje = 0;

                    esquema.Funcionario.Add(Entidad);
                    esquema.SaveChanges();

                    RegistrarExitoEnBitacora(esquema, UsuarioId, Entidad.nFuncionarioId, AccionBitacora.Creacion, EntidadToDetalleBitacora(Entidad));
                    return Entidad;
                }
                catch (BussinessException ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Creacion, EntidadToDetalleBitacora(Entidad), ex);
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Creacion, EntidadToDetalleBitacora(Entidad), ex);
                    throw ex;
                }
            }
        }


        public Funcionario Modificar(Funcionario EntidadNueva, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(EntidadNueva, esquema);

                    Funcionario Entidad = (from e in esquema.Funcionario where e.nFuncionarioId == EntidadNueva.nFuncionarioId && e.nTipo == (int)TipoFuncionario.Administrador select e).FirstOrDefault();
                    if (Entidad == null)
                    {
                        throw new BussinessException("El funcionario con Identificador " + EntidadNueva.nFuncionarioId + " no existe.");
                    }
                    Entidad.nFuncionarioId = EntidadNueva.nFuncionarioId;
                    Entidad.sNombre = EntidadNueva.sNombre;
                    Entidad.sApellido = EntidadNueva.sApellido;
                    Entidad.sCi = EntidadNueva.sCi;
                    Entidad.nSexo = EntidadNueva.nSexo;

                    Entidad.Usuario.Permiso.Clear();
                    esquema.SaveChanges();
                    ICollection<Permiso> permisos = new HashSet<Permiso>();
                    foreach (var item in EntidadNueva.Usuario.Permiso)
                    {
                        Permiso p = (from c in esquema.Permiso where c.PermisoId == item.PermisoId select c).FirstOrDefault();
                        if (p != null)
                        {
                            CompletarInversamenteR(permisos, p);
                            CompletarPermisosR(permisos, EntidadNueva.Usuario, p);

                        }
                    }
                    Entidad.Usuario.Permiso = permisos;
                    Entidad.Usuario.Perfil.Clear();
                    esquema.SaveChanges();
                    ICollection<Perfil> perfiles = new HashSet<Perfil>();
                    foreach (var item in EntidadNueva.Usuario.Perfil)
                    {
                        Perfil p = (from c in esquema.Perfil where c.PerfilId == item.PerfilId select c).FirstOrDefault();
                        if (p != null)
                        {
                            perfiles.Add(p);
                        }
                    }
                    Entidad.Usuario.Perfil = perfiles;
                    if (!string.IsNullOrWhiteSpace(EntidadNueva.sFotoPefil))
                    {

                        Entidad.sFotoPefil = EUtils.InsertarImagen("Funcionarios", EntidadNueva.sFotoPefil);
                    }

                    Entidad.Usuario.AccesoAplicacion = EntidadNueva.Usuario.AccesoAplicacion;

                    Entidad.Usuario.FechaModificacion = EUtils.Now;
                    Entidad.Usuario.UsuarioModificadorId = UsuarioId;


                    Entidad.Usuario.NickName = EntidadNueva.Usuario.NickName;
                    Entidad.Usuario.TipoUsuario = EntidadNueva.Usuario.TipoUsuario;
                    Entidad.dtFechaModificacion = EUtils.Now;
                    Entidad.nUsuarioModificador = UsuarioId;
                    Entidad.Usuario.AccesoAplicacion = false;

                    esquema.SaveChanges();
                    RegistrarExitoEnBitacora(esquema, UsuarioId, Entidad.nFuncionarioId, AccionBitacora.Modificacion, EntidadToDetalleBitacora(Entidad));
                    return Entidad;
                }
                catch (BussinessException ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Modificacion, EntidadToDetalleBitacora(EntidadNueva), ex);
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Modificacion, EntidadToDetalleBitacora(EntidadNueva), ex);
                    throw ex;
                }
            }
        }

        public Funcionario ModificarColaborador(Funcionario EntidadNueva, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    ValidarColaborador(EntidadNueva, esquema);

                    Funcionario Entidad = (from e in esquema.Funcionario where e.nFuncionarioId == EntidadNueva.nFuncionarioId && e.nTipo == (int)TipoFuncionario.Colaborador select e).FirstOrDefault();
                    if (Entidad == null)
                    {
                        throw new BussinessException("El funcionario con Identificador " + EntidadNueva.nFuncionarioId + " no existe.");
                    }
                    Entidad.nFuncionarioId = EntidadNueva.nFuncionarioId;
                    Entidad.sNombre = EntidadNueva.sNombre;
                    Entidad.sApellido = EntidadNueva.sApellido;
                    Entidad.sCi = EntidadNueva.sCi;
                    Entidad.nSexo = EntidadNueva.nSexo;
                    Entidad.nCiudadId = EntidadNueva.nCiudadId;
                    Entidad.nAreaId = EntidadNueva.nAreaId;

                    Entidad.Usuario.Permiso.Clear();
                    esquema.SaveChanges();
                    ICollection<Permiso> permisos = new HashSet<Permiso>();
                    foreach (var item in EntidadNueva.Usuario.Permiso)
                    {
                        Permiso p = (from c in esquema.Permiso where c.PermisoId == item.PermisoId select c).FirstOrDefault();
                        if (p != null)
                        {
                            CompletarInversamenteR(permisos, p);
                            CompletarPermisosR(permisos, EntidadNueva.Usuario, p);

                        }
                    }
                    Entidad.Usuario.Permiso = permisos;
                    Entidad.Usuario.Perfil.Clear();
                    esquema.SaveChanges();
                    ICollection<Perfil> perfiles = new HashSet<Perfil>();
                    foreach (var item in EntidadNueva.Usuario.Perfil)
                    {
                        Perfil p = (from c in esquema.Perfil where c.PerfilId == item.PerfilId select c).FirstOrDefault();
                        if (p != null)
                        {
                            perfiles.Add(p);
                        }
                    }
                    Entidad.Usuario.Perfil = perfiles;
                    if (!string.IsNullOrWhiteSpace(EntidadNueva.sFotoPefil))
                    {

                        Entidad.sFotoPefil = EUtils.InsertarImagen("Funcionarios", EntidadNueva.sFotoPefil);
                    }

                    Entidad.Usuario.AccesoAplicacion = EntidadNueva.Usuario.AccesoAplicacion;

                    Entidad.Usuario.FechaModificacion = EUtils.Now;
                    Entidad.Usuario.UsuarioModificadorId = UsuarioId;


                    Entidad.Usuario.NickName = EntidadNueva.Usuario.NickName;
                    Entidad.Usuario.TipoUsuario = EntidadNueva.Usuario.TipoUsuario;
                    Entidad.dtFechaModificacion = EUtils.Now;
                    Entidad.nUsuarioModificador = UsuarioId;
                    Entidad.Usuario.AccesoAplicacion = false;

                    esquema.SaveChanges();
                    RegistrarExitoEnBitacora(esquema, UsuarioId, Entidad.nFuncionarioId, AccionBitacora.Modificacion, EntidadToDetalleBitacora(Entidad));
                    return Entidad;
                }
                catch (BussinessException ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Modificacion, EntidadToDetalleBitacora(EntidadNueva), ex);
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Modificacion, EntidadToDetalleBitacora(EntidadNueva), ex);
                    throw ex;
                }
            }
        }

        public void Validar(Funcionario Entidad, dbFexpoCruzEntities esquema)
        {
            if (string.IsNullOrWhiteSpace(Entidad.sNombre))
            {
                throw new BussinessException("Ingrese un nombre válido.");
            }
            if (string.IsNullOrWhiteSpace(Entidad.sApellido))
            {
                throw new BussinessException("Ingrese un apellido válido.");
            }
            if (string.IsNullOrWhiteSpace(Entidad.sCi))
            {
                throw new BussinessException("Ingrese un documento de identidad válido.");
            }
            if (Entidad.nSexo < 0 || Entidad.nSexo > 2)
            {
                throw new BussinessException("Seleccione un género válido.");
            }

            if (string.IsNullOrWhiteSpace(Entidad.Usuario.NickName))
            {
                throw new BussinessException("Ingrese un usuario válido.");
            }
            if (Entidad.Usuario.Perfil == null || Entidad.Usuario.Perfil.Count == 0)
            {
                throw new BussinessException("Debe asignar al menos un perfil al funcionario.");
            }
            Usuario usss = (from p in esquema.Usuario where p.UsuarioId != Entidad.Usuario.UsuarioId && p.NickName == Entidad.Usuario.NickName && p.Estado != (int)Estado.Eliminado select p).FirstOrDefault();
            if (usss != null)
            {
                StringBuilder builder = new StringBuilder();
                if (usss.NickName == Entidad.Usuario.NickName)
                {
                    builder.Append("El NickName de usuario ya existe.");
                }
                throw new BussinessException(builder.ToString());
            }
        }


        public void ValidarColaborador(Funcionario Entidad, dbFexpoCruzEntities esquema)
        {
            if (string.IsNullOrWhiteSpace(Entidad.sNombre))
            {
                throw new BussinessException("Ingrese un nombre válido.");
            }
            if (string.IsNullOrWhiteSpace(Entidad.sApellido))
            {
                throw new BussinessException("Ingrese un apellido válido.");
            }
            if (string.IsNullOrWhiteSpace(Entidad.sCi))
            {
                throw new BussinessException("Ingrese un documento de identidad válido.");
            }
            if (Entidad.nSexo < 0 || Entidad.nSexo > 2)
            {
                throw new BussinessException("Seleccione un género válido.");
            }
            if (string.IsNullOrWhiteSpace(Entidad.nCiudadId.ToString()))
            {
                throw new BussinessException("La ciudad es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(Entidad.nAreaId.ToString()))
            {
                throw new BussinessException("El area es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(Entidad.Usuario.NickName))
            {
                throw new BussinessException("Ingrese un usuario válido.");
            }
            if (Entidad.Usuario.Perfil == null || Entidad.Usuario.Perfil.Count == 0)
            {
                throw new BussinessException("Debe asignar al menos un perfil al funcionario.");
            }
            Usuario usss = (from p in esquema.Usuario where p.UsuarioId != Entidad.Usuario.UsuarioId && p.NickName == Entidad.Usuario.NickName && p.Estado != (int)Estado.Eliminado select p).FirstOrDefault();
            if (usss != null)
            {
                StringBuilder builder = new StringBuilder();
                if (usss.NickName == Entidad.Usuario.NickName)
                {
                    builder.Append("El NickName de usuario ya existe.");
                }
                throw new BussinessException(builder.ToString());
            }
        }
        

        public string ObtenerImagenBase64Empleado(long id)
        {
            using (var esquema = GetEsquema())
            {
                Funcionario entidad = Obtener(esquema, id);
                if (entidad == null)
                {
                    return "";
                }
                return EUtils.ObtenerImagenBase64(entidad.sFotoPefil);
            }
        }



        public int getNameDayForDate(DateTime fecha)
        {
            DateTimeFormatInfo info = new DateTimeFormatInfo();
            var dia = fecha.DayOfWeek;
            string nombreDia = info.GetDayName(dia);
            if (nombreDia=="Monday")
            {
                return 1;
            }
            else
            if (nombreDia == "Tuesday")
            {
                return 2;
            }
            else
            if (nombreDia == "Wednesday")
            {
                return 3;
            }
            else
            if (nombreDia == "Thursday")
            {
                return 4;
            }
            else
            if (nombreDia == "Friday")
            {
                return 5;
            }
            else
            if (nombreDia == "Saturday")
            {
                return 6;
            }  else{
                return 7;
            }
        }

     

        public bool validarHorarioProfesional(dbFexpoCruzEntities esquema, Funcionario colaborador, DateTime Fecha, TimeSpan HoraInicio, TimeSpan HoraFin)
        {
            int dia = getNameDayForDate(Fecha);
            var ColaboradorId = colaborador.nFuncionarioId;
            List<HorarioColaborador> horarioDeProfesional = (from x in esquema.HorarioColaborador
                                                     where x.nEstado == (int)Estado.Habilitado &&
                                                     x.nDiaId == dia && x.nFuncionarioId == ColaboradorId
                                                     select x).ToList();
            var resultado = false;
            if (horarioDeProfesional != null)
            {
                if (horarioDeProfesional.Count > 0)
                {
                    foreach (HorarioColaborador horario in horarioDeProfesional)
                    {
                        if ((horario.tHoraInicio <= HoraInicio && HoraInicio <= horario.tHoraFin) && (horario.tHoraInicio <= HoraFin && HoraFin <= horario.tHoraFin))
                        {
                            resultado = true;
                            return resultado;
                        }
                    }
                }
            }
            return resultado;
        }

        #region Api
        public Funcionario ObtenerColaboradorCola()
        {
            using (var esquema = GetEsquema())
            {
                List<Funcionario> funcionarios = (from u in esquema.Funcionario
                             where u.nEstado == (int)Estado.Habilitado 
                             && u.nTipo == (int)TipoFuncionario.Colaborador
                             orderby u.nFuncionarioId ascending
                             select u).ToList();
                int numeroFuncionarios = funcionarios.Count - 1;
                var ultimoFuncionario = (from h in esquema.Hilo
                             join eh in esquema.EventoHilo on h.nHiloId equals eh.nHiloId
                             join e in esquema.Evento on eh.nEventoId equals e.nEventoId
                             join f in esquema.Funcionario on h.nFuncionarioId equals f.nFuncionarioId
                             where (e.nEstadoEventoId == (int)Entidad.Enums.EstadoDeEvento.Asignado || e.nEstadoEventoId == (int)Entidad.Enums.EstadoDeEvento.Reasignado)
                                    orderby h.nHiloId descending
                                    select f).FirstOrDefault();
                if (ultimoFuncionario!=null)
                {
                    int posUltimoFuncionario = getPosColaborador(ultimoFuncionario, funcionarios);
                    if (posUltimoFuncionario != -1)
                    {
                        posUltimoFuncionario = posUltimoFuncionario < numeroFuncionarios ? posUltimoFuncionario+1 : 0;
                        return funcionarios[posUltimoFuncionario];
                    }
                }

                if (numeroFuncionarios > 0)
                {
                    return funcionarios[0];
                }
                else
                {
                    return null;
                }

            }
        }

        public Funcionario ObtenerColaboradorColaPorCiudad(long ciudadId)
        {
            using (var esquema = GetEsquema())
            {
                List<Funcionario> funcionariosVerificarHorarios = (from u in esquema.Funcionario
                                                  where u.nEstado == (int)Estado.Habilitado
                                                  && u.nTipo == (int)TipoFuncionario.Colaborador && u.nCiudadId==ciudadId
                                                  orderby u.nFuncionarioId ascending
                                                  select u).ToList();
                List<Funcionario> funcionarios = new List<Funcionario>();
                List<long> idFuncionarios = new List<long>();

                foreach (var f in funcionariosVerificarHorarios)
                {

                    DateTime fechaActual = EUtils.Now;
                    TimeSpan horaInicio = fechaActual.TimeOfDay;
                    TimeSpan horaFin = fechaActual.TimeOfDay;

                    if (LFuncionario.Instancia.LFuncionario.validarHorarioProfesional(esquema, f, fechaActual, horaInicio,horaFin))
                    {
                        funcionarios.Add(f);
                        idFuncionarios.Add(f.nFuncionarioId);
                    }
                }
                int numeroFuncionarios = funcionarios.Count-1;
                var ultimoFuncionario = (from h in esquema.Hilo
                                         join eh in esquema.EventoHilo on h.nHiloId equals eh.nHiloId
                                         join e in esquema.Evento on eh.nEventoId equals e.nEventoId
                                         join f in esquema.Funcionario on h.nFuncionarioId equals f.nFuncionarioId
                                         where (e.nEstadoEventoId == (int)Entidad.Enums.EstadoDeEvento.Asignado || e.nEstadoEventoId == (int)Entidad.Enums.EstadoDeEvento.Reasignado)
                                         && f.nCiudadId==ciudadId
                                         && idFuncionarios.Contains(f.nFuncionarioId)
                                         orderby h.nHiloId descending
                                         select f).FirstOrDefault();
                if (ultimoFuncionario != null && funcionarios.Count > 0)
                {
                    int posUltimoFuncionario = getPosColaborador(ultimoFuncionario, funcionarios);
                    if (posUltimoFuncionario != -1)
                    {
                        posUltimoFuncionario = posUltimoFuncionario < numeroFuncionarios ? posUltimoFuncionario + 1 : 0;
                        return funcionarios[posUltimoFuncionario];
                    }
                }

                if (funcionarios.Count > 0)
                {
                    return funcionarios[0];
                }
                else
                {
                    return null;
                }

            }
        }
        public int getPosColaborador(Funcionario ultimoFuncionario,List<Funcionario> funcionarios)
        {
            int pos = -1;
            for (int i=0;i< funcionarios.Count;i++)
            {
                if (funcionarios[i].nFuncionarioId==ultimoFuncionario.nFuncionarioId)
                {
                    return i;
                }
            }
            return pos;
        }

        #endregion
    }
}
