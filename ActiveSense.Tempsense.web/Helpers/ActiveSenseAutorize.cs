
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
        private const string USER_PROFILE = "Usuario";
        private const string PROFILE_Administrator= "Administrator";
        private readonly string[] userAssignedRoles;

        private const string STATE_USER_INHABILITADO = "NO_HABILITADO";
        private const string STATE_USER_HABILITADO = "HABILITADO";


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

            //¿Esta el Users autenticado?
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
                //Redireccion a sitio inicio de perfil Administrator
                if (user.IsInRole(PROFILE_Administrator))
                {
                    RouteValueDictionary routeValues = new RouteValueDictionary
                    {
                        {"controller" , "Home"},
                        {"action" , "Index"},
                        {"area" , "Administrator"}
                     };
                    filterContext.Result = new RedirectToRouteResult(routeValues);
                }
                //Redireccion a sitio inicio de perfil usuario
                if (user.IsInRole(USER_PROFILE))
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
                if (user.IsInRole(USER_PROFILE))
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

        private bool isEnableUser(string idUser)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var perfil = UserManager.FindById(idUser);
            return (perfil.State == STATE_USER_INHABILITADO) ? false : true;
        }

    }
}