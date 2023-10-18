using Arch.Core;
using Arch.Core.Extensions;
using Arch.Relationships;
using UnityEngine;

public struct Position
{
    public float X;
    public float Y;
}

public struct Velocity
{
    public float Dx;
    public float Dy;
}

public struct Dead
{
    
}

public struct PlayerTag
{
    
}

public class EcsBoostrap : MonoBehaviour
{
    public static void InitSystem()
    {
        
    }
    
    private void Start()
    {
        var world = World.Create();
        for (var index = 0; index < 1000; index++) 
            world.Create(new Position{ X = 0, Y = 0}, new Velocity{ Dx = 1, Dy = 1});

        var query = new QueryDescription().WithAll<Position, Velocity>();
        world.Query(in query, (ref Position pos, ref Velocity vel) => {
            pos.X += vel.Dx;
            pos.Y += vel.Dy;
        });
        
        world.Query(query, (Entity entity, ref Position pos) =>
        {
            if (entity.Has<Dead, PlayerTag>())
            {
                
            }
        });
    }
}