using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(LifetimeSystem))]
public partial struct VelocitySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new VelocityJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        state.Dependency = job.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    public partial struct VelocityJob : IJobEntity
    {
        public float DeltaTime;

        public void Execute(ref LocalTransform transform, in VelocityData velocity)
        {
            transform.Position += velocity.Value * DeltaTime;
        }
    }
}