
using ActiveSense.Tempsense.web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ActiveSense.Tempsense.web.Helpers
{
    public class ActiveSenseAutorize : AuthorizeAttribute, IActionFilter
    {
        private const string PERFIL_USUARIO = "Usuario";
        private const string PERFIL_ADMINISTRADOR = "Administrador";
        private readonly string[] userAssignedRoles;

        private const string ESTADO_USUARIO_INHABILITADO = "NO_HABILITADO";
        private const string ESTADO_USUARIO_HABILITADO = "HABILITADO";


        public ActiveSenseAutorize(params string[] roles)
        {
            userAssignedRoles = roles;
        }


        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool estadoAutorizacion = false;
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            //¿Esta el usuarios autenticado?
            var user = httpContext.User;
            if (!user.Identity.IsAuthenticated)
                return estadoAutorizacion;

            //usuario autenticado pero ¿hay validaciones en acciones?
            if (userAssignedRoles.Length > 0)
            {
                estadoAutorizacion = userAssignedRoles.Any(user.IsInRole) ? true : false;
            }
            else
            {   //no hay validaciones en acciones
                estadoAutorizacion = true;
            }
            return estadoAutorizacion;
        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var user = filterContext.HttpContext.User;
            // si el usuario esta autenticado se redirecciona al portal inicio de su area
            if (user.Identity.IsAuthenticated)
            {
                //Redireccion a sitio inicio de perfil administrador
                if (user.IsInRole(PERFIL_ADMINISTRADOR))
                {
                    RouteValueDictionary routeValues = new RouteValueDictionary
                    {
                        {"controller" , "Home"},
                        {"action" , "Index"},
                        {"area" , "Administrador"}
                     };
                    filterContext.Result = new RedirectToRouteResult(routeValues);
                }
                //Redireccion a sitio inicio de perfil usuario
                if (user.IsInRole(PERFIL_USUARIO))
                {
                    RouteValueDictionary routeValues = new RouteValueDictionary
                    {
                        {"controller" , "Home"},
                        {"action" , "Index"},
                        {"area" , "Usuario"}
                     };
                    filterContext.Result = new RedirectToRouteResult(routeValues);
                }
            }
            else
            {   // Se redirecciona a pagina de inicio por defecto
                filterContext.Result = new HttpUnauthorizedResult();
            }

        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = filterContext.HttpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                if (user.IsInRole(PERFIL_USUARIO))
                {
                    var nameControl = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                    var enableUser = isEnableUser(user.Identity.GetUserId());
                    filterContext.Controller.ViewBag.IsEnable = enableUser;
                    string[] controllerPublic = { "Home", "Account", "DefaultCaptcha" };

                    if (!enableUser && !controllerPublic.Contains(nameControl))
                    {
                        RouteValueDictionary routeValues = new RouteValueDictionary
                        {
                            {"controller" , "Home"},
                            {"action" , "Index"},
                            {"area" , "Usuario"}
                         };
                        filterContext.Result = new RedirectToRouteResult(routeValues);
                    }

                }
            }
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new NotImplementedException();
        }

        private bool isEnableUser(string idusuario)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var perfil = UserManager.FindById(idusuario);
            return (perfil.State == ESTADO_USUARIO_INHABILITADO) ? false : true;
        }

    }
}