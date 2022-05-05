namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    public class TestSurveyDbSet : TestDbSet<Data.Shore.Survey>
    {
        public override Data.Shore.Survey Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
