namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    public class TestvCriteriaDbSet : TestDbSet<Data.Shore.vCriteria>
    {
        public override Data.Shore.vCriteria Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
