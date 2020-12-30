using SpotCC.Enum;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotcc.Services.Models
{
    public class Artist
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }

        public string SpotifyId { get; set; }

        [ForeignKey(typeof(Account))]
        public int AccountId { get; set; }
        //[ManyToOne]
        //public Account Account { get; set; }

        public long TotalStreams { get; set; }
        public int MonthlyListeners { get; set; }
        public int FollowerCount { get; set; }
        public StreamType StreamType { get; set; }
    }
}
