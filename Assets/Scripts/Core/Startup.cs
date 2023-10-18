using System;
using Arch.Core;
using Arch.System;
using Components;
using Config;
using Systems;
using UnityEditor.Experimental.GraphView;
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
            _systems = new Group<float>();
            _systems
                .Add(
                    new InitSystem(_world, Config),
                    new LifeSimulationTriggerSystem(_world, Config),
                    new LifeSimulationSystem(_world, Config),
                    new NextLifeSimApplySystem(_world),
                    new CameraUpdateSystem(_world, Config),
                    new LifeRenderSystem(_world, Config),
                    new RemoveAllSystem<SimulateLife>(_world),
                    new RestartSystem(_world, _systems)
                );
        }

        private void Start()
        {
            _systems.Initialize();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            _systems.BeforeUpdate(deltaTime);
            _systems.Update(deltaTime);
            _systems.AfterUpdate(deltaTime);
        }

        private void OnDestroy()
        {
            _systems.Dispose();
            _world.Dispose();
        }
    }
}