using Microsoft.Extensions.Logging;
using Spotcc.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotcc.Services
{
    public class PersistPlayCountSubcriber
    {
        private readonly IRepository<Account> _accountRepo;
        private readonly IRepository<Artist> _artistRepo;
        private readonly IRepository<PlayCount> _playCountRepo;
        private readonly ILogger<PersistPlayCountSubcriber> _logger;

        public PersistPlayCountSubcriber(ILogger<PersistPlayCountSubcriber> logger, IRepository<Account> accountRepo, IRepository<Artist> artistRepo, IRepository<PlayCount> playCountRepo)
        {
            _logger = logger;
            _accountRepo = accountRepo;
            _artistRepo = artistRepo;
            _playCountRepo = playCountRepo;
        }

        public void Persist(PlayCount playCountModel)
        {
            if (playCountModel != null)
            {
                _logger.LogDebug($"get playcount artist: {playCountModel.ArtistName} - date {playCountModel.Date}");
                var playCount = _playCountRepo.FindFirst(p => p.ArtistName == playCountModel.ArtistName && p.Date == playCountModel.Date);
                if (playCount == null)
                {
                    var latestPlayCount = _playCountRepo.Get(p => p.ArtistName == playCountModel.ArtistName && p.Date < playCountModel.Date)
                                                        .OrderByDescending(p => p.Date).FirstOrDefault();
                    if (latestPlayCount != null && playCountModel.Playcount > 0)
                    {
                        playCountModel.Delta = playCountModel.Playcount - latestPlayCount.Playcount;
                    }

                    _playCountRepo.Insert(playCountModel);

                    var account = _accountRepo.FindFirst(a => a.Name == playCountModel.Account);
                    var artist = account.Artists.First(a => a.Name == playCountModel.ArtistName);

                    artist.TotalStreams = playCountModel.Playcount;
                    artist.MonthlyListeners = playCountModel.MonthlyListeners;
                    artist.FollowerCount = playCountModel.FollowerCount;
                    _artistRepo.Update(artist);

                    account.TotalStreams = account.Artists.Sum(a => a.TotalStreams);
                    _accountRepo.Update(account);
                }
                else
                {
                    _logger.LogDebug($"Playcount artist: {playCountModel.ArtistName} - date {playCountModel.Date} is NULL");
                }
            }
        }
    }
}
