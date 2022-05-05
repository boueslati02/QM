namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    public class TestCruiseCriterionShipDbSet : TestDbSet<Data.Shore.CruiseCriterionShip>
    {
        public override Data.Shore.CruiseCriterionShip Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.IdShip == (int)keyValues.Single());
        }
    }
}
