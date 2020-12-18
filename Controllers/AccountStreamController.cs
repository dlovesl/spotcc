using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spotcc.Services;
using Spotcc.Services.Models;
using SpotCC.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SpotCC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountStreamController : ControllerBase
    {
        private readonly IRepository<PlayCount> _repository;
        protected readonly ILogger<AccountStreamController> _logger;

        public AccountStreamController(IRepository<PlayCount> repository, ILogger<AccountStreamController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("[action]")]
        public IEnumerable<AccountStream> LastWeek()
        {
            var lastWeek = DateTime.Now.AddDays(-7);
            return GetPlayCountByDateRange(lastWeek);
        }

        [HttpGet("[action]")]
        public IEnumerable<AccountStream> Weekly()
        {
            var lastMonth = DateTime.Now.AddMonths(-1);
            return GetPlayCountByDateRange(lastMonth);
        }

        [HttpGet("[action]")]
        public IEnumerable<AccountStream> Daily()
        {
            var lastMonth = DateTime.Now.AddMonths(-1);
            return GetPlayCountByDateRange(lastMonth);
        }

        [HttpGet("[action]")]
        public IEnumerable<AccountStream> MonthAndYear(string name, int month, int year)
        {
            DateTime firstDay;
            DateTime? lastDay;

            if (month == 0 && year == 0)
            {
                firstDay = new DateTime(2020, 1, 1);
                lastDay = null;
            }
            else
            {
                month = month == 0 ? DateTime.Now.Month : month;
                year = year == 0 ? DateTime.Now.Year : year;
                firstDay = new DateTime(year, month, 1);
                lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            }

            return GetPlayCountByDateRangeAndArtist(name, firstDay, lastDay);
        }

        [HttpPost("[action]")]
        public IEnumerable<ChartDownload> DateRange(ChartDownloadModel model)
        {
            if (model == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                return GetPlayCountByCustomDateRange(model.From, model.To);
            }
            return GetPlayCountByCustomDateRangeAndArtist(model.Name, model.From, model.To);
        }

        [HttpGet("[action]")]
        public IEnumerable<AccountStream> Monthly()
        {
            var lastYear = DateTime.Now.AddYears(-1);
            var data = GetPlayCountByDateRange(lastYear);

            return data.Select(d => new AccountStream
            {
                Account = d.Account,
                StreamInfos = d.StreamInfos
                                    .Select(s => new 
                                    {
                                        Day = DateTime.Parse(s.Day).ToString("MMM"),
                                        s.Streams
                                    })
                                    .GroupBy(s => s.Day)
                                    .Select(g => new StreamInfo
                                    {
                                        Day = g.Key,
                                        Streams = g.Sum(i => i.Streams)
                                    })
            });
        }

        private IEnumerable<AccountStream> GetPlayCountByDateRange(DateTime from, DateTime? to = null)
        {
            Expression<Func<PlayCount, bool>> predicate = p => p.Date >= from;
            if (to.HasValue)
            {
                predicate = p => p.Date >= from && p.Date <= to;
            }

            var playCounts = _repository.Get(predicate);
            var accountStreams = playCounts.GroupBy(p => new { p.Account, p.StreamType })
                            .Select(g => new AccountStream()
                            {
                                Account = g.Key.Account + "-" + g.Key.StreamType.ToString(),
                                StreamInfos = g.GroupBy(i => i.Date)
                                                .Select(i => new StreamInfo
                                                {
                                                    Day = i.Key.ToString("d"),
                                                    Streams = i.Sum(p => p.Delta)
                                                })
                            }).Where(x => x.StreamInfos.Count() > 1);

            return accountStreams;
        }

        private IEnumerable<AccountStream> GetPlayCountByDateRangeAndArtist(string artistName, DateTime from, DateTime? to = null)
        {
            Expression<Func<PlayCount, bool>> predicate = p => p.ArtistName == artistName && p.Date >= from;
            if (to.HasValue)
            {
                predicate = p => p.ArtistName == artistName && p.Date >= from && p.Date <= to;
            }

            var playCounts = _repository.Get(predicate);
            var accountStreams = playCounts.GroupBy(p => new { p.Account, p.StreamType })
                            .Select(g => new AccountStream()
                            {
                                Account = g.Key.Account + "-" + g.Key.StreamType.ToString(),
                                TotalStreams = g.Max(s => s.Playcount),
                                StreamInfos = g.GroupBy(i => i.Date)
                                                .Select(i => new StreamInfo
                                                {
                                                    Day = i.Key.ToString("d"),
                                                    Streams = i.Sum(p => p.Delta)
                                                })
                            }).Where(x => x.StreamInfos.Count() > 1);

            return accountStreams;
        }

        private IEnumerable<ChartDownload> GetPlayCountByCustomDateRangeAndArtist(string artistName, DateTime from, DateTime? to = null)
        {
            Expression<Func<PlayCount, bool>> predicate = p => p.ArtistName == artistName && p.Date >= from;
            if (to.HasValue)
            {
                predicate = p => p.ArtistName == artistName && p.Date >= from && p.Date <= to;
            }

            var playCounts = _repository.Get(predicate);
            var accountStreams = playCounts.OrderBy(i => i.Date).Select(g => new ChartDownload()
                            {
                                Day = g.Date.ToString(),
                                Downloads = g.Delta,
                            });

            return accountStreams;
        }

        private IEnumerable<ChartDownload> GetPlayCountByCustomDateRange(DateTime from, DateTime? to = null)
        {
            Expression<Func<PlayCount, bool>> predicate = p => p.Date >= from;
            if (to.HasValue)
            {
                predicate = p => p.Date >= from && p.Date <= to;
            }

            var playCounts = _repository.Get(predicate);
            var accountStreams = playCounts.GroupBy(s => s.Date).OrderBy(x => x.Key).Select(g => new ChartDownload()
            {
                Day = g.Key.Date.ToString(),
                Downloads = g.Sum(i => i.Delta),
            });

            return accountStreams;
        }

        public class AccountStream
        {
            public string Account { get; set; }
            public float TotalStreams { get; set; }
            public IEnumerable<StreamInfo> StreamInfos { get; set; }
        }

        public class StreamInfo
        {
            public int Streams { get; set; }
            public string Day { get; set; }
        }

        public class ChartDownload
        {
            public string Day { get; set; }
            public int Downloads { get; set; }
        }

        public class ChartDownloadModel
        {
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public string Name { get; set; }
        }
    }
}
