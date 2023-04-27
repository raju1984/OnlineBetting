using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class RandomBonu
    {
        public int Id { get; set; }
        public int NoOfTicket { get; set; }
        public decimal WinAmount { get; set; }
        public int SoldTicket { get; set; }
    }
}
