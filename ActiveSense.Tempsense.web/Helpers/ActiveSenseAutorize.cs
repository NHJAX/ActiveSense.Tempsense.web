
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
        private const string USER_PROFILE = "User";
        private const string PROFILE_Administrator= "Administrator";
        private readonly string[] userAssignedRoles;

        private const string STATE_USER_DISABLED = "NOT_ENABLED";
        private const string STATE_USER_ENABLED = "ENABLED";


        public ActiveSenseAutorize(params string[] roles)
        {
            userAssignedRoles = roles;
        }


        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool StateAuthorization = false;
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            //¿This the Authenticated Users?
            var user = httpContext.User;
            if (!user.Identity.IsAuthenticated)
                return StateAuthorization;

            //Authenticated user but there are validations in actions?
            if (userAssignedRoles.Length > 0)
            {
                StateAuthorization = userAssignedRoles.Any(user.IsInRole) ? true : false;
            }
            else
            {   //There is no validation in action
                StateAuthorization = true;
            }
            return StateAuthorization;
        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var user = filterContext.HttpContext.User;
            // If the user is authenticated is redirected to the portal home in your area
            if (user.Identity.IsAuthenticated)
            {
                //Redirect to site home profile Administrator
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
                //Redirect to site home of user profile
                if (user.IsInRole(USER_PROFILE))
                {
                    RouteValueDictionary routeValues = new RouteValueDictionary
                    {
                        {"controller" , "Home"},
                        {"action" , "Index"},
                        {"area" , "User"}
                     };
                    filterContext.Result = new RedirectToRouteResult(routeValues);
                }
            }
            else
            {   // Is redirected to home page default
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
                            {"area" , "User"}
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
            return (perfil.State == STATE_USER_DISABLED) ? false : true;
        }

    }
}