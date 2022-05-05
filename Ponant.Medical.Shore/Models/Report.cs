namespace Ponant.Medical.Shore.Models
{
    using Microsoft.Reporting.WebForms;
    using System;
    using System.Web.UI.WebControls;

    public class Report
    {
        private string _reportPath { get; set; }

        public Report(string reportPath)
        {
            _reportPath = reportPath;
        }

        public ReportViewer GetReportViewer()
        {
            ReportViewer reportViewer = new ReportViewer
            {
                ProcessingMode = ProcessingMode.Remote,
                SizeToReportContent = true,
                Width = Unit.Percentage(100),
                Height = Unit.Percentage(100)
            };

            reportViewer.ServerReport.ReportServerUrl = new Uri(AppSettings.ReportServerUrl);
            reportViewer.ServerReport.ReportPath = _reportPath;

            return reportViewer;
        }
    }
}