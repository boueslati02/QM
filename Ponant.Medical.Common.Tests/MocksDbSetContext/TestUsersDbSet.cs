namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using Ponant.Medical.Data.Auth;
    using System.Linq;

    public class TestUsersDbSet : TestDbSet<AspNetUsers>
    {
        public override AspNetUsers Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (string)keyValues.Single());
        }
    }
}
