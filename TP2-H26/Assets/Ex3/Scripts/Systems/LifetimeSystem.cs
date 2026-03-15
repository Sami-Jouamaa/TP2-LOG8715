using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct LifetimeSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridConfigSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var gridConfig = SystemAPI.GetSingleton<GridConfigSingleton>();
        float dt = SystemAPI.Time.DeltaTime;
        var random = Random.CreateFromIndex(gridConfig.RandomSeed ^ (uint)SystemAPI.Time.ElapsedTime.GetHashCode());

        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (lifetime, lt, entity) in SystemAPI.Query<RefRW<LifetimeData>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            lifetime.ValueRW.CurrentLifetime -= dt * lifetime.ValueRO.DecreasingFactor;

            if (lifetime.ValueRO.CurrentLifetime > 0f) continue;

            if (lifetime.ValueRO.Reproduced || lifetime.ValueRO.AlwaysReproduce)
            {
                // Respawn
                float newLife = random.NextFloat(5f, 15f);
                lifetime.ValueRW.StartingLifetime = newLife;
                lifetime.ValueRW.CurrentLifetime = newLife;
                lifetime.ValueRW.Reproduced = false;
                lifetime.ValueRW.DecreasingFactor = 1f;

                lt.ValueRW.Position = new float3(
                    random.NextInt(-gridConfig.HalfWidth, gridConfig.HalfWidth),
                    random.NextInt(-gridConfig.HalfHeight, gridConfig.HalfHeight),
                    0f
                );
            }
            else
            {
                // Deactivate — disable the entity
                ecb.AddComponent<Disabled>(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}