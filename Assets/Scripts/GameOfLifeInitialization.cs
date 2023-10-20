using Config;
using Data;
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
        var positions = new int2[cellCount];
        var drawMatrix = instance.DrawMatrix;
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            tempCellGrid[x, y] = cellCounter;
            drawMatrix[cellCounter] = Matrix4x4.TRS(
                pos: config.DrawOrigin +
                     new Vector3(x + x * config.DrawWidthSpacing, y + y * config.DrawHeightSpacing),
                q: Quaternion.identity,
                s: Vector3.one);
            positions[cellCounter] = new int2(x, y);
            cellCounter++;
        }

        InitNeighbours(instance, positions, tempCellGrid, width, height);

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

    private static void InitNeighbours(GameInstance instance, int2[] positions, int[,] tempCellGrid, int gridWidth, int gridHeight)
    {
        for (var i = 0; i < positions.Length; i++)
        {
            var pos = positions[i];
            var x = pos.x;
            var y = pos.y;
            var neighbours = instance.Neighbours[i];
            InitNeighbour(x, y, 0, 1, tempCellGrid, gridWidth, gridHeight, out neighbours.S);
            InitNeighbour(x, y, 1, 1, tempCellGrid, gridWidth, gridHeight, out neighbours.SE);
            InitNeighbour(x, y, 1, 0, tempCellGrid, gridWidth, gridHeight, out neighbours.E);
            InitNeighbour(x, y, 1, -1, tempCellGrid, gridWidth, gridHeight, out neighbours.NE);
            InitNeighbour(x, y, 0, -1, tempCellGrid, gridWidth, gridHeight, out neighbours.N);
            InitNeighbour(x, y, -1, -1, tempCellGrid, gridWidth, gridHeight, out neighbours.NW);
            InitNeighbour(x, y, -1, 0, tempCellGrid, gridWidth, gridHeight, out neighbours.W);
            InitNeighbour(x, y, -1, 1, tempCellGrid, gridWidth, gridHeight, out neighbours.SW);
            instance.Neighbours[i] = neighbours;
        }
    }

    private static void InitNeighbour(
        int posX,
        int posY,
        int offsetX,
        int offsetY,
        int[,] grid,
        int gridWidth,
        int gridHeight,
        out int neighbourSlot)
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