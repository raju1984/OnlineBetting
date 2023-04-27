using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class AgentPromotionEntry
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int ApprovedAdminId { get; set; }
        public int AgentType { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsApproved { get; set; }
    }
}
