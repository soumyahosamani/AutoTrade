using AutoTrade.Core.FeedProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoTrade.Core;
using System.Configuration;
using System.IO;

namespace AutoTrade.Csv.FeedIterator
{
    public class CsvFeedIterator : IFeedIterator
    {
        private string filePath;
        private List<Tick> ticks = new List<Tick>();
        private int currentID = 1;
        public CsvFeedIterator(string symbol)
        {
            Symbol = symbol;
            var baseFolder = ConfigurationManager.AppSettings["CsvProviderFolder"];
            filePath = (Path.Combine(baseFolder, symbol + ".csv"));
            LoadAllTicks();
        }

        public string Symbol { get; private set; }

        private void LoadAllTicks()
        {
           var quotes = File.ReadAllLines(filePath).Skip(1);
            int tickId = 0;

            foreach (var quote in quotes)
            {
                var temp = quote.Split(',');
                ticks.Add(new Tick(tickId++, temp[0].Trim('"'), double.Parse(temp[8].Trim('"')), DateTime.Parse(temp[2].Trim('\"'))));
            }            
        }

        public Tick CurrentTick()
        {
            return ticks.First(t => t.Id == currentID);
        }

        public Tick First()
        {
            return ticks.First();
        }

        public bool IsDone()
        {
            return currentID == ticks.Count();
        }

        public Tick Next()
        {
            currentID++;
            if(IsDone() == false)
                return ticks.First(t => t.Id == currentID);
            return null;
        }
    }
}
