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
                    var neighboursArr = cellChunk.GetSpan<Neighbours>();
                    var isLifeArr = cellChunk.GetSpan<IsLife>();
                    var isLifeNextSimArr = cellChunk.GetSpan<IsLifeNextSim>();
                    
                    foreach (var cellEntityIdx in cellChunk)
                    {
                        var neighbours = neighboursArr[cellEntityIdx];
                        var isLife = isLifeArr[cellEntityIdx].Value;
                        ref var isLifeNextSim = ref isLifeNextSimArr[cellEntityIdx].Value;
                        var lifeNeighbours = 0;

                        bool CheckNeighbour(ref Entity entity) => entity != Entity.Null && World.Get<IsLife>(entity).Value;
                        if (CheckNeighbour(ref neighbours.N)) lifeNeighbours++;
                        if (CheckNeighbour(ref neighbours.NE)) lifeNeighbours++;
                        if (CheckNeighbour(ref neighbours.E)) lifeNeighbours++;
                        if (CheckNeighbour(ref neighbours.SE)) lifeNeighbours++;
                        if (CheckNeighbour(ref neighbours.S)) lifeNeighbours++;
                        if (CheckNeighbour(ref neighbours.SW)) lifeNeighbours++;
                        if (CheckNeighbour(ref neighbours.W)) lifeNeighbours++;
                        if (CheckNeighbour(ref neighbours.NW)) lifeNeighbours++;
                        
                        var lifeTestArray = isLife ? _config.LifeNeighboursToLive : _config.LifeNeighboursToBecomeLife;
                        isLifeNextSim = Array.IndexOf(lifeTestArray, lifeNeighbours) != -1;
                    }
                }
            }
        }
    }
}