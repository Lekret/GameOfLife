using Arch.Core;
using Arch.System;
using Core;
using UnityEngine;

namespace Systems
{
    public class RestartSystem : BaseSystem
    {
        private readonly ISystem<float> _systems;

        public RestartSystem(World world, ISystem<float> systems) : base(world)
        {
            _systems = systems;
        }

        public override void Update(in float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                World.Clear();
                _systems.Initialize();
            }
        }
    }
}