using DataP;
using Entidad;
using Entidad.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Logica
{
    public class LUsuarioBackEnd : LBaseCC<Usuario>
    {
        public static string urlRedirectEvento { get { return ConfigurationManager.AppSettings.Get("urlRedirectEvento"); } }
        public bool TieneAcceso(Usuario usuario, string controller)
        {
            foreach (var perfil in usuario.Perfil)
            {
                if (perfil.Permiso.Any(x => x.Pantalla != null && x.Pantalla.Split('/')[0].ToUpper().Equals(controller.ToUpper())))
                {
                    return true;
                }
            }
            if (usuario.Permiso.Any(x => x.Pantalla != null && x.Pantalla.Split('/')[0].ToUpper().Equals(controller.ToUpper())))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Usuario ObtenerPerfilesPermisos(Usuario entidad)
        {
            foreach (var perfil in entidad.Perfil)
            {
                foreach (var permiso in perfil.Permiso)
                {
                    permiso.Nombre.Trim();
                }
            }
            foreach (var permiso in entidad.Permiso)
            {
                permiso.Nombre.Trim();
            }
            return entidad;
        }

        public Usuario IniciarSesion(long usuarioId)
        {
            using (var esquema = GetEsquema())
            {
                Usuario usuario = null;
                try
                {
                    usuario = (from u in esquema.Usuario
                               where u.UsuarioId == usuarioId && u.Estado == (int)Estado.Habilitado
                               select u).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw new BussinessException("Error: " + ex.Message);
                }

                if (usuario != null)
                {
                    usuario.Funcionario.sNombre.ToString();
                    usuario = ObtenerPerfilesPermisos(usuario);
                    return usuario;
                }
                else
                {
                    throw new BussinessException("Nombre de <b>Usuario</b> o <b>Contraseña</b> incorrectos");
                }
            }
        }

        public Usuario IniciarSesion(string NickName, string Password, bool UsuarioAction)
        {
            using (var esquema = GetEsquema())
            {
                Usuario usuario = null;
                try
                {
                    usuario = (from u in esquema.Usuario
                               where u.NickName == NickName && u.Password == Password && u.Estado == (int)Estado.Habilitado
                               select u).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, null, null, AccionBitacora.InicioSesion, "NickName: " + NickName + ", Password: " + Password, ex);
                    throw new BussinessException("Error: " + ex.Message);
                }
                if (usuario != null)
                {
                    if (usuario.Funcionario.nTipo == (int)TipoFuncionario.AplicacionMovil && usuario.TipoUsuario == (int)TipoUsuario.TipoNormal)
                    {
                        RegistrarErrorEnBitacora(esquema, null, null, AccionBitacora.InicioSesion, "NickName: " + NickName + ", Password: " + Password, "Intento de sesion de un usuario de aplicacion movil.");
                        throw new BussinessException("Error: " + "Acceso no disponible.");
                    }
                    if (UsuarioAction)
                    {
                        RegistrarExitoEnBitacora(esquema, usuario.UsuarioId, usuario.UsuarioId, AccionBitacora.InicioSesion, "NickName: " + NickName + ", Password: " + Password);
                    }
                    usuario.NickName.ToString();
                    return ObtenerPerfilesPermisos(usuario);
                }
                else
                {
                    RegistrarErrorEnBitacora(esquema, null, null, AccionBitacora.InicioSesion, "NickName: " + NickName + ", Password: " + Password);
                    throw new BussinessException("Nombre de <b>Usuario</b> o <b>Contraseña</b> incorrectos");
                }
            }
        }
        public List<EMenu> DevolverMenu(Usuario usr)
        {
            using (var esquema = GetEsquema())
            {
                List<Permiso> perms = new List<Permiso>();
                Usuario usuario = esquema.Usuario.Where(x => x.UsuarioId == usr.UsuarioId).FirstOrDefault();
                foreach (var per in usuario.Perfil)
                {
                    List<Permiso> Permisos = per.Permiso.Where(p => p.Menu && p.Tipo >= usr.TipoUsuario && p.PermisoPadreId == null).OrderBy(p => p.Orden).ToList();
                    DevolverMenuR(esquema, usr, perms, Permisos, per.Permiso.ToList(), usr.TipoUsuario);
                }
                List<Permiso> permisosDirectos = usuario.Permiso.Where(p => p.Tipo >= usr.TipoUsuario && p.Menu && p.PermisoPadreId == null).OrderBy(p => p.Orden).ToList();
                DevolverMenuR(esquema, usr, perms, permisosDirectos, usuario.Permiso.ToList(), usr.TipoUsuario);
                perms = perms.OrderBy(x => x.Orden).ToList();
                List<EMenu> menu = new List<EMenu>();
                foreach (var item in perms)
                {
                    menu.Add(new EMenu(item));
                }
                return menu;
            }
        }

        public void DevolverMenuR(dbFexpoCruzEntities esquema, Usuario usr, List<Permiso> PermisosReturn, List<Permiso> Permisos, List<Permiso> PermisosPerfil, int TipoUsuarioL)
        {
            foreach (var item in Permisos)
            {
                if (PermisosPerfil.Where(p => p.PermisoId == item.PermisoId).Count() > 0)
                {
                    List<Permiso> ppps = (from x in esquema.Permiso where x.PermisoPadreId == item.PermisoId && x.Tipo >= usr.TipoUsuario orderby x.Orden select x).ToList();
                    List<Permiso> hijos = new List<Permiso>();
                    DevolverMenuR(esquema, usr, hijos, ppps, PermisosPerfil, TipoUsuarioL);
                    var itemAux = PermisosReturn.Where(pp => pp.PermisoId == item.PermisoId).FirstOrDefault();
                    if (itemAux == null)
                    {
                        itemAux = item;
                        PermisosReturn.Add(itemAux);
                        itemAux.Hijos = hijos;
                    }
                    else
                    {
                        foreach (var item1 in hijos)
                        {
                            itemAux.Hijos.Add(item1);
                        }
                    }
                }
            }
        }

        public Usuario CambiarContrasena(long UsuarioId, string Password, string NuevaContrasena, bool resetear = false)
        {
            using (var esquema = GetEsquema())
            {
                try
                {

                    var usuario = (from u in esquema.Usuario where u.UsuarioId == UsuarioId && u.Password == Password && u.Estado == (int)Estado.Habilitado select u).FirstOrDefault();
                    if (usuario != null)
                    {
                        usuario.Password = NuevaContrasena;
                        usuario.PrimeraVez = resetear;
                        esquema.SaveChanges();
                        RegistrarExitoEnBitacora(esquema, UsuarioId, UsuarioId, AccionBitacora.CambioPassword, "UsuarioId: " + UsuarioId + ", NewPassword: " + NuevaContrasena);
                        usuario.Funcionario.sNombre.ToString();
                        return ObtenerPerfilesPermisos(usuario);
                    }
                    else
                    {
                        RegistrarErrorEnBitacora(esquema, UsuarioId, UsuarioId, AccionBitacora.CambioPassword, "UsuarioId: " + UsuarioId + ", Password: " + Password);
                        throw new BussinessException("Nombre de <b>Usuario</b> o <b>Contraseña</b> incorrectos");
                    }
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, UsuarioId, AccionBitacora.CambioPassword, "Password: " + Password + ", NewPassword: " + NuevaContrasena, ex);
                    throw new BussinessException("Error: " + ex.Message);
                }
            }
        }

        public List<Usuario> ObtenerUsuarios()
        {
            using (var esquema = GetEsquema())
            {
                List<Usuario> usuarios = (from u in esquema.Usuario where u.Estado != (int)Estado.Eliminado select u).ToList();
                foreach (var us in usuarios)
                {
                    us.NickName.ToString();
                }
                return usuarios;
            }
        }

        public List<Usuario> ObtenerUsuarios(string tipo, string Login, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                var usuarios = new List<Usuario>();
                if (tipo == "0")
                {
                    usuarios = (from u in esquema.Usuario
                                where u.NickName.Contains(Login) && u.Estado == (int)Estado.Habilitado && u.TipoUsuario == (int)TipoUsuario.TipoNormal
                                select u).ToList();
                }
                else if (tipo == "1")
                {
                    usuarios = (from u in esquema.Usuario
                                where u.NickName.Contains(Login) && u.Estado == (int)Estado.Desabilitado && u.TipoUsuario == (int)TipoUsuario.TipoNormal
                                select u).ToList();
                }
                else
                {
                    usuarios = (from u in esquema.Usuario
                                where u.NickName.Contains(Login) && u.Estado != (int)Estado.Eliminado && u.TipoUsuario == (int)TipoUsuario.TipoNormal
                                select u).ToList();
                }
                foreach (var us in usuarios)
                {
                    us.NickName.ToString();
                }
                return usuarios;
            }
        }

       
        public Usuario ObtenerUsuario(long UsuarioId, long UsuarioLogueadoId)
        {
            try
            {
                using (var esquema = GetEsquema())
                {
                    var usuario = (from u in esquema.Usuario where u.UsuarioId == UsuarioId && u.Estado != (int)Estado.Eliminado select u).FirstOrDefault();
                    if (usuario != null)
                    {
                        Usuario user = ObtenerPerfilesPermisos(usuario);
                        user.NickName.ToString();
                        return user;
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


        public void RegistrarTokenWeb(long UsuarioId, string token )
        {
            using (var esquema = GetEsquema())
            {
                var usuario = (from u in esquema.Usuario where u.UsuarioId == UsuarioId && u.Estado != (int)Estado.Eliminado select u).FirstOrDefault();
                if (usuario != null)
                {
                    usuario.sLastPushIdWeb = token;
                    esquema.SaveChanges();
                }
                else
                {
                    throw new BussinessException("Nombre de <b>Usuario</b> o <b>Contraseña</b> incorrectos");
                }
            }
        }


        public List<Usuario> getAdministradoresConPermisoSolicitud()
        {
            List<Usuario> respuesta = new List<Usuario>();
            using (var esquema = GetEsquema())
            {

                List<Usuario> usuarios = (from u in esquema.Usuario
                                          where u.Estado == (int)Estado.Habilitado
                                          select u).ToList();

                foreach (Usuario usuario in usuarios)
                {
                    
                    bool encontrado = false;
                    foreach (var item in usuario.Perfil)
                    {
                        foreach (var p in item.Permiso)
                        {
                            if (p.Pantalla == "Solicitud/Index")
                            {
                                encontrado = true;
                                break;
                            }
                        }
                    }

                    if (encontrado)
                    {
                        respuesta.Add(usuario);
                    }

                }
            }

            return respuesta;
        }



        public bool verificarPermisoSolicitud(long UsuarioId)
        {
            bool resultado = false;
            using (var esquema = GetEsquema())
            {

                Usuario usuario = (from u in esquema.Usuario
                                            where u.Estado == (int)Estado.Habilitado && u.UsuarioId== UsuarioId
                                          select u).FirstOrDefault();
                if (usuario!=null)
                {
                    foreach (var item in usuario.Perfil)
                    {
                        foreach (var p in item.Permiso)
                        {
                            if (p.Pantalla== "Solicitud/Index")
                            {
                                resultado = true;
                                break;
                            }
                        }
                    }

                }
            }
            return resultado;
        }
        public Usuario Habilitar(long UsuarioId, long UsuarioLogueadoId)
        {
            Usuario usuario = ModificarEstado(UsuarioId, Estado.Habilitado, UsuarioLogueadoId, AccionBitacora.Modificacion);
            usuario.NickName.ToString();
            return usuario;
        }

        private string EntidadToDetalleBitacora(Usuario Entidad)
        {
            return Entidad.NickName + "|" + Entidad.TipoUsuario + "|" + Entidad.Estado + "|" + Entidad.NickName + "|";
        }

        public Usuario ModificarEstado(long UsuarioId, Estado Estado, long UsuarioLogueadoId, AccionBitacora AccionBitacora)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    var entidad = (from u in esquema.Usuario where u.UsuarioId == UsuarioId && u.Estado != (int)Estado.Eliminado select u).FirstOrDefault();
                    if (entidad != null)
                    {
                        entidad.Estado = (int)Estado;
                        entidad.UsuarioModificadorId = UsuarioLogueadoId;
                        esquema.SaveChanges();
                        RegistrarExitoEnBitacora(esquema, UsuarioLogueadoId, UsuarioId, AccionBitacora, EntidadToDetalleBitacora(entidad));
                        entidad.NickName.ToString();
                        return entidad;
                    }
                    else
                    {
                        throw new BussinessException("Usuario con Id " + UsuarioId + " no existe.");
                    }
                }
                catch (BussinessException ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioLogueadoId, UsuarioId, AccionBitacora, ex);
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioLogueadoId, UsuarioId, AccionBitacora, ex);
                    throw ex;
                }
            }

        }

        public Usuario Deshabilitar(long UsuarioId, long UsuarioLogueadoId)
        {
            Usuario usuario = ModificarEstado(UsuarioId, Estado.Desabilitado, UsuarioLogueadoId, AccionBitacora.Modificacion);
            usuario.NickName.ToString();
            return usuario;
        }

        public Usuario Eliminar(long UsuarioId, long UsuarioLogueadoId)
        {
            Usuario usuario = ModificarEstado(UsuarioId, Estado.Eliminado, UsuarioLogueadoId, AccionBitacora.Eliminacion);
            usuario.NickName.ToString();
            return usuario;
        }

        public List<Usuario> ObtenerUsuariosHabilitados(long UsuarioLogueadoId)
        {
            return ObtenerUsuarios("0", "", UsuarioLogueadoId);
        }

        public Usuario ModificarUsuario(Usuario entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Usuario usuario = (from u in esquema.Usuario where u.UsuarioId == entidad.UsuarioId select u).FirstOrDefault();
                    if (usuario != null)
                    {
                        Usuario usss = (from p in esquema.Usuario where p.NickName == entidad.NickName && p.Estado != (int)Estado.Eliminado && p.UsuarioId != usuario.UsuarioId select p).FirstOrDefault();
                        if (usss != null)
                        {
                            StringBuilder builder = new StringBuilder();
                            if (usss.NickName == entidad.NickName)
                            {
                                builder.Append("El NickName de usuario ya existe.");
                            }
                            throw new BussinessException(builder.ToString());
                        }
                        usuario.Perfil.Clear();
                        usuario.Permiso.Clear();
                        esquema.SaveChanges();
                        ICollection<Permiso> permisos = new HashSet<Permiso>();
                        foreach (var item in entidad.Permiso)
                        {
                            Permiso p = (from c in esquema.Permiso where c.PermisoId == item.PermisoId select c).FirstOrDefault();
                            if (p != null)
                            {
                                permisos.Add(p);
                                CompletarPermisosR(permisos, entidad, p);
                            }
                        }
                        entidad.Permiso = permisos;

                        ICollection<Perfil> perfiles = new HashSet<Perfil>();
                        foreach (var item in entidad.Perfil)
                        {
                            Perfil p = (from c in esquema.Perfil where c.PerfilId == item.PerfilId select c).FirstOrDefault();
                            if (p != null)
                            {
                                perfiles.Add(p);
                            }
                        }
                        entidad.Perfil = perfiles;

                        usuario.NickName = entidad.NickName;
                        usuario.Perfil = entidad.Perfil;
                        usuario.Permiso = entidad.Permiso;
                        usuario.TipoUsuario = entidad.TipoUsuario;
                        usuario.FechaModificacion = EUtils.Now;
                        usuario.UsuarioModificadorId = UsuarioId;
                        esquema.SaveChanges();
                        RegistrarExitoEnBitacora(esquema, UsuarioId, usuario.UsuarioId, AccionBitacora.Modificacion, EntidadToDetalleBitacora(usuario));
                        usuario.Funcionario.sNombre.ToString();
                        usuario.NickName.ToString();
                        return usuario;
                    }
                    else
                    {
                        throw new BussinessException("Nombre de <b>Usuario</b> o <b>Contraseña</b> incorrectos");
                    }
                }
                catch (BussinessException ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, entidad.UsuarioId, AccionBitacora.Modificacion, EntidadToDetalleBitacora(entidad), ex);
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, entidad.UsuarioId, AccionBitacora.Modificacion, EntidadToDetalleBitacora(entidad), ex);
                    throw ex;
                }
            }
        }

        public Usuario NuevoUsuario(Usuario entidad, long UsuarioId)
        {

            using (var esquema = GetEsquema())
            {
                try
                {
                    Usuario usss = (from p in esquema.Usuario where p.NickName == entidad.NickName && p.Estado != (int)Estado.Eliminado select p).FirstOrDefault();
                    if (usss != null)
                    {
                        StringBuilder builder = new StringBuilder();
                        if (usss.NickName == entidad.NickName)
                        {
                            builder.Append("El NickName de usuario ya existe.");
                        }
                        throw new BussinessException(builder.ToString());
                    }


                    ICollection<Permiso> permisos = new HashSet<Permiso>();
                    foreach (var item in entidad.Permiso)
                    {
                        Permiso p = (from c in esquema.Permiso where c.PermisoId == item.PermisoId select c).FirstOrDefault();
                        if (p != null)
                        {
                            permisos.Add(p);
                            CompletarPermisosR(permisos, entidad, p);
                        }
                    }
                    entidad.Permiso = permisos;

                    ICollection<Perfil> perfiles = new HashSet<Perfil>();
                    foreach (var item in entidad.Perfil)
                    {
                        Perfil p = (from c in esquema.Perfil where c.PerfilId == item.PerfilId select c).FirstOrDefault();
                        if (p != null)
                        {
                            perfiles.Add(p);
                        }
                    }
                    entidad.Perfil = perfiles;

                    entidad.Password = EUtils.MD5Hash(entidad.NickName + "123");
                    entidad.PrimeraVez = true;
                    entidad.TipoUsuario = (int)TipoUsuario.TipoNormal;
                    entidad.Estado = (int)Estado.Habilitado;
                    entidad.FechaCreacion = EUtils.Now;
                    entidad.FechaModificacion = EUtils.Now;
                    entidad.UsuarioCreadorId = UsuarioId;
                    entidad.UsuarioModificadorId = UsuarioId;
                    esquema.Usuario.Add(entidad);
                    esquema.SaveChanges();
                    RegistrarExitoEnBitacora(esquema, UsuarioId, entidad.UsuarioId, AccionBitacora.Creacion, EntidadToDetalleBitacora(entidad));
                    return entidad;
                }
                catch (BussinessException ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Creacion, EntidadToDetalleBitacora(entidad), ex);
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, null, AccionBitacora.Creacion, EntidadToDetalleBitacora(entidad), ex);
                    throw ex;
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


        public Usuario IniciarSesionMovil(string NickName, string Password, bool UsuarioAction)
        {
            using (var esquema = GetEsquema())
            {
                Usuario usuario = null;
                try
                {
                    usuario = (from u in esquema.Usuario
                               where u.NickName == NickName && u.Password == Password && u.Estado == (int)Estado.Habilitado && u.AccesoAplicacion == true
                               select u).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, null, null, AccionBitacora.InicioSesion, "NickName: " + NickName + ", Password: " + Password, ex);
                    throw new BussinessException("Error: " + ex.Message);
                }
                if (usuario != null)
                {
                    if (UsuarioAction)
                    {
                        RegistrarExitoEnBitacora(esquema, usuario.UsuarioId, usuario.UsuarioId, AccionBitacora.InicioSesion, "NickName: " + NickName + ", Password: " + Password);
                    }
                    usuario.NickName.ToString();

                    return usuario;
                }
                else
                {
                    RegistrarErrorEnBitacora(esquema, null, null, AccionBitacora.InicioSesion, "NickName: " + NickName + ", Password: " + Password);
                    throw new BussinessException("Nombre de Usuario o Contraseña incorrectos");
                }
            }
        }




        public List<long> RegistrarNotificacionAdministrador(long funcionarioId,string titulo, string descripcion, int idImplicado, TipoNotificacionFuncionario tipoNotificacionFuncionario)
        {
            List<long> ids = new List<long>();
            using (var esquema = GetEsquema())
            {
                
                    ids.Add(funcionarioId);
                    NotificacionFuncionario Entidad = new NotificacionFuncionario();
                    Entidad.nFuncionarioId = funcionarioId;
                    Entidad.sTitulo = titulo;
                    Entidad.sDescripcion = descripcion;
                    Entidad.sUrl = urlRedirectEvento;
                    Entidad.dFechaCreacion = EUtils.Now;
                    Entidad.nEstado = (int)Estado.Habilitado;
                    Entidad.nLeido = 0;
                    Entidad.nIdImplicado = idImplicado;
                    Entidad.nTipoNotificacion = (int)tipoNotificacionFuncionario;
                    esquema.NotificacionFuncionario.Add(Entidad);
                    esquema.SaveChanges();


            }
            return ids;
        }









    }
}
