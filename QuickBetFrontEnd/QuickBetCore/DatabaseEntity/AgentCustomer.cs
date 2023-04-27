using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class AgentCustomer
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int UserId { get; set; }
        public int CustomerRetentionPeriod { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User Agent { get; set; }
        public virtual User User { get; set; }
    }
}
