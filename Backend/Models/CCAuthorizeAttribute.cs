using DataP;
using Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Backend.Models
{
    public class CCAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorized = base.AuthorizeCore(httpContext);

            var routeData = httpContext.Request.RequestContext.RouteData;
            var controller = routeData.GetRequiredString("controller");
            var action = routeData.GetRequiredString("action");

            httpContext.Session.Remove("UsuarioTemp");

            httpContext.Items["CodigoError"] = 0;

            if (httpContext.Request.IsAjaxRequest())
            {

            }
            if (controller.Equals("ObjetivoProyecto"))
            {
                return true;
            }
            if (httpContext.Session["Usuario"] != null && (httpContext.Session["Bloqueado"] == null || !bool.Parse(httpContext.Session["Bloqueado"].ToString())))
            {
                try
                {
                    Usuario oUsuario = (Usuario)httpContext.Session["Usuario"];
                    LUsuarioBackEnd NUsuarioAdministrativo = Logica.LBaseCC<Usuario>.Instancia.LUsuarioBackEnd;

                    try
                    {
                        var Result = NUsuarioAdministrativo.IniciarSesion(oUsuario.NickName, oUsuario.Password, false);
                        httpContext.Session["Usuario"] = Result;

                        if (httpContext.Session["timeOut"] == null)
                        {
                            httpContext.Session["timeOut"] = long.Parse("1200000"); 
                        }
                        if (httpContext.Session["timeToast"] == null)
                        {
                            httpContext.Session["timeToast"] = long.Parse("20000");
                        }

                        if (httpContext.Session["ImagenE"] == null)
                        {
                            try
                            {
                                // httpContext.Session["ImagenE"] = NEmpresa.Instancia.NEmpresa.ObtenerImagenEmpresaByEmpleadoId(oUsuario.UsuarioId);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        if (httpContext.Session["ImagenU"] == null)
                        {
                            try
                            {
                                httpContext.Session["ImagenU"] = LFuncionario.Instancia.LFuncionario.ObtenerImagenBase64Empleado(oUsuario.UsuarioId);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        oUsuario = (Usuario)httpContext.Session["Usuario"];

                    }
                    catch (Exception ex)
                    {
                        httpContext.Session.RemoveAll();
                        if (httpContext.Request.IsAjaxRequest())
                        {
                            httpContext.Session["Error"] = "Sus permisos han sido modificados, inicie sesión nuevamente";
                        }
                        return false;
                    }


                    if (controller.Equals("ObjetivoProyecto") || controller.Equals("CambioContrasena") || controller.Equals("MiPerfil") || NUsuarioAdministrativo.TieneAcceso(oUsuario, controller))
                    {
                        return true;
                    }
                    else
                    {
                        if (controller.ToUpper().Equals("INICIO"))
                        {
                            return true;
                        }
                        else
                        {
                            httpContext.Items["CodigoError"] = 403;
                            return false;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                if ((httpContext.Session["Bloqueado"] != null && bool.Parse(httpContext.Session["Bloqueado"].ToString())))
                {
                    httpContext.Items["CodigoError"] = 444;
                }
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            int CotasCodigoError = 0;
            if (filterContext.HttpContext.Items["CodigoError"] != null)
            {
                CotasCodigoError = Convert.ToInt32(filterContext.HttpContext.Items["CodigoError"].ToString());
            }

            string action = "Index";
            if (filterContext.HttpContext.Session["Bloqueado"] != null && bool.Parse(filterContext.HttpContext.Session["Bloqueado"].ToString()))
            {
                action = "Bloquear";
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var urlHelper = new UrlHelper(filterContext.RequestContext);
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        Error = "NotAuthorized",
                        LogOnUrl = urlHelper.Action(action, "Login")
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                switch (CotasCodigoError)
                {
                    case 444:
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", action } });
                        break;
                    case 403:
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", action } });
                        break;
                    default:
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", action } });
                        base.HandleUnauthorizedRequest(filterContext);
                        break;
                }
            }
        }
    }
}