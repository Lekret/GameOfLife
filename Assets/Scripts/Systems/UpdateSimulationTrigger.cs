using Arch.Core;
using Arch.Core.Extensions;
using Components;
using UnityEngine;

namespace Systems
{
    public class LifeSimulationTriggerSystem : BaseSystem
    {
        public LifeSimulationTriggerSystem(World world) : base(world)
        {
        }

        public override void Update(in float deltaTime)
        {
            var gameData = World.GetGameData();
            var config = gameData.Config;

            if (config.SimulateByKey)
            {
                if (Input.GetKeyDown(config.SimulateKey))
                    World.GetGlobalEntity().Add<SimulateGame>();
                return;
            }

            var query = new QueryDescription().WithAll<GameInterval>();
            foreach (var chunk in World.Query(query))
            {
                foreach (var entity in chunk)
                {
                    ref var gameInterval = ref chunk.Get<GameInterval>(entity).Value;
                    gameInterval -= deltaTime;
                    if (gameInterval <= 0)
                    {
                        gameInterval = config.SimulationInterval;
                        World.GetGlobalEntity().Add<SimulateGame>();
                    }
                }
            }
        }
    }
}