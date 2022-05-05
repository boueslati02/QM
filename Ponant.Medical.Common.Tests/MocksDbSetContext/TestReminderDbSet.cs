namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Linq;

    public class TestReminderDbSet : TestDbSet<Data.Shore.Reminder>
    {
        public override Data.Shore.Reminder Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
