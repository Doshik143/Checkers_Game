using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services
{
    public class EasyStrategy : IAIStrategy
    {
        private readonly IRandomProvider _rnd;
        public EasyStrategy(IRandomProvider rnd) => _rnd = rnd;
        public Move ChooseMove(Board board, PlayerType player, List<Move> moves)
            => moves.Count == 0 ? null : moves[_rnd.Next(moves.Count)];
    }
}
