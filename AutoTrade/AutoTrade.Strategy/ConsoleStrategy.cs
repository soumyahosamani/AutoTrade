using AutoTrade.DTO;
using AutoTrade.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoTrade.Strategy
{
    public class ConsoleStrategy :IStrategy
    {
        public ConsoleStrategy(string name, string symbol)
        {
            Name = name;
            Symbol = symbol;
        }
        public string Name
        {
            get; private set;
        }
        public string Symbol { get; private set; }

        public void OnTick(Tick tick)
        {
            var randomNumber = Randomizer.GetRandomNumber() / 2;
            Console.WriteLine(Name + "sleeping for " + randomNumber);
            Thread.Sleep(randomNumber);
            Thread.Sleep(5000);
            Console.WriteLine("Thread {0} : Strategy {1} : {2}", Thread.CurrentThread.ManagedThreadId, Name, tick);
        }
    }
}
