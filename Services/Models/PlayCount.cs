
using SpotCC.Enum;
using SQLite;
using System;

namespace Spotcc.Services.Models
{
    public class PlayCount
    {
        public PlayCount()
        {
            //bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
            //var asiaTimeZone = isWindows ? TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time") : TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");

            //Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, asiaTimeZone).Date;
            Date = DateTime.UtcNow.Date;
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
