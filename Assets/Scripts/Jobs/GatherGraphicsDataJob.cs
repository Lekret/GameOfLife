using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Jobs
{
    [BurstCompile]
    public struct GatherGraphicsDataJob : IJob
    {
        [ReadOnly] public int CellCount;
        [ReadOnly] public NativeArray<bool> IsLife;
        [ReadOnly] public NativeArray<Matrix4x4> DrawMatrix;
        [WriteOnly] public NativeArray<Matrix4x4> LifeMatrices;
        [WriteOnly] public NativeArray<Matrix4x4> DeathMatrices;
        [WriteOnly] public NativeArray<int> LifeCount;
        [WriteOnly] public NativeArray<int> DeathCount;

        public void Execute()
        {
            var lifeCount = 0;
            var deathCount = 0;
            
            for (var i = 0; i < CellCount; i++)
            {
                if (IsLife[i])
                {
                    LifeMatrices[lifeCount] = DrawMatrix[i];
                    lifeCount++;
                }
                else
                {
                    DeathMatrices[deathCount] = DrawMatrix[i];
                    deathCount++;
                }
            }

            LifeCount[0] = lifeCount;
            DeathCount[0] = deathCount;
        }
    }
}