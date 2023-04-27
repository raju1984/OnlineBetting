using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class Gamelist
    {
        public int Id { get; set; }
        public string GameId { get; set; }
        public string GameName { get; set; }
        public string Caption { get; set; }
        public decimal JackpotAmount { get; set; }
        public decimal TicketCost { get; set; }
        public string Gameimg { get; set; }
        public int CateId { get; set; }
        public string CateName { get; set; }
        public bool IsEnable { get; set; }
        public bool DeletedFromGeneGame { get; set; }
    }
}
