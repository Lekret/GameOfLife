using Arch.Core;
using Arch.Core.Extensions;
using Components;
using Core;

namespace Systems
{
    public class KillSystem : BaseSystem
    {
        private readonly QueryDescription _query = new QueryDescription().WithAll<Kill, Life>();
        
        public KillSystem(World world) : base(world)
        {
        }

        public override void Update(in float deltaTime)
        {
            World.Query(_query, entity => entity.Remove<Kill, Life>());
        }
    }
}