using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(LifetimeSystem))]
public partial struct PlantScaleSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new PlantScaleJob();
        state.Dependency = job.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    [WithAll(typeof(PlantTag))]
    public partial struct PlantScaleJob : IJobEntity
    {
        public void Execute(ref LocalTransform transform, in LifetimeData lifetime)
        {
            float progression = lifetime.CurrentLifetime / lifetime.StartingLifetime;
            transform.Scale = progression;
        }
    }
}