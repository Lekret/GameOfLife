﻿using Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Jobs
{
    [BurstCompile]
    public struct SimulateCellsJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<bool> IsLife;
        [ReadOnly] public NativeArray<Neighbours> Neighbours;
        [ReadOnly] public NativeArray<NeighboursCount> NumToNeighboursCount;
        [ReadOnly] public NeighboursCount LifeNeighboursToLive;
        [ReadOnly] public NeighboursCount LifeNeighboursToBecomeLife;
        [WriteOnly] public NativeArray<bool> IsLifeNextSim;

        public void Execute(int i)
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
            var neighboursCountFlag = NumToNeighboursCount[lifeNeighbours];
            IsLifeNextSim[i] = (neighboursTestFlags & neighboursCountFlag) != 0;
        }

        private bool IsNeighbourLife(int neighbour)
        {
            return neighbour != -1 && IsLife[neighbour];
        }
    }
}