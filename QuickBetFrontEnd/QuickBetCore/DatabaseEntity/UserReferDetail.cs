using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class UserReferDetail
    {
        public int Id { get; set; }
        public int ReferPeriods { get; set; }
        public decimal ReferPercentage { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
