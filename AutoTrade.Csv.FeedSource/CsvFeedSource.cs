using AutoTrade.Core.FeedProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoTrade.Core;
using System.IO;
using System.Configuration;

namespace AutoTrade.Csv.FeedSource
{
    public class CsvFeedSource : IFeedSource
    {
        private string filePath;
        public CsvFeedSource(string symbol)
        {
            var baseFolder = ConfigurationManager.AppSettings["CsvProviderFolder"];
            filePath = (Path.Combine(baseFolder, symbol + ".csv"));
          
        }
        public List<Tick> LoadTicks()
        {
            var ticks = new List<Tick>();
            var quotes = File.ReadAllLines(filePath).Skip(1);
            int tickId = 0;

            foreach (var quote in quotes)
            {
                var temp = quote.Split(',');
                ticks.Add(new Tick(tickId++, temp[0].Trim('"'), double.Parse(temp[8].Trim('"')), DateTime.Parse(temp[2].Trim('\"'))));
            }
            return ticks;
        }
    }
}
