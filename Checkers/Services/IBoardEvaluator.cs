using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services
{
    public interface IBoardEvaluator
    {
        int Evaluate(Board board, PlayerType maximizingPlayer);
    }
}
