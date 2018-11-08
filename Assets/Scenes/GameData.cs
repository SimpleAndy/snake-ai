using System;

namespace Snake
{
    [Serializable]
    public class GameData
    {
        public int highScore;

        public GameData(int score)
        {
            highScore = score;
        }
    }
}
