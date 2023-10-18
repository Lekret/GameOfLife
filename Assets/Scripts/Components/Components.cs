using Arch.Core;
using Arch.Core.Extensions;

namespace Components
{
    public struct Cell
    {
    }

    public struct Life
    {
    }

    public struct MakeLife
    {
    }

    public struct Kill
    {
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
        
        public void SetIsLife(int x, int y, bool isLife)
        {
            if (isLife)
                Value[x, y].Add<Life>();
            else
                Value[x, y].Remove<Life>();
        }
    }
}