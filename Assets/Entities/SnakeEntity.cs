using AI;
using Assets.Core;
using Snake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Entities
{
    public class SnakeEntity
    {
        public GameObject HeadObject { get; set; }

        public Node HeadNode { get; set; }

        public Sprite HeadSprite { get; set; }

        public GameObject TailParentObject { get; set; }

        public List<Node> AvailableNodes { get; set; }

        public List<SpecialNode> TailNodes { get; set; }

        public int[,] Snapshot { get; set; }

        public double[,] SnapshotInputs { get; set; }

        public MatrixBrain Brain { get; set; }

        public Direction CurrentDirection { get; set; }

        public Direction TargetDirection { get; set; }

        public bool IsDead { get; set; }

        public int Score { get; set; }

        public SnakeEntity()
        {
            HeadObject = new GameObject();
            HeadNode = new Node();
            HeadSprite = new Sprite();
            TailParentObject = new GameObject();
            AvailableNodes = new List<Node>();
            TailNodes = new List<SpecialNode>();
            Brain = new MatrixBrain();
        }
    }
}
