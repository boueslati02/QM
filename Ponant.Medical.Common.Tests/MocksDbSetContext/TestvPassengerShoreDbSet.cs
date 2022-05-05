namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    public class TestvPassengerShoreDbSet : TestDbSet<Data.Shore.vPassengerShore>
    {
        public override Data.Shore.vPassengerShore Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
