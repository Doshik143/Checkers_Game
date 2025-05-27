using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services
{
    public class ProStrategy : HardStrategy
    {
        public ProStrategy(MinimaxSearcher searcher) : base(searcher, 6) { }
    }
}
