namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    class TestvCruiseBoardDbSet : TestDbSet<Data.Shore.vCruiseBoard>
    {
        public override Data.Shore.vCruiseBoard Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
