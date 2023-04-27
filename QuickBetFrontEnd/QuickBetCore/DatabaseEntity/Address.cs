using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class Address
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Emailaddress { get; set; }
        public int? AddressType { get; set; }
        public string Street { get; set; }
        public string StreetNo { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string CountryCode { get; set; }
        public string Phone { get; set; }
        public int UserId { get; set; }
        public bool? IsDefault { get; set; }
        public string Landmark { get; set; }

        public virtual User User { get; set; }
    }
}
