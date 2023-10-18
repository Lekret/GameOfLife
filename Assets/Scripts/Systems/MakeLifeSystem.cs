using Arch.Core;
using Arch.Core.Extensions;
using Components;
using Core;

namespace Systems
{
    public class MakeLifeSystem : BaseSystem
    {
        private readonly QueryDescription _query = new QueryDescription().WithAll<MakeLife>().WithNone<Life>();

        public MakeLifeSystem(World world) : base(world)
        {
        }

        public override void Update(in float deltaTime)
        {
            World.Query(_query, entity =>
            {
                entity.Add<Life>();
                entity.Remove<MakeLife>();
            });
        }
    }
}