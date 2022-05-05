namespace Ponant.Medical.Common.Tests.MocksDbSetContext
{
    using System.Data.Entity;
    using Ponant.Medical.Data.Shore;

    public class TestShoreEntitiesContext : IShoreEntities
    {
        public DbSet<Agency> Agency { get; set; }
        public DbSet<CruiseCriterionDestination> CruiseCriterionDestination { get; set; }
        public DbSet<CruiseCriterionShip> CruiseCriterionShip { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<Information> Information { get; set; }
        public DbSet<Language> Language { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<Data.Shore.Lov> Lov { get; set; }
        public DbSet<LovType> LovType { get; set; }
        public DbSet<Reminder> Reminder { get; set; }
        public DbSet<Survey> Survey { get; set; }
        public DbSet<CruiseCriterion> CruiseCriterion { get; set; }
        public DbSet<vCriteria> vCriteria { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<BookingActivity> BookingActivity { get; set; }
        public DbSet<Passenger> Passenger { get; set; }
        public DbSet<vSurvey> vSurvey { get; set; }
        public DbSet<vCruiseShore> vCruiseShore { get; set; }
        public DbSet<vCruiseBoard> vCruiseBoard { get; set; }
        public DbSet<vPassengerBoard> vPassengerBoard { get; set; }
        public DbSet<BookingCruisePassenger> BookingCruisePassenger { get; set; }
        public DbSet<Cruise> Cruise { get; set; }
        public DbSet<vPassengerShore> vPassengerShore { get; set; }
        public Database Database { get; }
        public DbSet<AgencyAccessRight> AgencyAccessRight { get; set; }
        public DbSet<vAgencyAccessRight> vAgencyAccessRight { get; set; }
        public DbSet<Assignment> Assignment { get; set; }
        public DbSet<vAssignment> vAssignment { get; set; }
        public TestShoreEntitiesContext()
        {
            this.Agency = new TestAgencyDbSet();
            this.CruiseCriterionDestination = new TestCruiseCriterionDestinationDbSet();
            this.CruiseCriterionShip = new TestCruiseCriterionShipDbSet();
            this.Document = new TestDocumentDbSet();
            this.Information = new TestInformationDbSet();
            this.Language = new TestLanguageDbSet();
            this.Log = new TestLogDbSet();
            this.Lov = new TestLovDbSet();
            this.LovType = new TestLovTypeDbSet();
            this.Reminder = new TestReminderDbSet();
            this.Survey = new TestSurveyDbSet();
            this.CruiseCriterion = new TestCruiseCriterionDbSet();
            this.vCriteria = new TestvCriteriaDbSet();
            this.Booking = new TestBookingDbSet();
            this.BookingActivity = new TestBookingActivityDbSet();
            this.Passenger = new TestPassengerDbSet();
            this.vSurvey = new TestvSurveyDbSet();
            this.vCruiseShore = new TestvCruiseShoreDbSet();
            this.vCruiseBoard = new TestvCruiseBoardDbSet();
            this.vPassengerBoard = new TestvPassengerBoardDbSet();
            this.BookingCruisePassenger = new TestBookingCruisePassengerDbSet();
            this.Cruise = new TestCruiseDbSet();
            this.vPassengerShore = new TestvPassengerShoreDbSet();
            this.Database = new TestDbContext().Database;
        }

        public void Dispose() { }

        public int SaveChanges()
        {
            return 0;
        }
    }
}
