using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class DisputeChatHistory
    {
        public int Id { get; set; }
        public int GenerateByUserId { get; set; }
        public string Meesage { get; set; }
        public DateTime CreatedDate { get; set; }
        public int DisputeTicketId { get; set; }
        public int Status { get; set; }

        public virtual DisputeTicked DisputeTicket { get; set; }
        public virtual User GenerateByUser { get; set; }
    }
}
