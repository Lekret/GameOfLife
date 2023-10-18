using System;
using Arch.Core;
using Arch.Core.Extensions;
using Components;

public static class EcsUtils
{
    public static bool Any(this World world, in QueryDescription query)
    {
        return world.GetSingle(query) != Entity.Null;
    }

    public static Entity GetSingle(this World world, in QueryDescription query)
    {
        foreach (var chunks in world.Query(query))
        {
            foreach (var entityId in chunks)
            {
                return chunks.Entity(entityId);
            }

            break;
        }

        return Entity.Null;
    }
}