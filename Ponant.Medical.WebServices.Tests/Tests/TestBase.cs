namespace Ponant.Medical.WebServices.Tests.Tests
{
    using Ponant.Medical.Common.Tests.Mocks;
    using Ponant.Medical.Common.Tests.MocksDbSetContext;
    using Ponant.Medical.WebServices.Tests.Helpers;
    using Ponant.Medical.WebServices.Tests.Mocks;
    using System;
    using System.Web;

    public class TestBase : IDisposable
    {
        #region Properties

        protected TestShoreEntitiesContext _testShoreEntities;

        protected TestAuthContext _testAuthContext;

        protected TestHttpContext _testHttpContext;

        protected TestFileHelper _testFileHelper;

        protected TestArchiveHelper _testArchiveHelper;

        protected CreateObject _createObject;

        #endregion

        #region Constructor
        public TestBase()
        {
            _testShoreEntities = new TestShoreEntitiesContext();
            _testAuthContext = new TestAuthContext();
            _testFileHelper = new TestFileHelper();
            _testArchiveHelper = new TestArchiveHelper();
            _testHttpContext = new TestHttpContext();
            _createObject = new CreateObject();
            HttpContext.Current = _testHttpContext.httpContext;
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            _testShoreEntities.Dispose();
            _testAuthContext.Dispose();
            _testFileHelper = null;
            _testArchiveHelper = null;
            _testHttpContext = null;
            _createObject = null;
            HttpContext.Current = null;
        }
        #endregion
    }
}
