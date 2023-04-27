using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class BankDetail
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public int? UserId { get; set; }
        public bool IsDefault { get; set; }
        public bool Isdeleted { get; set; }

        public virtual User User { get; set; }
    }
}
