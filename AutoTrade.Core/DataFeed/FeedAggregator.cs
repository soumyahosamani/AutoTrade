using AutoTrade.Core.FeedProvider;
using AutoTrade.Utils;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoTrade.Core
{
    public delegate void NewTickEventHandler(object sender, TickEventArgs e);
    public class FeedAggregator
    {
        private List<IFeedProvider> feedProviders ;

        private ConcurrentBag<string> subscribedSymbols = new ConcurrentBag<string>();
        private ConcurrentBag<Thread> processThreads = new ConcurrentBag<Thread>();        

        public event NewTickEventHandler NewTickEvent;

        public FeedAggregator(List<IFeedProvider> feedProviders)
        {
            this.feedProviders = feedProviders;
        }

        public void Subscribe(string symbol)
        {
            if (subscribedSymbols.Contains(symbol) == false)
            {
                subscribedSymbols.Add(symbol);
                StartFeed(symbol);
            }
        }

        private void StartFeed(string symbol)
        {
            var feedProvider = feedProviders.FirstOrDefault(f => f.Symbol == symbol);
            if (feedProvider == null)
                throw new  ArgumentException("Feedprovider not found for symbol " + symbol);
            var processThread = new Thread(() => { ProcessFeed(feedProvider); })
                                {
                                    Name = symbol,
                                    IsBackground = false
                                };
            processThreads.Add(processThread);
            processThread.Start();
        }

        private void ProcessFeed(IFeedProvider feedProvider)
        {
            while (feedProvider.HasNext)
            {
                var randomeMilliSeconds = Randomizer.GetRandomNumber(1, 1) * 1000;
                //Thread.Sleep(randomeMilliSeconds);

                // raise new tick event 
                NewTickEvent?.Invoke(this, new TickEventArgs(feedProvider.NextTick()));
            }

        }

        public void UnSubscribe(string symbol)
        {
            if (subscribedSymbols.Contains(symbol))
            {
                StopFeed(symbol);
            }
        }

        private void StopFeed(string symbol)
        {
            var thread = processThreads.FirstOrDefault(t => t.Name == symbol);

            if (thread.IsAlive)
            {
                thread.Abort();
            }
        }
    }

}
