namespace Ponant.Medical.Common.Tests.Helpers
{
    using Ponant.Medical.Data.Shore;
    using System.Collections.Generic;
    using System.Linq;

    public class CreateObject
    {
        #region LovCreate
        public Lov LovCreate(
            int id,
            string code = null,
            string name = null)
        {
            Lov lov = new Lov
            {
                Id = id,
                Name = name,
                Code = code
            };

            return lov;
        }
        #endregion

        #region CruiseCriterionCreate
        public CruiseCriterion CruiseCriterionCreate(
            int id,
            string cruiseTypeName = "",
            string cruiseCriterionDestinationName = "", 
            string cruiseCriterionShipName = "",
            int length = 0,
            string cruiseNames = null,
            string activityNames = null)
        {
            CruiseCriterionDestination cruiseCriterionDestination = new CruiseCriterionDestination
            {
                LovDestination = LovCreate(1, cruiseCriterionDestinationName)
            };

            CruiseCriterionShip cruiseCriterionShip = new CruiseCriterionShip
            {
                LovShip = LovCreate(2, cruiseCriterionShipName)
            };

            List<CruiseCriterionDestination> cruiseCriterionDestinationList = new List<CruiseCriterionDestination>
            {
                cruiseCriterionDestination
            };

            List<CruiseCriterionShip> cruiseCriterionShipsList = new List<CruiseCriterionShip>
            {
                cruiseCriterionShip
            };

            CruiseCriterion cruiseCriterion = new CruiseCriterion
            {
                Id = id,
                IdCruiseType = 1,
                IdSurvey = 0,
                Survey = new Survey(),
                LovCruiseType = LovCreate(3, name: cruiseTypeName),
                CruiseCriterionDestination = cruiseCriterionDestinationList,
                CruiseCriterionShip = cruiseCriterionShipsList,
                Length = length,
                Cruise = cruiseNames,
                Activity = activityNames
            };

            return cruiseCriterion;
        }
        #endregion

        #region BookingCruisePassengerEmptyBookingActivityCreate
        public BookingCruisePassenger BookingCruisePassengerEmptyBookingActivityCreate(
          string lovName,
          string lovDestinationCode,
          string lovShipCode,
          int sailingLengthDays,
          string cruiseCode,
          string activityCode)
        {
            Booking booking = new Booking
            {
                BookingActivity = new List<BookingActivity>() { }
            };

            BookingCruisePassenger bookingCruisePassenger = new BookingCruisePassenger
            {
                Id = 0,
                IdPassenger = 0,
                IdCruise = 0,
                IdBooking = 0,
                Booking = booking,
                Cruise = new Cruise
                {
                    LovTypeCruise = LovCreate(2, name: lovName),
                    LovDestination = LovCreate(3, lovDestinationCode),
                    LovShip = LovCreate(4, lovShipCode),
                    SailingLengthDays = sailingLengthDays,
                    Code = cruiseCode
                }
            };

            return bookingCruisePassenger;
        }
        #endregion

        #region BookingCruisePassengerCreate
        public BookingCruisePassenger BookingCruisePassengerCreate(
          string lovName,
          string lovDestinationCode,
          string lovShipCode,
          int sailingLengthDays,
          string cruiseCode,
          string activityCode)
        {
            BookingActivity bookingActivity = new BookingActivity
            {
                IdPassenger = 0,
                LovActivity = LovCreate(1, activityCode)
            };

            Booking booking = new Booking
            {
                BookingActivity = new List<BookingActivity>() { bookingActivity }
            };

            BookingCruisePassenger bookingCruisePassenger = new BookingCruisePassenger
            {
                Id = 0,
                IdPassenger = 0,
                IdCruise = 0,
                IdBooking = 0,
                Booking = booking,
                Cruise = new Cruise
                {
                    LovTypeCruise = LovCreate(2, name: lovName),
                    LovDestination = LovCreate(3, lovDestinationCode),
                    LovShip = LovCreate(4, lovShipCode),
                    SailingLengthDays = sailingLengthDays,
                    Code = cruiseCode
                }
            };

            return bookingCruisePassenger;
        }
        #endregion

        #region BookingCruisePassengerExceptionCreate
        public BookingCruisePassenger BookingCruisePassengerExceptionCreate(
          string lovName,
          string lovDestinationCode,
          string lovShipCode,
          int sailingLengthDays,
          string cruiseCode,
          string activityCode)
        {
            BookingCruisePassenger bookingCruisePassenger = new BookingCruisePassenger
            {
                Id = 0,
                IdPassenger = 0,
                IdCruise = 0,
                IdBooking = 0,
                Booking = new Booking
                {
                    BookingActivity = new List<BookingActivity>()
                },
                Cruise = new Cruise
                {
                    LovTypeCruise = LovCreate(2, name: lovName),
                    LovDestination = LovCreate(3, lovDestinationCode),
                    LovShip = LovCreate(4, lovShipCode),
                    SailingLengthDays = sailingLengthDays,
                    Code = cruiseCode
                }
            };

            return bookingCruisePassenger;
        }
        #endregion

        #region CreateBooking
        public Booking CreateBooking(
            int id,
            int number,
            string creator)                        
        {
            Booking booking = new Booking()
            {
                Id = id,
                Number = number,
                Creator = creator
            };
            return booking;
        }
        #endregion

        #region CompareBookingList        
        public List<Booking> CompareBookingList(List<Booking> bookingList, string propertyName)
        {
            PropertyComparer<Booking> customComparer = new PropertyComparer<Booking>(propertyName);
            List<Booking> equalBookings = new List<Booking>();

            foreach (Booking booking in bookingList.Distinct(customComparer))
            {
                equalBookings.Add(booking);
            }

            return equalBookings;
        }
        #endregion
    }
}
