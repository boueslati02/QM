using Ponant.Medical.Common.Tests.Mocks;
using Ponant.Medical.Common.Tests.MocksDbSetContext;
using Ponant.Medical.WebServices.Tests.Helpers;
using System;

namespace Ponant.Medical.Shore.Tests.Tests
{
    public class TestBase : IDisposable
    {
        #region Properties

        protected TestShoreEntitiesContext _testShoreEntities;

        protected TestAuthContext _testAuthContext;

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
            _createObject = new CreateObject();
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            _testShoreEntities.Dispose();
            _testAuthContext.Dispose();
            _testFileHelper = null;
            _testArchiveHelper = null;
            _createObject = null;
        }
        #endregion
    }
}
