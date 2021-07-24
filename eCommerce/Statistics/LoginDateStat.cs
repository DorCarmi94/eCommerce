using System;
using System.Collections.Generic;

namespace eCommerce.Statistics
{
    public class LoginDateStat
    {
        public List<Tuple<string, int>> Stat { get; set; }

        public LoginDateStat(List<Tuple<string, int>> stat)
        {
            Stat = stat;
        }
    }
}