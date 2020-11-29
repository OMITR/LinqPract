using System;
using System.Collections.Generic;

namespace dbPract.Data.Models
{
    public partial class Towns
    {
        public int TownId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Addresses> Addresses { get; set; }
    }
}
