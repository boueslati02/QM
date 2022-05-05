namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using Ponant.Medical.Data.Auth;
    using System.Linq;

    class TestAspNetRolesDbSet : TestDbSet<AspNetRoles>
    {
        public override AspNetRoles Find(params object[] keyValues)
        {
            return this.SingleOrDefault(c => c.Id == (string)keyValues.Single());
        }
    }
}
