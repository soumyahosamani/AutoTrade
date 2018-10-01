using AutoTrade.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrade.Core.FeedProvider
{
    public class TickEventArgs : EventArgs
    {
        public TickEventArgs(Tick tick)
        {
            Tick = tick;
        }
        public Tick Tick { get; private set; }
    }
}
