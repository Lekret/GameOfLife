﻿using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Data
{
    public class GameInstance : IDisposable
    {
        public int GridWidth;
        public int GridHeight;
        public float SimulationInterval;
        public NativeArray<bool> IsLife;
        public NativeArray<bool> IsLifeNextSim;
        public NativeArray<Neighbours> Neighbours;
        public NativeArray<Matrix4x4> DrawMatrix;
        public int CellCount;
        public Matrix4x4[] DeathDrawMatrices;
        public Matrix4x4[] LifeDrawMatrices;
        
        public void CreateArrays(int length)
        {
            IsLife = new NativeArray<bool>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            IsLifeNextSim = new NativeArray<bool>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            Neighbours = new NativeArray<Neighbours>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            DrawMatrix = new NativeArray<Matrix4x4>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            DeathDrawMatrices = new Matrix4x4[length];
            LifeDrawMatrices = new Matrix4x4[length];
        }
        
        public void Dispose()
        {
            IsLife.Dispose();
            IsLifeNextSim.Dispose();
            Neighbours.Dispose();
            DrawMatrix.Dispose();
        }
    }
}