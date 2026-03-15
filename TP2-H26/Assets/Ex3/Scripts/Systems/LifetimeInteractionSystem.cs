using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(LifetimeSystem))]
[UpdateBefore(typeof(MoveSystem))]
public partial struct LifetimeInteractionSystem : ISystem
{
    private EntityQuery _plantQuery;
    private EntityQuery _preyQuery;
    private EntityQuery _predatorQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridConfigSingleton>();

        _plantQuery = SystemAPI.QueryBuilder().WithAll<PlantTag, LocalTransform, LifetimeData>().Build();
        _preyQuery = SystemAPI.QueryBuilder().WithAll<PreyTag, LocalTransform, LifetimeData>().Build();
        _predatorQuery = SystemAPI.QueryBuilder().WithAll<PredatorTag, LocalTransform, LifetimeData>().Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var plantTransforms = _plantQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        var preyTransforms = _preyQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        var predatorTransforms = _predatorQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        var plantPositions = new NativeArray<float3>(plantTransforms.Length, Allocator.TempJob);
        var preyPositions = new NativeArray<float3>(preyTransforms.Length, Allocator.TempJob);
        var predatorPositions = new NativeArray<float3>(predatorTransforms.Length, Allocator.TempJob);

        for (int i = 0; i < plantTransforms.Length; i++)
            plantPositions[i] = plantTransforms[i].Position;

        for (int i = 0; i < preyTransforms.Length; i++)
            preyPositions[i] = preyTransforms[i].Position;

        for (int i = 0; i < predatorTransforms.Length; i++)
            predatorPositions[i] = predatorTransforms[i].Position;

        float touchDistSq = Ex3Config.TouchingDistance * Ex3Config.TouchingDistance;

        var plantJob = new PlantLifetimeJob
        {
            PreyPositions = preyPositions,
            TouchDistSq = touchDistSq
        };

        var preyJob = new PreyLifetimeJob
        {
            PlantPositions = plantPositions,
            PreyPositions = preyPositions,
            PredatorPositions = predatorPositions,
            TouchDistSq = touchDistSq
        };

        var predatorJob = new PredatorLifetimeJob
        {
            PreyPositions = preyPositions,
            PredatorPositions = predatorPositions,
            TouchDistSq = touchDistSq
        };

        JobHandle plantHandle = plantJob.ScheduleParallel(state.Dependency);
        JobHandle preyHandle = preyJob.ScheduleParallel(plantHandle);
        JobHandle predatorHandle = predatorJob.ScheduleParallel(preyHandle);

        predatorHandle.Complete();

        plantTransforms.Dispose();
        preyTransforms.Dispose();
        predatorTransforms.Dispose();

        plantPositions.Dispose();
        preyPositions.Dispose();
        predatorPositions.Dispose();
    }

    [BurstCompile]
    [WithAll(typeof(PlantTag))]
    public partial struct PlantLifetimeJob : IJobEntity
    {
        [ReadOnly] public NativeArray<float3> PreyPositions;
        public float TouchDistSq;

        public void Execute(ref LifetimeData lifetime, in LocalTransform transform)
        {
            float factor = 1f;
            float3 pos = transform.Position;

            for (int i = 0; i < PreyPositions.Length; i++)
            {
                if (math.distancesq(pos, PreyPositions[i]) < TouchDistSq)
                {
                    factor *= 2f;
                    break;
                }
            }

            lifetime.DecreasingFactor = factor;
        }
    }

    [BurstCompile]
    [WithAll(typeof(PreyTag))]
    public partial struct PreyLifetimeJob : IJobEntity
    {
        [ReadOnly] public NativeArray<float3> PlantPositions;
        [ReadOnly] public NativeArray<float3> PreyPositions;
        [ReadOnly] public NativeArray<float3> PredatorPositions;
        public float TouchDistSq;

        public void Execute(ref LifetimeData lifetime, in LocalTransform transform, [EntityIndexInQuery] int entityIndex)
        {
            float factor = 1f;
            bool reproduced = false;
            float3 pos = transform.Position;

            for (int i = 0; i < PlantPositions.Length; i++)
            {
                if (math.distancesq(pos, PlantPositions[i]) < TouchDistSq)
                {
                    factor /= 2f;
                    break;
                }
            }

            for (int i = 0; i < PredatorPositions.Length; i++)
            {
                if (math.distancesq(pos, PredatorPositions[i]) < TouchDistSq)
                {
                    factor *= 2f;
                    break;
                }
            }

            for (int i = 0; i < PreyPositions.Length; i++)
            {
                if (i == entityIndex) continue;

                if (math.distancesq(pos, PreyPositions[i]) < TouchDistSq)
                {
                    reproduced = true;
                    break;
                }
            }

            lifetime.DecreasingFactor = factor;
            lifetime.Reproduced = reproduced;
        }
    }

    [BurstCompile]
    [WithAll(typeof(PredatorTag))]
    public partial struct PredatorLifetimeJob : IJobEntity
    {
        [ReadOnly] public NativeArray<float3> PreyPositions;
        [ReadOnly] public NativeArray<float3> PredatorPositions;
        public float TouchDistSq;

        public void Execute(ref LifetimeData lifetime, in LocalTransform transform, [EntityIndexInQuery] int entityIndex)
        {
            float factor = 1f;
            bool reproduced = false;
            float3 pos = transform.Position;

            for (int i = 0; i < PredatorPositions.Length; i++)
            {
                if (i == entityIndex) continue;

                if (math.distancesq(pos, PredatorPositions[i]) < TouchDistSq)
                {
                    reproduced = true;
                    break;
                }
            }

            for (int i = 0; i < PreyPositions.Length; i++)
            {
                if (math.distancesq(pos, PreyPositions[i]) < TouchDistSq)
                {
                    factor /= 2f;
                }
            }

            lifetime.DecreasingFactor = factor;
            lifetime.Reproduced = reproduced;
        }
    }
}