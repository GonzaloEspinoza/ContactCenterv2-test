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
    public class LPermiso : LBaseCC<Permiso>
    {

        public static EPermiso Convert(Permiso entidad, int TipoUsuarioL, bool ParsearHijos)
        {
            EPermiso EPermiso = new EPermiso()
            {
                Descripcion = entidad.Descripcion,
                PermisoId = entidad.PermisoId,
                Nombre = entidad.Nombre,
                PermisoPadreId = entidad.PermisoPadreId,
                Orden = entidad.Orden,
                Seleccionable = entidad.Seleccionable,
                PermisosHijos = new List<EPermiso>()
            };
            if (ParsearHijos)
            {

                List<Permiso> Permisos = entidad.Hijos.Where(x => x.Seleccionable && x.Tipo >= TipoUsuarioL).OrderBy(x => x.Orden).ToList();
                foreach (var item in Permisos)
                {
                    EPermiso.PermisosHijos.Add(Convert(item, TipoUsuarioL, ParsearHijos));
                }
            }
            return EPermiso;
        }

        public static EPermiso Convert(Permiso entidad, int TipoUsuarioL, long PerfilId)
        {
            EPermiso EPermiso = new EPermiso()
            {
                Descripcion = entidad.Descripcion,
                PermisoId = entidad.PermisoId,
                Nombre = entidad.Nombre,
                PermisoPadreId = entidad.PermisoPadreId,
                Orden = entidad.Orden,
                Seleccionable = entidad.Seleccionable,
                PermisosHijos = new List<EPermiso>()
            };
            List<Permiso> Permisos = entidad.Hijos.Where(x => x.Seleccionable && x.Tipo >= TipoUsuarioL && x.Perfil.Where(y => y.PerfilId == PerfilId).Count() > 0).OrderBy(x => x.Orden).ToList();
            foreach (var item in Permisos)
            {
                EPermiso.PermisosHijos.Add(Convert(item, TipoUsuarioL, PerfilId));
            }
            return EPermiso;
        }
        public static EPermiso Convert(long UsuarioId, int TipoUsuarioL, Permiso entidad)
        {
            EPermiso EPermiso = new EPermiso()
            {
                Descripcion = entidad.Descripcion,
                PermisoId = entidad.PermisoId,
                Nombre = entidad.Nombre,
                PermisoPadreId = entidad.PermisoPadreId,
                Orden = entidad.Orden,
                Seleccionable = entidad.Seleccionable,
                PermisosHijos = new List<EPermiso>()
            };
            List<Permiso> Permisos = entidad.Hijos.Where(x => x.Seleccionable && x.Tipo >= TipoUsuarioL && x.Usuario.Where(y => y.UsuarioId == UsuarioId).Count() > 0).OrderBy(x => x.Orden).ToList();
            foreach (var item in Permisos)
            {
                EPermiso.PermisosHijos.Add(Convert(UsuarioId, TipoUsuarioL, item));
            }
            return EPermiso;
        }
        public List<Permiso> ObtenerPermisos(int TipoUsuarioL)
        {
            using (var esquema = GetEsquema())
            {
                return esquema.Permiso.Where(x => x.Tipo >= TipoUsuarioL).OrderBy(x => x.Orden).ToList();
            }
        }
        public List<Permiso> ObtenerJerarquia(int TipoUsuarioL)
        {
            using (var esquema = GetEsquema())
            {
                List<Permiso> list = (from p in esquema.Permiso where p.Tipo >= TipoUsuarioL && p.PermisoPadreId == null select p).ToList();
                foreach (var item in list)
                {
                    ObtenerHijosR(item, TipoUsuarioL);
                }
                return list;
            }
        }
        public void ObtenerHijosR(Permiso Permiso, int TipoUsuarioL)
        {
            Permiso.Nombre.Trim();
            foreach (var P in Permiso.Hijos.Where(x => x.Tipo >= TipoUsuarioL))
            {
                ObtenerHijosR(P, TipoUsuarioL);
            }
        }

        public Permiso ObtenerPermiso(long PermisoId, int TipoUsuarioL)
        {
            using (var esquema = GetEsquema())
            {
                return (from p in esquema.Permiso where p.PermisoId == PermisoId && p.Tipo >= TipoUsuarioL select p).FirstOrDefault();
            }
        }

        public string EntidadToDetalleBitacora(Permiso Entidad)
        {
            return Entidad.Nombre + "|" + Entidad.Pantalla + "|" + Entidad.Menu + "|" + Entidad.Seleccionable + "|" + Entidad.Tipo + "|" + Entidad.Descripcion + "|" + Entidad.Orden;
        }

        public Permiso NuevoPermiso(Permiso Entidad, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);

                    esquema.Permiso.Add(Entidad);
                    esquema.SaveChanges();
                    RegistrarExitoEnBitacora(esquema, UsuarioId, Entidad.PermisoId, AccionBitacora.Creacion, EntidadToDetalleBitacora(Entidad));
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

        public Permiso ModificarPermiso(Permiso Entidad, long UsuarioId)
        {

            using (var esquema = GetEsquema())
            {
                try
                {
                    Validar(Entidad);
                    Permiso EntidadEx = (from p in esquema.Permiso where p.PermisoId == Entidad.PermisoId select p).FirstOrDefault();
                    EntidadEx.Nombre = Entidad.Nombre;
                    EntidadEx.Pantalla = Entidad.Pantalla;
                    EntidadEx.Descripcion = Entidad.Descripcion;
                    EntidadEx.Menu = Entidad.Menu;
                    EntidadEx.Accion = Entidad.Accion;
                    EntidadEx.IconoMenu = Entidad.IconoMenu;
                    EntidadEx.Seleccionable = Entidad.Seleccionable;
                    EntidadEx.Tipo = Entidad.Tipo;

                    esquema.SaveChanges();
                    RegistrarExitoEnBitacora(esquema, UsuarioId, Entidad.PermisoId, AccionBitacora.Modificacion, EntidadToDetalleBitacora(Entidad));
                    return Entidad;
                }
                catch (BussinessException ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, Entidad.PermisoId, AccionBitacora.Modificacion, EntidadToDetalleBitacora(Entidad), ex);
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, Entidad.PermisoId, AccionBitacora.Modificacion, EntidadToDetalleBitacora(Entidad), ex);
                    throw ex;
                }
            }
        }

        public void GuardarJerarquia(List<Permiso> Permisos, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {

                foreach (var item in Permisos)
                {
                    GuardarOrdenR(esquema, item, UsuarioId);
                }
                esquema.SaveChanges();

            }
        }

        public void GuardarOrdenR(dbFexpoCruzEntities esquema, Permiso Entidad, long UsuarioId)
        {

            Permiso EntidadEx = (from p in esquema.Permiso where p.PermisoId == Entidad.PermisoId select p).FirstOrDefault();
            if (EntidadEx != null)
            {
                try
                {
                    EntidadEx.Orden = Entidad.Orden;
                    EntidadEx.PermisoPadreId = Entidad.PermisoPadreId;
                    if (Entidad.Hijos != null)
                    {
                        foreach (var item in Entidad.Hijos)
                        {
                            GuardarOrdenR(esquema, item, UsuarioId);
                        }
                    }
                    RegistrarExitoEnBitacora(esquema, UsuarioId, Entidad.PermisoId, AccionBitacora.Modificacion, Entidad.Orden + "|" + Entidad.PermisoPadreId);
                }
                catch (BussinessException ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, Entidad.PermisoId, AccionBitacora.Modificacion, Entidad.Orden + "|" + Entidad.PermisoPadreId, ex);
                    throw new BussinessException(ex.Message);
                }
                catch (Exception ex)
                {
                    RegistrarErrorEnBitacora(esquema, UsuarioId, Entidad.PermisoId, AccionBitacora.Modificacion, EntidadToDetalleBitacora(Entidad), ex);
                    throw ex;
                }
            }
        }

        public void EliminarPermiso(long PermisoId, long UsuarioId)
        {
            using (var esquema = GetEsquema())
            {

                Permiso EntidadEx = (from p in esquema.Permiso where p.PermisoId == PermisoId select p).FirstOrDefault();
                if (EntidadEx != null)
                {
                    try
                    {
                        if (EntidadEx.Hijos.Count > 0)
                        {
                            throw new BussinessException("El permiso es padre de otros permisos.");
                        }
                        if (EntidadEx.Perfil.Where(x => x.Estado != (int)Estado.Eliminado).Count() > 0)
                        {
                            throw new BussinessException("Hay perfiles referenciando a este permiso.");
                        }
                        if (EntidadEx.Usuario.Where(x => x.Estado != (int)Estado.Eliminado).Count() > 0)
                        {
                            throw new BussinessException("Hay usuarios referenciando a este permiso.");
                        }
                        EntidadEx.Perfil.Clear();
                        EntidadEx.Usuario.Clear();
                        esquema.Permiso.Remove(EntidadEx);
                        esquema.SaveChanges();
                        RegistrarExitoEnBitacora(esquema, UsuarioId, PermisoId, AccionBitacora.Eliminacion, EntidadEx.Orden + "|" + EntidadEx.PermisoPadreId);
                    }
                    catch (BussinessException ex)
                    {
                        RegistrarErrorEnBitacora(esquema, UsuarioId, PermisoId, AccionBitacora.Modificacion, EntidadEx.Orden + "|" + EntidadEx.PermisoPadreId, ex);
                        throw new BussinessException(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        RegistrarErrorEnBitacora(esquema, UsuarioId, EntidadEx.PermisoId, AccionBitacora.Modificacion, EntidadToDetalleBitacora(EntidadEx), ex);
                        throw ex;
                    }
                }
                else
                {
                    Exception Ex = new Exception("No existe el permisos con id " + PermisoId);
                    RegistrarErrorEnBitacora(esquema, UsuarioId, EntidadEx.PermisoId, AccionBitacora.Modificacion, EntidadToDetalleBitacora(EntidadEx), Ex);
                    throw Ex;
                }
            }
        }

        public void Validar(Permiso Entidad)
        {
            if (string.IsNullOrEmpty(Entidad.Nombre))
            {
                throw new BussinessException("El campo nombre es obligatorio.");
            }
            else if (Entidad.Nombre.Length > 100)
            {
                throw new BussinessException("La longitud maxima del nombre, debe ser 100 caracteres.");
            }
        }

        public List<Permiso> DevolverPermisoMenu(Usuario UsuarioL, string httpAction)
        {
            Permiso P = new Permiso();
            foreach (var item in UsuarioL.Perfil)
            {
                foreach (var p in item.Permiso)
                {
                    if (!string.IsNullOrWhiteSpace(p.Pantalla) && httpAction.Equals(p.Pantalla.Split('/')[0].ToUpper()))
                    {
                        P = p;
                        break;
                    }
                }
            }
            foreach (var p in UsuarioL.Permiso)
            {
                if (!string.IsNullOrWhiteSpace(p.Pantalla) && httpAction.Equals(p.Pantalla.Split('/')[0].ToUpper()))
                {
                    P = p;
                    break;
                }
            }
            List<Permiso> Hijos = new List<Permiso>();
            foreach (var item in UsuarioL.Perfil)
            {
                Hijos.AddRange(item.Permiso.Where(x => x.PermisoPadreId == P.PermisoId).ToList());
            }
            Hijos.AddRange(UsuarioL.Permiso.Where(x => x.PermisoPadreId == P.PermisoId).ToList());
            return Hijos;
        }

        public bool TienePermisoEspecifico(List<Permiso> Hijos, AccionPermiso AccionPermiso)
        {
            if (Hijos != null)
            {
                if (Hijos.Count == 0)
                {
                    return true;
                }
                else if (Hijos.Where(x => x.Accion == (int)AccionPermiso || x.Accion == (int)AccionPermiso).Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
