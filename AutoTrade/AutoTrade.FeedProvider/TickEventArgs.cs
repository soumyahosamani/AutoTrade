using AutoTrade.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrade.FeedProvider
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
