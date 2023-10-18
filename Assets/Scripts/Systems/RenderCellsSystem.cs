using Arch.Core;
using Arch.Core.Extensions;
using Components;
using UnityEngine;

namespace Systems
{
    public class RenderLifeSystem : BaseSystem
    {
        private static readonly GraphicsEngine _graphicsEngine = new();

        public RenderLifeSystem(World world) : base(world)
        {
        }

        public override void Update(in float deltaTime)
        {
            var globalEntity = World.GetGlobalEntity();
            var gameData = globalEntity.Get<GameData>();
            var config = gameData.Config;
            var lifeGrid = globalEntity.Get<LifeGrid>();
            var width = lifeGrid.GetWidth();
            var height = lifeGrid.GetHeight();

            _graphicsEngine.Clear();
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var alive = lifeGrid.IsAlive(x, y);
                    var material = alive ? config.AliveMaterial : config.DeadMaterial;
                    var drawPos = config.DrawOrigin + new Vector3(
                        x + config.DrawWidthSpacing * x,
                        y + config.DrawHeightSpacing * y);
                    _graphicsEngine.DrawMesh(drawPos, config.CellMesh, material);
                }
            }
        }
    }
}