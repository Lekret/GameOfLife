using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Jobs
{
    [BurstCompile]
    public struct ApplyLifeNextSim : IJob
    {
        public int Count;
        [ReadOnly] public NativeArray<bool> IsLifeNextSim;
        public NativeArray<bool> IsLife;

        public void Execute()
        {
            for (var i = 0; i < Count; i++)
            {
                IsLife[i] = IsLifeNextSim[i];
            }
        }
    }
}