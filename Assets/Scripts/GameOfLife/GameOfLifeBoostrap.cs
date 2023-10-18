using Arch.Core;
using GameOfLife.Components;
using UnityEngine;

namespace GameOfLife
{
    public class GameOfLifeBoostrap : MonoBehaviour
    {
        public GameConfig Config;

        private SystemCollection _systems;
        private World _world;

        private void Awake()
        {
            _world = World.Create();
            InitWorld();
            _systems = new SystemCollection(_world)
                .Init(GameOfLifeSystems.Init)
                .AddTick(GameOfLifeSystems.UpdateSimulationTrigger)
                .AddTick(GameOfLifeSystems.SimulateLife)
                .AddTick(GameOfLifeSystems.UpdateCameraPosition)
                .AddTick(GameOfLifeSystems.RenderLife)
                .AddTick(GameOfLifeSystems.RemoveAll<SimulateGame>);
        }

        private void InitWorld()
        {
            _world.Create(new GlobalEntity(), new GameData(Config));
        }
        
        private void Start()
        {
            _systems.Init();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _world.Clear();
                InitWorld();
                _systems.Init();
                return;
            }
            
            _systems.Tick(Time.deltaTime);
        }
    }
}