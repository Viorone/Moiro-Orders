using System;


namespace Moiro_Orders.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string NameEvent { get; set; } 
        public string Description { get; set; } 
        public string Place { get; set; } 
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int StatusId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
