//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ponant.Medical.Board.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BoardEntities : DbContext
    {
        public BoardEntities()
            : base("name=BoardEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Cruise> Cruise { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<Information> Information { get; set; }
        public virtual DbSet<Lov> Lov { get; set; }
        public virtual DbSet<LovType> LovType { get; set; }
        public virtual DbSet<Passenger> Passenger { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Log> Log { get; set; }
    }
}
