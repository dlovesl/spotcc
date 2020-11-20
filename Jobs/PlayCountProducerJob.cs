using Microsoft.Extensions.Logging;
using Spotcc.Services;
using Spotcc.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Spotcc.Jobs
{
    public class PlayCountProducerJob
    {
        private readonly IPlayCountProducer _playCountProducer;
        private readonly ILibrespotApi _librespotApi;
        private readonly IRepository<Account> _accountRepo;
        protected readonly ILogger<PlayCountProducerJob> _logger;

        public PlayCountProducerJob(IPlayCountProducer playCountProducer, ILibrespotApi librespotApi, IRepository<Account> accountRepo, ILogger<PlayCountProducerJob> logger)
        {
            _playCountProducer = playCountProducer;
            _librespotApi = librespotApi;
            _accountRepo = accountRepo;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var accounts = _accountRepo.Get()
                            .Where(a => a.Artists != null && a.Artists.Any())
                            .GroupBy(a => a.Name)
                                .Select(ac => new {
                                    Account = ac.Key,
                                    Artists = ac.SelectMany(a => a.Artists).Select(a => 
                                        new {
                                             a.SpotifyId,
                                             a.StreamType
                                        })
                                }).ToList();

            foreach (var account in accounts)
            {
                var playCounts = account.Artists.Select(artist =>
                {
                    if (artist.StreamType == SpotCC.Enum.StreamType.Removed)
                    {
                        return null;
                    }
                    var artistInfoTask = _librespotApi.GetArtistInfoAsync(artist.SpotifyId);
                    var artistInsightsTask = _librespotApi.GetArtistInsightsAsync(artist.SpotifyId);

                    Task.WaitAll(artistInfoTask, artistInsightsTask);

                    var artistInfoResult = artistInfoTask.Result;
                    var artistInsightResult = artistInsightsTask.Result;

                    if (artistInfoResult.Success && artistInsightResult.Success)
                    {
                        _logger.LogDebug($"add playcount for artist: {artistInfoResult.ArtistInfo.Name} - playcount: {artistInsightResult.ArtistInsights.FollowerCount}");
                        var playCount = new PlayCount
                        {
                            Account = account.Account,
                            ArtistName = artistInfoResult.ArtistInfo.Name,
                            StreamType = artist.StreamType,
                            Playcount = artistInfoResult.ArtistInfo.TotalPlays,
                            FollowerCount = artistInsightResult.ArtistInsights.FollowerCount,
                            MonthlyListeners = artistInsightResult.ArtistInsights.MonthlyListeners,
                        };

                        return playCount;
                    }

                    return null;
                });

                foreach (var playCount in playCounts.Where(p => p != null))
                {
                    await _playCountProducer.ProduceAsync(playCount, CancellationToken.None);
                }
            }
        }
    }
}
