using Arch.Core;
using Arch.System;
using Components;
using UnityEngine;

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
            World.Query(_query, (ref LifeGrid lifeGrid) =>
            {
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
            });
        }
    }
}