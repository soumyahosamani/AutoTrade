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
    public class CsvFeedProvider
    {
        private string filePath;
        private string symbol; 
        
        public event NewTickEventHandler NewTickEvent;

        public CsvFeedProvider(string symbol)
        {
            this.symbol = symbol;
            LoadFilePath();
        }
        public void ProcessCsvFile()
        {
            var randomNumber = Randomizer.GetRandomNumber(1, 5);
            // handle concern of file being held open for such a long time?? or keeping entire csv data in memory which  is better?
            var quotes = File.ReadAllLines(filePath);
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

        private void LoadFilePath()
        {
            var baseFolder = ConfigurationManager.AppSettings["CsvProviderFolder"];
            symbol = ConfigurationManager.AppSettings["Symbol"];
            filePath = (Path.Combine(baseFolder, symbol + ".csv"));
        }

        private void RaiseNewTickEvent(Tick tick)
        {
            NewTickEvent?.Invoke(this, new TickEventArgs(tick));
        }
    }

    public class AbstractFeedProvider : IFeedProvider
    {
        // list of subscribes for which i have start the feed 
        public event NewTickEventHandler NewTickEvent;

        

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Subscribe(string Symbol)
        {
            throw new NotImplementedException();
        }
    }
}
