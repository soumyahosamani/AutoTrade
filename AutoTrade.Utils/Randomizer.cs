using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrade.Utils
{
    public class Randomizer
    {
        private static Random random;
        static Randomizer()
        {
            random = new Random();
        }
        public static int GetRandomNumber(int start = 1, int end = 10)
        {
            return random.Next(start, end) * 1000;
        }
    }
}
