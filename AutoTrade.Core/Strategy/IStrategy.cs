using AutoTrade.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrade.Core.Strategy
{
    public interface IStrategy
    {
        string Name { get; }
        string Symbol { get; }
        void OnTick(Tick tick);
    }
}
