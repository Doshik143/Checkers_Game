using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services
{
    public interface IRandomProvider
    {
        int Next(int maxValue);
    }
}
