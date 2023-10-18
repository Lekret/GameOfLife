using System.Collections.Generic;
using UnityEngine;

namespace View
{
    public class GraphicsEngine
    {
        private struct Renderable
        {
            public Transform Transform;
            public MeshRenderer Renderer;
            public MeshFilter Filter;
        }
        
        private readonly Queue<Renderable> _visibleObjects = new();
        private readonly Queue<Renderable> _pooledObjects = new();
        private Transform _parent;

        private Transform GetParent()
        {
            if (_parent == null)
                _parent = new GameObject("Graphics").transform;
            return _parent;
        }
        
        public void Clear()
        {
            while (_visibleObjects.TryDequeue(out var renderable))
            {
                renderable.Renderer.enabled = false;
                _pooledObjects.Enqueue(renderable);
            }
        }
        
        public void DrawMesh(Vector3 position, Mesh mesh, Material material)
        {
            if (!_pooledObjects.TryDequeue(out var renderable))
            {
                var newObj = new GameObject("Cell");
                renderable = new Renderable
                {
                    Transform = newObj.transform,
                    Renderer = newObj.AddComponent<MeshRenderer>(),
                    Filter = newObj.AddComponent<MeshFilter>()
                };
                renderable.Transform.SetParent(GetParent());
            }

            renderable.Transform.position = position;
            renderable.Filter.mesh = mesh;
            renderable.Renderer.material = material;
            renderable.Renderer.enabled = true;
            _visibleObjects.Enqueue(renderable);
        }
    }
}