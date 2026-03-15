using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(LifetimeSystem))]
public partial struct VelocitySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (lt, vel) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<VelocityData>>())
        {
            lt.ValueRW.Position += vel.ValueRO.Value * dt;
        }
    }
}