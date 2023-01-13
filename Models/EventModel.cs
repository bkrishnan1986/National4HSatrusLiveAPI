using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace National4HSatrusLive.Models
{
    public class EventModel
    {
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}