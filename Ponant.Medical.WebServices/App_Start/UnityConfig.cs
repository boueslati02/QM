namespace Ponant.Medical.WebServices
{
    using Ponant.Medical.Common;
    using Ponant.Medical.Common.Interfaces;
    using Ponant.Medical.Data.Auth;
    using Ponant.Medical.Data.Auth.Interfaces;
    using Ponant.Medical.Data.Shore;
    using System.Web.Http;
    using Unity;
    using Unity.WebApi;

    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            UnityContainer container = new UnityContainer();

            container.RegisterType<IShoreEntities, ShoreEntities>();
            container.RegisterType<IAuthContext, AuthContext>();
            container.RegisterType<IFileHelper, FileHelper>();
            container.RegisterType<IArchiveHelper, Archive>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}