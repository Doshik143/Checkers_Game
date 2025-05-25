using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services
{
    public class RandomProvider : IRandomProvider
    {
        private readonly Random _random = new Random();
        public int Next(int maxValue) => _random.Next(maxValue);
    }
}
