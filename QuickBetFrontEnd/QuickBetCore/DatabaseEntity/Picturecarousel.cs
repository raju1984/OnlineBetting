using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class Picturecarousel
    {
        public int Id { get; set; }
        public string PictureUrl { get; set; }
        public int Status { get; set; }
        public int IsDefault { get; set; }
        public string Titile { get; set; }
        public string Description { get; set; }
        public string ButtonLink { get; set; }
        public string ButtonName { get; set; }
    }
}
