using Arch.Core;
using Arch.System;
using Components;
using Systems;
using UnityEngine;

namespace Infrastructure
{
    public class Startup : MonoBehaviour
    {
        public GameConfig Config;

        private Group<float> _group;
        private World _world;

        private void Awake()
        {
            _world = World.Create();
            InitWorld();
            _group = new Group<float>()
                .Add(new InitSystem(_world),
                    new LifeSimulationTriggerSystem(_world),
                    new LifeSimulationSystem(_world),
                    new CameraUpdateSystem(_world),
                    new RenderLifeSystem(_world),
                    new RemoveAllSystem<SimulateGame>(_world)
                );
        }

        private void InitWorld()
        {
            _world.Create(new GlobalEntity(), new GameData(Config));
        }

        private void Start()
        {
            _group.Initialize();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _world.Clear();
                InitWorld();
                _group.Initialize();
                return;
            }

            var deltaTime = Time.deltaTime;
            _group.BeforeUpdate(deltaTime);
            _group.Update(deltaTime);
            _group.AfterUpdate(deltaTime);
        }
    }
}