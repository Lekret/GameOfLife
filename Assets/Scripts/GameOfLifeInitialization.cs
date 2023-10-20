using Config;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public static class GameOfLifeInitialization
{
    public static GameInstance CreateInstance(GameConfig config)
    {
        var instance = new GameInstance
        {
            GridWidth = config.GridWidth,
            GridHeight = config.GridHeight,
            SimulationInterval = config.SimulationInterval,
        };
        var cellCount = config.GridWidth * config.GridHeight;
        instance.CellCount = cellCount;
        instance.CreateArrays(cellCount);

        var width = config.GridWidth;
        var height = config.GridHeight;
        var tempCellGrid = new int[width, height];

        var cellCounter = 0;
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                instance.Position[cellCounter] = new int2(x, y);
                tempCellGrid[x, y] = cellCounter;
                cellCounter++;
            }
        }
            
        InitNeighbours(instance, tempCellGrid, width, height);

        var startPattern = config.StartPattern;
        if (startPattern.UseRandom)
        {
            if (startPattern.UseSeed)
                Random.InitState(startPattern.Seed);
            Debug.Log($"Generating with seed: {Random.seed}");

            foreach (var cellIdx in tempCellGrid)
            {
                if (startPattern.LifeProbability > Random.Range(0f, 1f))
                {
                    instance.IsLife[cellIdx] = true;
                    instance.IsLifeNextSim[cellIdx] = true;
                }
            }
        }
        else
        {
            foreach (var lifeCellPosition in startPattern.LifeCells)
            {
                var cellIdx = tempCellGrid[lifeCellPosition.x, lifeCellPosition.y];
                instance.IsLife[cellIdx] = true;
                instance.IsLifeNextSim[cellIdx] = true;
            }
        }

        return instance;
    }

    private static void InitNeighbours(GameInstance instance, int[,] tempCellGrid, int gridWidth, int gridHeight)
    {
        foreach (var cellIdx in tempCellGrid)
        {
            var neighbours = instance.Neighbours[cellIdx];
            var position = instance.Position[cellIdx];
            var x = position.x;
            var y = position.y;
            InitNeighbour(x, y, 0, 1, ref neighbours.N, tempCellGrid, gridWidth, gridHeight);
            InitNeighbour(x, y, 1, 1, ref neighbours.NE, tempCellGrid, gridWidth, gridHeight);
            InitNeighbour(x, y, 1, 0, ref neighbours.E, tempCellGrid, gridWidth, gridHeight);
            InitNeighbour(x, y, 1, -1, ref neighbours.SE, tempCellGrid, gridWidth, gridHeight);
            InitNeighbour(x, y, 0, -1, ref neighbours.S, tempCellGrid, gridWidth, gridHeight);
            InitNeighbour(x, y, -1, -1, ref neighbours.SW, tempCellGrid, gridWidth, gridHeight);
            InitNeighbour(x, y, -1, 0, ref neighbours.W, tempCellGrid, gridWidth, gridHeight);
            InitNeighbour(x, y, -1, 1, ref neighbours.NW, tempCellGrid, gridWidth, gridHeight);
            instance.Neighbours[cellIdx] = neighbours;
        }
    }

    private static void InitNeighbour(
        int posX,
        int posY,
        int offsetX,
        int offsetY,
        ref int neighbourSlot,
        int[,] grid,
        int gridWidth,
        int gridHeight)
    {
        var testX = posX + offsetX;
        var testY = posY + offsetY;
        if (testX < 0 || testX >= gridWidth)
        {
            neighbourSlot = -1;
            return;
        }

        if (testY < 0 || testY >= gridHeight)
        {
            neighbourSlot = -1;
            return;
        }

        neighbourSlot = grid[testX, testY];
    }
}