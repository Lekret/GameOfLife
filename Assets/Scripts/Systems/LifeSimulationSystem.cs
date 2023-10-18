using System;
using System.Runtime.CompilerServices;
using Arch.Core;
using Arch.Core.Extensions;
using Components;
using Config;
using Core;

namespace Systems
{
    public class LifeSimulationSystem : BaseSystem
    {
        private readonly QueryDescription _simulateQuery = new QueryDescription().WithAll<SimulateLife>();

        private readonly QueryDescription _cellQuery =
            new QueryDescription().WithAll<Cell, Position, Neighbours, IsLife, IsLifeNextSim>();

        private readonly GameConfig _config;

        public LifeSimulationSystem(World world, GameConfig config) : base(world)
        {
            _config = config;
        }

        public override void Update(in float deltaTime)
        {
            foreach (var gridChunk in World.Query(_simulateQuery))
            foreach (var _ in gridChunk)
            {                
                foreach (var cellChunk in World.Query(_cellQuery))
                {
                    var neighboursArr = cellChunk.GetArray<Neighbours>();
                    var isLifeArr = cellChunk.GetArray<IsLife>();
                    var isLifeNextSimArr = cellChunk.GetArray<IsLifeNextSim>();

                    for (int cellEntityIdx = 0, chunkSize = cellChunk.Size; cellEntityIdx < chunkSize; cellEntityIdx++)
                    {
                        ref var neighbours = ref neighboursArr[cellEntityIdx];
                        ref var isLife = ref isLifeArr[cellEntityIdx].Value;
                        ref var isLifeNextSim = ref isLifeNextSimArr[cellEntityIdx].Value;
                        var lifeNeighbours = 0;
                        
                        bool IsNeighbourLife(ref Entity entity) => entity != Entity.Null && World.Get<IsLife>(entity).Value;
                        if (IsNeighbourLife(ref neighbours.N)) lifeNeighbours++;
                        if (IsNeighbourLife(ref neighbours.NE)) lifeNeighbours++;
                        if (IsNeighbourLife(ref neighbours.E)) lifeNeighbours++;
                        if (IsNeighbourLife(ref neighbours.SE)) lifeNeighbours++;
                        if (IsNeighbourLife(ref neighbours.S)) lifeNeighbours++;
                        if (IsNeighbourLife(ref neighbours.SW)) lifeNeighbours++;
                        if (IsNeighbourLife(ref neighbours.W)) lifeNeighbours++;
                        if (IsNeighbourLife(ref neighbours.NW)) lifeNeighbours++;
                        
                        var lifeTestArray = isLife ? _config.LifeNeighboursToLive : _config.LifeNeighboursToBecomeLife;
                        isLifeNextSim = Array.IndexOf(lifeTestArray, lifeNeighbours) != -1;
                    }
                }
            }
        }
    }
}