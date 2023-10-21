using System;
using Config;
using Data;
using Jobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

public class GameOfLife : MonoBehaviour
{
    [SerializeField] private GameConfig _config;
    [SerializeField] private Camera _camera;

    private NativeArray<NeighboursCount> _numToNeighboursCount;
    private GameInstance _instance;
    private CommandBuffer _commandBuffer;

    private void Start()
    {
        var neighboursCountManaged = (NeighboursCount[]) Enum.GetValues(typeof(NeighboursCount));
        _numToNeighboursCount = new NativeArray<NeighboursCount>(neighboursCountManaged, Allocator.Persistent);
        _commandBuffer = new CommandBuffer();
        _camera.orthographic = true;
        _camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, _commandBuffer);
        RecreateGame();
    }

    private void OnDestroy()
    {
        if (_numToNeighboursCount.IsCreated)
            _numToNeighboursCount.Dispose();

        if (_instance != null)
            _instance.Dispose();

        if (_commandBuffer != null)
            _commandBuffer.Dispose();
    }

    private void RecreateGame()
    {
        if (_instance != null)
        {
            _instance.Dispose();
        }

        _instance = GameOfLifeInitialization.CreateInstance(_config);
        UpdateCellGraphics();
    }

    private void Update()
    {
        if (ShouldTriggerSimulation())
        {
            ApplyLifeNextSim();
            SimulateCells();
            UpdateCellGraphics();
        }

        UpdateCamera();

        if (Input.GetKeyDown(_config.RestartKey))
        {
            RecreateGame();
        }
    }

    private void ApplyLifeNextSim()
    {
        (_instance.IsLife, _instance.IsLifeNextSim) = (_instance.IsLifeNextSim, _instance.IsLife);
    }

    private bool ShouldTriggerSimulation()
    {
        if (_config.SimulateByKey)
            return Input.GetKeyDown(_config.SimulateKey);

        _instance.SimulationInterval -= Time.deltaTime;
        if (_instance.SimulationInterval > 0)
            return false;

        _instance.SimulationInterval = _config.SimulationInterval;
        return true;
    }

    private void SimulateCells()
    {
        new SimulateCellsJob
        {
            IsLife = _instance.IsLife,
            IsLifeNextSim = _instance.IsLifeNextSim,
            Neighbours = _instance.Neighbours,
            LifeNeighboursToLive = _config.LifeNeighboursToLive,
            LifeNeighboursToBecomeLife = _config.LifeNeighboursToBecomeLife,
            NumToNeighboursCount = _numToNeighboursCount
        }.Schedule(_instance.CellCount, _config.SimulateCellsBatchCount).Complete();
    }

    private void UpdateCellGraphics()
    {
        var job = new GatherGraphicsDataJob
        {
            CellCount = _instance.CellCount,
            IsLife = _instance.IsLife,
            DrawMatrix = _instance.DrawMatrix,
            LifeMatrices = new NativeArray<Matrix4x4>(_instance.CellCount, Allocator.TempJob),
            DeathMatrices = new NativeArray<Matrix4x4>(_instance.CellCount, Allocator.TempJob),
            LifeCount = new NativeArray<int>(1, Allocator.TempJob),
            DeathCount = new NativeArray<int>(1, Allocator.TempJob)
        };
        job.Schedule().Complete();

        job.LifeMatrices.CopyTo(_instance.LifeDrawMatrices);
        job.DeathMatrices.CopyTo(_instance.DeathDrawMatrices);
        var lifeCount = job.LifeCount[0];
        var deathCount = job.DeathCount[0];

        job.LifeMatrices.Dispose();
        job.DeathMatrices.Dispose();
        job.LifeCount.Dispose();
        job.DeathCount.Dispose();
        
        _commandBuffer.Clear();
        _commandBuffer.DrawMeshInstanced(_config.CellMesh, 0, _config.LifeMaterial, -1, _instance.LifeDrawMatrices, lifeCount);
        _commandBuffer.DrawMeshInstanced(_config.CellMesh, 0, _config.DeathMaterial, -1, _instance.DeathDrawMatrices, deathCount);
    }

    private void UpdateCamera()
    {
        var width = _instance.GridWidth;
        var height = _instance.GridHeight;
        var worldHalfWidth = width / 2f + _config.DrawWidthSpacing * (width / 2f);
        var worldHalfHeight = height / 2f + _config.DrawHeightSpacing * (height / 2f);
        var cameraPosition = new Vector3(worldHalfWidth, worldHalfHeight, -10);
        _camera.transform.position = cameraPosition;
        var screenRatio = (float) Screen.width / Screen.height;
        _camera.orthographicSize = Mathf.Max(
            worldHalfHeight,
            worldHalfWidth / screenRatio);
    }
}