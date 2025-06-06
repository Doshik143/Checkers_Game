using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Strategies
{
    public class RegularGameStrategy : IGameSessionStrategy
    {
        private readonly GameStatistics _stats;
        private readonly Stopwatch _gameTimer;

        public RegularGameStrategy(GameStatistics stats, Stopwatch gameTimer)
        {
            _stats = stats;
            _gameTimer = gameTimer;
        }

        public void OnGameOver(PlayerType winner, string style)
        {
            _gameTimer.Stop();
            _stats.RecordGame(winner, _gameTimer.Elapsed);
            _stats.SaveToFile("stats.dat");
        }

        public void OnGameStart()
        {
            _gameTimer.Restart();
        }
    }
}
