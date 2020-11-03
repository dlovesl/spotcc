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

        public int TotalStreams { get; set; }
        public int OrderPreStreams { get; set; }
        public int OrdeFreeStreams { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Artist> Artists { get; set; }
    }
}
