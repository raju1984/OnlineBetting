using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class Bannermanage
    {
        public int Id { get; set; }
        public string GoogleplayLink { get; set; }
        public string PlaystoreLink { get; set; }
    }
}
