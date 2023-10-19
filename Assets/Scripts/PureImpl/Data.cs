using System;
using System.Collections.Generic;
using UnityEngine;

namespace PureImpl
{
    public class GameInstance
    {
        public int GridWidth;
        public int GridHeight;
        public float SimulationInterval;
        public List<Cell> Cells = new();
    }

    public class Cell
    {
        public bool IsLife;
        public bool IsLifeNextSim;
        public Vector2Int Position;
        public Neighbours Neighbours;
        public Renderable Renderable;
    }

    public struct Neighbours
    {
        public Cell N;
        public Cell NE;
        public Cell E;
        public Cell SE;
        public Cell S;
        public Cell SW;
        public Cell W;
        public Cell NW;
    }

    public struct Renderable
    {
        public MeshRenderer Renderer;
        public MeshFilter Filter;
    }

    [Flags]
    public enum NeighbourFlags
    {
        Zero = 0,
        One = 1 << 0,
        Two = 1 << 1,
        Three = 1 << 2,
        Four = 1 << 3,
        Five = 1 << 4,
        Seven = 1 << 5,
        Eight = 1 << 6,
        Nine = 1 << 7,
    }
}