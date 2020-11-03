﻿using Microsoft.AspNetCore.Mvc;
using Spotcc.Services;
using Spotcc.Services.Models;
using SpotCC.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotCC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountStreamController : ControllerBase
    {
        private readonly IRepository<PlayCount> _repository;

        public AccountStreamController(IRepository<PlayCount> repository)
        {
            _repository = repository;
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
            if (!to.HasValue)
            {
                to = DateTime.Now;
            }

            var playCounts = _repository.Get(p => p.Date >= from && p.Date <= to);

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

        public class AccountStream
        {
            public string Account { get; set; }
            public IEnumerable<StreamInfo> StreamInfos { get; set; }
        }

        public class StreamInfo
        {
            public int Streams { get; set; }
            public string Day { get; set; }
        }
    }
}