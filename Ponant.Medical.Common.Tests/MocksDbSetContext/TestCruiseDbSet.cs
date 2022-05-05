namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    public class TestCruiseDbSet : TestDbSet<Data.Shore.Cruise>
    {
        public override Data.Shore.Cruise Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
