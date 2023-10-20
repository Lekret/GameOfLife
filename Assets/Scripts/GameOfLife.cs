using System;
using System.Buffers;
using Config;
using Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class GameOfLife : MonoBehaviour
{
    [SerializeField] private KeyCode _restartKey = KeyCode.R;
    [SerializeField] private GameConfig _config;

    private NativeArray<NeighbourFlags> _neighboursToFlag;
    private GameInstance _instance;
    private CommandBuffer _commandBuffer;

    private void Start()
    {
        var neighboursToFlagManaged = (NeighbourFlags[]) Enum.GetValues(typeof(NeighbourFlags));
        _neighboursToFlag = new NativeArray<NeighbourFlags>(neighboursToFlagManaged, Allocator.Persistent);
        _commandBuffer = new CommandBuffer();
        Camera.main.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, _commandBuffer);
        RecreateGame();
    }

    private void OnDestroy()
    {
        if (_neighboursToFlag.IsCreated)
            _neighboursToFlag.Dispose();

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
            NeighboursToFlag = _neighboursToFlag
        }.Schedule().Complete();
    }
    
    private unsafe void UpdateCellGraphics()
    {
        _commandBuffer.Clear();
        
        var isLifePtr = _instance.IsLife.GetUnsafePtr();
        var drawMatrixPtr = _instance.DrawMatrix.GetUnsafePtr();
        var cellCount = _instance.CellCount;
        var lifeMatrices = ArrayPool<Matrix4x4>.Shared.Rent(cellCount);
        var lifeMatrixIdx = 0;
        var deathMatrices = ArrayPool<Matrix4x4>.Shared.Rent(cellCount);
        var deathMatrixIdx = 0;
        
        for (var i = 0; i < cellCount; i++)
        {
            var isLife = UnsafeUtility.ReadArrayElement<bool>(isLifePtr, i);
            var matrix = UnsafeUtility.ReadArrayElement<Matrix4x4>(drawMatrixPtr, i);
            
            if (isLife)
            {
                lifeMatrices[lifeMatrixIdx] = matrix;
                lifeMatrixIdx++;
            }
            else
            {
                deathMatrices[deathMatrixIdx] = matrix;
                deathMatrixIdx++;
            }
        }
        
        _commandBuffer.DrawMeshInstanced(_config.CellMesh, 0, _config.LifeMaterial, -1, lifeMatrices, lifeMatrixIdx);
        _commandBuffer.DrawMeshInstanced(_config.CellMesh, 0, _config.DeathMaterial, -1, deathMatrices, deathMatrixIdx);
        
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
        Camera.main.transform.position = cameraPosition;
    }
}