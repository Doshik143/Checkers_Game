using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services
{
    public interface IAIStrategy
    {
        Move ChooseMove(Board board, PlayerType player, List<Move> moves);
    }
}
