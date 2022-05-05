namespace Ponant.Medical.Data.Auth
{
    using Ponant.Medical.Data.Auth.Interfaces;
    using System.Data.Entity;

    public partial class AuthContext : DbContext, IAuthContext
    {
        public AuthContext()
            : base("name=AuthConnectionString")
        {
        }

        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }

        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }

        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }

        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }

        public virtual DbSet<AspNetUserShips> AspNetUserShips { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoles>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserShips)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.IdUser)
                .WillCascadeOnDelete(false);
        }
    }
}
