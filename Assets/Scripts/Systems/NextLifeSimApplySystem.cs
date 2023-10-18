using Arch.Core;
using Components;
using Core;

namespace Systems
{
    public class NextLifeSimApplySystem : BaseSystem
    {
        private readonly QueryDescription _query = new QueryDescription().WithAll<IsLife, IsLifeNextSim>();
        
        public NextLifeSimApplySystem(World world) : base(world)
        {
        }

        public override void Update(in float deltaTime)
        {
            World.Query(_query, (ref IsLife IsLife, ref IsLifeNextSim isLifeNextSim) =>
            {
                IsLife.Value = isLifeNextSim.Value;
            });
        }
    }
}