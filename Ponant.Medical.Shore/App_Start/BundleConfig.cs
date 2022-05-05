namespace Ponant.Medical.Shore
{
    using System.Web.Optimization;

    public static class BundleConfig
    {
        // Pour plus d'informations sur le regroupement, visitez http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Ajout pour forcer le rechargement des styles et scripts lors d'un changement de version 
            string assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Styles.DefaultTagFormat = "<link href=\"{0}?vers=" + assemblyVersion + "\" rel=\"stylesheet\" type=\"text/css\"/>";
            Scripts.DefaultTagFormat = "<script src=\"{0}?vers=" + assemblyVersion + "\" type=\"text/javascript\"></script>";

            bundles.Add(new ScriptBundle("~/bundles/jquery_bundle").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui.js",
                        "~/Scripts/common.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval_bundle").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive-ajax.js"
                        ));

            // Utilisez la version de développement de Modernizr pour le développement et l'apprentissage. Puis, une fois
            // prêt pour la production, utilisez l'outil de génération (bluid) sur http://modernizr.com pour choisir uniquement les tests dont vous avez besoin.
            bundles.Add(new ScriptBundle("~/bundles/modernizr_bundle").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap_bundle").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-datepicker.js",
                      "~/Scripts/bootstrap-multiselect.js",
                      "~/Scripts/jquery.bootstrap-touchspin.js",
                      "~/Scripts/fileinput.js",
                      "~/Scripts/locales/fr.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css_bundle").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-datepicker.css",
                      "~/Content/bootstrap-multiselect.css",
                      "~/Content/bootstrap-fileinput/css/fileinput.css",
                      "~/Content/jquery.bootstrap-touchspin.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/site.css",
                      "~/Content/navbar.css",
                      "~/Content/modal.css",
                      "~/Content/components.css"
                      ));

            //Bundle View Reminder 
            bundles.Add(new ScriptBundle("~/bundles/reminder_bundle").IncludeDirectory("~/Scripts/Reminder/", "*.js", false));
            bundles.Add(new StyleBundle("~/Content/reminder_bundle").IncludeDirectory("~/Content/Reminder/", "*.css", false));

            //Bundle View Language
            bundles.Add(new ScriptBundle("~/bundles/language_bundle").IncludeDirectory("~/Scripts/Language/", "*.js", false));
            bundles.Add(new StyleBundle("~/Content/language_bundle").IncludeDirectory("~/Content/Language/", "*.css", false));

            //Bundle View Cruise
            bundles.Add(new ScriptBundle("~/bundles/cruise_bundle").IncludeDirectory("~/Scripts/Cruise/", "*.js", false));
            bundles.Add(new StyleBundle("~/Content/cruise_bundle").IncludeDirectory("~/Content/Cruise/", "*.css", false));

            //Bundle View Passenger
            bundles.Add(new ScriptBundle("~/bundles/passenger_bundle").IncludeDirectory("~/Scripts/Passenger/", "*.js", false));
            bundles.Add(new StyleBundle("~/Content/passenger_bundle").IncludeDirectory("~/Content/Passenger/", "*.css", false));

            //Bundle View Medical
            bundles.Add(new ScriptBundle("~/bundles/medical_bundle").IncludeDirectory("~/Scripts/Medical/", "*.js", false));
            bundles.Add(new StyleBundle("~/Content/medical_bundle").IncludeDirectory("~/Content/Medical/", "*.css", false));

            //Bundle View AvailableDocument
            bundles.Add(new ScriptBundle("~/bundles/availableDocument_bundle").IncludeDirectory("~/Scripts/AvailableDocument/", "*.js", false));
            bundles.Add(new StyleBundle("~/Content/availableDocument_bundle").IncludeDirectory("~/Content/AvailableDocument/", "*.css", false));

            //Bundle View PassengerDocument
            bundles.Add(new ScriptBundle("~/bundles/passengerDocument_bundle").IncludeDirectory("~/Scripts/PassengerDocument/", "*.js", false));
            bundles.Add(new StyleBundle("~/Content/passengerDocument_bundle").IncludeDirectory("~/Content/PassengerDocument/", "*.css", false));

            //Bundle View User
            bundles.Add(new ScriptBundle("~/bundles/user_bundle").IncludeDirectory("~/Scripts/User/", "*.js", false));

            //Bundle View Agency Access Right
            bundles.Add(new ScriptBundle("~/bundles/agencyAccessRight_bundle").IncludeDirectory("~/Scripts/AgencyAccessRight/", "*.js", false));

            //Bundle View Upload
            bundles.Add(new StyleBundle("~/Content/upload_bundle").IncludeDirectory("~/Content/Upload/", "*.css", false));
            bundles.Add(new ScriptBundle("~/bundles/upload_bundle").IncludeDirectory("~/Scripts/Upload/", "*.js", false));
        }
    }
}
