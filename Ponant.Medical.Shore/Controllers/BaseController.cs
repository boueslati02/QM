namespace Ponant.Medical.Shore.Controllers
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Ponant.Medical.Data.Shore;
    using Ponant.Medical.Shore.Models;
    using Ponant.Medical.Shore.Properties;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using static System.Net.Mime.MediaTypeNames;

    public class BaseController : Controller
    {
        #region Properties & Constructors

        protected readonly IShoreEntities _shoreEntities;

        protected readonly ApplicationDbContext _applicationDbContext;

        public BaseController()
        {
            _shoreEntities = new ShoreEntities();
            _applicationDbContext = new ApplicationDbContext();
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Récupération et initialisation du nom et prénom de l'utilisateur connecté
        /// </summary>
        /// <param name="requestContext"></param>
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (requestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (requestContext.HttpContext.Session["UserName"] == null && requestContext.HttpContext.User != null)
                {
                    ApplicationUserManager userManager = requestContext.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                    if (userManager != null)
                    {
                        ApplicationUser user = userManager.FindById(requestContext.HttpContext.User.Identity.GetUserId());

                        if (user != null)
                        {
                            requestContext.HttpContext.Session["UserName"] = string.Format("{0} {1}", user.LastName, user.FirstName);
                        }
                    }
                }
            }
            else
            {
                requestContext.HttpContext.Session.Remove("UserName");
            }
        }
        #endregion

        #region Help
        /// <summary>
        /// Affiche le fichier d'aide
        /// </summary>
        /// <returns>Le fichier d'aide en PDF</returns>
        [HttpGet]
        public FileResult Help()
        {
            if (User.IsInRole(Data.Constants.ROLE_NAME_AGENCY_ADMINISTRATOR) || User.IsInRole(Data.Constants.ROLE_NAME_AGENCY))
            {
                return File(Resources.Shore_agency_manual, Application.Pdf);
            }
            else
            {
                return File(Resources.Shore_manual, Application.Pdf);
            }
        }
        #endregion

        #region Errors
        /// <summary>
        /// Affichage de la page d'erreur HTTP 404 : Page non trouvé
        /// </summary>
        /// <returns>Page d'erreur personnalisé pour l'erreur 404</returns>
        public ActionResult Error404()
        {
            return View();
        }

        /// <summary>
        /// Affichage de la page d'erreur HTTP 500 : Erreur du serveur interne
        /// </summary>
        /// <returns>Page d'erreur personnalisé pour l'erreur 500</returns>
        public ActionResult Error500()
        {
            return View();
        }
        #endregion
    }
}