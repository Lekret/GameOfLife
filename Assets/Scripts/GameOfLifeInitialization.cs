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

        var cellCounter = 0;
        var drawMatrix = instance.DrawMatrix;
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            drawMatrix[cellCounter] = Matrix4x4.TRS(
                pos: config.DrawOrigin +
                     new Vector3(x + x * config.DrawWidthSpacing, y + y * config.DrawHeightSpacing),
                q: Quaternion.identity,
                s: Vector3.one);
            cellCounter++;
        }

        InitNeighbours(instance);

        var startPattern = config.StartPattern;
        if (startPattern.UseRandom)
        {
            if (startPattern.UseSeed)
                Random.InitState(startPattern.Seed);
            Debug.Log($"Generating with seed: {Random.seed}");

            for (var cellIdx = 0; cellIdx < cellCount; cellIdx++)
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
            foreach (var cellPosition in startPattern.LifeCells)
            {
                var cellIdx = CellPositionToIndex(instance.GridWidth, cellPosition.x, cellPosition.y);
                instance.IsLife[cellIdx] = true;
                instance.IsLifeNextSim[cellIdx] = true;
            }
        }

        return instance;
    }

    private static void InitNeighbours(GameInstance instance)
    {
        var gridWidth = instance.GridWidth;
        var gridHeight = instance.GridHeight;
        
        for (var i = 0; i < instance.CellCount; i++)
        {
            var pos = CellIndexToPosition(gridWidth, i);
            var x = pos.x;
            var y = pos.y;
            var neighbours = instance.Neighbours[i];
            InitNeighbour(x, y, 0, 1, gridWidth, gridHeight, out neighbours.S);
            InitNeighbour(x, y, 1, 1, gridWidth, gridHeight, out neighbours.SE);
            InitNeighbour(x, y, 1, 0, gridWidth, gridHeight, out neighbours.E);
            InitNeighbour(x, y, 1, -1, gridWidth, gridHeight, out neighbours.NE);
            InitNeighbour(x, y, 0, -1, gridWidth, gridHeight, out neighbours.N);
            InitNeighbour(x, y, -1, -1, gridWidth, gridHeight, out neighbours.NW);
            InitNeighbour(x, y, -1, 0, gridWidth, gridHeight, out neighbours.W);
            InitNeighbour(x, y, -1, 1, gridWidth, gridHeight, out neighbours.SW);
            instance.Neighbours[i] = neighbours;
        }
    }

    private static void InitNeighbour(
        int posX,
        int posY,
        int offsetX,
        int offsetY,
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

        neighbourSlot = CellPositionToIndex(gridWidth, testX, testY);
    }
    
    private static int2 CellIndexToPosition(int gridWidth, int index)
    {
        return new int2(index / gridWidth, index % gridWidth);
    }

    private static int CellPositionToIndex(int gridWidth, int x, int y)
    {
        return gridWidth * x + y;
    }
}