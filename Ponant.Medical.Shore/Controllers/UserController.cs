namespace Ponant.Medical.Shore.Controllers
{
    using Ponant.Medical.Common;
    using Ponant.Medical.Data;
    using Ponant.Medical.Shore.Helpers;
    using Ponant.Medical.Shore.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Validation;
    using System.IO;
    using System.Net;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    [Authorize(Roles = "IT Administrator, Booking Administrator, Agency Administrator, Group")]
    public class UserController : BaseController
    {
        #region Properties & Constructors

        private readonly UserClass _userClass;
        private readonly CruiseClass _cruiseClass;
        private readonly AgencyAccessRightClass _AgencyAccessRightClass;

        public UserController()
        {
            _userClass = new UserClass(_applicationDbContext);
            _cruiseClass = new CruiseClass(_shoreEntities);
            _AgencyAccessRightClass = new AgencyAccessRightClass(_shoreEntities);
        }

        #endregion

        #region Index
        /// <summary>
        /// Affiche la liste des utilisateurs et les filtres
        /// </summary>
        /// <returns>La liste des utilisateurs</returns>
        [HttpGet]
        public ActionResult Index()
        {
            LoadViewBag();
            return View();
        }
        #endregion

        #region Reset
        /// <summary>
        /// Réinitialise le mot de passe de l'utilisateur et lui envoi par mail
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <returns>Un message pour dire que le mot de passe a été réinitialisé et lui sera communiqué par mail</returns>
        [HttpGet]
        public async Task<ActionResult> Reset(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (await _userClass.ResetPassword(id))
            {
                TempData["Message"] = "The password has been reset and communicated by email to the user.";
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Create
        /// <summary>
        /// Formulaire de création d'un utilisateur
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _Create()
        {
            LoadViewBag();

            int? IdAgency = null;
            string AgencyName = null;

            if (HttpContext.User.IsInRole(Constants.ROLE_NAME_AGENCY_ADMINISTRATOR))
            {
                string curentIdUser = _userClass.GetUserId(HttpContext.User.Identity.Name);
                IdAgency = _userClass.GetAgencyId(curentIdUser);
                AgencyName = _userClass.GetAgencyName(curentIdUser);
            }

            if (HttpContext.User.IsInRole(Constants.ROLE_NAME_BOOKING_ADMINISTRATOR))
            {
                IdAgency = AppSettings.IdAgency;
                AgencyName = AppSettings.AgencyName;
            }

            return PartialView(new CreateUserViewModel
            {
                Enabled = true,
                Password = UserHelper.CreateRandomPassword(AppSettings.RequireDigit, AppSettings.RequireLowercase, AppSettings.RequireNonLetterOrDigit, AppSettings.RequireUppercase, AppSettings.RequiredLength),
                IdAgency = IdAgency,
                AgencyName = AgencyName
            });
        }

        /// <summary>
        /// Création de l'utilisateur
        /// </summary>
        /// <param name="user">Instance du nouvel utilisateur</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Create([Bind(Include = "IdAgency,LastName,FirstName,UserName,Password,Email,Enabled,Role,LogoName,IdShip")] CreateUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userClass.Create(user);
                    return Json(new { result = true, url = "/User" });
                }
                catch (DbEntityValidationException e)
                {
                    foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                    {
                        foreach (DbValidationError ve in eve.ValidationErrors)
                        {
                            ModelState.AddModelError("", ve.ErrorMessage);
                        }
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
                }
            }

            LoadViewBag();
            return PartialView(user);
        }
        #endregion

        #region CheckRoleValueShip
        /// <summary>
        /// Vérification du le Ship soit selectionné si le role = Board
        /// </summary>
        /// <param name="RoleId">Le role id</param>
        /// <param name="IdShip">Le Ship</param>
        /// <returns>Return faux si le roleId = Board ET que le Ship vaut null</returns>
        [HttpGet]
        public ActionResult CheckRoleValueShip(int? idShip,string role)
        {
            bool result = !(role == Data.Constants.ROLE_ID_BOARD && idShip == 0);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Edit
        /// <summary>
        /// Formulaire de modification d'un utilisateur
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LoadViewBag();
            return PartialView(_userClass.GetUser(id));
        }

        /// <summary>
        /// Modification d'un utilisateur
        /// </summary>
        /// <param name="user">Instance de l'utilisateur modifié</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Edit([Bind(Include = "Id,IdAgency,LastName,FirstName,Enabled,Email,UserName,Role,LogoName,IdShip")] EditUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userClass.Edit(user);
                    return Json(new { result = true, url = "/User" });
                }
                catch (DbEntityValidationException e)
                {
                    foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                    {
                        foreach (DbValidationError ve in eve.ValidationErrors)
                        {
                            ModelState.AddModelError("", ve.ErrorMessage);
                        }
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
                }
            }

            LoadViewBag();
            return PartialView(user);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Suppression d'un utilisateur
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur</param>
        /// <returns>Redirection vers la liste des utilisateurs</returns>
        [HttpGet]
        public ActionResult Delete(string id)
        {
            ActionResult redirectUserList = _CheckDeleteLogo(id);
            _userClass.Delete(id);
            return redirectUserList;
        }
        #endregion

        #region CheckUserNameValue
        /// <summary>
        /// Remote Validation User Name
        /// </summary>
        /// <param name="userName">Login à valider</param>
        /// <param name="id">Id de l'utilisateur courrant</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CheckUserNameValue(string userName, string id)
        {
            bool result = _userClass.IsValidUserName(userName, id);
            if (!result)
            {
                if (string.IsNullOrEmpty(id))
                {
                    LogManager.InsertLog(LogManager.LogLevel.Warning, LogManager.LogType.User, LogManager.LogAction.Add, User.Identity.Name, "Add user with existing login. Login : " + userName.ToString());
                }
                else
                {
                    LogManager.InsertLog(LogManager.LogLevel.Warning, LogManager.LogType.User, LogManager.LogAction.Edit, User.Identity.Name, "Add user with existing login. Login : " + userName.ToString() + " Id : " + id);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region LogoUpload
        /// <summary>
        /// Telechargement d'un logo vers le serveur
        /// </summary>
        /// <param name="name">Nom du logo</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogoUpload(string name)
        {
            bool result = false;
            HttpPostedFileBase file = Request.Files[0];

            if (file != null)
            {
                string tmpFilename = User.Identity.Name + "_" + name + file.FileName;
                FileManager.FileSave(AppSettings.FolderTemp, tmpFilename, file);
                result = true;
            }

            return Json(result);
        }
        #endregion

        #region LogoDownload
        /// <summary>
        /// Telechargement d'un logo depuis le serveur
        /// </summary>
        /// <param name="idUser">Identifiant de l'utilisateur</param>
        /// <returns>Fichier a retourner</returns>
        [HttpGet]
        public ActionResult LogoDownload(string idUser)
        {
            if (string.IsNullOrEmpty(idUser))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            string filename = _userClass.GetFileName(idUser);
            if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string path = AppSettings.FolderLogos;
            string filePath = FileManager.FileGetPath(path, (idUser + filename));
            if (string.IsNullOrEmpty(filePath))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            
            byte[] fileData = System.IO.File.ReadAllBytes(filePath);
            string contentType = MimeMapping.GetMimeMapping(filePath);

            ContentDisposition cd = new ContentDisposition
            {
                FileName = HttpContext.Server.UrlPathEncode(filename),
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return (File(fileData, contentType) as FileResult);
        }
        #endregion

        #region LogoDelete
        /// <summary>
        /// Suppression d'un logo de l'utilisateur
        /// </summary>
        /// <param name="idUser">Identifiant de l'utilisateur</param>
        /// <returns>Redirection vers l'edition de l'utilisateur</returns>
        [HttpGet]
        public ActionResult LogoDelete(string idUser)
        {
            return _CheckDeleteLogo(idUser);
        }
        #endregion

        #region TemporaryLogosDelete
        /// <summary>
        /// Suppression des logos temporaires
        /// </summary>
        /// <param name="suppressionType">Type de suppression</param>
        [HttpGet]
        public bool TemporaryLogosDelete(int suppressionType)
        {
            switch (suppressionType)
            {
                case 1: // Suppression par utilisateur
                    string userName = User.Identity.Name;
                    FileManager.FileManyDelete(AppSettings.FolderTemp, (userName + "_"));
                    break;
            }
            return true;
        }
        #endregion

        #region Private

        #region LoadViewBag
        /// <summary>
        /// Chargement des liste du modéle commune
        /// </summary>
        private void LoadViewBag()
        {
            ViewBag.Roles = _userClass.GetRoles();

            List<SelectListItem> lstShip = _cruiseClass.GetLovList(Data.Constants.LOV_SHIP);
            lstShip.Insert(0, new SelectListItem { Value = "0", Text = "ALL" });
            ViewBag.ShipList = lstShip;

            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem() { Value = "True", Text = "Yes" },
                new SelectListItem() { Value = "False", Text = "No" }
            };
            ViewBag.Enabled = items;
        }
        #endregion
        
        #region _CheckDeleteLogo
        private ActionResult _CheckDeleteLogo(string idUser)
        {
            if (!string.IsNullOrEmpty(idUser))
            {
                try
                {
                    bool isDeleted = _userClass.FileDelete(idUser);
                    if (!isDeleted)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                    }
                }
                catch (Exception e)
                {
                    TempData["ErrorMessage"] = "An error occurred while deleting this document";
                    ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
                }
            }
            return RedirectToAction("Index");
        }
        #endregion

        #endregion
    }
}
