using Arch.Core;
using Arch.Core.Extensions;
using Components;
using Config;
using Core;
using UnityEngine;

namespace Systems
{
    public class LifeRenderSystem : BaseSystem
    {
        private readonly QueryDescription _query = new QueryDescription().WithAll<Cell, IsLife, Renderable>();
        private readonly GameConfig _config;
        private readonly Transform _parent;

        public LifeRenderSystem(World world, GameConfig config) : base(world)
        {
            _config = config;
            _parent = new GameObject("Graphics").transform;
        }

        public override void Initialize()
        {
            var query = new QueryDescription().WithAll<Cell, Position>().WithNone<Renderable>();
            World.Query(query, (Entity entity, ref Position position) =>
            {
                var cellObj = new GameObject("Cell");
                var renderable = new Renderable
                {
                    Renderer = cellObj.AddComponent<MeshRenderer>(),
                    Filter = cellObj.AddComponent<MeshFilter>()
                };
                cellObj.transform.SetParent(_parent);
                cellObj.transform.position = _config.DrawOrigin + new Vector3(
                    position.X + _config.DrawWidthSpacing * position.X,
                    position.Y + _config.DrawHeightSpacing * position.Y);
                entity.Add(renderable);
            });
        }

        public override void Update(in float deltaTime)
        {
            foreach (var chunk in World.Query(_query))
            {
                var renderableArr = chunk.GetArray<Renderable>();
                var isLifeArr = chunk.GetArray<IsLife>();

                for (int entityIdx = 0, chunkSize = chunk.Size; entityIdx < chunkSize; entityIdx++)
                {
                    var renderable = renderableArr[entityIdx];
                    var isLife = isLifeArr[entityIdx].Value;
                    var material = isLife ? _config.LifeMaterial : _config.DeathMaterial;
                    renderable.Renderer.material = material;
                    renderable.Filter.mesh = _config.CellMesh;
                }
            }
        }

        public override void Dispose()
        {
            var query = new QueryDescription().WithAll<Renderable>();
            World.Query(query, (ref Renderable renderable) =>
            {
                Object.Destroy(renderable.Renderer.gameObject);
            });
        }
    }
}