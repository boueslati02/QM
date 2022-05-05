namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    public class TestAgencyDbSet : TestDbSet<Data.Shore.Agency>
    {
        public override Data.Shore.Agency Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
