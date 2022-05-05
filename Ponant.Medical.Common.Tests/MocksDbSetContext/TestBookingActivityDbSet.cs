namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    class TestBookingActivityDbSet : TestDbSet<Data.Shore.BookingActivity>
    {
        public override Data.Shore.BookingActivity Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.IdActivity == (int)keyValues.Single());
        }
    }
}
