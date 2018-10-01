using AutoTrade.Core;
using AutoTrade.Core.FeedProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoTrade.Core.Entites;

namespace AutoTrade.Csv.FeedProvider
{
    public class CsvFeedProvider : IFeedProvider
    {
        private string filePath;
        private List<Tick> ticks = null;
        private int currentCount = 0;
        private int currentID = 0;

        public CsvFeedProvider(string symbol)
        {
            Symbol = symbol;
            filePath = Path.Combine(@"E:\Soumya\git\AutoTrade\AutoTrade.Data", symbol + ".csv");
        }

        public bool HasNext
        {
            get
            {
                return ticks == null ? true : currentCount < ticks.Count();
            }
        }

        public string Symbol
        {
            get; private set;
        }

        public Tick CurrentTick { get; private set; }

        public Tick NextTick()
        {
            // lazy loading
            if (ticks == null)
            {
                LoadAllTicks();
            }

            currentID++;
            currentCount++;
            CurrentTick = ticks.First(t => t.Id == currentID);
            return CurrentTick;
        }

        private void LoadAllTicks()
        {
            ticks = new List<Tick>();
            var quotes = File.ReadAllLines(filePath).Skip(1);
            int tickId = 1;

            foreach (var quote in quotes)
            {
                var temp = quote.Split(',');
                ticks.Add(new Tick(tickId++, temp[0].Trim('"'), double.Parse(temp[8].Trim('"')), DateTime.Parse(temp[2].Trim('\"'))));
            }

            currentID = 0;
            currentCount = 0;
        }
    }
}
