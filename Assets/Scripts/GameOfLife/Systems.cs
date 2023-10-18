using System.Collections.Generic;
using Arch.Core;

namespace GameOfLife
{
    public delegate void SystemDelegate(World world);

    public delegate void TickSystemDelegate(World world, float deltaTime);

    public class SystemCollection
    {
        private readonly World _world;
        private readonly List<SystemDelegate> _initSystems = new();
        private readonly List<TickSystemDelegate> _tickSystems = new();

        public SystemCollection(World world)
        {
            _world = world;
        }

        public SystemCollection Init(SystemDelegate initSystem)
        {
            _initSystems.Add(initSystem);
            return this;
        }

        public SystemCollection AddTick(TickSystemDelegate initSystem)
        {
            _tickSystems.Add(initSystem);
            return this;
        }

        public void Init()
        {
            for (var i = 0; i < _initSystems.Count; i++)
            {
                _initSystems[i](_world);
            }
        }

        public void Tick(float deltaTime)
        {
            for (var i = 0; i < _tickSystems.Count; i++)
            {
                _tickSystems[i](_world, deltaTime);
            }
        }
    }
}