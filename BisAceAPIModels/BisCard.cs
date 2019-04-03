using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BisAceAPIModels.Models
{
    public class BisCard
    {
        public string CardNumber { get; set; }
        public string CardName { get; set; }
        public string CardStartValidDate { get; set; }
        public string CardExpiryDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public List<int> DoorAccessGroupIds { get; set; }
        public List<int> LiftAccessGroupIds { get; set; }
        public int DefaultFloorId { get; set; }
        public bool IsVisitor { get; set; }
    }
}