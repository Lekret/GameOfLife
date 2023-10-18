using Arch.Core;
using UnityEngine;

namespace Components
{
    public struct Cell
    {
    }

    public struct Neighbours
    {
        public Entity N;
        public Entity NE;
        public Entity E;
        public Entity SE;
        public Entity S;
        public Entity SW;
        public Entity W;
        public Entity NW;
    }

    public struct IsLife
    {
        public bool Value;
    }

    public struct IsLifeNextSim
    {
        public bool Value;
    }

    public struct Position
    {
        public int X;
        public int Y;
    }

    public struct SimulationInterval
    {
        public float Value;
    }

    public struct SimulateLife
    {
    }

    public struct LifeGrid
    {
        public int Width;
        public int Height;
    }

    public struct Renderable
    {
        public MeshRenderer Renderer;
        public MeshFilter Filter;
    }
}