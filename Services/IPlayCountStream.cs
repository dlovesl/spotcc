using Microsoft.Extensions.Logging;
using Spotcc.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Spotcc.Services
{
    public interface IPlayCountStream
    {
        void Publish(PlayCount playCountMessage);
        void Subscribe(string subscriberName, Action<PlayCount> action);
    }

    public class PlayCountStream : IPlayCountStream, IDisposable
    {
        private readonly IDictionary<string, IDisposable> _subscribers;
        private readonly Subject<PlayCount> _playCountMessageSubject;
        private readonly ILogger<PlayCountStream> _logger;

        public PlayCountStream(ILogger<PlayCountStream> logger)
        {
            _logger = logger;
            _playCountMessageSubject = new Subject<PlayCount>();
            _subscribers = new Dictionary<string, IDisposable>();
        }

        public void Dispose()
        {
            _playCountMessageSubject.Dispose();

            foreach (var subscriber in _subscribers)
            {
                subscriber.Value.Dispose();
            }
        }

        public void Publish(PlayCount playCountMessage)
        {
            _playCountMessageSubject.OnNext(playCountMessage);
        }

        public void Subscribe(string subscriberName, Action<PlayCount> action)
        {
            if (!_subscribers.ContainsKey(subscriberName))
            {
                _subscribers.Add(subscriberName, _playCountMessageSubject.Subscribe(action));
            }
        }
    }
}
