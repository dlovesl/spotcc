using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotcc.Services.Models
{
    public class Account
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }

        [Indexed]
        public string Name { get; set; }

        public string Token { get; set; }
        public DateTime ExpiredDate { get; set; }
        public bool Enable { get; set; }

        public long TotalStreams { get; set; }
        public long OrderPreStreams { get; set; }
        public long OrdeFreeStreams { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Artist> Artists { get; set; }
    }
}
