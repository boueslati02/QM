namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    public class TestvCruiseShoreDbSet : TestDbSet<Data.Shore.vCruiseShore>
    {
        public override Data.Shore.vCruiseShore Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
