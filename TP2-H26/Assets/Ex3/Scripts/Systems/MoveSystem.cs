using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(VelocitySystem))]
public partial struct MoveSystem : ISystem
{
    private EntityQuery _plantQuery;
    private EntityQuery _preyQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridConfigSingleton>();

        _plantQuery = SystemAPI.QueryBuilder().WithAll<PlantTag, LocalTransform>().Build();
        _preyQuery = SystemAPI.QueryBuilder().WithAll<PreyTag, LocalTransform>().Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var plantTransforms = _plantQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        var preyTransforms = _preyQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        var plantPositions = new NativeArray<float3>(plantTransforms.Length, Allocator.TempJob);
        var preyPositions = new NativeArray<float3>(preyTransforms.Length, Allocator.TempJob);

        for (int i = 0; i < plantTransforms.Length; i++)
            plantPositions[i] = plantTransforms[i].Position;

        for (int i = 0; i < preyTransforms.Length; i++)
            preyPositions[i] = preyTransforms[i].Position;

        var preyJob = new MovePreyJob
        {
            PlantPositions = plantPositions,
            Speed = Ex3Config.PreySpeed
        };

        var predatorJob = new MovePredatorJob
        {
            PreyPositions = preyPositions,
            Speed = Ex3Config.PredatorSpeed
        };

        JobHandle preyHandle = preyJob.ScheduleParallel(state.Dependency);
        JobHandle predatorHandle = predatorJob.ScheduleParallel(preyHandle);
        predatorHandle.Complete();

        plantTransforms.Dispose();
        preyTransforms.Dispose();
        plantPositions.Dispose();
        preyPositions.Dispose();
    }

    [BurstCompile]
    [WithAll(typeof(PreyTag))]
    public partial struct MovePreyJob : IJobEntity
    {
        [ReadOnly] public NativeArray<float3> PlantPositions;
        public float Speed;

        public void Execute(ref VelocityData velocity, in LocalTransform transform)
        {
            float3 pos = transform.Position;
            float minDistSq = float.MaxValue;
            float3 target = pos;

            for (int i = 0; i < PlantPositions.Length; i++)
            {
                float distSq = math.distancesq(pos, PlantPositions[i]);
                if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                    target = PlantPositions[i];
                }
            }

            velocity.Value = (target - pos) * Speed;
        }
    }

    [BurstCompile]
    [WithAll(typeof(PredatorTag))]
    public partial struct MovePredatorJob : IJobEntity
    {
        [ReadOnly] public NativeArray<float3> PreyPositions;
        public float Speed;

        public void Execute(ref VelocityData velocity, in LocalTransform transform)
        {
            float3 pos = transform.Position;
            float minDistSq = float.MaxValue;
            float3 target = pos;

            for (int i = 0; i < PreyPositions.Length; i++)
            {
                float distSq = math.distancesq(pos, PreyPositions[i]);
                if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                    target = PreyPositions[i];
                }
            }

            velocity.Value = (target - pos) * Speed;
        }
    }
}