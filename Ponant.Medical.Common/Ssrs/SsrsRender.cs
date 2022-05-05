using Ponant.Medical.Common.wsSsrsExecution;
using System.Collections.Generic;
using System.Net;

namespace Ponant.Medical.Common.Ssrs
{
    public class SsrsRender
    {
        internal const int SendTimeout = 300000;

        private readonly ReportExecutionService _wsReportExecutionService;

        public enum RenderReportFormat
        {
            XML,
            CSV,
            IMAGE,
            PDF,
            EXCEL,
            WORD,
            MHTML
        }

        public SsrsRender(string url, string userName, string password, string domain)
        {
            _wsReportExecutionService = new ReportExecutionService();
            _wsReportExecutionService.Url = url;
            _wsReportExecutionService.Credentials = new NetworkCredential(userName, password, domain);
            _wsReportExecutionService.Timeout = SendTimeout;
        }

        public byte[] Render(string reportPath, RenderReportFormat renderReportFormat, Dictionary<string, string> parameters)
        {
            ExecutionInfo info = _wsReportExecutionService.LoadReport(reportPath, null);

            ExecutionHeader header = new ExecutionHeader();
            header.ExecutionID = info.ExecutionID;
            _wsReportExecutionService.ExecutionHeaderValue = header;

            ParameterValue[] parameterValues = GetParameterValues(parameters);
            _wsReportExecutionService.SetExecutionParameters(parameterValues, "fr-FR");

            return _wsReportExecutionService.Render(renderReportFormat.ToString(), null, out string extension, out string mimeType, out string encoding, out Warning[] warnings, out string[] streamids);
        }

        private ParameterValue[] GetParameterValues(Dictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return new ParameterValue[0];
            }

            List<ParameterValue> parameterValues = new List<ParameterValue>();

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                parameterValues.Add(new ParameterValue
                {
                    Name = parameter.Key,
                    Value = parameter.Value
                });
            }

            return parameterValues.ToArray();
        }
    }
}
