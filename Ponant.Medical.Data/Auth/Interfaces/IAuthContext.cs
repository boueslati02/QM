namespace Ponant.Medical.Data.Auth.Interfaces
{
    using System;
    using System.Data.Entity;

    public interface IAuthContext : IDisposable
    {
        DbSet<AspNetRoles> AspNetRoles { get; set; }

        DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }

        DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }

        DbSet<AspNetUsers> AspNetUsers { get; set; }

        DbSet<AspNetUserShips> AspNetUserShips { get; set; }

        int SaveChanges();
    }
}
