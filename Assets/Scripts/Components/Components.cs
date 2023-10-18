using Arch.Core;
using Arch.Core.Extensions;

namespace Components
{
    public struct Cell
    {
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
        public Entity[,] Value;

        public int GetWidth() => Value.GetUpperBound(0) + 1;

        public int GetHeight() => Value.GetUpperBound(1) + 1;

        public Entity Get(int x, int y) => Value[x, y];
    }
}