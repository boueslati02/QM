namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    public class TestvPassengerBoardDbSet : TestDbSet<Data.Shore.vPassengerBoard>
    {
        public override Data.Shore.vPassengerBoard Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
