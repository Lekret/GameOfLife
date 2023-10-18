using Arch.Core;
using Arch.Core.Extensions;
using Components;
using Config;
using Core;
using UnityEngine;

namespace Systems
{
    public class LifeSimulationTriggerSystem : BaseSystem
    {
        private readonly QueryDescription _query = new QueryDescription().WithAll<LifeGrid, SimulationInterval>();
        private readonly GameConfig _config;

        public LifeSimulationTriggerSystem(World world, GameConfig config) : base(world)
        {
            _config = config;
        }

        public override void Update(in float deltaTime)
        {
            foreach (var chunk in World.Query(_query))
            foreach (var entityId in chunk)
            {
                var entity = chunk.Entity(entityId);

                if (_config.SimulateByKey)
                {
                    if (Input.GetKeyDown(_config.SimulateKey))
                        entity.Add<SimulateLife>();

                    continue;
                }

                ref var interval = ref chunk.Get<SimulationInterval>(entityId).Value;
                interval -= deltaTime;

                if (interval <= 0)
                {
                    interval = _config.SimulationInterval;
                    entity.Add<SimulateLife>();
                }
            }
        }
    }
}