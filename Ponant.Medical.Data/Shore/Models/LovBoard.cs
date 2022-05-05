using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponant.Medical.Data.Shore.Models
{
    public class LovBoard
    {
        public int Id { get; set; }
        public int IdLovType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Creator { get; set; }
        public System.DateTime CreationDate { get; set; }
        public string Editor { get; set; }
        public System.DateTime ModificationDate { get; set; }
        public bool IsEnabled { get; set; }
    }
}
