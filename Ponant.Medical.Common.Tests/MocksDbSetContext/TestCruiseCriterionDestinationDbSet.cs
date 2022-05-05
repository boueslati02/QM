namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    public class TestCruiseCriterionDestinationDbSet : TestDbSet<Data.Shore.CruiseCriterionDestination>
    {
        public override Data.Shore.CruiseCriterionDestination Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.IdCruiseCriterion == (int)keyValues.Single());
        }
    }
}
