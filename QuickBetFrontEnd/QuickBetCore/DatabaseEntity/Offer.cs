using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class Offer
    {
        public int Id { get; set; }
        public decimal CashBackPercent { get; set; }
        public int CashBackType { get; set; }
        public bool IsActive { get; set; }
    }
}
