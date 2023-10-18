using System.Collections.Generic;
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

        private static void InitNeighbour(
            int posX,
            int posY,
            int offsetX,
            int offsetY,
            ref Entity neighbourSlot,
            Entity[,] grid,
            int gridWidth,
            int gridHeight)
        {
            var testX = posX + offsetX;
            var testY = posY + offsetY;
            if (testX < 0 || testX >= gridWidth)
            {
                neighbourSlot = Entity.Null;
                return;
            }

            if (testY < 0 || testY >= gridHeight)
            {
                neighbourSlot = Entity.Null;
                return;
            }

            neighbourSlot = grid[testX, testY];
        }

        public override void Initialize()
        {
            var lifeGrid = new LifeGrid {Width = _config.GridWidth, Height = _config.GridHeight};
            var interval = new SimulationInterval {Value = _config.SimulationInterval};
            World.Create(lifeGrid, interval);

            var width = lifeGrid.Width;
            var height = lifeGrid.Height;
            var entityGrid = new Entity[width, height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    entityGrid[x, y] = World.Create(
                        new Cell(),
                        new Neighbours(),
                        new Position {X = x, Y = y},
                        new IsLife {Value = false},
                        new IsLifeNextSim {Value = false}
                    );
                }
            }
            
            foreach (var entity in entityGrid)
            {
                ref var neighbours = ref entity.Get<Neighbours>();
                var position = entity.Get<Position>();
                var x = position.X;
                var y = position.Y;
                InitNeighbour(x, y, 0, 1, ref neighbours.N, entityGrid, width, height);
                InitNeighbour(x, y, 1, 1, ref neighbours.NE, entityGrid, width, height);
                InitNeighbour(x, y, 1, 0, ref neighbours.E, entityGrid, width, height);
                InitNeighbour(x, y, 1, -1, ref neighbours.SE, entityGrid, width, height);
                InitNeighbour(x, y, 0, -1, ref neighbours.S, entityGrid, width, height);
                InitNeighbour(x, y, -1, -1, ref neighbours.SW, entityGrid, width, height);
                InitNeighbour(x, y, -1, 0, ref neighbours.W, entityGrid, width, height);
                InitNeighbour(x, y, -1, 1, ref neighbours.NW, entityGrid, width, height);
            }

            var startPattern = _config.StartPattern;
            if (startPattern.UseRandom)
            {
                if (startPattern.UseSeed)
                    Random.InitState(startPattern.Seed);
                Debug.Log($"Generating with seed: {Random.seed}");

                foreach (var entity in entityGrid)
                {
                    if (startPattern.LifeProbability > Random.Range(0f, 1f))
                    {
                        entity.Get<IsLife>().Value = true;
                        entity.Get<IsLifeNextSim>().Value = true;
                    }
                }
            }
            else
            {
                foreach (var lifeCell in startPattern.LifeCells)
                {
                    var entity = entityGrid[lifeCell.x, lifeCell.y];
                    entity.Get<IsLife>().Value = true;
                    entity.Get<IsLifeNextSim>().Value = true;
                }
            }
        }
    }
}