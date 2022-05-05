namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using Ponant.Medical.Data.Auth;
    using Ponant.Medical.Data.Auth.Interfaces;
    using System.Data.Entity;

    public class TestAuthContext : IAuthContext
    {
        public DbSet<AspNetRoles> AspNetRoles { get; set; }

        public DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }

        public DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }

        public DbSet<AspNetUsers> AspNetUsers { get; set; }

        public virtual DbSet<AspNetUserShips> AspNetUserShips { get; set; }

        public TestAuthContext()
        {
            this.AspNetRoles = new TestAspNetRolesDbSet();
            this.AspNetUserClaims = new TestAspNetUserClaimsDbSet();
            this.AspNetUserLogins = new TestAspNetUserLoginsDbSet();
            this.AspNetUsers = new TestUsersDbSet();
        }

        public void Dispose() { }

        public int SaveChanges()
        {
            return 0;
        }
    }
}
