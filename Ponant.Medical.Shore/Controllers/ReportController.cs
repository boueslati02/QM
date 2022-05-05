namespace Ponant.Medical.Shore.Controllers
{
    using Ponant.Medical.Shore.Models;
    using System.Web.Mvc;

    [Authorize(Roles = "IT Administrator, Booking Administrator, Doctor, Medical, Medical Administrator, Group")]
    public class ReportController : BaseController
    {
        
        public ActionResult QmStatistics()
        {
            Report report = new Report(AppSettings.ReportPathQMStatistics);

            return View(report.GetReportViewer());
        }

        public ActionResult QmTreatment()
        {
            Report report = new Report(AppSettings.ReportPathQMTreatment);

            return View(report.GetReportViewer());
        }
    }
}