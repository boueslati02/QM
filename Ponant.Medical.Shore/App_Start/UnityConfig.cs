namespace Ponant.Medical.Shore
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Ponant.Medical.Data.Shore;
    using Ponant.Medical.Shore.Controllers;
    using Ponant.Medical.Shore.Models;
    using System.Web.Mvc;
    using Unity;
    using Unity.Injection;
    using Unity.Mvc5;

    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            UnityContainer container = new UnityContainer();

            container.RegisterType<ApplicationDbContext>();
            container.RegisterType<ApplicationSignInManager>();
            container.RegisterType<ApplicationUserManager>();
            container.RegisterType<EmailService>();
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new InjectionConstructor(typeof(ApplicationDbContext)));
            container.RegisterType<AccountController>(new InjectionConstructor(typeof(ApplicationUserManager), typeof(ApplicationSignInManager)));
            container.RegisterType<AccountController>(new InjectionConstructor());
            container.RegisterType<UserManager<ApplicationUser>>();
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
            //container.RegisterFactory<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication);

            container.RegisterType<IShoreEntities, ShoreEntities>();
            
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}