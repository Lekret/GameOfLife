using Arch.Core;
using Components;
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
            var lifeGrid = new LifeGrid {Value = new bool[_config.GridWidth, _config.GridHeight]};
            var interval = new SimulationInterval {Value = _config.SimulationInterval};
            World.Create(
                lifeGrid,
                interval);
            
            var startPattern = _config.StartPattern;

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
        }
    }
}