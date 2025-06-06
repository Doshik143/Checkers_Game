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
    public class SaveGameCommand : IGameCommand
    {
        private readonly Game _game;
        private readonly GameSaver _saver;

        public SaveGameCommand(Game game, GameSaver saver)
        {
            _game = game;
            _saver = saver;
        }

        public void Execute()
        {
            using (var saveDialog = new SaveFileDialog
            {
                Filter = "JSON файли (*.json)|*.json",
                Title = "Зберегти гру",
                DefaultExt = "json",
                AddExtension = true
            })
            {
                if (saveDialog.ShowDialog() == DialogResult.OK &&
                    _saver.Save(_game, saveDialog.FileName))
                {
                    MessageBox.Show("Гра успішно збережена!", "Успіх",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
