using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class UserOffer
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public int OfferType { get; set; }
        public int UserId { get; set; }
        public bool IsRedeem { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual User User { get; set; }
    }
}
