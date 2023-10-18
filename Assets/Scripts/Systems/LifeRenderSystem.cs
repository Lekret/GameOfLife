using Arch.Core;
using Components;
using Config;
using Core;
using UnityEngine;
using View;

namespace Systems
{
    public class LifeRenderSystem : BaseSystem
    {
        private readonly QueryDescription _query = new QueryDescription().WithAll<Cell, Position>();
        private readonly GraphicsEngine _graphicsEngine = new();
        private readonly GameConfig _config;

        public LifeRenderSystem(World world, GameConfig config) : base(world)
        {
            _config = config;
        }

        public override void Update(in float deltaTime)
        {
            _graphicsEngine.Clear();
            
            foreach (var chunk in World.Query(_query))
            foreach (var entityId in chunk)
            {
                var position = chunk.Get<Position>(entityId);
                var x = position.X;
                var y = position.Y;

                var isLife = chunk.Get<IsLife>(entityId).Value;
                var material = isLife ? _config.LifeMaterial : _config.DeathMaterial;
                var drawPos = _config.DrawOrigin + new Vector3(
                    x + _config.DrawWidthSpacing * x,
                    y + _config.DrawHeightSpacing * y);
                _graphicsEngine.DrawMesh(drawPos, _config.CellMesh, material);
            }
        }
    }
}