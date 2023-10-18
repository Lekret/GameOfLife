using Arch.Core;
using Arch.Core.Extensions;
using Core;

namespace Systems
{
    public class RemoveAllSystem<T> : BaseSystem
    {
        private readonly QueryDescription _query = new QueryDescription().WithAll<T>();
        
        public RemoveAllSystem(World world) : base(world)
        {
        }

        public override void Update(in float deltaTime)
        {
            World.Query(_query, entity => entity.Remove<T>());
        }
    }
}