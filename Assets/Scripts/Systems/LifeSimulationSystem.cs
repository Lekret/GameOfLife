using System;
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
                foreach (var cellEntityId in cellChunk)
                {
                    var neighbours = cellChunk.Get<Neighbours>(cellEntityId);
                    var isLife = cellChunk.Get<IsLife>(cellEntityId).Value;
                    ref var isLifeNextSim = ref cellChunk.Get<IsLifeNextSim>(cellEntityId).Value;
                    var lifeNeighbours = 0;

                    bool CheckNeighbour(Entity entity) => entity != Entity.Null && entity.Get<IsLife>().Value;
                    if (CheckNeighbour(neighbours.N)) lifeNeighbours++;
                    if (CheckNeighbour(neighbours.NE)) lifeNeighbours++;
                    if (CheckNeighbour(neighbours.E)) lifeNeighbours++;
                    if (CheckNeighbour(neighbours.SE)) lifeNeighbours++;
                    if (CheckNeighbour(neighbours.S)) lifeNeighbours++;
                    if (CheckNeighbour(neighbours.SW)) lifeNeighbours++;
                    if (CheckNeighbour(neighbours.W)) lifeNeighbours++;
                    if (CheckNeighbour(neighbours.NW)) lifeNeighbours++;

                    var lifeTestArray = isLife ? _config.LifeNeighboursToLive : _config.LifeNeighboursToBecomeLife;
                    isLifeNextSim = Array.IndexOf(lifeTestArray, lifeNeighbours) != -1;
                }
            }
        }
    }
}