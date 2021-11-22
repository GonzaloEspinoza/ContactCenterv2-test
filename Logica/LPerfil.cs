using DataP;
using Entidad;
using Entidad.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica
{
    public class LPerfil : LBaseCC<Perfil>
    {
        public List<EPermiso> ObtenerPermisosSeleccionables(int TipoUsuarioL)
        {
            using (var esquema = GetEsquema())
            {
                var permisos = (from p in esquema.Permiso where p.Seleccionable && p.PermisoPadreId == null orderby p.Orden select p).ToList();
                List<EPermiso> rtnPermisos = new List<EPermiso>();
                foreach (var permiso in permisos)
                {
                    rtnPermisos.Add(LPermiso.Convert(permiso, TipoUsuarioL, true));
                }
                return rtnPermisos.ToList();
            }
        }

        public List<EPerfil> ObtenerPerfilesListar(int TipoUsuarioL)
        {
            using (var esquema = GetEsquema())
            {
                List<EPerfil> rtnPerfiles = new List<EPerfil>();
                var perfiles = (from p in esquema.Perfil where p.Estado != (int)Estado.Eliminado select p).ToList();
                foreach (var perfil in perfiles)
                {
                    EPerfil ePerfil = new EPerfil(perfil);
                    foreach (var permiso in perfil.Permiso)
                    {
                        ePerfil.ListaPermiso.Add(LPermiso.Convert(permiso, TipoUsuarioL, true));
                    }
                    ePerfil.TituloDeshabilitar = perfil.Usuario.Where(x => x.Estado != (int)Estado.Eliminado).Count() > 0 ? "No es posible deshabilitar el perfil, ya que tiene usuarios referenciados." : "";
                    ePerfil.TituloEliminar = perfil.Usuario.Where(x => x.Estado != (int)Estado.Eliminado).Count() > 0 ? "No es posible eliminar el perfil, ya que tiene usuarios referenciados." : "";
                    rtnPerfiles.Add(ePerfil);
                }
                return rtnPerfiles;
            }
        }

        public List<EPerfil> ObtenerPerfilesHabilitados(int TipoUsuarioL)
        {
            using (var esquema = GetEsquema())
            {
                List<EPerfil> rtnPerfiles = new List<EPerfil>();
                var perfiles = (from p in esquema.Perfil where p.Tipo >= TipoUsuarioL && p.Estado == (int)Estado.Habilitado && p.Seleccionable == true select p).ToList();
                foreach (var perfil in perfiles)
                {
                    EPerfil ePerfil = new EPerfil(perfil);
                    foreach (var permiso in perfil.Permiso)
                    {
                        ePerfil.ListaPermiso.Add(LPermiso.Convert(permiso, TipoUsuarioL, true));
                    }
                    rtnPerfiles.Add(ePerfil);
                }
                return rtnPerfiles;
            }
        }

        public EPerfil ObtenerPerfil(long PerfilId, int TipoUsuarioL)
        {
            using (var esquema = GetEsquema())
            {
                Perfil perfil = (from p in esquema.Perfil where p.PerfilId == PerfilId select p).FirstOrDefault();
                if (perfil != null)
                {
                    EPerfil ePerfil = new EPerfil(perfil);
                    List<Permiso> permisos = (from p in perfil.Permiso where p.Tipo >= TipoUsuarioL && p.Seleccionable && p.PermisoPadreId == null orderby p.Orden select p).ToList();
                    foreach (var permiso in permisos)
                    {
                        ePerfil.ListaPermiso.Add(LPermiso.Convert(permiso, TipoUsuarioL, PerfilId));
                    }
                    ePerfil.TituloDeshabilitar = perfil.Usuario.Where(x => x.Estado != (int)Estado.Eliminado).Count() > 0 ? "No es posible deshabilitar el perfil, ya que tiene usuarios referenciados." : "";
                    ePerfil.TituloEliminar = perfil.Usuario.Where(x => x.Estado != (int)Estado.Eliminado).Count() > 0 ? "No es posible eliminar el perfil, ya que tiene usuarios referenciados." : "";
                    return ePerfil;
                }
                else
                {
                    throw new BussinessException("Perfil con Id " + PerfilId + " no existe.");
                }
            }
        }

        private string EntidadToDetalleBitacora(Perfil Entidad)
        {
            return Entidad.Nombre + "|" + Entidad.Decripcion + "|" + Entidad.Estado + "|" + Entidad.Tipo;
        }

        public Perfil NuevoPerfil(Perfil Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    if ((from p in esquema.Perfil where p.Nombre == Entidad.Nombre && p.Estado != (int)Estado.Eliminado select p.Nombre).FirstOrDefault() != null)
                    {
                        throw new BussinessException("El nombre de perfil ya existe.");
                    }
                    ICollection<Permiso> permisos = new HashSet<Permiso>();
                    foreach (var item in Entidad.Permiso)
                    {
                        Permiso p = (from c in esquema.Permiso where c.PermisoId == item.PermisoId select c).FirstOrDefault();
                        if (p != null)
                        {
                            permisos.Add(p);
                            CompletarPermisosR(permisos, Entidad, p);
                        }
                    }
                    Entidad.Permiso = permisos;
                    Entidad.FechaCreacion = EUtils.Now;
                    Entidad.UsuarioCreadorId = UsuarioId;
                    Entidad.FechaModificacion = EUtils.Now;
                    Entidad.UsuarioModificadorId = UsuarioId;
                    esquema.Perfil.Add(Entidad);
                    esquema.SaveChanges();
                    RegistrarExitoEnBitacora(esquema, UsuarioId, Entidad.PerfilId, AccionBitacora.Creacion, EntidadToDetalleBitacora(Entidad));
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

        public Perfil ModificarPerfil(Perfil Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Perfil oPerfil = (from p in esquema.Perfil where p.PerfilId == Entidad.PerfilId select p).FirstOrDefault();
                    if (oPerfil != null)
                    {
                        if (oPerfil.Nombre != Entidad.Nombre)
                        {
                            if ((from p in esquema.Perfil where p.Nombre == Entidad.Nombre && p.Estado != (int)Estado.Eliminado select p.Nombre).FirstOrDefault() != null)
                            {
                                throw new BussinessException("El nombre de perfil ya existe.");
                            }
                        }
                    }
                    else
                    {
                        BussinessException Ex = new BussinessException("Perfil con Identificador " + Entidad.PerfilId + " no existe."); ;
                        throw Ex;
                    }
                    oPerfil.Nombre = Entidad.Nombre;
                    oPerfil.Tipo = Entidad.Tipo;
                    oPerfil.Estado = Entidad.Estado;
                    oPerfil.Decripcion = Entidad.Decripcion;
                    oPerfil.Permiso.Clear();

                    ICollection<Permiso> permisos = new HashSet<Permiso>();
                    foreach (var item in Entidad.Permiso)
                    {
                        Permiso p = (from c in esquema.Permiso where c.PermisoId == item.PermisoId select c).FirstOrDefault();
                        if (p != null)
                        {
                            permisos.Add(p);
                            CompletarPermisosR(permisos, Entidad, p);
                        }
                    }
                    oPerfil.Permiso = permisos;
                    oPerfil.FechaModificacion = EUtils.Now;
                    oPerfil.UsuarioModificadorId = UsuarioId;
                    esquema.SaveChanges();
                    RegistrarExitoEnBitacora(esquema, UsuarioId, Entidad.PerfilId, AccionBitacora.Modificacion, EntidadToDetalleBitacora(Entidad));
                    return oPerfil;
                }
                catch (BussinessException ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, Entidad.PerfilId, AccionBitacora.Modificacion, EntidadToDetalleBitacora(Entidad), ex);
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, Entidad.PerfilId, AccionBitacora.Modificacion, EntidadToDetalleBitacora(Entidad), ex);
                    throw ex;
                }
            }
        }

        public void CompletarPermisosR(ICollection<Permiso> permisos, Perfil Entidad, Permiso Padre)
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

        public EPerfil Habilitar(long PerfilId, long UsuarioId)
        {
            return ModificarEstado(PerfilId, Estado.Habilitado, UsuarioId, AccionBitacora.Habilitacion);
        }

        public EPerfil ModificarEstado(long PerfilId, Estado Estado, long UsuarioId, AccionBitacora AccionBitacora)
        {

            using (var esquema = GetEsquema())
            {
                try
                {
                    Perfil perfil = (from p in esquema.Perfil where p.PerfilId == PerfilId select p).FirstOrDefault();
                    if (Estado == Estado.Eliminado)
                    {
                        ValidarEliminacionLogica(perfil);
                    }
                    else if (Estado == (int)Estado.Desabilitado)
                    {
                        ValidarDesabilitacion(perfil);
                    }
                    if (perfil != null)
                    {
                        perfil.Estado = (int)Estado;
                        perfil.FechaModificacion = EUtils.Now;
                        perfil.UsuarioModificadorId = UsuarioId;
                        esquema.SaveChanges();
                        RegistrarExitoEnBitacora(esquema, UsuarioId, PerfilId, AccionBitacora, EntidadToDetalleBitacora(perfil));
                        return new EPerfil(perfil);
                    }
                    else
                    {
                        Exception Ex = new BussinessException("Perfil con Identificador " + PerfilId + " no existe."); ;
                        throw Ex;
                    }
                }
                catch (BussinessException ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, PerfilId, AccionBitacora, ex);
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, PerfilId, AccionBitacora, ex);
                    throw ex;
                }
            }
        }

        public EPerfil Deshabilitar(long PerfilId, long UsuarioId)
        {
            return ModificarEstado(PerfilId, Estado.Desabilitado, UsuarioId, AccionBitacora.Deshabilitacion);
        }

        public EPerfil Eliminar(long PerfilId, long UsuarioId)
        {
            return ModificarEstado(PerfilId, Estado.Eliminado, UsuarioId, AccionBitacora.Eliminacion);
        }

        public void ValidarEliminacionLogica(Perfil Perfil)
        {
            if (Perfil.Usuario.Where(x => x.Estado != (int)Estado.Eliminado).Count() > 0)
            {
                throw new BussinessException("No es posible eliminar el perfil, ya que tiene usuarios referenciados.");
            }
        }

        public void ValidarDesabilitacion(Perfil Perfil)
        {
            if (Perfil.Usuario.Where(x => x.Estado != (int)Estado.Eliminado).Count() > 0)
            {
                throw new BussinessException("No es posible deshabilitar el perfil, ya que tiene usuarios referenciados.");
            }
        }
    }
}
