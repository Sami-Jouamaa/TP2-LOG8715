using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct LifetimeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridConfigSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gridConfig = SystemAPI.GetSingleton<GridConfigSingleton>();

        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var job = new LifetimeJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            HalfWidth = gridConfig.HalfWidth,
            HalfHeight = gridConfig.HalfHeight,
            Seed = gridConfig.RandomSeed,
            ECB = ecb.AsParallelWriter()
        };

        state.Dependency = job.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public partial struct LifetimeJob : IJobEntity
    {
        public float DeltaTime;
        public int HalfWidth;
        public int HalfHeight;
        public uint Seed;
        public EntityCommandBuffer.ParallelWriter ECB;

        public void Execute(
            [EntityIndexInQuery] int entityIndex,
            ref LifetimeData lifetime,
            ref LocalTransform transform,
            Entity entity)
        {
            lifetime.CurrentLifetime -= DeltaTime * lifetime.DecreasingFactor;

            if (lifetime.CurrentLifetime > 0f)
                return;

            if (lifetime.Reproduced || lifetime.AlwaysReproduce)
            {
                var random = Random.CreateFromIndex(Seed + (uint)(entityIndex * 997 + 17));

                float newLife = random.NextFloat(5f, 15f);
                lifetime.StartingLifetime = newLife;
                lifetime.CurrentLifetime = newLife;
                lifetime.Reproduced = false;
                lifetime.DecreasingFactor = 1f;

                transform.Position = new float3(
                    random.NextInt(-HalfWidth, HalfWidth),
                    random.NextInt(-HalfHeight, HalfHeight),
                    0f
                );
            }
            else
            {
                ECB.AddComponent<Disabled>(entityIndex, entity);
            }
        }
    }
}