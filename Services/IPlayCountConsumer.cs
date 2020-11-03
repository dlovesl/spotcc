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
    public interface IPlayCountConsumer
    {
        Task ListenAsync(CancellationToken cancellationToken);
    }

    public class PlayCountConsumer : IPlayCountConsumer
    {
        private readonly IPlayCountStream _playCountStream;
        private readonly ChannelReader<PlayCount> _reader;
        private readonly ILogger<PlayCountConsumer> _logger;

        public PlayCountConsumer(ILogger<PlayCountConsumer> logger, IPlayCountStream playCountStream, ChannelReader<PlayCount> reader)
        {
            _logger = logger;
            _playCountStream = playCountStream;
            _reader = reader;
        }

        public async Task ListenAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            while (true)
            {
                var playCount = await _reader.ReadAsync(cancellationToken);
                if (playCount != null)
                {
                    _playCountStream.Publish(playCount);
                }
            }
        }
    }
}
