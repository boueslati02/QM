namespace Ponant.Medical.WebServices
{
    using Ponant.Medical.WebServices.Converters;
    using Ponant.Medical.WebServices.Filters;
    using System.Web.Http;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuration et services API Web
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;
            
            // Itinéraires de l'API Web
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Ajout du converter spécifique
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new EmptyToNullConverter());

            // Ajout du filtre de trace
            config.Filters.Add(new TraceFilter());

            UnityConfig.RegisterComponents();
        }
    }
}
