using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class HacksawgamingUserTokenMap
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public int UserId { get; set; }
        public int? AgentId { get; set; }
        public DateTime Createdtime { get; set; }

        public virtual User User { get; set; }
    }
}
