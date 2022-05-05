namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    class TestBookingDbSet : TestDbSet<Data.Shore.Booking>
    {
        public override Data.Shore.Booking Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
