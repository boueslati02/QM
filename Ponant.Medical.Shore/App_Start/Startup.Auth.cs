namespace Ponant.Medical.Shore
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;
    using Owin;
    using Ponant.Medical.Shore.Models;
    using System;

    public partial class Startup
    {
        // Pour plus d’informations sur la configuration de l’authentification, rendez-vous sur http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configurer le contexte de base de données, le gestionnaire des utilisateurs et le gestionnaire des connexions pour utiliser une instance unique par demande
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Autoriser l’application à utiliser un cookie pour stocker des informations pour l’utilisateur connecté
            // et pour utiliser un cookie à des fins de stockage temporaire des informations sur la connexion utilisateur avec un fournisseur de connexion tiers
            // Configurer le cookie de connexion
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Home/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Permet à l'application de valider le timbre de sécurité quand l'utilisateur se connecte.
                    // Cette fonction de sécurité est utilisée quand vous changez un mot de passe ou ajoutez une connexion externe à votre compte.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(AppSettings.ValidateInterval),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                },
                CookieSecure = CookieSecureOption.Always,
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromMinutes(AppSettings.ExpireTimeSpan)
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Permet à l'application de stocker temporairement les informations utilisateur lors de la vérification du second facteur dans le processus d'authentification à 2 facteurs.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(AppSettings.TwoFactorInterval));
        }
    }
}