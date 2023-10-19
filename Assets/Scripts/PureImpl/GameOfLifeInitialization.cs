using Config;
using UnityEngine;

namespace PureImpl
{
    public static class GameOfLifeInitialization
    {
        private static void InitNeighbour(
            int posX,
            int posY,
            int offsetX,
            int offsetY,
            ref Cell neighbourSlot,
            Cell[,] grid,
            int gridWidth,
            int gridHeight)
        {
            var testX = posX + offsetX;
            var testY = posY + offsetY;
            if (testX < 0 || testX >= gridWidth)
                return;

            if (testY < 0 || testY >= gridHeight)
                return;

            neighbourSlot = grid[testX, testY];
        }
        
        public static GameInstance CreateInstance(GameConfig config)
        {
            var instance = new GameInstance
            {
                GridWidth = config.GridWidth,
                GridHeight = config.GridHeight,
                SimulationInterval = config.SimulationInterval
            };

            var width = config.GridWidth;
            var height = config.GridHeight;
            var tempCellGrid = new Cell[width, height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var cell = new Cell
                    {
                        Position = new Vector2Int(x, y)
                    };
                    tempCellGrid[x, y] = cell;
                    instance.Cells.Add(cell);
                }
            }
            
            foreach (var cell in tempCellGrid)
            {
                ref var neighbours = ref cell.Neighbours;
                var position = cell.Position;
                var x = position.x;
                var y = position.y;
                InitNeighbour(x, y, 0, 1, ref neighbours.N, tempCellGrid, width, height);
                InitNeighbour(x, y, 1, 1, ref neighbours.NE, tempCellGrid, width, height);
                InitNeighbour(x, y, 1, 0, ref neighbours.E, tempCellGrid, width, height);
                InitNeighbour(x, y, 1, -1, ref neighbours.SE, tempCellGrid, width, height);
                InitNeighbour(x, y, 0, -1, ref neighbours.S, tempCellGrid, width, height);
                InitNeighbour(x, y, -1, -1, ref neighbours.SW, tempCellGrid, width, height);
                InitNeighbour(x, y, -1, 0, ref neighbours.W, tempCellGrid, width, height);
                InitNeighbour(x, y, -1, 1, ref neighbours.NW, tempCellGrid, width, height);
            }

            var startPattern = config.StartPattern;
            if (startPattern.UseRandom)
            {
                if (startPattern.UseSeed)
                    Random.InitState(startPattern.Seed);
                Debug.Log($"Generating with seed: {Random.seed}");

                foreach (var cell in tempCellGrid)
                {
                    if (startPattern.LifeProbability > Random.Range(0f, 1f))
                    {
                        cell.IsLife = true;
                        cell.IsLifeNextSim = true;
                    }
                }
            }
            else
            {
                foreach (var lifeCellPosition in startPattern.LifeCells)
                {
                    var cell = tempCellGrid[lifeCellPosition.x, lifeCellPosition.y];
                    cell.IsLife = true;
                    cell.IsLifeNextSim = true;
                }
            }

            return instance;
        }
    }
}