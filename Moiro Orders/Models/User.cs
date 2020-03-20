using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiro_Orders.Models
{
    class User
    {
        public int Id { get; set; }

        public string Login { get; set; }
    
        public string FullName { get; set; }
        public string OrganizationalUnit { get; set; }
        public int Room { get; set; }
        public DateTime LastLogin { get; set; } = DateTime.Now;
        public bool Admin { get; set; }

        

    }
}
