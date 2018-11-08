using Snake;
using UnityEngine;

namespace Assets.Entities
{
    public class MapEntity
    {
        public SnakeEntity Snake { get; set; }

        public GameObject MapObject { get; set; }

        public SpriteRenderer SpriteRenderer { get; set; }

        public GameObject AppleObject { get; set; }

        public Node AppleNode { get; set; }

        public Node[,] Grid { get; set; }

        public bool IsGameOver { get; set; }

        public bool HasGameStarted { get; set; }

        public int Seed { get; set; }

        public MapEntity(int width, int height, int seed)
        {
            MapObject = new GameObject();
            SpriteRenderer = new SpriteRenderer();
            AppleObject = new GameObject();
            AppleNode = new Node();
            Grid = new Node[width, height];
            Seed = seed;
        }
    }
}
