using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
namespace Justbok.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;
            BundleTable.Bundles.UseCdn = true;
            StyleBundle(bundles);
            ScriptBundle(bundles);
        }

        public static void StyleBundle(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/css")
                     .Include("~/Content/bootstrap.css")
                     .Include("~/Content/themes/base/jquery-ui.css")
                     .Include("~/Content/bootstrap-datetimepicker.min.css")
                     .Include("~/Content/bootstrap-timepicker.css")
                     .Include("~/Theme/css/bootstrap-multiselect.css")
                     .Include("~/Theme/css/wickedpicker.min.css")
                     .Include("~/Theme/css/bootstrap-datetimepicker.css")
                     .Include("~/Content/fullcalendar.css")
                     .Include("~/Content/toastr.min.css")
                     .Include("~/Content/select2.min.css")
                     .Include("~/Theme/css/AdminLTE.min.css") 
                     .Include("~/Theme/css/datepicker.min.css") 
                     .Include("~/Theme/css/skin-purple.min.css")
                    );
            bundles.Add(new StyleBundle("~/Upload")
                     .Include("~/Plugins/Upload/css/jquery.fileupload-ui.css")
                     .Include("~/Plugins/Upload/css/jquery.fileupload.css")
                    );
            //bundles.Add(new StyleBundle("~/bundles/auto-complete", "//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css")
            //     .Include("~/Content/auto-complete.css"));
           
        }

        public static void ScriptBundle(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js")
                     .Include("~/Scripts/jquery-{version}.js")
                     .Include("~/Scripts/jquery-ui-1.12.1.js")
                     .Include("~/Theme/js/bootstrap-multiselect.js")
                      .Include("~/Theme/js/wickedpicker.min.js")
                       .Include("~/Theme/js/jQuery.print.js")
                      .Include("~/Scripts/jquery.validate.js")
                      .Include("~/Scripts/moment.js")
                      .Include("~/Scripts/bootstrap-datetimepicker.js")
                      .Include("~/Scripts/bootstrap-timepicker.js")
                      .Include("~/Scripts/jquery.validate.unobtrusive.js")
                       .Include("~/Scripts/toast.js")
                       .Include("~/Scripts/toastr.min.js")
                       .Include("~/Scripts/jquery.twbsPagination.min.js")
                       .Include("~/Scripts/fullcalendar.js")
                        .Include("~/Scripts/bootstrap.js")
                        .Include("~/Scripts/select2.full.min.js")
                        .Include("~/Scripts/Common.js")
                        .Include("~/Theme/js/jquery.webcam.js")
                         .Include("~/Theme/js/adminlte.min.js")
                        .Include("~/Theme/js/bootstrap-datetimepicker.fr.js")
                        .Include("~/Theme/js/gulpfile.min.js")
                         .Include("~/Scripts/bootstrap-datepicker.js")
                        .Include("~/Content/Common/_layout.js"));
            //bundles.Add(new ScriptBundle("~/bundles/jquery", "https://code.jquery.com/ui/1.12.1/jquery-ui.js").Include(
            //           "~/Scripts/jquery-{version}.js"));
        }
    }
}
