namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using Ponant.Medical.Data.Auth;
    using System.Linq;

    class TestAspNetUserLoginsDbSet : TestDbSet<AspNetUserLogins>
    {
        public override AspNetUserLogins Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.UserId == (string)keyValues.Single());
        }
    }
}
