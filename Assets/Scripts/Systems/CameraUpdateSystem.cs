using Arch.Core;
using Arch.System;
using Components;
using UnityEngine;

namespace Systems
{
    public class CameraUpdateSystem : BaseSystem
    {
        private readonly QueryDescription _query = new QueryDescription().WithAll<LifeGrid>();
        private readonly GameConfig _config;
        
        public CameraUpdateSystem(World world, GameConfig config) : base(world)
        {
            _config = config;
        }

        public override void Update(in float deltaTime)
        {
            World.Query(_query, (ref LifeGrid lifeGrid) =>
            {
                var width = lifeGrid.GetWidth();
                var height = lifeGrid.GetHeight();
                var cameraPosition = new Vector3
                (
                    width / 2f + _config.DrawWidthSpacing * (width / 2f),
                    height / 2f + _config.DrawHeightSpacing * (height / 2f),
                    -_config.CameraDistance
                );
                Camera.main.transform.position = cameraPosition;
            });
        }
    }
}