namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using Ponant.Medical.Data.Shore;
    using System.Linq;

    public class TestDocumentDbSet : TestDbSet<Document>
    {
        public override Document Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
