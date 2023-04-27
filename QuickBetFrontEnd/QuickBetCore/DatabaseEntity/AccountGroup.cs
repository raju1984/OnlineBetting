using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class AccountGroup
    {
        public AccountGroup()
        {
            AccountGroupObjectMappings = new HashSet<AccountGroupObjectMapping>();
            AccountGroupUserMappings = new HashSet<AccountGroupUserMapping>();
        }

        public int Id { get; set; }
        public string AccountName { get; set; }
        public bool Isdeleted { get; set; }

        public virtual ICollection<AccountGroupObjectMapping> AccountGroupObjectMappings { get; set; }
        public virtual ICollection<AccountGroupUserMapping> AccountGroupUserMappings { get; set; }
    }
}
