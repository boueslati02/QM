namespace Ponant.Medical.Common.Tests.Tests
{
    using Ponant.Medical.Data.Shore;
    using System;
    using System.Collections.Generic;
    using System.Xml.Schema;
    using Xunit;

    public class TestPropertyComparer : TestBase
    {
        #region Tests

        [Theory(DisplayName = "PropertyComparer_ShouldThrowException")]
        [InlineData("Mesage")]
        public void PropertyComparer_ShouldThrowException(string propertyName)
        {
            Assert.Throws<ArgumentException>(() => new PropertyComparer<ValidationEventArgs>(propertyName));
        }

        [Theory(DisplayName = "PropertyComparer_InvalidPropertyName")]
        [InlineData("ID")]
        public void PropertyComparer_InvalidPropertyName(string propertyName)
        {
            Booking b1 = _createObject.CreateBooking(0, 0, "myself");
            List<Booking> bookingList = new List<Booking> { b1 };
            
            Assert.Throws<ArgumentException>(() => _createObject.CompareBookingList(bookingList, propertyName));
        }

        [Theory(DisplayName = "PropertyComparer_NullCreatorProperty")]
        [InlineData("Creator")]
        public void PropertyComparer_NullCreatorProperty(string propertyName)
        {
            Booking b1 = _createObject.CreateBooking(0, 0, null);
            Booking b2 = _createObject.CreateBooking(1, 1, null);
            List<Booking> bookingList = new List<Booking> { b1, b2 };

            List<Booking> result = _createObject.CompareBookingList(bookingList, propertyName);
            Assert.Single(result);
        }

        [Theory(DisplayName = "PropertyComparer_DistinctOnEveryProperty_TwoValues")]
        [InlineData("Id")]
        [InlineData("Number")]
        [InlineData("Creator")]
        public void PropertyComparer_DistinctOnEveryProperty_TwoValues(string propertyName)
        {
            Booking b1 = _createObject.CreateBooking(0, 0, "myself");
            Booking b2 = _createObject.CreateBooking(1, 1, "yourself");
            List<Booking> bookingList = new List<Booking>{ b1, b2 };

            List<Booking> result = _createObject.CompareBookingList(bookingList, propertyName);
            Assert.Equal(2, result.Count);
        }

        [Theory(DisplayName = "PropertyComparer_DistinctOnEveryProperty_ThreeValues")]
        [InlineData("Id")]
        [InlineData("Number")]
        [InlineData("Creator")]
        public void PropertyComparer_DistinctOnEveryProperty_ThreeValues(string propertyName)
        {
            Booking b1 = _createObject.CreateBooking(0, 0, "myself");
            Booking b2 = _createObject.CreateBooking(1, 1, "yourself");
            Booking b3 = _createObject.CreateBooking(2, 2, "them");
            List<Booking> bookingList = new List<Booking> { b1, b2, b3 };

            List<Booking> result = _createObject.CompareBookingList(bookingList, propertyName);
            Assert.Equal(3, result.Count);
        }

        [Theory(DisplayName = "PropertyComparer_ThreeSameValuesOnEachProperty")]
        [InlineData("Id")]
        [InlineData("Number")]
        [InlineData("Creator")]
        public void PropertyComparer_ThreeSameValuesOnEachProperty(string propertyName)
        {
            Booking b1 = _createObject.CreateBooking(0, 0, "myself");
            Booking b2 = _createObject.CreateBooking(0, 0, "myself");
            Booking b3 = _createObject.CreateBooking(0, 0, "myself");
            List<Booking> bookingList = new List<Booking> { b1, b2, b3 };

            List<Booking> result = _createObject.CompareBookingList(bookingList, propertyName);
            Assert.Single(result);
        }

        #endregion
    }
}
