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

    public class PassengerTest : TestBase
    {
        #region PassengerTest
        public PassengerTest()
        {
            BookingCruisePassenger bcp = _createObject.BookingCruisePassengerCreate(1, false, "test");

            Passenger p0 = _createObject.PassengerCreate(_testHttpContext, 0, "Dupond", "TEST", "test.dupond@gmail.com",
                Constants.SHORE_STATUS_QM_SENT, Constants.ADVICE_FAVORABLE_OPINION, true);

            Passenger p1 = _createObject.PassengerCreate(_testHttpContext, 3, "Dupond1", "TEST1", "test1.dupond1@gmail.com",
                Constants.SHORE_STATUS_QM_SENT);

            Passenger p2 = _createObject.PassengerCreate(_testHttpContext, 5, "Dupond2", "TEST2", "test2.dupond2@gmail.com",
                Constants.SHORE_STATUS_QM_SENT);

            Lov lov = _createObject.LovCreate(Constants.ADVICE_FAVORABLE_OPINION, 0, null, "Favorable opinion");

            vPassengerBoard vpb = _createObject.VPassengerBoardCreate(0, 1, 3);

            Document document = _createObject.DocumentCreate(1, 0, "test@test.com", "filename");

            p0.BookingCruisePassenger.Add(bcp);
            _testShoreEntities.Passenger.Add(p0);
            _testShoreEntities.Passenger.Add(p1);
            _testShoreEntities.Passenger.Add(p2);
            _testShoreEntities.Lov.Add(lov);
            _testShoreEntities.vPassengerBoard.Add(vpb);
            _testShoreEntities.Document.Add(document);
        }
        #endregion

        #region Tests

        [Theory(DisplayName = "ChangeStatusPassenger_ShouldReturnException")]
        [InlineData(0)]
        public void ChangeStatusPassenger_ShouldReturnException(int passengerId)
        {
            PassengerController controller = new PassengerController(null, null, null);
            ExceptionResult result = controller.ChangeStatusPassenger(passengerId) as ExceptionResult;

            Assert.NotNull(result);
            Assert.IsType<ExceptionResult>(result);
        }

        [Theory(DisplayName = "ChangeStatusPassenger_InvalidPassengerId")]
        [InlineData(2)]
        public void ChangeStatusPassenger_InvalidPassengerId(int passengerId)
        {
            PassengerController controller = new PassengerController(_testShoreEntities, null, null);
            NotFoundResult result = controller.ChangeStatusPassenger(passengerId) as NotFoundResult;

            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory(DisplayName = "ChangeStatusPassenger_ValidPassengerId")]
        [InlineData(3)]
        [InlineData(5)]
        public void ChangeStatusPassenger_ValidPassengerId(int passengerId)
        {
            PassengerController controller = new PassengerController(_testShoreEntities, null, null);
            OkResult result = controller.ChangeStatusPassenger(passengerId) as OkResult;

            Passenger currentPassenger = _testShoreEntities.Passenger.Find(passengerId);

            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
            Assert.Equal(true, currentPassenger.IsDownloaded);
        }

        [Theory(DisplayName = "EditAdvice_IdentifiersDoNotMatch")]
        [InlineData(0, 1, 1, 1)]
        public void EditAdvice_IdentifiersDoNotMatch(
            int idPassenger,
            int idPassengerAdvice,
            int idAdvice,
            int idCruise)
        {
            PassengerController controller = new PassengerController(_testShoreEntities, null, null);
            AdviceBoard adviceBoard = _createObject.AdviceBoardCreate(idPassengerAdvice, idAdvice, idCruise);
            BadRequestErrorMessageResult result = controller.EditAdvice(idPassenger, adviceBoard) as BadRequestErrorMessageResult;

            Assert.NotNull(result);
            Assert.IsType<BadRequestErrorMessageResult>(result);
            Assert.Equal("The two identifiers do not match.", result.Message);
        }

        [Theory(DisplayName = "EditAdvice_IdentifiersMatch_NonExistentPassenger")]
        [InlineData(1, 1, 1, 1)]
        public void EditAdvice_IdentifiersMatch_NonExistentPassenger(
            int idPassenger,
            int idPassengerAdvice,
            int idAdvice,
            int idCruise)
        {
            PassengerController controller = new PassengerController(_testShoreEntities, null, null);
            AdviceBoard adviceBoard = _createObject.AdviceBoardCreate(idPassengerAdvice, idAdvice, idCruise);
            NotFoundResult result = controller.EditAdvice(idPassenger, adviceBoard) as NotFoundResult;

            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

        //[Fact(Skip = "specific reason")]
        //[InlineData(0, 0, Constants.ADVICE_FAVORABLE_OPINION, 1, "commentTest")]
        //public void EditAdvice_IdentifiersMatch_PassengerExists(
        //    int idPassenger,
        //    int idPassengerAdvice,
        //    int idAdvice,
        //    int idCruise,
        //    string comment)
        //{
        //    PassengerController controller = new PassengerController(_testShoreEntities, null, null);
        //    AdviceBoard adviceBoard = _createObject.AdviceBoardCreate(idPassengerAdvice, idAdvice, idCruise, comment);
        //    OkResult result = controller.EditAdvice(idPassenger, adviceBoard) as OkResult;

        //    Passenger passenger = _testShoreEntities.Passenger.Find(0);

        //    Assert.NotNull(result);
        //    Assert.IsType<OkResult>(result);
        //    Assert.Equal(passenger.IdAdvice, Constants.ADVICE_FAVORABLE_OPINION);
        //    Assert.Equal(passenger.IdStatus, Constants.SHORE_STATUS_QM_CLOSED);
        //    Assert.False(passenger.IsExtract);
        //    Assert.Equal(comment, passenger.Review);
        //}

        [Theory(DisplayName = "ExtractPassenger_ShouldReturnException")]
        [InlineData(0, 1)]
        public void ExtractPassenger_ShouldReturnException(int passengerId, int cruiseId)
        {
            PassengerController controller = new PassengerController(_testShoreEntities, null, null);
            Passenger passenger = _testShoreEntities.Passenger.Find(0);
            BookingCruisePassenger bcp = new List<BookingCruisePassenger>(passenger.BookingCruisePassenger).First();
            bcp.Cruise = null;
            ExceptionResult result = controller.ExtractPassenger(passengerId, cruiseId) as ExceptionResult;

            Assert.NotNull(result);
            Assert.IsType<ExceptionResult>(result);
            Assert.NotNull(result.Exception);
        }

        [Theory(DisplayName = "ExtractPassenger_NonExistentPassenger")]
        [InlineData(6, 1)]
        public void ExtractPassenger(int passengerId, int cruiseId)
        {
            PassengerController controller = new PassengerController(_testShoreEntities, null, null);
            IHttpActionResult result = controller.ExtractPassenger(passengerId, cruiseId);

            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory(DisplayName = "ExtractPassenger_WithBookingCruisePassenger_WrongCruiseId")]
        [InlineData(0, 2)]
        public void ExtractPassenger_WithBookingCruisePassenger_WrongCruiseId(int passengerId, int cruiseId)
        {
            PassengerController controller = new PassengerController(_testShoreEntities, null, null);
            OkResult result = controller.ExtractPassenger(passengerId, cruiseId) as OkResult;

            Passenger passenger = _testShoreEntities.Passenger.Find(0);

            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
            Assert.True(passenger.IsExtract);
            Assert.Equal(passenger.IdStatus, Constants.SHORE_STATUS_QM_IN_PROGRESS);
        }

        //[Theory(DisplayName = "ExtractPassenger_WithBookingCruisePassenger_RightCruiseId")]
        //[InlineData(0, 1)]
        //public void ExtractPassenger_WithBookingCruisePassenger_RightCruiseId(int passengerId, int cruiseId)
        //{
        //    PassengerController controller = new PassengerController(_testShoreEntities, null, null);
        //    OkResult result = controller.ExtractPassenger(passengerId, cruiseId) as OkResult;

        //    Passenger passenger = _testShoreEntities.Passenger.Find(0);
        //    BookingCruisePassenger bcp = new List<BookingCruisePassenger>(passenger.BookingCruisePassenger).First();

        //    Assert.NotNull(result);
        //    Assert.IsType<OkResult>(result);
        //    Assert.True(passenger.IsExtract);
        //    Assert.Equal(passenger.IdStatus, Constants.SHORE_STATUS_QM_IN_PROGRESS);
        //    Assert.True(bcp.Cruise.IsExtract);
        //    Assert.Equal(bcp.Cruise.Extract, HttpContext.Current.User.Identity.Name);
        //}

        [Theory(DisplayName = "GetPassenger_WrongCruiseId")]
        [InlineData(0, 2)]
        public void GetPassenger_WrongCruiseId(int passengerId, int cruiseId)
        {
            PassengerController controller = new PassengerController(_testShoreEntities, null, _testArchiveHelper);
            ExceptionResult result = controller.GetPassenger(passengerId, cruiseId) as ExceptionResult;

            Assert.NotNull(result);
            Assert.IsType<ExceptionResult>(result);
            Assert.NotNull(result.Exception);
        }

        [Theory(DisplayName = "GetPassenger_RightPassengerId_RightCruiseId")]
        [InlineData(0, 1)]
        public void GetPassenger_RightPassengerId_RightCruiseId(int passengerId, int cruiseId)
        {
            PassengerController controller = new PassengerController(_testShoreEntities, _testFileHelper, _testArchiveHelper);
            JsonResult<vPassengerBoard> result = controller.GetPassenger(passengerId, cruiseId) as JsonResult<vPassengerBoard>;

            Assert.NotNull(result);
            Assert.IsType<JsonResult<vPassengerBoard>>(result);
            Assert.Equal(0, result.Content.Id);
            Assert.Equal(1, result.Content.IdCruise);
            Assert.Equal(120, result.Content.Documents.Length);
        }

        [Theory(DisplayName = "GetPassengerByCruise_ShouldReturnException")]
        [InlineData(1, 3)]
        public void GetPassengerByCruise_ShouldReturnException(int cruiseId, int idStatus)
        {
            PassengerController controller = new PassengerController(null, null, null);
            ExceptionResult result = controller.GetPassengersByCruise(cruiseId, idStatus) as ExceptionResult;

            Assert.NotNull(result);
            Assert.IsType<ExceptionResult>(result);
            Assert.NotNull(result.Exception);
            Assert.NotNull(result.Exception.Message);
        }

        [Theory(DisplayName = "GetPassengerByCruise")]
        [InlineData(1, 3)]
        public void GetPassengerByCruise(int cruiseId, int idStatus)
        {
            PassengerController controller = new PassengerController(_testShoreEntities, null, null);
            JsonResult<List<PassengerCruise>> result = controller.GetPassengersByCruise(cruiseId, idStatus) as JsonResult<List<PassengerCruise>>;

            Assert.NotNull(result);
            Assert.IsType<JsonResult<List<PassengerCruise>>>(result);
            PassengerCruise pc = result.Content.ElementAt(0);
            Assert.Equal(0, pc.IdPassenger);
            Assert.Equal(1, pc.IdCruise);
        }

        #endregion
    }
}
