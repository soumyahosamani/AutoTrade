using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrade.DTO
{
    public class Tick
    {
        public double Price;
        public DateTime Time;
        public int Id;

        public Tick(int id, string symbol, double price, DateTime time)
        {
            Price = price;
            Time = time;
            Id = id;
            Symbol = symbol;
        }

        public string Symbol { get; private set; }

        public override string ToString()
        {
            return base.ToString();
            //return string.Format("{0}: {1} {2} {3} ", Id, Time, Symbol, Price);
        }
    }
}
