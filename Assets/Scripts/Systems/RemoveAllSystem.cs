using Arch.CommandBuffer;
using Arch.Core;
using Arch.Core.Extensions;
using Core;

namespace Systems
{
    public class RemoveAllSystem<T> : BaseSystem
    {
        private readonly QueryDescription _query = new QueryDescription().WithAll<T>();
        private InlineQuery _inlineQuery;
        
        public RemoveAllSystem(World world) : base(world)
        {
        }

        public override void Update(in float deltaTime)
        {
            World.InlineEntityQuery<InlineQuery, T>(_query, ref _inlineQuery);
        }

        private struct InlineQuery : IForEachWithEntity<T>
        {
            public void Update(Entity entity, ref T t0)
            {
                entity.Remove<T>();
            }
        }
    }
}