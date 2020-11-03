using Microsoft.Extensions.Logging;
using Spotcc.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Spotcc.Services
{
    public interface IPlayCountProducer
    {
        Task ProduceAsync(PlayCount playCount, CancellationToken cancellationToken);
    }

    public class PlayCountProducer : IPlayCountProducer
    {
        private readonly ChannelWriter<PlayCount> _writer;
        private readonly ILogger<PlayCountProducer> _logger;

        public PlayCountProducer(ILogger<PlayCountProducer> logger, ChannelWriter<PlayCount> writer)
        {
            _logger = logger;
            _writer = writer;
        }

        public async Task ProduceAsync(PlayCount playCount, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _writer.WriteAsync(playCount, cancellationToken);
        }
    }
}
