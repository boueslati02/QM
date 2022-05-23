using Newtonsoft.Json;
using Ponant.Medical.Data.Shore;
using Ponant.Medical.Data.Shore.Models;
using Ponant.Medical.Shore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ponant.Medical.Shore.Controllers
{
    public class EmailController : BaseController
    {
        private SendEmail _sendEmail;
        public EmailController()
        {
            _sendEmail = new SendEmail(_shoreEntities);
        }
        // GET: Email
        public ActionResult ValidQM()
        {
            string token = HttpContext.Request.QueryString["token"];
            string DecryptToken = _sendEmail.DecryptString(token);
            UserToken DescryptUser = JsonConvert.DeserializeObject<UserToken>(DecryptToken);
            Cruise cruise = _shoreEntities.Cruise.Find(6);
            if(cruise == null)
            {
                ViewBag.Message = "The cruise is cancelled";
                return View("~/Views/ConfirmQM/CruiseCanceled.cshtml");
            }else
            {
                int result = DateTime.Compare(cruise.SailingDate, DateTime.Now);
                if (result < 0)
                {
                    ViewBag.Message = "Cruise already passed";
                    return View("~/Views/ConfirmQM/CruiseCanceled.cshtml");
                }
                else
                {
                    Passenger passenger = _shoreEntities.Passenger.Where(p => p.Number.ToString() == DescryptUser.PassengerNo).FirstOrDefault();
                    if (passenger.IdAdvice == 44)
                    {
                        ViewBag.Message = "You have already validated your medical questionnaire";
                        return View("~/Views/ConfirmQM/CruiseCanceled.cshtml");
                    }
                    else
                    {
                        ViewBag.Token = token.Replace(" ", "+");
                        ViewBag.DescryptUser = DescryptUser;
                        ViewBag.Language = _shoreEntities.Language.Where(l => l.Id == 46).FirstOrDefault();
                        return View("~/Views/ConfirmQM/ConfirmQM.cshtml");
                    }
                }
            }
            
            
            
        }
        [HttpPost]
        public JsonResult ChangeStatusPassager()
        {
            string token = Request.Form["Token"];
            string ConfirmQM = Request.Form["ConfirmQM"];
            string DecryptToken = _sendEmail.DecryptString(token);
            UserToken DescryptUser = JsonConvert.DeserializeObject<UserToken>(DecryptToken);
            Passenger passenger = _shoreEntities.Passenger.Where(p => p.Number.ToString() == DescryptUser.PassengerNo).FirstOrDefault();
            _sendEmail.ValidQM(passenger.Id);
            return Json(true);
        }


         
    }
}