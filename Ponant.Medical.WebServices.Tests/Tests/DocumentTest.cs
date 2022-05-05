namespace Ponant.Medical.WebServices.Tests.Tests
{
    using Ponant.Medical.Data;
    using Ponant.Medical.Data.Shore;
    using Ponant.Medical.WebServices.Controllers;
    using System.Linq;
    using System.Web;
    using System.Web.Http.Results;
    using Xunit;

    public class DocumentTest : TestBase
    {
        #region Constructor
        public DocumentTest()
        {
            Document document = _createObject.DocumentCreate(1, 1, "test.test@test.com", "test.test", 1, "test", "test.test");

            _testShoreEntities.Document.Add(document);
        }
        #endregion

        #region Tests

        [Fact(DisplayName = "UnlinkDocument_ShouldReturnException")]
        public void UnlinkDocument_ShouldReturnException()
        {
            DocumentController controller = new DocumentController(null, null);
            ExceptionResult result = controller.UnlinkDocument(0) as ExceptionResult;

            Assert.NotNull(result);
            Assert.IsType<ExceptionResult>(result);
        }

        [Fact(DisplayName = "UnlinkDocument_ShouldReturnNotFound")]
        public void UnlinkDocument_ShouldReturnNotFound()
        {
            DocumentController controller = new DocumentController(_testShoreEntities, _testFileHelper);
            NotFoundResult result = controller.UnlinkDocument(0) as NotFoundResult;

            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact(DisplayName = "UnlinkDocument_ShouldReturnStatusCode")]
        public void UnlinkDocument_ShouldReturnStatusCode()
        {
            DocumentController controller = new DocumentController(_testShoreEntities, _testFileHelper);
            OkResult result = controller.UnlinkDocument(1) as OkResult;

            Document document = _testShoreEntities.Document.First();

            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
            Assert.Equal(Constants.NOT_APPLICABLE_NOT_APPLICABLE, document.IdPassenger);
            Assert.Equal(HttpContext.Current.User.Identity.Name, document.Editor);
        }

        #endregion
    }
}