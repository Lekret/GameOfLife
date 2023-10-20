using System;
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
        public NativeArray<int2> Position;
        public NativeArray<Neighbours> Neighbours;
        public NativeArray<Matrix4x4> DrawMatrix;
        public int CellCount;
        
        public void CreateArrays(int length)
        {
            IsLife = new NativeArray<bool>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            IsLifeNextSim = new NativeArray<bool>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            Position = new NativeArray<int2>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            Neighbours = new NativeArray<Neighbours>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            DrawMatrix = new NativeArray<Matrix4x4>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        }
        
        public void Dispose()
        {
            IsLife.Dispose();
            IsLifeNextSim.Dispose();
            Position.Dispose();
            Neighbours.Dispose();
            DrawMatrix.Dispose();
        }
    }
}