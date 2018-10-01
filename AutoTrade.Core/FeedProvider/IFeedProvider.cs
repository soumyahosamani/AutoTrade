using AutoTrade.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrade.Core.FeedProvider
{
    public interface IFeedProvider
    {
        string Symbol { get; }
        Tick NextTick();
        Tick CurrentTick { get; }
        bool HasNext { get; }
    }

}
