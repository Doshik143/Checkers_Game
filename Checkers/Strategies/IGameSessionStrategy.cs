using Checkers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Strategies
{
    public interface IGameSessionStrategy
    {
        void OnGameOver(PlayerType winner, string style);
        void OnGameStart();
    }
}
