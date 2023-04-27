using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class SupportTicked
    {
        public SupportTicked()
        {
            SupportChatHistories = new HashSet<SupportChatHistory>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public string SupportTicketId { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<SupportChatHistory> SupportChatHistories { get; set; }
    }
}
