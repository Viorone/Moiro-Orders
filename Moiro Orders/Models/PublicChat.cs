using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiro_Orders.Models
{
    public class PublicChat
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
