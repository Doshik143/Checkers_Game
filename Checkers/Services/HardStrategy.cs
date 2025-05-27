using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Services
{
    public class HardStrategy : IAIStrategy
    {
        private readonly MinimaxSearcher _searcher;
        private readonly int _depth;
        public HardStrategy(MinimaxSearcher searcher, int depth)
        {
            _searcher = searcher;
            _depth = depth;
        }
        public Move ChooseMove(Board board, PlayerType player, List<Move> moves)
            => _searcher.FindBestMove(board, player, moves, _depth);
    }
}
