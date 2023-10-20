using System;

namespace Data
{
    [Flags]
    public enum NeighboursCount
    {
        Zero = 0,
        One = 1 << 0,
        Two = 1 << 1,
        Three = 1 << 2,
        Four = 1 << 3,
        Five = 1 << 4,
        Six = 1 << 5,
        Seven = 1 << 6,
        Eight = 1 << 7,
    }
}