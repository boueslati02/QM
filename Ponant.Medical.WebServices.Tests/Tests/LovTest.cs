namespace Ponant.Medical.WebServices.Tests.Tests
{
    using Ponant.Medical.Data.Shore;
    using Ponant.Medical.Data.Shore.Models;
    using Ponant.Medical.WebServices.Controllers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.Results;
    using Xunit;

    public class LovTest : TestBase
    {
        #region Constructor
        public LovTest()
        {
            Lov lov1 = _createObject.LovCreate(1, 1, "Test1", "Test1");
            Lov lov2 = _createObject.LovCreate(2, 2, "Test2", "Test2");
            Lov lov3 = _createObject.LovCreate(3, 3, "Test3", "Test3");

            _testShoreEntities.Lov.Add(lov1);
            _testShoreEntities.Lov.Add(lov2);
            _testShoreEntities.Lov.Add(lov3);
        }
        #endregion

        #region Tests

        [Fact(DisplayName = "GetLov_ShouldReturnException")]
        public void GetLov_ShouldReturnException()
        {
            LovController controller = new LovController(null);
            ExceptionResult result = controller.GetLov() as ExceptionResult;

            Assert.NotNull(result);
            Assert.IsType<ExceptionResult>(result);
            Assert.NotNull(result.Exception);
            Assert.NotNull(result.Exception.Message);
        }

        [Theory(DisplayName = "GetLov_ShouldReturnAllLines")]
        [InlineData(3)]
        public void GetLov_ShouldReturnAllLines(int lovCount)
        {
            LovController controller = new LovController(_testShoreEntities);
            JsonResult<List<LovBoard>> result = controller.GetLov() as JsonResult<List<LovBoard>>;

            Data.Shore.Lov lov = _testShoreEntities.Lov.First();
            LovBoard lovBoard = result.Content.First();

            Assert.NotNull(result);
            Assert.NotNull(result.Content);
            Assert.IsType<List<LovBoard>>(result.Content);
            Assert.Equal(lovCount, result.Content.Count);
            Assert.Equal(lov.Id, lovBoard.Id);
            Assert.Equal(lov.IdLovType, lovBoard.IdLovType);
            Assert.Equal(lov.Code, lovBoard.Code);
            Assert.Equal(lov.Name, lovBoard.Name);
            Assert.Equal(lov.IsEnabled, lovBoard.IsEnabled);
            Assert.Equal(lov.Creator, lovBoard.Creator);
            Assert.Equal(lov.CreationDate, lovBoard.CreationDate);
            Assert.Equal(lov.Editor, lovBoard.Editor);
            Assert.Equal(lov.ModificationDate, lovBoard.ModificationDate);
        }

        #endregion
    }
}