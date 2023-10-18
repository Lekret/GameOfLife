using System;
using Arch.Core;
using Arch.Core.Extensions;
using GameOfLife.Components;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace GameOfLife
{
    public struct UselessData
    {
        public Matrix4x4 Matrix1;
        public Matrix4x4 Matrix2;
        public Matrix4x4 Matrix3;
    }

    public static class GameOfLifeSystems
    {
        private static GraphicsEngine _graphicsEngine = new();

        public static void Init(World world)
        {
            var globalEntity = world.GetGlobalEntity();
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

            /*
            for (var i = 0; i < 10000; i++)
            {
                world.Create(new UselessData());
            }

            var chunks = new List<Chunk>();
            world.GetChunks(new QueryDescription().WithAll<UselessData>(), chunks);

            // totalMem / columnMem = capacity
            // totalMem = capacity * columnMem
            // columnMem = totalMem / capacity

            foreach (var chunk in chunks)
            {
                var totalMem = 16 * 1024;
                var singleEntityMem = Marshal.SizeOf<Entity>() + Marshal.SizeOf<UselessData>();
                var expectedCapacity = totalMem / singleEntityMem;
                var supposedSingleEntityMem = totalMem / chunk.Capacity;
                Debug.Log($"Cache line byte size: {totalMem}");

                Debug.Log($"Expected chunk capacity: {expectedCapacity}");
                Debug.Log($"Actual chunk capacity: {chunk.Capacity}");

                Debug.Log($"Expected entity mem in chunk: {singleEntityMem}");
                Debug.Log($"Supposed entity mem in chunk: {supposedSingleEntityMem}");

                Debug.Log($"If capacity = {chunk.Capacity} and {singleEntityMem} " +
                          $"is correct entity size then {singleEntityMem * chunk.Capacity} will be sent to cache");

                break;
            }
            */
        }

        public static void UpdateSimulationTrigger(World world, float deltaTime)
        {
            var gameData = world.GetGameData();
            var config = gameData.Config;

            if (config.SimulateByKey)
            {
                if (Input.GetKeyDown(config.SimulateKey))
                    world.GetGlobalEntity().Add<SimulateGame>();
                return;
            }

            var query = new QueryDescription().WithAll<GameInterval>();
            foreach (var chunk in world.Query(query))
            {
                foreach (var entity in chunk)
                {
                    ref var gameInterval = ref chunk.Get<GameInterval>(entity).Value;
                    gameInterval -= deltaTime;
                    if (gameInterval <= 0)
                    {
                        gameInterval = config.SimulationInterval;
                        world.GetGlobalEntity().Add<SimulateGame>();
                    }
                }
            }
        }

        public static void SimulateLife(World world, float deltaTime)
        {
            var globalEntity = world.GetGlobalEntity();
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

        public static void UpdateCameraPosition(World world, float deltaTime)
        {
            var globalEntity = world.GetGlobalEntity();
            var lifeGrid = globalEntity.Get<LifeGrid>();
            var width = lifeGrid.GetWidth();
            var height = lifeGrid.GetHeight();

            var gameData = world.GetGameData();
            var config = gameData.Config;
            
            var cameraPosition = new Vector3
            (
                width / 2f + config.DrawWidthSpacing * (width / 2f),
                height / 2f + config.DrawHeightSpacing * (height / 2f),
                -config.CameraDistance
            );
            Camera.main.transform.position = cameraPosition;
        }

        public static void RenderLife(World world, float _)
        {
            var globalEntity = world.GetGlobalEntity();
            var gameData = globalEntity.Get<GameData>();
            var config = gameData.Config;
            var lifeGrid = globalEntity.Get<LifeGrid>();
            var width = lifeGrid.GetWidth();
            var height = lifeGrid.GetHeight();

            _graphicsEngine.Clear();
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var alive = lifeGrid.IsAlive(x, y);
                    var material = alive ? config.AliveMaterial : config.DeadMaterial;
                    var drawPos = config.DrawOrigin + new Vector3(
                        x + config.DrawWidthSpacing * x,
                        y + config.DrawHeightSpacing * y);
                    _graphicsEngine.DrawMesh(drawPos, config.CellMesh, material);
                }
            }
        }

        public static void RemoveAll<T>(World world, float deltaTime)
        {
            var query = new QueryDescription().WithAll<T>();
            world.Query(query, entity => entity.Remove<T>());
        }
    }
}