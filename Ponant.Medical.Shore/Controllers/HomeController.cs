namespace Ponant.Medical.Shore.Controllers
{
    using System.Web.Mvc;

    [Authorize(Roles = "Booking, Group, Booking Administrator, Medical Administrator, Medical, Doctor, IT Administrator, Agency Administrator, Agency")]
    public class HomeController : BaseController
    {
        #region Index
        /// <summary>
        /// Page d'accueil
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Redirection des compte booking et group
            if (User.IsInRole(Data.Constants.ROLE_NAME_BOOKING) || User.IsInRole(Data.Constants.ROLE_NAME_GROUP) || User.IsInRole(Data.Constants.ROLE_NAME_AGENCY_ADMINISTRATOR) || User.IsInRole(Data.Constants.ROLE_NAME_AGENCY))
            {
                return RedirectToAction("Index", "Cruise");
            }
            return View();
        }
        #endregion

        #region Login
        /// <summary>
        /// Ecran de login
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        #endregion
    }
}