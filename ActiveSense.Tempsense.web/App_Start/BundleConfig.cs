using System.Web;
using System.Web.Optimization;

namespace ActiveSense.Tempsense.web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                      "~/Scripts/jquery-ui.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/calendar").Include(
                       "~/Scripts/bootstrap-datetimepicker.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/dataTable").Include(
                       "~/Scripts/jquery.dataTables.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/utilities").Include(
                      "~/Scripts/closemain.js",
                      "~/Scripts/togglemenu.js"));

            bundles.Add(new ScriptBundle("~/bundles/toastr").Include(
                      //"~/Scripts/toastr.js",
                      "~/Scripts/toastr.min.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/signalR").Include(
                     "~/Scripts/jquery.signalR-2.2.1.js"));

            bundles.Add(new StyleBundle("~/Content/Master").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/Site.css",
                        "~/Content/Css/font-awesome.css",
                        "~/Content/Css/atlas.css",
                        "~/Content/Css/bars.css",
                        "~/Content/Css/bootstrap-markdown.min.css",
                        "~/Content/Css/bootstrap.min.css",
                        "~/Content/Css/chocolat.css",
                        "~/Content/Css/clndr.css",
                        "~/Content/Css/fabochart.css",
                        "~/Content/Css/icon-font.min.css",
                        "~/Content/Css/jqvmap.css",
                        "~/Content/Css/popuo-box.css",
                        "~/Content/Css/style.css",
                        "~/Content/Css/vroom.css",
                        "~/Content/Css/bootstrap-datetimepicker.min.css",
                        "~/Content/Css/bootstrap-combined.min.css",
                        "~/Content/Css/jquery.dataTables.min.css",
                        "~/Content/Css/bootstrap_calendar.css",
                         "~/Content/Css/toastr.min.css"
                       ));

        }
    }
}
