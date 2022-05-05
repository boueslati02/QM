namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    class TestPassengerDbSet : TestDbSet<Data.Shore.Passenger>
    {
        public override Data.Shore.Passenger Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
