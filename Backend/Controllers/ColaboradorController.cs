using Backend.Models;
using DataP;
using Entidad;
using Entidad.Enums;
using Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackEnd.Controllers
{
    public class ColaboradorController : Controller
    {
        // GET: funcionario
        private LFuncionario LFuncionario = LFuncionario.Instancia.LFuncionario;
        private LHorarioColaborador LHorarioColaborador = LHorarioColaborador.Instancia.LHorarioColaborador;
        private LDia LDia = LDia.Instancia.LDia;
        private LCiudad LCiudad = LCiudad.Instancia.LCiudad;
        private LArea LArea = LArea.Instancia.LArea;
        private LUsuarioBackEnd LUsuarioBackEnd = LUsuarioBackEnd.Instancia.LUsuarioBackEnd;
        private LPerfil LPerfil = LPerfil.Instancia.LPerfil;

        private static string Controller = "Colaborador";
        // GET: Funcionario
        public ActionResult Index()
        {
            ViewBag.Toasts = Session["Toasts"];
            Session["Toasts"] = null;
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            return View(new List<EFuncionario>());
        }



        [HttpPost]
        public ActionResult Listar(string opcion)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            List<EFuncionario> Entidades = new List<EFuncionario>();
            try
            {
                Usuario usuarioL = (Usuario)Session["Usuario"];
                Entidades = LFuncionario.ObtenerListaColaboradores(opcion);
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            return PartialView("_ListadoColaboradores", Entidades);
        }

        [HttpPost]
        public ActionResult Deshabilitar(long id, string opcion)
        {
            try
            {
                List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
                ViewBag.PermisosH = Hijos;
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
                {
                    return new HttpStatusCodeResult(400, "MostrarMensaje('Usted no puede editar Colaboradores. Contactese con el administrador.');");
                }
                Usuario usuarioL = (Usuario)Session["Usuario"];
                LFuncionario.Deshabilitar(id, usuarioL.UsuarioId);
                List<EFuncionario> Entidades = LFuncionario.ObtenerLista(opcion, usuarioL.TipoUsuario);
                return PartialView("_ListadoColaboradores", Entidades);
            }
            catch (BussinessException ex)
            {
                return new HttpStatusCodeResult(400, ex.Message);
                //return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, "Hubo un problema, contacte al administrador.");
                //return JavaScript("MostrarMensaje('Hubo un problema, contacte al administrador.');");
            }
        }

        [HttpPost]
        public ActionResult Habilitar(long id, string opcion)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
            {
                return new HttpStatusCodeResult(400, "Usted no puede editar Colaboradores. Contactese con el administrador.");
            }
            try
            {
                Usuario usuarioL = (Usuario)Session["Usuario"];
                LFuncionario.Habilitar(id, usuarioL.UsuarioId);
                List<EFuncionario> Entidades = LFuncionario.ObtenerLista(opcion, usuarioL.TipoUsuario);
                return PartialView("_ListadoColaboradores", Entidades);
            }
            catch (BussinessException ex)
            {
                return new HttpStatusCodeResult(400, ex.Message);
                //return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, "Hubo un problema, contacte al administrador.");
                //return JavaScript("MostrarMensaje('Hubo un problema, contacte al administrador.');");
            }
        }

        [HttpPost]
        public ActionResult Eliminar(long id, string opcion)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Eliminar))
            {
                return new HttpStatusCodeResult(400, "Usted no puede eliminar Colaboradores. Contactese con el administrador.");
            }
            try
            {
                Usuario usuarioL = (Usuario)Session["Usuario"];
                LFuncionario.Eliminar(id, usuarioL.UsuarioId);
                List<EFuncionario> Entidades = LFuncionario.ObtenerLista(opcion, usuarioL.TipoUsuario);
                return PartialView("_ListadoColaboradores", Entidades);
            }
            catch (BussinessException ex)
            {
                return new HttpStatusCodeResult(400, ex.Message);
                //return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, "Hubo un problema, contacte al administrador.");
                //return JavaScript("MostrarMensaje('Hubo un problema, contacte al administrador.');");
            }
        }

        [HttpPost]
        public JavaScriptResult ResetearPassword(long idFuncionario, string opcion)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
            {
                return JavaScript("MostrarMensaje('Usted no puede editar Colaboradores. Contactese con el administrador.');");
            }
            try
            {
                Usuario usuarioL = (Usuario)Session["Usuario"];
                Usuario Usuario = LUsuarioBackEnd.ObtenerUsuario(idFuncionario, usuarioL.UsuarioId);
                LUsuarioBackEnd.CambiarContrasena(idFuncionario, Usuario.Password, EUtils.MD5Hash(LFuncionario.GenerarPasswordByNickName(Usuario.NickName)), true);


                Funcionario emp = new Funcionario();

                List<EPermiso> PermisosDisponibles = new List<EPermiso>();
                List<EPerfil> PerfilesDisponibles = new List<EPerfil>();
                emp = LFuncionario.Obtener(idFuncionario);
                PerfilesDisponibles = LPerfil.ObtenerPerfilesHabilitados(usuarioL.TipoUsuario);
                PermisosDisponibles = LPerfil.ObtenerPermisosSeleccionables(usuarioL.TipoUsuario);

                List<EPermiso> PermisosTodos = new List<EPermiso>();
                List<EPermiso> PermisosAux = new List<EPermiso>();
                if (emp.Usuario != null)
                {
                    foreach (var Perfil in emp.Usuario.Perfil)
                    {
                        foreach (var Permiso in Perfil.Permiso)
                        {
                            PermisosAux.Add(LPermiso.Convert(Permiso, usuarioL.TipoUsuario, false));
                        }
                    }
                    foreach (var p in PermisosDisponibles)
                    {
                        actualizarActivo(p, PermisosAux, emp.Usuario.Permiso.ToList());
                        PermisosTodos.Add(p);
                    }
                }
                else
                {
                    emp.Usuario = new Usuario();
                    emp.Usuario.NickName = "";
                }
                ViewBag.Permisos = PermisosTodos;
                ViewBag.PerfilesDisponibles = PerfilesDisponibles;

                Session["Toasts"] = new List<string>() { "Resetear contraseña,Se ha Reseteado la contreaseña del colaborador " + emp.sNombre + " " + emp.sApellido + " " + " correctamente." };


                return JavaScript("redireccionar('" + Url.Action("Index", Controller) + "');");
            }
            catch (BussinessException ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
        }

        [HttpPost]
        public ActionResult ResetearContrasena(long id, string opcion)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
            {
                return new HttpStatusCodeResult(400, "Usted no puede editar Funcionarios. Contactese con el administrador.");
            }
            try
            {
                Usuario usuarioL = (Usuario)Session["Usuario"];
                Usuario Usuario = LUsuarioBackEnd.ObtenerUsuario(id, usuarioL.UsuarioId);
                LUsuarioBackEnd.CambiarContrasena(id, Usuario.Password, EUtils.MD5Hash(LFuncionario.GenerarPasswordByNickName(Usuario.NickName)), true);
                if (opcion != null)
                {
                    List<EFuncionario> Entidades = LFuncionario.ObtenerListaColaboradores(opcion);
                    return PartialView("_ListadoColaboradores", Entidades);
                }
                else
                {
                    return PartialView("_ListadoColaboradores", new List<Funcionario>());
                }

            }
            catch (BussinessException ex)
            {
                return new HttpStatusCodeResult(400, ex.Message);
                //return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, ex.Message);
                //return JavaScript("MostrarMensaje('Hubo un problema, contacte al administrador.');");
            }
        }

        public ActionResult colaborador(long id)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            Usuario usuarioL = (Usuario)Session["Usuario"];
            ViewBag.areas = LArea.ObtenerLista("1");
            ViewBag.ciudades = LCiudad.ObtenerLista("1");
            if (id > 0)
            {
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
                {
                    ViewBag.Titulo = "Colaboradores";
                    ViewBag.Mensaje = "Usted no puede editar colaboradores. Contacte al administrador.";
                    ViewBag.Controlador = Controller;
                    return PartialView("NotFound404");
                }

                ViewBag.Accion = "Modificar";
                ViewBag.Title = "Editar colaborador";

                Funcionario emp = new Funcionario();

                List<EPermiso> PermisosDisponibles = new List<EPermiso>();
                List<EPerfil> PerfilesDisponibles = new List<EPerfil>();

                try
                {
                    emp = LFuncionario.ObtenerColaborador(id);
                    PerfilesDisponibles = LPerfil.ObtenerPerfilesHabilitados(usuarioL.TipoUsuario);
                    PermisosDisponibles = LPerfil.ObtenerPermisosSeleccionables(usuarioL.TipoUsuario);
                }
                catch (BussinessException ex)
                {
                    ViewBag.Titulo = "Colaboradores";
                    ViewBag.Mensaje = ex.Message;
                    ViewBag.Controlador = Controller;
                    return PartialView("NotFound404");
                }
                catch (Exception ex)
                {
                    ViewBag.Titulo = "Colaboradores";
                    ViewBag.Mensaje = "Error, Contacte al administrador.";
                    ViewBag.Controlador = Controller;
                    return PartialView("NotFound404");
                }

                if (emp == null)
                {
                    ViewBag.Titulo = "Colaboradores";
                    ViewBag.Mensaje = "No es posible encontrar el empleado seleccionado.";
                    ViewBag.Controlador = Controller;
                    return PartialView("NotFound404");
                }

                List<EPermiso> PermisosTodos = new List<EPermiso>();
                List<EPermiso> PermisosAux = new List<EPermiso>();
                if (emp.Usuario != null)
                {
                    foreach (var Perfil in emp.Usuario.Perfil)
                    {
                        foreach (var Permiso in Perfil.Permiso)
                        {
                            PermisosAux.Add(LPermiso.Convert(Permiso, usuarioL.TipoUsuario, false));
                        }
                    }
                    foreach (var p in PermisosDisponibles)
                    {
                        actualizarActivo(p, PermisosAux, emp.Usuario.Permiso.ToList());
                        PermisosTodos.Add(p);
                    }
                }
                else
                {
                    emp.Usuario = new Usuario();
                    emp.Usuario.NickName = "";
                }
                ViewBag.Permisos = PermisosTodos;
                ViewBag.PerfilesDisponibles = PerfilesDisponibles;

                //  ViewBag.Funcionarios = LFuncionario.ObtenerFuncionariosSimplesHabilitados();
                return View("FormularioColaborador", emp);
            }
            else
            {
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Crear))
                {
                    ViewBag.Titulo = "Colaboradores";
                    ViewBag.Mensaje = "Usted no puede dar de alta Colaboradores. Contacte al administrador.";
                    ViewBag.Controlador = Controller;
                    return PartialView("NotFound404");
                }
                ViewBag.Accion = "Nuevo";
                ViewBag.Title = "Nuevo colaborador";

                List<EPermiso> Permisos = new List<EPermiso>();
                List<EPerfil> PerfilesDisponibles = new List<EPerfil>();

                try
                {
                    Permisos = LPerfil.ObtenerPermisosSeleccionables(usuarioL.TipoUsuario);
                    PerfilesDisponibles = LPerfil.ObtenerPerfilesHabilitados(usuarioL.TipoUsuario);
                }
                catch (BussinessException ex)
                {
                    ViewBag.Titulo = "Funcionarios";
                    ViewBag.Mensaje = ex.Message;
                    ViewBag.Controlador = Controller;
                    return PartialView("NotFound404");
                }
                catch (Exception ex)
                {
                    ViewBag.Titulo = "Funcionarios";
                    ViewBag.Mensaje = "Error, Contacte al administrador.";
                    ViewBag.Controlador = Controller;
                    return PartialView("NotFound404");
                }
                ViewBag.Permisos = Permisos;
                ViewBag.PerfilesDisponibles = PerfilesDisponibles;

                // ViewBag.Funcionarios = LFuncionario.ObtenerFuncionariosSimplesHabilitados();
                Funcionario emp = new Funcionario();
                emp.Usuario = new Usuario();
                return View("FormularioColaborador", emp);
            }
        }

        public bool actualizarActivo(EPermiso p, List<EPermiso> ListPermisosPerfil, List<Permiso> ListPermisosUsuario)
        {
            if (ListPermisosPerfil != null && ListPermisosPerfil.Exists(x => x.PermisoId == p.PermisoId))
            {
                p.Activado = true;
                p.InPerfil = true;
            }
            if (ListPermisosUsuario != null && ListPermisosUsuario.Exists(x => x.PermisoId == p.PermisoId))
            {
                p.Activado = true;
            }


            foreach (var item in ListPermisosPerfil)
            {
                actualizarActivo(p, item.PermisosHijos, ListPermisosUsuario);
            }

            foreach (var item in p.PermisosHijos)
            {
                actualizarActivo(item, ListPermisosPerfil, ListPermisosUsuario);
            }
            return p.Activado;
        }

        public ActionResult MarcarPantallas(long id, List<long> PerfilesUsuario)
        {
            LPerfil LPerfil = new LPerfil();
            Usuario usr = new Usuario();
            List<EPermiso> PermisosDisponibles = new List<EPermiso>();
            Usuario usuarioL = (Usuario)Session["Usuario"];
            try
            {
                usr = LUsuarioBackEnd.ObtenerUsuario(id, usuarioL.UsuarioId);
            }
            catch (Exception ex)
            {

            }

            try
            {
                PermisosDisponibles = LPerfil.ObtenerPermisosSeleccionables(usuarioL.TipoUsuario);
            }
            catch (Exception ex)
            {

            }
            List<EPermiso> PermisosTodos = new List<EPermiso>();
            List<EPermiso> PermisosAux = new List<EPermiso>();

            if (PerfilesUsuario != null)
            {
                EPerfil Perfil = new EPerfil();
                foreach (var p in PerfilesUsuario)
                {
                    if (p != 0)
                    {
                        EPerfil oP = new EPerfil();
                        oP.PerfilId = p;
                        Perfil = LPerfil.ObtenerPerfil(oP.PerfilId, usuarioL.TipoUsuario);
                        foreach (var perm in Perfil.ListaPermiso)
                        {
                            PermisosAux.Add(perm);
                        }
                    }
                }
            }

            foreach (var p in PermisosDisponibles)
            {
                actualizarActivo(p, PermisosAux, usr.Permiso.ToList());
                PermisosTodos.Add(p);
            }
            return PartialView("_PermisosArbol", PermisosTodos);
        }

        private Funcionario CapturarDatos(long id, string usuario, string nombres, string apellidos, string nroCI, string cargo, List<long> perfiles, string permisos, string imagen, int tipo, string password, int sexo,long CiudadId, long AreaId)
        {
            Funcionario E = new Funcionario();
            E.nFuncionarioId = id;
            E.sNombre = nombres;
            E.sApellido = apellidos;
            E.nCiudadId = CiudadId;
            E.nAreaId = AreaId;
            E.nSexo = sexo;
            E.sCi = nroCI;
            // E.Cargo = cargo;

            if (!string.IsNullOrWhiteSpace(imagen))
            {
                E.sFotoPefil = imagen.Substring(imagen.IndexOf(",") + 1);
            }


            E.Usuario = new Usuario();
            E.Usuario.UsuarioId = id;
            E.Usuario.NickName = usuario;
            E.Usuario.Password = EUtils.MD5Hash(password);
            E.Usuario.TipoUsuario = tipo;
            var auxPerfil = new List<Perfil>();

            if (perfiles != null)
            {
                foreach (var p in perfiles)
                {
                    if (p != 0)
                    {
                        Perfil oP = new Perfil();
                        oP.PerfilId = p;
                        auxPerfil.Add(oP);
                    }
                }
                E.Usuario.Perfil = auxPerfil;
            }

            if (permisos.Length > 0)
            {
                string[] permisosArray = permisos.Split(',');
                var auxPermiso = new List<Permiso>();
                foreach (var per in permisosArray)
                {
                    Permiso oPer = new Permiso();
                    oPer.PermisoId = int.Parse(per);
                    auxPermiso.Add(oPer);
                }
                E.Usuario.Permiso = auxPermiso;
            }
            return E;
        }

        [HttpPost]
        public JavaScriptResult Nuevo(string usuario, string nombres, string apellidos, string nroCI, string cargo, List<long> perfiles, string permisos, string imagen, int tipo, string password, int sexo,long CiudadId,long AreaId)
        {
            try
            {
                List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Crear))
                {
                    return JavaScript("MostrarMensaje('Usted no puede dar de alta Funcionarios. Contactese con el administrador.');");
                }
                Usuario usuarioL = (Usuario)Session["Usuario"];
                Funcionario Entidad = CapturarDatos(0, usuario, nombres, apellidos, nroCI, cargo, perfiles, permisos, imagen, tipo, password, sexo,CiudadId,AreaId);
                Entidad.Usuario.AccesoAplicacion = true;
                LFuncionario.NuevoColaborador(Entidad, usuarioL.UsuarioId);
                Session["Toasts"] = new List<string>() { "Nuevo usuario,Los datos del nuevo usuario se guardaron exitosamente." };
                return JavaScript("redireccionar('" + Url.Action("Index", Controller) + "');");
            }
            catch (BussinessException ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
        }


        [HttpPost]
        public JavaScriptResult Modificar(long id, string usuario, string nombres, string apellidos, string nroCI, string cargo, List<long> perfiles, string permisos, string imagen, int tipo, string password, int sexo, long CiudadId, long AreaId)
        {
            try
            {
                List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
                {
                    return JavaScript("MostrarMensaje('Usted no puede editar Colaboradores. Contactese con el administrador.');");
                }
                Usuario usuarioL = (Usuario)Session["Usuario"];
                Funcionario Entidad = CapturarDatos(id, usuario, nombres, apellidos, nroCI, cargo, perfiles, permisos, imagen, tipo, password, sexo,  CiudadId,  AreaId);
                //Entidad.Usuario.AccesoAplicacion = aplicacion;
                LFuncionario.ModificarColaborador(Entidad, usuarioL.UsuarioId);
                Session["Toasts"] = new List<string>() { "Actualización de colaborador ,Los datos del colaborador se guardaron exitosamente." };
                return JavaScript("redireccionar('" + Url.Action("Index", Controller) + "');");
            }
            catch (BussinessException ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
        }




        public ActionResult Horarios(long id)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            Usuario usuarioL = (Usuario)Session["Usuario"];
            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Crear))
            {
                ViewBag.Titulo = "Colaboradores";
                ViewBag.Mensaje = "Usted no puede dar de alta Colaboradores. Contacte al administrador.";
                ViewBag.Controlador = Controller;
                return PartialView("NotFound404");
            }
            ViewBag.Accion = "Horarios de colaborador";
            ViewBag.Title = "Horarios de colaborador";
            EFuncionario funcionario = null;
            try
            {
                funcionario = LFuncionario.ObtenerFuncionario(id);
            }
            catch (BussinessException ex)
            {
                ViewBag.Titulo = "Horarios de colaborador";
                ViewBag.Mensaje = ex.Message;
                ViewBag.Controlador = Controller;
                return PartialView("NotFound404");
            }
            catch (Exception ex)
            {
                ViewBag.Titulo = "Horarios de colaborador";
                ViewBag.Mensaje = "Error, Contacte al administrador.";
                ViewBag.Controlador = Controller;
                return PartialView("NotFound404");
            }
            ViewBag.funcionario = funcionario;
            return View("HorarioColaborador",new List<EHorarioColaborador>());
        }




        [HttpPost]
        public ActionResult ListarHorarios(string estado,long id)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            List<EHorarioColaborador> Entidades = new List<EHorarioColaborador>();
            try
            {
                Usuario usuarioL = (Usuario)Session["Usuario"];
                Entidades = LHorarioColaborador.ObtenerLista(estado, id);
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            return PartialView("_ListadoHorarioColaborador", Entidades);
        }



        [HttpPost]
        public ActionResult CambiarEstadoHorario(long id, Estado estado,long nFuncionarioId)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            if (estado == Estado.Eliminado)
            {
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Eliminar))
                {
                    return new HttpStatusCodeResult(400, "Usted no puede eliminar el horario. Contactese con el administrador.");
                }
            }
            else
            {
                if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Modificar))
                {
                    return new HttpStatusCodeResult(400, "Usted no puede editar el horario. Contactese con el administrador.");
                }
            }

            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                LHorarioColaborador.CambiarEstado(id, estado, usuario.UsuarioId);
                EFuncionario funcionario = null;
                funcionario = LFuncionario.ObtenerFuncionario(nFuncionarioId);
                ViewBag.funcionario = funcionario;
                return View("HorarioColaborador", new List<EHorarioColaborador>());

            }
            catch (BussinessException ex)
            {
                return new HttpStatusCodeResult(400, ex.Message);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, "Hubo un problema, contacte al administrador.");
            }
        }


        public ActionResult HorarioFuncionario(long id)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            EHorarioColaborador Entidad = new EHorarioColaborador();
            EFuncionario funcionario = null;
            try
            {
                Usuario usuarioL = (Usuario)Session["Usuario"];
                funcionario = LFuncionario.ObtenerFuncionario(id);
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            ViewBag.Title = "Nuevo horario colaborador";
            ViewBag.dias = LDia.ObtenerLista();
            ViewBag.funcionario = funcionario;
            return PartialView("FormularioHorarioColaborador", Entidad);
        }

        
        [HttpPost]
        [ValidateInput(false)]
        public JavaScriptResult NuevoHorarioColaborador(long diaId, string horaInicio, string horaFin, long nFuncionarioId)
        {
            List<Permiso> Hijos = Util.DevolverPermisoMenu(Session, Controller);
            ViewBag.PermisosH = Hijos;
            if (!LPermiso.Instancia.LPermiso.TienePermisoEspecifico(Hijos, AccionPermiso.Crear))
            {
                return JavaScript("MostrarMensaje('Usted no puede dar de alta el horario del colaborador. Contactese con el administrador.');");
            }
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                TimeSpan HI = TimeSpan.Parse(horaInicio);
                TimeSpan HF = TimeSpan.Parse(horaFin);
                HorarioColaborador HorarioColaborador = new HorarioColaborador()
                {
                    nFuncionarioId = nFuncionarioId,
                    nDiaId = diaId,
                    tHoraInicio = HI,
                    tHoraFin = HF
                };
               
                LHorarioColaborador.Nuevo(HorarioColaborador, usuario.UsuarioId);
                Session["Toasts"] = new List<string>() { "Nuevo horario colaborador, los datos del nuevo horario colaborador se guardaron exitosamente." };
                return JavaScript("redireccionar('" + Url.Action("Horarios", Controller, new { id = nFuncionarioId }) + "');");
            }
            catch (BussinessException ex)
            {
                return JavaScript("MostrarMensaje('" + ex.Message + "');");
            }
            catch (Exception ex)
            {
                return JavaScript("MostrarMensaje('Hubo un problema, contacte al administrador.');");
            }
        }


    }
}