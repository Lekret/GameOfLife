using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

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

public struct Neighbours
{
    public int N;
    public int NE;
    public int E;
    public int SE;
    public int S;
    public int SW;
    public int W;
    public int NW;
}

public struct Renderable
{
    public MeshRenderer Renderer;
    public MeshFilter Filter;
}

[Flags]
public enum NeighbourFlags
{
    Zero = 0,
    One = 1 << 0,
    Two = 1 << 1,
    Three = 1 << 2,
    Four = 1 << 3,
    Five = 1 << 4,
    Six = 1 << 5,
    Seven = 1 << 6,
    Eight = 1 << 7,
}