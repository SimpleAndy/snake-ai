using AI;
using Assets.Core;
using Assets.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Snake
{
    public class GameManager : MonoBehaviour
    {
        public int maxHeight = 15;
        public int maxWidth = 17;

        public Color color1;
        public Color color2;
        public Color appleColor = Color.red;
        public Color playerColor = Color.black;

        public Transform cameraHolder;

        MapEntity map;
        
        int highScore;
        
        public float moveRate = 0.15f;
        float timer;

        public Text currentScoreText;
        public Text highscoreText;

        public UnityEvent onStart;
        public UnityEvent onGameOver;
        public UnityEvent onFirstInput;
        public UnityEvent onScore;

        #region Initialise
        private void Start()
        {
            var seed = 10;
            map = new MapEntity(maxWidth, maxHeight, seed);
            map.Snake = new SnakeEntity();
            map.Snake.Brain = new MatrixBrain(maxWidth, maxHeight, new List<int> { 50, 20, 5 }, new SigmoidActivationFunction());
            map.Snake.Brain.RandomiseWeightsAndBiases();
            onStart.Invoke();
        }

        public void StartNewGame()
        {
            ClearReferences();
            CreateMap();
            PlacePlayer();
            PlaceCamera();
            CreateApple();
            map.Snake.CurrentDirection = Direction.Up;

            var save = new Save();
            save.LoadFile();
            highScore = save.currentHighScore;

            map.IsGameOver = false;
            map.Snake.Score = 0;
            UpdateScore();
        }

        public void ClearReferences()
        {
            if (map.MapObject != null)
                Destroy(map.MapObject);
            if (map.Snake.HeadObject != null)
                Destroy(map.Snake.HeadObject);
            if (map.AppleObject != null)
                Destroy(map.AppleObject);
            foreach (var t in map.Snake.TailNodes)
            {
                if (t.gameObject != null)
                    Destroy(t.gameObject);
            }
            map.Snake.TailNodes.Clear();
            if (map.Snake.TailParentObject != null)
                Destroy(map.Snake.TailParentObject);

            map.Snake.AvailableNodes.Clear();
            map.Grid = null;
        }

        void CreateMap()
        {
            map.MapObject = new GameObject("Map");
            map.SpriteRenderer = map.MapObject.AddComponent<SpriteRenderer>();

            map.Grid = new Node[maxWidth, maxHeight];

            var tp = Vector3.zero;

            Texture2D texture = new Texture2D(maxWidth, maxHeight);
            for (int x = 0; x < maxWidth; x++)
            {
                for (int y = 0; y < maxHeight; y++)
                {
                    tp.x = x;
                    tp.y = y;

                    Node n = new Node()
                    {
                        x = x,
                        y = y,
                        worldPosition = tp
                    };

                    map.Grid[x, y] = n;

                    map.Snake.AvailableNodes.Add(n);

                    #region MapColor
                    if (x % 2 == 0)
                    {
                        if (y % 2 == 0)
                        {
                            texture.SetPixel(x, y, color1);
                        }
                        else
                        {
                            texture.SetPixel(x, y, color2);
                        }
                    }
                    else
                    {
                        if (y % 2 == 0)
                        {
                            texture.SetPixel(x, y, color2);
                        }
                        else
                        {
                            texture.SetPixel(x, y, color1);
                        }
                    }
                    #endregion

                }
            }

            texture.filterMode = FilterMode.Point;

            texture.Apply();

            Rect rect = new Rect(0, 0, maxWidth, maxHeight);

            Sprite sprite = Sprite.Create(texture, rect, Vector2.zero, 1, 0, SpriteMeshType.FullRect);
            map.SpriteRenderer.sprite = sprite;

            InitialiseSnapshot();
        }

        void PlacePlayer()
        {
            map.Snake.HeadObject = new GameObject("Player");
            var playerRender = map.Snake.HeadObject.AddComponent<SpriteRenderer>();
            map.Snake.HeadSprite = CreateSprite(playerColor);
            playerRender.sprite = map.Snake.HeadSprite;
            playerRender.sortingOrder = 1;

            map.Snake.HeadNode = GetNode(3, 3);

            map.Snake.Snapshot[3, 3] = (int)Tile.Head;

            PlacePlayerObject(map.Snake.HeadObject, map.Snake.HeadNode);
            map.Snake.HeadObject.transform.localScale = Vector3.one * 0.9f;

            map.Snake.TailParentObject = new GameObject("TailParent");
        }

        void PlaceCamera()
        {
            Node n = GetNode(maxWidth / 2, maxHeight / 2);
            var p = n.worldPosition;
            p += Vector3.one * 0.5f;

            cameraHolder.position = p;
        }

        void CreateApple()
        {
            map.AppleObject = new GameObject("Apple");
            var appleRenderer = map.AppleObject.AddComponent<SpriteRenderer>();
            appleRenderer.sprite = CreateSprite(appleColor);
            appleRenderer.sortingOrder = 1;
            RandomlyPlaceApple();
        }
        #endregion

        #region Update
        Direction GetInput()
        {
            if (Input.GetButtonDown("Up"))
            {
                return Direction.Up;
            }
            else if (Input.GetButtonDown("Down"))
            {
                return Direction.Down;
            }
            else if (Input.GetButtonDown("Left"))
            {
                return Direction.Left;
            }
            else if (Input.GetButtonDown("Right"))
            {
                return Direction.Right;
            }

            return Direction.None;
        }

        void SetPlayerDirection(Direction direction)
        {
            if ((direction == Direction.Up && map.Snake.CurrentDirection != Direction.Down) ||
                (direction == Direction.Left && map.Snake.CurrentDirection != Direction.Right) ||
                (direction == Direction.Down && map.Snake.CurrentDirection != Direction.Up) ||
                (direction == Direction.Right && map.Snake.CurrentDirection != Direction.Left))
            {
                map.Snake.TargetDirection = direction;
            }
        }

        int CalculateBrainOutput()
        {
            ConvertSnapshotToInputs();
            var output = map.Snake.Brain.CalculateOutput(map.Snake.SnapshotInputs);
            if (output <= ((double)1 / 3))
            {
                return -1;
            }
            else if (output > ((double)2 / 3))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        void SetPlayerDirectionFromBrain(int output)
        {
            var currentDirectionInt = (int)map.Snake.CurrentDirection;

            switch (output)
            {
                case -1:
                    map.Snake.TargetDirection = (Direction)((currentDirectionInt - 1) % 4);
                    break;
                case 0:
                    map.Snake.TargetDirection = map.Snake.CurrentDirection;
                    break;
                case 1:
                    map.Snake.TargetDirection = (Direction)((currentDirectionInt + 1) % 4);
                    break;
                default:
                    break;
            }
        }

        void MovePlayer()
        {
            int x = 0;
            int y = 0;

            switch (map.Snake.CurrentDirection)
            {
                case Direction.Up:
                    y = 1;
                    break;
                case Direction.Down:
                    y = -1;
                    break;
                case Direction.Left:
                    x = -1;
                    break;
                case Direction.Right:
                    x = 1;
                    break;
            }

            Node targetNode = GetNode(map.Snake.HeadNode.x + x, map.Snake.HeadNode.y + y);

            if (targetNode == null || IsTailNode(targetNode))
            {
                // Game Over
                onGameOver.Invoke();
                return;
            }

            bool isScore = false;

            if (targetNode == map.AppleNode)
            {
                isScore = true;
            }

            var previousNode = map.Snake.HeadNode;

            map.Snake.Snapshot[previousNode.x, previousNode.y] = (int)Tile.Floor;

            map.Snake.AvailableNodes.Add(previousNode);

            if (isScore)
            {
                map.Snake.TailNodes.Add(CreateTailNode(previousNode.x, previousNode.y));
                map.Snake.AvailableNodes.Remove(previousNode);
            }

            MoveTail();

            map.Snake.Snapshot[targetNode.x, targetNode.y] = (int)Tile.Head;

            PlacePlayerObject(map.Snake.HeadObject, targetNode);
            map.Snake.HeadNode = targetNode;

            map.Snake.AvailableNodes.Remove(map.Snake.HeadNode);

            if (isScore)
            {
                map.Snake.Score++;
                if (map.Snake.Score >= highScore)
                {
                    highScore = map.Snake.Score;
                }
                onScore.Invoke();

                if (map.Snake.AvailableNodes.Count > 0)
                {
                    RandomlyPlaceApple();
                }
                else
                {
                    // You won!
                }
            }
        }

        void MoveTail()
        {
            var previousNode = new Node();

            for (int i = 0; i < map.Snake.TailNodes.Count; i++)
            {
                var p = map.Snake.TailNodes[i];
                map.Snake.AvailableNodes.Add(p.node);

                if (i == 0)
                {
                    previousNode = p.node;
                    p.node = map.Snake.HeadNode;
                }
                else
                {
                    var prev = p.node;
                    p.node = previousNode;
                    previousNode = prev;
                }

                map.Snake.AvailableNodes.Remove(p.node);

                map.Snake.Snapshot[p.node.x, p.node.y] = (int)Tile.Tail;

                PlacePlayerObject(p.gameObject, p.node);
            }
        }

        private void Update()
        {
            if (map.IsGameOver)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    onStart.Invoke();
                }
                return;
            }

            //var direction = GetInput();
            //SetPlayerDirection(direction);

            var brainOutput = CalculateBrainOutput();

            SetPlayerDirectionFromBrain(brainOutput);

            if (map.HasGameStarted)
            {
                timer += Time.deltaTime;

                if (timer > moveRate)
                {
                    timer = 0;
                    map.Snake.CurrentDirection = map.Snake.TargetDirection;
                    MovePlayer();
                }
            }
            else
            {
                //if (direction != Direction.None)
                //{
                //SetPlayerDirection(direction);
                SetPlayerDirectionFromBrain(brainOutput);
                map.HasGameStarted = true;
                onFirstInput.Invoke();
                //}
            }
        }
        #endregion

        #region Utilities

        public void InitialiseSnapshot()
        {
            map.Snake.Snapshot = new int[maxWidth, maxHeight];
            map.Snake.SnapshotInputs = new double[maxWidth, maxHeight];

            for (int i = 0; i < maxWidth; i++)
            {
                for (int j = 0; j < maxHeight; j++)
                {
                    map.Snake.Snapshot[i, j] = (int)Tile.Floor;
                }
            }
        }

        void ConvertSnapshotToInputs()
        {
            for (int i = 0; i < maxWidth; i++)
            {
                for (int j = 0; j < maxHeight; j++)
                {
                    map.Snake.SnapshotInputs[i, j] = ((double)(((map.Snake.Snapshot[i, j]) - 1) * 2) / (double)3) - 1;
                }
            }
        }

        public void GameOver()
        {
            map.IsGameOver = true;
            map.HasGameStarted = false;
            var save = new Save();
            save.LoadFile();
            if (map.Snake.Score > save.currentHighScore)
            {
                save.currentHighScore = map.Snake.Score;
                save.SaveFile();
            }
        }

        public void UpdateScore()
        {
            currentScoreText.text = map.Snake.Score.ToString();
            highscoreText.text = highScore.ToString();
        }

        bool IsTailNode(Node node)
        {
            for (int i = 0; i < map.Snake.TailNodes.Count; i++)
            {
                if (map.Snake.TailNodes[i].node == node)
                {
                    return true;
                }
            }

            return false;
        }

        void PlacePlayerObject(GameObject obj, Node node)
        {
            var position = node.worldPosition;

            position += Vector3.one * 0.5f;
            obj.transform.position = position;
        }

        void RandomlyPlaceApple()
        {
            var ran = UnityEngine.Random.Range(0, map.Snake.AvailableNodes.Count);

            var n = map.Snake.AvailableNodes[ran];

            map.Snake.Snapshot[n.x, n.y] = (int)Tile.Apple;

            PlacePlayerObject(map.AppleObject, n);
            map.AppleObject.transform.localScale = Vector3.one * 0.7f;

            map.AppleNode = n;
        }

        Node GetNode(int x, int y)
        {
            if (x < 0 || x >= maxWidth || y < 0 || y >= maxHeight)
            {
                return null;
            }

            return map.Grid[x, y];
        }

        SpecialNode CreateTailNode(int x, int y)
        {
            var s = new SpecialNode();
            s.node = GetNode(x, y);
            s.gameObject = new GameObject();

            s.gameObject.transform.parent = map.Snake.TailParentObject.transform;
            s.gameObject.transform.position = s.node.worldPosition;
            s.gameObject.transform.localScale = Vector3.one * 0.80f;

            var renderer = s.gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = map.Snake.HeadSprite;
            renderer.sortingOrder = 1;

            return s;
        }

        Sprite CreateSprite(Color targetColor)
        {
            var texture = new Texture2D(1, 1);

            texture.SetPixel(0, 0, targetColor);
            texture.filterMode = FilterMode.Point;

            texture.Apply();

            var rect = new Rect(0, 0, 1, 1);
            return Sprite.Create(texture, rect, Vector2.one * 0.5f, 1, 0, SpriteMeshType.FullRect);
        }
        #endregion
    }
}
