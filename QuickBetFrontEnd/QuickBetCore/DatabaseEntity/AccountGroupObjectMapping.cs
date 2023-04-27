using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class AccountGroupObjectMapping
    {
        public int Id { get; set; }
        public int ObjectType { get; set; }
        public int? AccountGroupId { get; set; }

        public virtual AccountGroup AccountGroup { get; set; }
    }
}
