namespace Ponant.Medical.Common.Tests.Tests
{
    using Ponant.Medical.Common.Tests.Helpers;
    using Ponant.Medical.Common.Tests.MocksDbSetContext;
    using System;

    public class TestBase : IDisposable
    {
        #region Properties

        protected TestShoreEntitiesContext _testShoreEntities;

        protected CreateObject _createObject;

        #endregion

        #region Constructor
        public TestBase()
        {
            _testShoreEntities = new TestShoreEntitiesContext();
            _createObject = new CreateObject();
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            _testShoreEntities.Dispose();
            _createObject = null;
        }
        #endregion
    }
}
