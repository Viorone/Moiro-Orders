using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiro_Orders.Models
{
    class Webinar
    {
        public int Id { get; set; }
        public string NameWebinar { get; set; }
        public string Description { get; set; }
        public string Place { get; set; }
        public DateTime DateStart { get; set; } //вермя начала 
        public DateTime DateEnd { get; set; } //время конца
        public DateTime Date { get; set; } //время подачи заявки на вебинар
        public bool IsCanceled { get; set; }
        public int UserId { get; set; } // пользователь который создал заявку на вебинар
        public int PlatformId { get; set; } // платформа на которой будет проводиться вебинар
        public string PlatformName { get; set; }
    }
}
