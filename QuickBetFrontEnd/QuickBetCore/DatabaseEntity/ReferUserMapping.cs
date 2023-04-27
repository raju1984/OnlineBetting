using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class ReferUserMapping
    {
        public int Id { get; set; }
        public int ReferedFromId { get; set; }
        public DateTime ReferedDate { get; set; }
        public int ReferedUserId { get; set; }
        public string TimePeriod { get; set; }
        public decimal Percentage { get; set; }
        public string ReferCode { get; set; }
    }
}
