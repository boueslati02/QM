namespace Ponant.Medical.WebServices.Tests.Mocks
{
    using System.IO;
    using System.Security.Principal;
    using System.Web;
    using System.Web.SessionState;

    public class TestHttpContext
    {
        public HttpContext httpContext { get; set; }

        public TestHttpContext()
        {
            HttpRequest httpRequest = new HttpRequest("", "http://localhost/", "");
            StringWriter stringWriter = new StringWriter();
            HttpResponse httpResponce = new HttpResponse(stringWriter);
            HttpContext httpContext = new HttpContext(httpRequest, httpResponce);

            HttpSessionStateContainer sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(), new HttpStaticObjectsCollection(), 10, true, HttpCookieMode.AutoDetect, SessionStateMode.InProc, false);
            SessionStateUtility.AddHttpSessionStateToContext(httpContext, sessionContainer);

            httpContext.User = new GenericPrincipal(new GenericIdentity("System"), new string[] { "" });
            this.httpContext = httpContext;
        }
    }
}
