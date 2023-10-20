using System;
using System.Buffers;
using Config;
using Data;
using Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

public class GameOfLife : MonoBehaviour
{
    [SerializeField] private KeyCode _restartKey = KeyCode.R;
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

        UpdateCameraPosition();

        if (Input.GetKeyDown(_restartKey))
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
            Count = _instance.CellCount,
            IsLife = _instance.IsLife,
            IsLifeNextSim = _instance.IsLifeNextSim,
            Neighbours = _instance.Neighbours,
            LifeNeighboursToLive = _config.LifeNeighboursToLive,
            LifeNeighboursToBecomeLife = _config.LifeNeighboursToBecomeLife,
            NumToNeighboursCount = _numToNeighboursCount
        }.Schedule().Complete();
    }
    
    private unsafe void UpdateCellGraphics()
    {
        _commandBuffer.Clear();
        
        var isLifePtr = _instance.IsLife.GetUnsafePtr();
        var drawMatrixPtr = _instance.DrawMatrix.GetUnsafePtr();
        var cellCount = _instance.CellCount;
        var lifeMatrices = ArrayPool<Matrix4x4>.Shared.Rent(cellCount);
        var lifeCount = 0;
        var deathMatrices = ArrayPool<Matrix4x4>.Shared.Rent(cellCount);
        var deathCount = 0;
        
        for (var i = 0; i < cellCount; i++)
        {
            var isLife = UnsafeUtility.ReadArrayElement<bool>(isLifePtr, i);
            var matrix = UnsafeUtility.ReadArrayElement<Matrix4x4>(drawMatrixPtr, i);
            
            if (isLife)
            {
                lifeMatrices[lifeCount] = matrix;
                lifeCount++;
            }
            else
            {
                deathMatrices[deathCount] = matrix;
                deathCount++;
            }
        }
        
        _commandBuffer.DrawMeshInstanced(_config.CellMesh, 0, _config.LifeMaterial, -1, lifeMatrices, lifeCount);
        _commandBuffer.DrawMeshInstanced(_config.CellMesh, 0, _config.DeathMaterial, -1, deathMatrices, deathCount);
        
        ArrayPool<Matrix4x4>.Shared.Return(lifeMatrices);
        ArrayPool<Matrix4x4>.Shared.Return(deathMatrices);
    }

    private void UpdateCameraPosition()
    {
        var width = _instance.GridWidth;
        var height = _instance.GridHeight;
        var cameraPosition = new Vector3
        (
            width / 2f + _config.DrawWidthSpacing * (width / 2f),
            height / 2f + _config.DrawHeightSpacing * (height / 2f),
            -_config.CameraDistance
        );
        _camera.transform.position = cameraPosition;
    }
}