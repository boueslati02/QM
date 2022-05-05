namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using Ponant.Medical.Data.Auth;
    using System.Linq;

    class TestAspNetUserClaimsDbSet : TestDbSet<AspNetUserClaims>
    {
        public override AspNetUserClaims Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (int)keyValues.Single());
        }
    }
}
