using Checkers.Models;
using Checkers.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers.Commands
{
    public class LoadGameCommand : IGameCommand
    {
        private readonly GameSaver _saver;
        private readonly Action<Game> _onGameLoaded;

        public LoadGameCommand(GameSaver saver, Action<Game> onGameLoaded)
        {
            _saver = saver;
            _onGameLoaded = onGameLoaded;
        }

        public void Execute()
        {
            using (var openDialog = new OpenFileDialog
            {
                Filter = "JSON файли (*.json)|*.json",
                Title = "Завантажити гру",
                CheckFileExists = true
            })
            {
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    var loaded = _saver.Load(openDialog.FileName);
                    if (loaded != null)
                    {
                        _onGameLoaded?.Invoke(loaded);
                        MessageBox.Show("Гра успішно завантажена!", "Успіх",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}
