﻿using System;
using Config;
using Jobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    [SerializeField] private KeyCode _restartKey = KeyCode.R;
    [SerializeField] private GameConfig _config;

    private NativeArray<NeighbourFlags> _neighboursToFlag;
    private GameInstance _instance;
    private Transform _parent;

    private void Start()
    {
        var neighboursToFlagManaged = (NeighbourFlags[]) Enum.GetValues(typeof(NeighbourFlags));
        _neighboursToFlag = new NativeArray<NeighbourFlags>(neighboursToFlagManaged, Allocator.Persistent);
            
        _parent = new GameObject("Graphics").transform;
        RecreateGame();
    }

    private void OnDestroy()
    {
        if (_neighboursToFlag.IsCreated)
            _neighboursToFlag.Dispose();

        if (_instance != null)
            _instance.Dispose();
    }

    private void RecreateGame()
    {
        if (_instance != null)
        {
            for (int i = 0, end = _instance.CellCount; i < end; i++)
            {
                var renderable = _instance.Renderables[i];
                if (renderable.Renderer)
                    Destroy(renderable.Renderer.gameObject);
            }

            _instance.Dispose();
        }

        _instance = GameOfLifeInitialization.CreateInstance(_config);

        for (int i = 0, end = _instance.CellCount; i < end; i++)
        {
            var cellObj = new GameObject("Cell");
            var renderable = new Renderable
            {
                Renderer = cellObj.AddComponent<MeshRenderer>(),
                Filter = cellObj.AddComponent<MeshFilter>()
            };
            cellObj.transform.SetParent(_parent);
            var position = _instance.Position[i];
            cellObj.transform.position = _config.DrawOrigin + new Vector3(
                position.x + position.x * _config.DrawWidthSpacing,
                position.y + position.y * _config.DrawHeightSpacing);
            _instance.Renderables[i] = renderable;
        }
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
        new ApplyLifeNextSim
        {
            Count = _instance.CellCount,
            IsLife = _instance.IsLife,
            IsLifeNextSim = _instance.IsLifeNextSim
        }.Schedule().Complete();
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
        
    private void UpdateCellGraphics()
    {
        for (int i = 0, end = _instance.CellCount; i < end; i++)
        {
            var renderable = _instance.Renderables[i];
            var material = _instance.IsLife[i] ? _config.LifeMaterial : _config.DeathMaterial;
            renderable.Renderer.material = material;
            renderable.Filter.mesh = _config.CellMesh;
        }
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