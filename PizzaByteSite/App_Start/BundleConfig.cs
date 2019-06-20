using System.Web.Optimization;

namespace PizzaByteSite
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            string components = "~/Scripts/PizzaByteSite/components/";
            string plugins = "~/Scripts/PizzaByteSite/plugins/";
            string js = "~/Scripts/PizzaByteSite/js/";
            string telas = "~/Scripts/PizzaByteSite/Telas/";
            string css = "~/Content/PizzaByteSite/css/";

            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include(plugins + "jQueryUI/jquery-3.3.1.js")
                .Include(plugins + "jQueryUI/jquery-ui.min.js")
                .Include(plugins + "bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js")
                .Include(plugins + "jQueryUI/jquery.mask.min.js")
                .Include(components + "bootstrap/dist/js/bootstrap.min.js")
                .Include(js + "PizzaByteSite.min.js")
                .Include(plugins + "jQueryUI/toastr.min.js")
                .Include(js + "SweetAlert.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/inicio")
                .Include(components + "raphael/raphael.min.js")
                .Include(components + "morris.js/morris.min.js")
                .Include(components + "chart.js/Chart.min.js")
                .Include(components + "Flot/jquery.flot.js")
                .Include(components + "Flot/jquery.flot.resize.js")
                .Include(components + "Flot/jquery.flot.pie.js")
                .Include(components + "Flot/jquery.flot.categories.js")
                .Include(components + "jquery-sparkline/dist/jquery.sparkline.min.js")
                .Include(plugins + "jvectormap/jquery-jvectormap-1.2.2.min.js")
                .Include(plugins + "jvectormap/jquery-jvectormap-world-mill-en.js")
                .Include(components + "jquery-knob/dist/jquery.knob.min.js")
                .Include(components + "moment/moment.js")
                .Include(components + "ckeditor/ckeditor.js")
                .Include(components + "datatables.net/js/jquery.dataTables.min.js")
                .Include(components + "datatables.net-bs/js/dataTables.bootstrap.min.js")
                .Include(components + "bootstrap-daterangepicker/daterangepicker.js")
                .Include(components + "bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js")
                .Include(components + "bootstrap-colorpicker/dist/js/bootstrap-colorpicker.min.js")
                .Include(plugins + "bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js")
                .Include(components + "jquery-slimscroll/jquery.slimscroll.min.js")
                .Include(components + "fastclick/lib/fastclick.js")
                .Include(plugins + "bootstrap-slider/bootstrap-slider.js")
                .Include(components + "select2/dist/js/select2.full.min.js")
                .Include(plugins + "timepicker/bootstrap-timepicker.min.js")
                .Include(plugins + "iCheck/icheck.min.js")
                .Include(components + "fullcalendar/dist/fullcalendar.min.js")
                .Include(telas + "Inicio.js"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/jQuery/toastr.css")
                .Include("~/Content/bootstrap.css")
                .Include(components + "bootstrap/dist/css/bootstrap.min.css")
                .Include(components + "font-awesome/css/font-awesome.min.css")
                .Include(components + "Ionicons/css/ionicons.min.css")
                .Include(components + "datatables.net-bs/css/dataTables.bootstrap.min.css")
                .Include(css + "Personalizado.css")
                .Include(css + "AdminLTE.min.css")
                .Include(css + "skins/_all-skins.min.css")
                .Include(components + "morris.js/morris.css")
                .Include(components + "jvectormap/jquery-jvectormap.css")
                .Include(components + "bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css")
                .Include(components + "bootstrap-daterangepicker/daterangepicker.css")
                .Include(plugins + "bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css")
                .Include(plugins + "bootstrap-slider/slider.css")
                .Include(components + "select2/dist/css/select2.min.css")
                .Include(components + "bootstrap-colorpicker/dist/css/bootstrap-colorpicker.min.css")
                .Include(plugins + "timepicker/bootstrap-timepicker.min.css")
                .Include(plugins + "iCheck/all.css")
                .Include(plugins + "pace/pace.min.css")
                .Include(components + "fullcalendar/dist/fullcalendar.min.css"));

            //bundles.Add(new ScriptBundle("~/bundles/inicio")
            //    .Include(telas + "Inicio.js"));

        }
    }
}
