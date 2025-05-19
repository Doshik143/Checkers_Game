using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Checkers.Models
{
    [Serializable]
    public class GameStatistics
    {
        public int WhiteWins { get; private set; }
        public int BlackWins { get; private set; }
        public int Draws { get; private set; }
        public TimeSpan TotalGameTime { get; private set; }
        public DateTime LastGameDate { get; private set; }

        public void RecordGame(PlayerType winner, TimeSpan duration)
        {
            switch (winner)
            {
                case PlayerType.White:
                    WhiteWins++;
                    break;
                case PlayerType.Black:
                    BlackWins++;
                    break;
                default:
                    Draws++;
                    break;
            }
            TotalGameTime += duration;
            LastGameDate = DateTime.Now;
        }

        public void SaveToFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
        }

        public static GameStatistics LoadFromFile(string path)
        {
            if (!File.Exists(path)) return new GameStatistics();

            using (var stream = new FileStream(path, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                return (GameStatistics)formatter.Deserialize(stream);
            }
        }

        public override string ToString()
        {
            return $"Статистика:\n" +
                   $"Перемоги білих: {WhiteWins}\n" +
                   $"Перемоги чорних: {BlackWins}\n" +
                   $"Нічиїх: {Draws}\n" +
                   $"Загальний час ігор: {TotalGameTime:hh\\:mm\\:ss}\n" +
                   $"Остання гра: {LastGameDate:g}";
        }
    }
}