namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Data.Entity;

    public class TestDbContext : DbContext
    {
        public new Database Database { get; }
    }
}
