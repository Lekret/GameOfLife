using Arch.Core;
using Arch.Core.Extensions;
using Components;
using UnityEngine;

namespace Systems
{
    public class InitSystem : BaseSystem
    {
        public InitSystem(World world) : base(world)
        {
        }

        public override void Initialize()
        {
            var globalEntity = World.GetGlobalEntity();
            var gameData = globalEntity.Get<GameData>();
            var config = gameData.Config;

            globalEntity.Add(new GameInterval {Value = config.SimulationInterval});
            var lifeGrid = new LifeGrid {Value = new bool[config.GridWidth, config.GridHeight]};

            var startPattern = config.StartPattern;

            if (startPattern.UseRandom)
            {
                var width = lifeGrid.GetWidth();
                var height = lifeGrid.GetHeight();
                if (startPattern.UseSeed)
                    Random.InitState(startPattern.Seed);
                Debug.Log($"Generating with seed: {Random.seed}");

                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        if (startPattern.AliveProbability > Random.Range(0f, 1f))
                            lifeGrid.SetAlive(x, y, true);
                    }
                }
            }
            else
            {
                foreach (var aliveCell in startPattern.AliveCells)
                {
                    lifeGrid.SetAlive(aliveCell.x, aliveCell.y, true);
                }
            }

            globalEntity.Add(lifeGrid);
        }
    }
}