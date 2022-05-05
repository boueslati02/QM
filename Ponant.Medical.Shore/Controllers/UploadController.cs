using Ponant.Medical.Common;
using Ponant.Medical.Data;
using Ponant.Medical.Shore.Models;
using System;
using System.IO;
using System.Net;
using System.Web.Mvc;

namespace Ponant.Medical.Shore.Controllers
{
    [AllowAnonymous]
    public class UploadController : BaseController
    {

        #region Properties & Constructors

        private readonly UploadClass _uploadClass;
        public UploadController()
        {
            _uploadClass = new UploadClass(_shoreEntities);
        }

        #endregion

        #region Index
        [HttpGet]
        public ActionResult Index(string token)
        {
            if (string.IsNullOrEmpty(token) || !StringHelper.IsBase64String(token))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UploadViewModel model = new UploadViewModel();
            model.Token = token;
            try
            {
                model = _uploadClass.GetPassenger(token);
                _uploadClass.CheckPassengerLanguage(model.IdLanguage);
                LoadViewBag(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", string.Concat(ex.Message, ex.InnerException?.Message));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UploadViewModel model)
        {
            LoadViewBag(model);
            _uploadClass.CheckPassengerLanguage(model.IdLanguage);
            if (ModelState.IsValid)
            {
                try
                {
                    _uploadClass.AddDocuments(model);
                    _uploadClass.SendMailConfirmation(model.IdPassenger, model.IdLanguage);
                    return RedirectToAction("Success");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", string.Concat(ex.Message, ex.InnerException?.Message));
                }
                _uploadClass.SendMailError(model.IdPassenger, model.IdLanguage);
                return RedirectToAction("Error");
            }
            return RedirectToAction("Index", new { token = model.Token });
        }
        #endregion

        #region Success
        public ActionResult Success()
        {
            return View();
        }
        #endregion

        #region Error
        public ActionResult Error()
        {
            return View();
        }
        #endregion

        #region _Form
        [HttpGet]
        public ActionResult _Form()
        {
            return View();
        }
        #endregion

        #region _CruiseDeparture
        [HttpGet]
        public ActionResult _CruiseDeparture()
        {
            return View();
        }
        #endregion

        #region private

        #region LoadViewBag
        private void LoadViewBag(UploadViewModel model)
        {
            ViewBag.CruiseDeparture = false;
            if (DateTime.Now.CompareTo(model.SaillingDate).Equals(1))
            {
                ViewBag.CruiseDeparture = true;
            }

            if (!string.IsNullOrEmpty(model.AgencyLogo))
            {
                ViewBag.AgencyLogoPath = Path.Combine(AppSettings.FolderVirtualLogo, model.AgencyLogo);
            }

            ViewBag.QmAlreadySent = false;
            if (model.IdStatus == Constants.SHORE_STATUS_QM_RECEIVED || model.IdStatus == Constants.SHORE_STATUS_QM_NEW_DOCUMENTS)
            {
                ViewBag.QmAlreadySent = true;
            }

            int MonthDiff = (model.SaillingDate.Month - DateTime.Now.Month) + 12 * (model.SaillingDate.Year - DateTime.Now.Year);
            ViewBag.SendQmThreeMonthBefore = false;
            if (MonthDiff > 3)
            {
                ViewBag.SendQmThreeMonthBefore = true;
            }
        }
        #endregion

        #endregion
    }
}