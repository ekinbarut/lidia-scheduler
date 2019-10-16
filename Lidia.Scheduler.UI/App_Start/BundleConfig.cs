using System.Web;
using System.Web.Optimization;

namespace Lidia.Scheduler.UI
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/datetimepicker").Include(
                        "~/Content/vendor/plugins/datepicker/js/bootstrap-datetimepicker.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/select2").Include(
                "~/Content/vendor/plugins/select2/select2.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/maskedinput").Include(
                "~/Content/admin-tools/admin-forms/js/jquery.maskedinput.js"));

            bundles.Add(new StyleBundle("~/Content/select2").Include(
                     "~/Content/vendor/plugins/select2/css/core.css"));

            bundles.Add(new StyleBundle("~/Content/datetimepicker").Include(
                     "~/Content/vendor/plugins/datepicker/css/bootstrap-datetimepicker.min.css"));

            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));
        }
    }
}
