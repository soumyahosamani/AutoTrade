using AutoTrade.Core;
using AutoTrade.Core.FeedProvider;
using AutoTrade.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoTrade.FeedProvider.CSV
{
    public class CsvFeedProvider : IFeedProvider
    {
        private IDictionary<string, string> filePaths = new Dictionary<string, string>();
        private ConcurrentBag<string> subscribedSymbols = new ConcurrentBag<string>();
        private ConcurrentBag<Thread> processThreads = new ConcurrentBag<Thread>();
        private bool isStarted = false;
        private object lockObject;

        public event NewTickEventHandler NewTickEvent;

        public CsvFeedProvider()
        {
            lockObject = new object();
            LoadFilePaths();
        }

        public void Subscribe(string symbol)
        {
            if (subscribedSymbols.Contains(symbol) == false)
            {
                subscribedSymbols.Add(symbol);
                StartFeed(symbol);
            }
        }

        public void Start()
        {
            lock (lockObject)
            {
                isStarted = true;
            }

            foreach (var symbol in subscribedSymbols)
            {
                StartFeed(symbol);
            }

        }

        public void Stop()
        {
            lock (lockObject)
            {
                if (isStarted)
                {
                    foreach (var thread in processThreads)
                    {
                        thread.Abort();
                    }
                    isStarted = false;
                }
            }

        }

        private void StartFeed(string symbol)
        {
            lock (lockObject)
            {
                if (isStarted)
                {
                    var file = filePaths.ContainsKey(symbol) ? filePaths[symbol] : null;
                    if (file != null)
                    {
                        var processThread = new Thread(() => { ProcessCsvFile(file); });
                        processThread.Name = symbol;
                        processThread.IsBackground = false;
                        processThreads.Add(processThread);
                        processThread.Start();
                    }
                }
            }
        }

        private void ProcessCsvFile(string file)
        {
            var randomNumber = Randomizer.GetRandomNumber(1, 5);
            // handle concern of file being held open for such a long time?? or keeping entire csv data in memory which  is better?
            var quotes = File.ReadAllLines(file);
            int tickId = 0;

            foreach (var quote in quotes)
            {
                // to skip header of csv file
                if (tickId == 0)
                {
                    tickId++;
                    continue;
                }

                Console.WriteLine("Thread {0} {1} Sleeping for {2} s", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name, randomNumber / 1000);
                //Thread.Sleep(randomNumber);
                var temp = quote.Split(',');
                var tick = new Tick(tickId, temp[0].Trim('"'), double.Parse(temp[8].Trim('"')), DateTime.Parse(temp[2].Trim('\"')));
                RaiseNewTickEvent(tick);
                tickId++;
            }
        }

        private void LoadFilePaths()
        {
            var baseFolder = ConfigurationManager.AppSettings["CsvProviderFolder"];
            var symbols = ConfigurationManager.AppSettings["Symbols"].Split(',');
            foreach (var symbol in symbols)
            {
                filePaths[symbol] = (Path.Combine(baseFolder, symbol + ".csv"));
            }
        }

        private void RaiseNewTickEvent(Tick tick)
        {
            NewTickEvent?.Invoke(this, new TickEventArgs(tick));
        }
    }
}
