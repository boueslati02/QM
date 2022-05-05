namespace Ponant.Medical.Shore.Controllers
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using MsgReader.Outlook;
    using Ponant.Medical.Common;
    using Ponant.Medical.Shore.Models;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Gestion de l'authentification
    /// </summary>
    [Authorize(Roles = "Booking, Group, Booking Administrator, Medical Administrator, Medical, Doctor, IT Administrator, Agency Administrator, Agency")]
    public class AccountController : BaseController
    {
        #region Properties & Constructors

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public AccountController()
        { }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        #endregion

        #region Login
        /// <summary>
        /// Connexion à la plateforme
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                // Ceci ne comptabilise pas les échecs de connexion pour le verrouillage du compte
                // Pour que les échecs de mot de passe déclenchent le verrouillage du compte, utilisez shouldLockout: true
                bool isShouldLockout = true;

#if DEV
                isShouldLockout = false;
#endif

                SignInStatus result = await SignInManager.PasswordSignInAsync(model.Login, model.Password, false, shouldLockout: isShouldLockout);
                ApplicationUser user = await UserManager.FindByNameAsync(model.Login);
                switch (result)
                {
                    case SignInStatus.Success:
                        if (string.IsNullOrWhiteSpace(returnUrl))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectToLocal(returnUrl);
                        }
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        if (user != null && user.Enabled)
                        {
                            using (Storage.Message message = new Storage.Message(Path.Combine(AppSettings.FolderMail, AppSettings.MailSendCode)))
                            {
                                string code = await UserManager.GenerateTwoFactorTokenAsync(user.Id, "EmailCode");
                                UserManager.SendEmail(user.Id, message.Subject, message.BodyHtml.Replace(AppSettings.TagLastName, user.LastName).Replace(AppSettings.TagFirstName, user.FirstName).Replace("{0}", code));
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Invalid connection attempt.";
                            return RedirectToAction("Index", "Home");
                        }

                        return RedirectToAction("VerifyCode", new { ReturnUrl = returnUrl });
                    default:
                        TempData["ErrorMessage"] = "Invalid connection attempt.";
                        return RedirectToAction("Index", "Home");
                }
            }
        }
        #endregion

        #region VerifyCode
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string returnUrl)
        {
            // Nécessiter que l'utilisateur soit déjà connecté via un nom d'utilisateur/mot de passe ou une connexte externe
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error500");
            }
            return View(new VerifyCodeViewModel { ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Le code suivant protège des attaques par force brute contre les codes à 2 facteurs. 
            // Si un utilisateur entre des codes incorrects pendant un certain intervalle, le compte de cet utilisateur 
            // est alors verrouillé pendant une durée spécifiée. 
            // Vous pouvez configurer les paramètres de verrouillage du compte dans IdentityConfig
            SignInStatus result = await SignInManager.TwoFactorSignInAsync("EmailCode", model.Code, isPersistent:false, rememberBrowser:false);
            switch (result)
            {
                case SignInStatus.Success:
                    string userId = await SignInManager.GetVerifiedUserIdAsync();
                    ApplicationUser user = await UserManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        user.LastConnectionDate = DateTime.Now;
                        await UserManager.UpdateAsync(user);
                        LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Connection, LogManager.LogAction.Get, user.UserName, "Connection success for this user");
                        return (user.PasswordChange) ? RedirectToAction("ChangePassword", new { model.ReturnUrl }) : RedirectToAction("Index", "Home");
                    }

                    return (string.IsNullOrWhiteSpace(model.ReturnUrl)) ? RedirectToAction("Index", "Home") : RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }
        #endregion

        #region ChangePassword
        /// <summary>
        /// Vue de modification du mot de passe
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult ChangePassword(string returnUrl)
        {
            return View(new ChangePasswordViewModel { ReturnUrl = returnUrl });
        }

        /// <summary>
        /// Modifie le mot de passe de l'utilisateur connecté
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    user.PasswordChange = false;
                    user.Editor = user.UserName;
                    user.ModificationDate = DateTime.Now;

                    await UserManager.UpdateAsync(user);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.User, LogManager.LogAction.Edit, user.UserName, "Edit password for this user");
                    TempData["Message"] = "Your password has been changed.";

                    if (string.IsNullOrWhiteSpace(model.ReturnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction(model.ReturnUrl);
                    }
                }
            }
            AddErrors(result);
            return View(model);
        }
        #endregion

        #region LogOff
        /// <summary>
        /// Déconnexion
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Libération des ressources
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Private methods
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}