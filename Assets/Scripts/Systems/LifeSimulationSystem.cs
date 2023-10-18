using System;
using Arch.Core;
using Components;
using Config;
using Core;
using UnityEngine.Pool;

namespace Systems
{
    public class LifeSimulationSystem : BaseSystem
    {
        private readonly QueryDescription _query = new QueryDescription().WithAll<LifeGrid, SimulateGame>();
        private readonly GameConfig _config;

        public LifeSimulationSystem(World world, GameConfig config) : base(world)
        {
            _config = config;
        }

        public override void Update(in float deltaTime)
        {
            foreach (var chunk in World.Query(_query))
            foreach (var entityId in chunk)
            {
                var lifeGrid = chunk.Get<LifeGrid>(entityId);
                var width = lifeGrid.GetWidth();
                var height = lifeGrid.GetHeight();

                var delayedSetAlive = ListPool<(int X, int Y, bool IsAlive)>.Get();

                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        var aliveNeighbours = 0;

                        void Test(ref LifeGrid lg, int xOffset, int yOffset)
                        {
                            var xWithOffset = x + xOffset;
                            var yWithOffset = y + yOffset;

                            if (xWithOffset < 0 || xWithOffset >= width)
                                return;

                            if (yWithOffset < 0 || yWithOffset >= height)
                                return;

                            if (lg.IsAlive(xWithOffset, yWithOffset))
                                aliveNeighbours++;
                        }

                        Test(ref lifeGrid, 1, 0);
                        Test(ref lifeGrid, -1, 0);
                        Test(ref lifeGrid, 0, 1);
                        Test(ref lifeGrid, 0, -1);
                        Test(ref lifeGrid, 1, 1);
                        Test(ref lifeGrid, -1, -1);
                        Test(ref lifeGrid, 1, -1);
                        Test(ref lifeGrid, -1, 1);

                        var isAlive = lifeGrid.IsAlive(x, y);
                        var testArray =
                            isAlive ? _config.AliveNeighboursToLive : _config.AliveNeighboursToBecomeAlive;
                        var shouldBeAlive = Array.IndexOf(testArray, aliveNeighbours) != -1;
                        delayedSetAlive.Add((x, y, shouldBeAlive));
                    }
                }

                foreach (var delayed in delayedSetAlive)
                {
                    lifeGrid.SetAlive(delayed.X, delayed.Y, delayed.IsAlive);
                }

                ListPool<(int X, int Y, bool IsAlive)>.Release(delayedSetAlive);
            }
        }
    }
}