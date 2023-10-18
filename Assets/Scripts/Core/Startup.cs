using Arch.Core;
using Arch.System;
using Components;
using Config;
using Systems;
using UnityEngine;

namespace Core
{
    public class Startup : MonoBehaviour
    {
        public GameConfig Config;

        private Group<float> _systems;
        private World _world;

        private void Awake()
        {
            _world = World.Create();
            _systems = new Group<float>()
                .Add(
                    new InitSystem(_world, Config),
                    new LifeSimulationTriggerSystem(_world, Config),
                    new LifeSimulationSystem(_world, Config),
                    new CameraUpdateSystem(_world, Config),
                    new LifeRenderSystem(_world, Config),
                    new RemoveAllSystem<SimulateGame>(_world)
                );
        }

        private void Start()
        {
            _systems.Initialize();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _world.Clear();
                _systems.Initialize();
                return;
            }

            var deltaTime = Time.deltaTime;
            _systems.BeforeUpdate(deltaTime);
            _systems.Update(deltaTime);
            _systems.AfterUpdate(deltaTime);
        }
    }
}