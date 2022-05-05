namespace Ponant.Medical.Shore
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security;
    using Ponant.Medical.Common.MailServer;
    using Ponant.Medical.Shore.Models;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            List<Recipient> recipients = new List<Recipient>
            {
                new Recipient("", message.Destination)
            };

            await MailServer.Send(new Mail()
            {
                From = AppSettings.AddressNoReply,
                Body = message.Body,
                Recipients = recipients,
                Subject = message.Subject,
            });
        }
    }

    // Configurer l'application que le gestionnaire des utilisateurs a utilisée dans cette application. UserManager est défini dans ASP.NET Identity et est utilisé par l'application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        { }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            ApplicationUserManager manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configurer la logique de validation pour les noms d'utilisateur
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = AppSettings.AllowOnlyAlphanumericUserNames,
                RequireUniqueEmail = AppSettings.RequireUniqueEmail
            };

            // Configurer la logique de validation pour les mots de passe
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = AppSettings.RequiredLength,
                RequireNonLetterOrDigit = AppSettings.RequireNonLetterOrDigit,
                RequireDigit = AppSettings.RequireDigit,
                RequireLowercase = AppSettings.RequireLowercase,
                RequireUppercase = AppSettings.RequireUppercase
            };

            // Configurer les valeurs par défaut du verrouillage de l'utilisateur
            manager.UserLockoutEnabledByDefault = AppSettings.UserLockoutEnabledByDefault;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(AppSettings.DefaultAccountLockoutTimeSpan);
            manager.MaxFailedAccessAttemptsBeforeLockout = AppSettings.MaxFailedAccessAttemptsBeforeLockout;

#if !DEV
            // Inscrire les fournisseurs d'authentification à 2 facteurs. Cette application utilise le téléphone et les e-mails comme procédure de réception de code pour confirmer l'utilisateur
            // Vous pouvez écrire votre propre fournisseur et le connecter ici.
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>());
            manager.EmailService = new EmailService();
#endif

            Microsoft.Owin.Security.DataProtection.IDataProtectionProvider dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                    {
                        TokenLifespan = TimeSpan.FromMinutes(AppSettings.TokenLifespan)
                    };
            }

            return manager;
        }
    }

    // Configurer le gestionnaire de connexion d'application qui est utilisé dans cette application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
