using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class Country
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string AccessCode { get; set; }
        public int Status { get; set; }
        public int Zone { get; set; }
    }
}
