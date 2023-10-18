using Arch.Core;
using Components;
using Config;
using Core;
using UnityEngine;

namespace Systems
{
    public class CameraUpdateSystem : BaseSystem
    {
        private readonly QueryDescription _query = new QueryDescription().WithAll<LifeGrid>();
        private readonly GameConfig _config;
        private readonly Camera _camera;

        public CameraUpdateSystem(World world, GameConfig config) : base(world)
        {
            _config = config;
            _camera = Camera.main;
        }

        public override void Update(in float deltaTime)
        {
            foreach (var chunk in World.Query(_query))
            foreach (var entityId in chunk)
            {
                var lifeGrid = chunk.Get<LifeGrid>(entityId);
                var width = lifeGrid.GetWidth();
                var height = lifeGrid.GetHeight();
                var cameraPosition = new Vector3
                (
                    width / 2f + _config.DrawWidthSpacing * (width / 2f),
                    height / 2f + _config.DrawHeightSpacing * (height / 2f),
                    -_config.CameraDistance
                );
                _camera.transform.position = cameraPosition;
            }
        }
    }
}