using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class AccountGroupUserMapping
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AccountGroupId { get; set; }

        public virtual AccountGroup AccountGroup { get; set; }
        public virtual User User { get; set; }
    }
}
