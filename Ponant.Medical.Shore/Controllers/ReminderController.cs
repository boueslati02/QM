namespace Ponant.Medical.Shore.Controllers
{
    using Ponant.Medical.Shore.Models;
    using System;
    using System.Data.Entity.Validation;
    using System.Web.Mvc;

    [Authorize(Roles = "IT Administrator, Booking Administrator")]
    public class ReminderController : BaseController
    {
        #region Properties & Constructors

        private readonly ReminderClass _reminderClass;

        public ReminderController()
        {
            _reminderClass = new ReminderClass(_shoreEntities);
        }
        #endregion

        #region Edit
        /// <summary>
        /// Formulaire de modification des rappels
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit()
        {
            ReminderViewModel model = _reminderClass.GetReminders();
            return View(model);     
        }

        /// <summary>
        /// Modification des rappels
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReminderViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _reminderClass.Edit(model);
                    TempData["Message"] = "Reminders has been saved.";
                    return RedirectToAction("Edit", "Reminder");
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
            return View(model);
        }
        #endregion
    }
}