using Checkers.Models;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Checkers.Services
{
    public class GameSaver
    {
        public void Save(Game game, string path)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                new BinaryFormatter().Serialize(stream, game);
            }
        }

        public Game Load(string path)
        {
            if (!File.Exists(path)) return null;

            using (var stream = new FileStream(path, FileMode.Open))
            {
                return (Game)new BinaryFormatter().Deserialize(stream);
            }
        }
    }
}