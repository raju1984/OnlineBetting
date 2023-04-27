using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class DisputeTicked
    {
        public DisputeTicked()
        {
            DisputeChatHistories = new HashSet<DisputeChatHistory>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public int DisputeType { get; set; }
        public int DisputeReferenceId { get; set; }
        public string DisputeTicketId { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<DisputeChatHistory> DisputeChatHistories { get; set; }
    }
}
