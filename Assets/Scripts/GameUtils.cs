using System;
using Arch.Core;
using Arch.Core.Extensions;
using Components;

public static class GameOfLifeUtils
{
    private static readonly QueryDescription GlobalEntityQuery = new QueryDescription().WithAll<GameData, GlobalEntity>();

    public static GameData GetGameData(this World world)
    {
        return world.GetGlobalEntity().Get<GameData>();
    }

    public static Entity GetGlobalEntity(this World world)
    {
        foreach (var chunk in world.Query(GlobalEntityQuery))
        {                
            foreach (var entityId in chunk)
            {
                return chunk.Entity(entityId);
            }
        }

        throw new Exception("Global Entity not found");
    }
}