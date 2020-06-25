using System;


namespace Moiro_Orders.Models
{
    public class Event
    {
        public int Id { get; set; } = 0;
        public string NameEvent { get; set; } = "message";
        public string Description { get; set; } 
        public string Place { get; set; } 
        public DateTime DateStart { get; set; } = DateTime.Now;
        public DateTime DateEnd { get; set; } = DateTime.Now;
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsCanceled { get; set; } = false;
        public int UserId { get; set; } = 0;
        public string UserName { get; set; }
        public int? AdminId { get; set; }
        public string AdminName { get; set; }
    }
}
