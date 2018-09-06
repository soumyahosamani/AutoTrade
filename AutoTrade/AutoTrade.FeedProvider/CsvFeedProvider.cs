using AutoTrade.DTO;
using AutoTrade.Lib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoTrade.FeedProvider
{
    public class CsvFeedProvider : IFeedProvider
    {
        private IDictionary<string, string> filePaths = new Dictionary<string, string>();
        private IList<string> subscribedSymbols = new List<string>();
        protected IList<Thread> processThreads = new List<Thread>();

        public event NewTickEventHandler NewTickEvent;

        public CsvFeedProvider()
        {
            LoadFilePaths();
        }        

        public void Subscribe(string symbol)
        {
            if (subscribedSymbols.Contains(symbol) == false)
                subscribedSymbols.Add(symbol);
        }

        public void Start()
        {
            CreateProcesThreads();
            foreach (var thread in processThreads)
            {
                thread.Start();
            }   
        }

        public void Stop()
        {
            foreach (var thread in processThreads)
            {
                thread.Abort();
            }
        }

        private void CreateProcesThreads()
        {
            foreach (var symbol in subscribedSymbols)
            {
                var file = filePaths.ContainsKey(symbol) ? filePaths[symbol] : null;
                if (file != null)
                {
                    var processThread = new Thread(() => { ProcessCsv(file); });
                    processThread.Name = "csv_" + symbol;
                    processThread.IsBackground = false;
                    processThreads.Add(processThread);
                }
            }
        }

        private void ProcessCsv(string file)
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
                Thread.Sleep(randomNumber);
                var temp = quote.Split(',');
                var tick = new Tick(tickId, temp[0].Trim('"'), double.Parse(temp[8].Trim('"')), DateTime.Parse(temp[2].Trim('\"')));
                RaiseNewTickEvent(tick);
                tickId++;
            }
        }

        private void LoadFilePaths()
        {
            var baseFolder = ConfigurationSettings.AppSettings["CsvProviderFolder"];
            var symbols = ConfigurationSettings.AppSettings["Symbols"].Split(',');
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
