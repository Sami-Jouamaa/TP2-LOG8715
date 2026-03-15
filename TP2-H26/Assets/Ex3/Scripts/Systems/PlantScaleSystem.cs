using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(LifetimeSystem))]
public partial struct PlantScaleSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (lt, lifetime) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<LifetimeData>>().WithAll<PlantTag>())
        {
            float progression = lifetime.ValueRO.CurrentLifetime / lifetime.ValueRO.StartingLifetime;
            lt.ValueRW.Scale = progression;
        }
    }
}