using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Moiro_Orders.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Problem { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int StatusId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Room { get; set; }
        public string StatusName { get; set; }
        public string UserLogin { get; set; }
        public string AdminComment { get; set; }
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public DateTime CompletionDate { get; set; }
    }
}
