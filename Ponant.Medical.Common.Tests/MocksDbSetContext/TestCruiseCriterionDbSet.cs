namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    public class TestCruiseCriterionDbSet : TestDbSet<Data.Shore.CruiseCriterion>
    {
        public override Data.Shore.CruiseCriterion Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
