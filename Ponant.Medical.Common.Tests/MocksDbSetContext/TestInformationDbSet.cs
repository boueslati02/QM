namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    public class TestInformationDbSet : TestDbSet<Data.Shore.Information>
    {
        public override Data.Shore.Information Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.IdInformation == (int)keyValues.Single());
        }
    }
}
