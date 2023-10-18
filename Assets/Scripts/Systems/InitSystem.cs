using Arch.Core;
using Arch.Core.Extensions;
using Components;
using Config;
using Core;
using UnityEngine;

namespace Systems
{
    public class InitSystem : BaseSystem
    {
        private readonly GameConfig _config;

        public InitSystem(World world, GameConfig config) : base(world)
        {
            _config = config;
        }

        public override void Initialize()
        {
            var lifeGrid = new LifeGrid {Value = new Entity[_config.GridWidth, _config.GridHeight]};
            var interval = new SimulationInterval {Value = _config.SimulationInterval};
            World.Create(
                lifeGrid,
                interval);

            var width = lifeGrid.GetWidth();
            var height = lifeGrid.GetHeight();

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    lifeGrid.Value[x, y] = World.Create(
                        new Cell(),
                        new Position {X = x, Y = y}
                    );
                }
            }

            var startPattern = _config.StartPattern;
            if (startPattern.UseRandom)
            {
                if (startPattern.UseSeed)
                    Random.InitState(startPattern.Seed);
                Debug.Log($"Generating with seed: {Random.seed}");

                foreach (var entity in lifeGrid.Value)
                {
                    if (startPattern.LifeProbability > Random.Range(0f, 1f))
                        entity.Add<Life>();
                }
            }
            else
            {
                foreach (var lifeCell in startPattern.LifeCells)
                {
                    lifeGrid.SetIsLife(lifeCell.x, lifeCell.y, true);
                }
            }
        }
    }
}