namespace Ponant.Medical.Data.Auth
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AspNetUserShips
    {
        [Key]
        [Column(Order = 0)]
        public string IdUser { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdShip { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }
    }
 }