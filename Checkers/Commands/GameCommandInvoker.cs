using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Commands
{
    public class GameCommandInvoker
    {
        private readonly Stack<IGameCommand> _commandHistory = new Stack<IGameCommand>();

        public void ExecuteCommand(IGameCommand command)
        {
            command.Execute();
            _commandHistory.Push(command);
        }

        public bool CanUndo => _commandHistory.Count > 0;
    }
}
