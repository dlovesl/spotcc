
using SpotCC.Enum;
using SQLite;
using System;

namespace Spotcc.Services.Models
{
    public class PlayCount
    {
        public PlayCount()
        {
            Date = DateTime.Now.Date;
            Delta = 0;
        }

        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }

        [Indexed]
        public string Account { get; set; }
        public string ArtistName { get; set; }
        public int Playcount { get; set; }
        public int MonthlyListeners { get; set; }
        public int FollowerCount { get; set; }

        [Indexed]
        public DateTime Date { get; set; }

        public int Delta { get; set; }
        public StreamType StreamType { get; set; }
    }
}
