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
        private readonly QueryDescription _simulateQuery = new QueryDescription().WithAll<LifeGrid, SimulateLife>();
        // private readonly QueryDescription _cellQuery = new QueryDescription().WithAll<Cell, Position, Life>(); TODO
        private readonly GameConfig _config;

        public LifeSimulationSystem(World world, GameConfig config) : base(world)
        {
            _config = config;
        }

        public override void Update(in float deltaTime)
        {
            foreach (var gridChunk in World.Query(_simulateQuery))
            foreach (var gridEntityId in gridChunk)
            {
                var lifeGrid = gridChunk.Get<LifeGrid>(gridEntityId);
                var width = lifeGrid.GetWidth();
                var height = lifeGrid.GetHeight();
                
                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < width; y++)
                    {
                        var lifeNeighbours = 0;

                        void Test(ref LifeGrid lg, int xOffset, int yOffset)
                        {
                            var xWithOffset = x + xOffset;
                            var yWithOffset = y + yOffset;

                            if (xWithOffset < 0 || xWithOffset >= width)
                                return;

                            if (yWithOffset < 0 || yWithOffset >= height)
                                return;

                            if (lg.Get(xWithOffset, yWithOffset).Has<Life>())
                                lifeNeighbours++;
                        }

                        Test(ref lifeGrid, 1, 0);
                        Test(ref lifeGrid, -1, 0);
                        Test(ref lifeGrid, 0, 1);
                        Test(ref lifeGrid, 0, -1);
                        Test(ref lifeGrid, 1, 1);
                        Test(ref lifeGrid, -1, -1);
                        Test(ref lifeGrid, 1, -1);
                        Test(ref lifeGrid, -1, 1);

                        var cellEntity = lifeGrid.Get(x, y);
                        var isLife = cellEntity.Has<Life>();
                        var lifeTestArray = isLife ? _config.LifeNeighboursToLive : _config.LifeNeighboursToBecomeLife;
                        var shouldBeLife = Array.IndexOf(lifeTestArray, lifeNeighbours) != -1;
                        if (shouldBeLife)
                        {
                            if (!isLife)
                                cellEntity.Add<MakeLife>();
                        }
                        else
                        {
                            if (isLife)
                                cellEntity.Add<Kill>();
                        }
                    }
                }
            }
        }
    }
}