using System;
using Arch.Core;
using Arch.Core.Extensions;
using Components;
using UnityEngine.Pool;

namespace Systems
{
    public class LifeSimulationSystem : BaseSystem
    {
        public LifeSimulationSystem(World world) : base(world)
        {
        }

        public override void Update(in float deltaTime)
        {
            var globalEntity = World.GetGlobalEntity();
            if (!globalEntity.Has<SimulateGame>())
                return;

            var gameData = globalEntity.Get<GameData>();
            var config = gameData.Config;
            var lifeGrid = globalEntity.Get<LifeGrid>();

            var width = lifeGrid.GetWidth();
            var height = lifeGrid.GetHeight();

            var delayedSetAlive = ListPool<(int X, int Y, bool IsAlive)>.Get();

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var aliveNeighbours = 0;

                    void Test(int xOffset, int yOffset)
                    {
                        var xWithOffset = x + xOffset;
                        var yWithOffset = y + yOffset;

                        if (xWithOffset < 0 || xWithOffset >= width)
                            return;

                        if (yWithOffset < 0 || yWithOffset >= height)
                            return;

                        if (lifeGrid.IsAlive(xWithOffset, yWithOffset))
                            aliveNeighbours++;
                    }

                    Test(1, 0);
                    Test(-1, 0);
                    Test(0, 1);
                    Test(0, -1);
                    Test(1, 1);
                    Test(-1, -1);
                    Test(1, -1);
                    Test(-1, 1);

                    var isAlive = lifeGrid.IsAlive(x, y);
                    var testArray =
                        isAlive ? config.AliveNeighboursToLive : config.AliveNeighboursToBecomeAlive;
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