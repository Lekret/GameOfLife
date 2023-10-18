using Arch.Core;
using Arch.Core.Extensions;
using Arch.Core.Utils;
using Arch.Relationships;
using UnityEngine;

namespace TransformHierarchy
{
    public struct Transform
    {
        public Matrix4x4 Value;
    }

    public struct ParentOf
    {
    }

    public struct ChildOf
    {
        public EntityReference Value;
    }

    public static class TransformHierarchyExtensions
    {
        public static void SetParent(this Entity child, Entity parent)
        {
            ref var childOf = ref child.AddOrGet<ChildOf>();
            if (childOf.Value.IsAlive())
            {
                childOf.Value.Entity.RemoveRelationship<ParentOf>(child);
            }

            childOf.Value = parent.Reference();
            parent.AddRelationship<ParentOf>(child);
        }

        public static void RemoveParent(this Entity child)
        {
            if (child.TryGet(out ChildOf childOf) && childOf.Value.IsAlive())
            {
                childOf.Value.Entity.RemoveRelationship<ParentOf>(child);
                child.Remove<ChildOf>();
            }
        }

        public static Entity GetParent(this Entity target)
        {
            if (target.TryGet(out ChildOf childOf))
                return childOf.Value;
            return Entity.Null;
        }

        public static Relationship<ParentOf> GetChildren(this Entity target)
        {
            return target.GetRelationships<ParentOf>();
        }
    }

    public class HierarchyTestSystem : BaseSystem
    {
        public HierarchyTestSystem(World world) : base(world)
        {
        }

        public override void Update(in float deltaTime)
        {
            var parent = World.Create(new Transform {Value = Matrix4x4.identity});
            var child1 = World.Create(new Transform {Value = Matrix4x4.identity});
            var child2 = World.Create(new Transform {Value = Matrix4x4.identity});
            child1.SetParent(parent);
            child2.SetParent(parent);

            Debug.Log($"Has child1: {parent.GetChildren().Contains(child1)}");
            Debug.Log($"Has child2: {parent.GetChildren().Contains(child2)}");
            
            child1.RemoveParent();
            
            Debug.Log($"Has child1: {parent.GetChildren().Contains(child1)}");
            Debug.Log($"Has child2: {parent.GetChildren().Contains(child2)}");
         
            child2.RemoveParent();
            
            Debug.Log($"Has child1: {parent.GetChildren().Contains(child1)}");
            Debug.Log($"Has child2: {parent.GetChildren().Contains(child2)}");
        }
    }
}