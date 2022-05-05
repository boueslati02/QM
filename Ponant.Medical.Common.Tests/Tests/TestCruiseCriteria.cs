namespace Ponant.Medical.Common.Tests.Tests
{
    using Ponant.Medical.Data.Shore;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class TestCruiseCriteria : TestBase
    {
        #region Constructor
        public TestCruiseCriteria()
        {
            CruiseCriterion cruiseCriterion = _createObject.CruiseCriterionCreate(1, "MyCruiseTypeName", "MyDestinationCode", 
                "MyShipCode", 5, "MyCruiseCode", "MyActivityCode");            

            _testShoreEntities.CruiseCriterion.Add(cruiseCriterion);
        }
        #endregion

        #region Tests

        [Theory(DisplayName = "GetCriteria_CatchException_ReturnNull")]
        [InlineData("MyCruiseTypeName", "MyDestinationCode", "MyShipCode", 6, "MyCruiseCode", "MyActivityCode")]
        public void GetCriteria_CatchException_ReturnNull(string lovName, string lovDestinationCode, string lovShipCode, int sailingLengthDays, string cruiseCode, string activityCode)
        {
            BookingCruisePassenger bcp = _createObject.BookingCruisePassengerExceptionCreate(lovName, lovDestinationCode, lovShipCode, sailingLengthDays, cruiseCode, activityCode);
            CruiseCriteria controller = new CruiseCriteria(_testShoreEntities);
            CruiseCriterion result = controller.GetCriteria(new List<BookingCruisePassenger> { bcp });

            Assert.Null(result);
        }

        [Theory(DisplayName = "GetCriteria_ShouldReturnNull")]
        [InlineData("BadCruiseTypeName", "MyDestinationCode", "MyShipCode", 6, "MyCruiseCode", "MyActivityCode")]
        [InlineData("MyCruiseTypeName", "BadDestinationCode", "MyShipCode", 6, "MyCruiseCode", "MyActivityCode")]
        [InlineData("MyCruiseTypeName", "MyDestinationCode", "BadShipCode", 6, "MyCruiseCode", "MyActivityCode")]
        [InlineData("MyCruiseTypeName", "MyDestinationCode", "MyShipCode", 5, "MyCruiseCode", "MyActivityCode")]
        [InlineData("MyCruiseTypeName", "MyDestinationCode", "MyShipCode", 6, "BadCruiseCode", "MyActivityCode")]
        [InlineData("MyCruiseTypeName", "MyDestinationCode", "MyShipCode", 6, "MyCruiseCode", "BadActivityCode")]
        public void GetCriteria_ShouldReturnNull(string lovName, string lovDestinationCode, string lovShipCode, int sailingLengthDays, string cruiseCode, string activityCode)
        {
            BookingCruisePassenger bcp = _createObject.BookingCruisePassengerCreate(lovName, lovDestinationCode, lovShipCode, sailingLengthDays, cruiseCode, activityCode);
            CruiseCriteria controller = new CruiseCriteria(_testShoreEntities);
            CruiseCriterion result = controller.GetCriteria(new List<BookingCruisePassenger> { bcp });

            Assert.Null(result);
        }

        [Theory(DisplayName = "GetCriteria_ShouldReturnNull_FilledActivityName")]
        [InlineData("MyCruiseTypeName", "MyDestinationCode", "MyShipCode", 6, "MyCruiseCode", "MyActivityCode")]
        public void GetCriteria_ShouldReturnNull_FilledActivityName(string lovName, string lovDestinationCode, string lovShipCode, int sailingLengthDays, string cruiseCode, string activityCode)
        {
            BookingCruisePassenger bcp = _createObject.BookingCruisePassengerEmptyBookingActivityCreate(lovName, lovDestinationCode, lovShipCode, sailingLengthDays, cruiseCode, activityCode);
            CruiseCriteria controller = new CruiseCriteria(_testShoreEntities);
            CruiseCriterion result = controller.GetCriteria(new List<BookingCruisePassenger> { bcp });

            Assert.Null(result);
        }

        [Theory(DisplayName = "GetCriteria_ShouldReturnNull_EmptyActivityName")]
        [InlineData("MyCruiseTypeName", "MyDestinationCode", "MyShipCode", 6, "MyCruiseCode", "MyActivityCode")]
        public void GetCriteria_ShouldReturnNull_EmptyActivityName(string lovName, string lovDestinationCode, string lovShipCode, int sailingLengthDays, string cruiseCode, string activityCode)
        {
            BookingCruisePassenger bcp = _createObject.BookingCruisePassengerEmptyBookingActivityCreate(lovName, lovDestinationCode, lovShipCode, sailingLengthDays, cruiseCode, activityCode);
            _testShoreEntities.CruiseCriterion.First().Activity = null;
            CruiseCriteria controller = new CruiseCriteria(_testShoreEntities);
            CruiseCriterion result = controller.GetCriteria(new List<BookingCruisePassenger> { bcp });

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("MyCruiseTypeName", result.LovCruiseType.Name);
            Assert.Equal("MyDestinationCode", result.CruiseCriterionDestination.First().LovDestination.Code);
            Assert.Equal("MyShipCode", result.CruiseCriterionShip.First().LovShip.Code);
            Assert.Equal(5, result.Length);
            Assert.Equal("MyCruiseCode", result.Cruise);
        }

        [Theory(DisplayName = "GetCriteria_ShouldReturnCorrectCruiseCriteria")]
        [InlineData("MyCruiseTypeName", "MyDestinationCode", "MyShipCode", 6, "MyCruiseCode", "MyActivityCode")]
        public void GetCriteria_ShouldReturnCorrectCruiseCriteria(string lovName, string lovDestinationCode, string lovShipCode, int sailingLengthDays, string cruiseCode, string activityCode)
        {
            BookingCruisePassenger bcp = _createObject.BookingCruisePassengerCreate(lovName, lovDestinationCode, lovShipCode, sailingLengthDays, cruiseCode, activityCode);
            CruiseCriteria controller = new CruiseCriteria(_testShoreEntities);
            CruiseCriterion result = controller.GetCriteria(new List<BookingCruisePassenger> { bcp });

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("MyCruiseTypeName", result.LovCruiseType.Name);
            Assert.Equal("MyDestinationCode", result.CruiseCriterionDestination.First().LovDestination.Code);
            Assert.Equal("MyShipCode", result.CruiseCriterionShip.First().LovShip.Code);
            Assert.Equal(5, result.Length);
            Assert.Equal("MyCruiseCode", result.Cruise);
            Assert.Equal("MyActivityCode", result.Activity);
        }

        #endregion
    }
}

