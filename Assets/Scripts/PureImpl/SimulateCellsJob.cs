using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace PureImpl
{
    [BurstCompile]
    public struct SimulateCellsJob : IJob
    {
        [ReadOnly] public NativeArray<bool> IsLife;
        [ReadOnly] public NativeArray<Neighbours> Neighbours;
        [ReadOnly] public NativeArray<NeighbourFlags> NeighboursToFlag;
        public NativeArray<bool> IsLifeNextSim;
        public int Count;
        public NeighbourFlags LifeNeighboursToLive;
        public NeighbourFlags LifeNeighboursToBecomeLife;

        public void Execute()
        {
            for (var i = 0; i < Count; i++)
            {
                var neighbours = Neighbours[i];
                var lifeNeighbours = 0;

                if (IsNeighbourLife(neighbours.N)) lifeNeighbours++;
                if (IsNeighbourLife(neighbours.NE)) lifeNeighbours++;
                if (IsNeighbourLife(neighbours.E)) lifeNeighbours++;
                if (IsNeighbourLife(neighbours.SE)) lifeNeighbours++;
                if (IsNeighbourLife(neighbours.S)) lifeNeighbours++;
                if (IsNeighbourLife(neighbours.SW)) lifeNeighbours++;
                if (IsNeighbourLife(neighbours.W)) lifeNeighbours++;
                if (IsNeighbourLife(neighbours.NW)) lifeNeighbours++;

                var neighboursTestFlags = IsLife[i] ? LifeNeighboursToLive : LifeNeighboursToBecomeLife;
                var neighboursCountFlag = NeighboursToFlag[lifeNeighbours];
                IsLifeNextSim[i] = (neighboursTestFlags & neighboursCountFlag) != 0;
            }
        }

        private bool IsNeighbourLife(int neighbour)
        {
            return neighbour != -1 && IsLife[neighbour];
        }
    }
}