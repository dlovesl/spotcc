using Newtonsoft.Json;
using RestEase;
using Spotcc.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotcc.Services
{
    public interface ILibrespotApi
    {
        [Get("artistInfo")]
        Task<GetArtistInfoResult> GetArtistInfoAsync(string artistid);

        [Get("artistInsights")]
        Task<GetArtistInsightsResult> GetArtistInsightsAsync(string artistid);
    }

    public class Track
    {
        public int Playcount { get; set; }
        public string Name { get; set; }
    }

    public class TopTracks
    {
        public List<Track> Tracks { get; set; }
    }

    [JsonConverter(typeof(JsonPathConverter))]
    public class ArtistInfo
    {
        [JsonProperty("info.name")]
        public string Name { get; set; }

        [JsonProperty("creator_about.monthlyListeners")]
        public int MonthlyListeners { get; set; }

        public int TotalPlays => TopTracks?.Tracks?.Sum(t => t.Playcount)?? 0;

        [JsonProperty("top_tracks")] 
        public TopTracks TopTracks { get; set; }
    }

    public class ArtistInsights
    {
        public int MonthlyListeners { get; set; }
        public int FollowerCount { get; set; }
    }

    public class GetArtistInfoResult
    {
        public bool Success { get; set; }

        [JsonProperty("data")]
        public ArtistInfo ArtistInfo { get; set; }
    }

    public class GetArtistInsightsResult
    {
        public bool Success { get; set; }

        [JsonProperty("data")]
        public ArtistInsights ArtistInsights { get; set; }
    }
}
