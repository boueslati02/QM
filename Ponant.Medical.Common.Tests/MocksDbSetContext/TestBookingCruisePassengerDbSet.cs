namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    public class TestBookingCruisePassengerDbSet : TestDbSet<Data.Shore.BookingCruisePassenger>
    {
        public override Data.Shore.BookingCruisePassenger Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
