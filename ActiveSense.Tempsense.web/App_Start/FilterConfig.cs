using ActiveSense.Tempsense.web.Helpers;
using System.Web;
using System.Web.Mvc;

namespace ActiveSense.Tempsense.web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ActiveSenseAutorize());
        }
    }
}
