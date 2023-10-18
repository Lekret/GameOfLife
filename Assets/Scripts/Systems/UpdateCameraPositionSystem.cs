using Arch.Core;
using Arch.Core.Extensions;
using Components;
using UnityEngine;

namespace Systems
{
    public class CameraUpdateSystem : BaseSystem
    {
        public CameraUpdateSystem(World world) : base(world)
        {
        }

        public override void Update(in float deltaTime)
        {
            var globalEntity = World.GetGlobalEntity();
            var lifeGrid = globalEntity.Get<LifeGrid>();
            var width = lifeGrid.GetWidth();
            var height = lifeGrid.GetHeight();

            var gameData = World.GetGameData();
            var config = gameData.Config;
            
            var cameraPosition = new Vector3
            (
                width / 2f + config.DrawWidthSpacing * (width / 2f),
                height / 2f + config.DrawHeightSpacing * (height / 2f),
                -config.CameraDistance
            );
            Camera.main.transform.position = cameraPosition;
        }
    }
}