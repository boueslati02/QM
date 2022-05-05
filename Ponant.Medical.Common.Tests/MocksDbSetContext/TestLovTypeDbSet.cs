namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    public class TestLovTypeDbSet : TestDbSet<Data.Shore.LovType>
    {
        public override Data.Shore.LovType Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
