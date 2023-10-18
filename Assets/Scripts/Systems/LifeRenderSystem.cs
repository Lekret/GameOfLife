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
        private readonly QueryDescription _query = new QueryDescription().WithAll<LifeGrid>();
        private readonly GraphicsEngine _graphicsEngine = new();
        private readonly GameConfig _config;

        public LifeRenderSystem(World world, GameConfig config) : base(world)
        {
            _config = config;
        }

        public override void Update(in float deltaTime)
        {
            foreach (var chunk in World.Query(_query))
            foreach (var entityId in chunk)
            {
                var lifeGrid = chunk.Get<LifeGrid>(entityId);
                var width = lifeGrid.GetWidth();
                var height = lifeGrid.GetHeight();

                _graphicsEngine.Clear();
                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        var alive = lifeGrid.IsAlive(x, y);
                        var material = alive ? _config.AliveMaterial : _config.DeadMaterial;
                        var drawPos = _config.DrawOrigin + new Vector3(
                            x + _config.DrawWidthSpacing * x,
                            y + _config.DrawHeightSpacing * y);
                        _graphicsEngine.DrawMesh(drawPos, _config.CellMesh, material);
                    }
                }
            }
        }
    }
}