namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    class TestvSurveyDbSet : TestDbSet<Data.Shore.vSurvey>
    {
        public override Data.Shore.vSurvey Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
