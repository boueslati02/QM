namespace Ponant.Medical.WebServices.Tests.Tests
{
    using Ponant.Medical.Data;
    using Ponant.Medical.Data.Shore;
    using Ponant.Medical.WebServices.Controllers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Results;
    using Xunit;

    public class CruiseTest : TestBase
    {
        #region Constructor
        public CruiseTest()
        {
            Cruise cruise = _createObject.CruiseCreate(_testHttpContext, 1, 0, 27, 22, "S0TEST", 10, 10, true);

            vCruiseBoard vCruiseBoard1 = _createObject.VCruiseBoardCreate(66, "S0TEST1", 10, 10, 1);
            vCruiseBoard vCruiseBoard2 = _createObject.VCruiseBoardCreate(67, "S0TEST2", 8, 7, 2);

            _testShoreEntities.Cruise.Add(cruise);
            _testShoreEntities.vCruiseBoard.Add(vCruiseBoard1);
            _testShoreEntities.vCruiseBoard.Add(vCruiseBoard2);
        }
        #endregion

        #region Tests

        [Theory(DisplayName = "FreeCruise_CheckReturnTypeNonExistentCruiseId")]
        [InlineData(0)]
        public void FreeCruise_CheckReturnTypeWrongCruiseId(int cruiseId)
        {
            CruiseController controller = new CruiseController(_testShoreEntities);
            NotFoundResult result = controller.FreeCruise(cruiseId) as NotFoundResult;

            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory(DisplayName = "FreeCruise_CheckReturnTypeRightCruiseId")]
        [InlineData(1)]
        public void FreeCruise_CheckReturnTypeRightCruiseId(int cruiseId)
        {
            CruiseController controller = new CruiseController(_testShoreEntities);
            OkResult result = controller.FreeCruise(cruiseId) as OkResult;

            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
        }

        [Theory(DisplayName = "FreeCruise_CheckReturnObjectRightCruiseId")]
        [InlineData(1)]
        public void FreeCruise_CheckReturnObjectRightCruiseId(int cruiseId)
        {
            CruiseController controller = new CruiseController(_testShoreEntities);
            OkResult result = controller.FreeCruise(cruiseId) as OkResult;

            Cruise cruise = _testShoreEntities.Cruise.First();

            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
            Assert.False(cruise.IsExtract);
            Assert.Equal(HttpContext.Current.User.Identity.Name, cruise.Editor);
            Assert.Equal(Constants.SHORE_STATUS_QM_RECEIVED, cruise.BookingCruisePassenger.First().Passenger.IdStatus);
            Assert.False(cruise.BookingCruisePassenger.First().Passenger.IsExtract);
            Assert.Equal(Constants.SHORE_STATUS_QM_SENT, cruise.BookingCruisePassenger.ElementAt(1).Passenger.IdStatus);
            Assert.True(cruise.BookingCruisePassenger.ElementAt(1).Passenger.IsExtract);
        }

        [Fact(DisplayName = "GetCruises_ShouldReturnException")]
        public void GetCruises_ShouldReturnException()
        {
            CruiseController controller = new CruiseController(null);
            ExceptionResult result = controller.GetCruises() as ExceptionResult;

            Assert.NotNull(result);
            Assert.IsType<ExceptionResult>(result);
            Assert.NotNull(result.Exception);
            Assert.NotNull(result.Exception.Message);
        }

        [Fact(DisplayName = "GetCruises_CheckReturnType")]
        public void GetCruises_CheckReturnType()
        {
            CruiseController controller = new CruiseController(_testShoreEntities);
            OkResult result = controller.GetCruises() as OkResult;

            Assert.Null(result);
        }

        [Fact(DisplayName = "GetCruises_CheckReturnObject")]
        public void GetCruises_CheckReturnObject()
        {
            CruiseController controller = new CruiseController(_testShoreEntities);
            JsonResult<List<vCruiseBoard>> result = controller.GetCruises() as JsonResult<List<vCruiseBoard>>;

            Assert.NotNull(result);
            Assert.Equal(2, result.Content.Count);
            Assert.Equal(66, result.Content.First().Id);
            Assert.Equal(67, result.Content.ElementAt(1).Id);
        }

        [Theory(DisplayName = "IsExtractCruise_NonExistentCruiseId")]
        [InlineData(0)]
        public void IsExtractCruise_WrongCruiseId(int cruiseId)
        {
            CruiseController controller = new CruiseController(_testShoreEntities);
            IHttpActionResult result = controller.IsExtractCruise(cruiseId);

            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory(DisplayName = "IsExtractCruise_RightCruiseId")]
        [InlineData(1)]
        public void IsExtractCruise_RightCruiseId(int cruiseId)
        {
            CruiseController controller = new CruiseController(_testShoreEntities);
            JsonResult<bool> result = controller.IsExtractCruise(cruiseId) as JsonResult<bool>;

            Assert.NotNull(result);
            Assert.True(result.Content);
        }

        #endregion
    }
}
