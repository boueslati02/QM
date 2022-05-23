using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponant.Medical.Data.Shore.Models
{
    public class UserToken
    {
        public string BookingNumber { get; set; }
        public string Email { get; set; }
        public string PassengerNo { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string CruiseNumber { get; set; }
        public string Language { get; set; }
    }
}
