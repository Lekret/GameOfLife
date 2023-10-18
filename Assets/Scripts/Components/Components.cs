namespace Components
{
    public struct GameInterval
    {
        public float Value;
    }

    public struct GlobalEntity
    {
    }

    public struct SimulateGame
    {
    }

    public struct LifeGrid
    {
        public bool[,] Value;

        public int GetWidth() => Value.GetUpperBound(0) + 1;
        
        public int GetHeight() => Value.GetUpperBound(1) + 1;

        public bool IsAlive(int x, int y) => Value[x, y];

        public void SetAlive(int x, int y, bool alive)
        {
            Value[x, y] = alive;
        }
    }
}