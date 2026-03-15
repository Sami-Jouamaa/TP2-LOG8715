using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(LifetimeSystem))]
[UpdateBefore(typeof(MoveSystem))]
public partial struct LifetimeInteractionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridConfigSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Gather positions into NativeArrays
        var plantQuery = SystemAPI.QueryBuilder().WithAll<PlantTag, LocalTransform>().Build();
        var preyQuery = SystemAPI.QueryBuilder().WithAll<PreyTag, LocalTransform>().Build();
        var predatorQuery = SystemAPI.QueryBuilder().WithAll<PredatorTag, LocalTransform>().Build();

        var plantPositions = new NativeArray<float3>(plantQuery.CalculateEntityCount(), Allocator.TempJob);
        var preyPositions = new NativeArray<float3>(preyQuery.CalculateEntityCount(), Allocator.TempJob);
        var predatorPositions = new NativeArray<float3>(predatorQuery.CalculateEntityCount(), Allocator.TempJob);

        int idx = 0;
        foreach (var lt in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PlantTag>())
            plantPositions[idx++] = lt.ValueRO.Position;

        idx = 0;
        foreach (var lt in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PreyTag>())
            preyPositions[idx++] = lt.ValueRO.Position;

        idx = 0;
        foreach (var lt in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PredatorTag>())
            predatorPositions[idx++] = lt.ValueRO.Position;

        float touchDist = Ex3Config.TouchingDistance;

        // Update plant lifetimes
        foreach (var (lifetime, lt) in SystemAPI.Query<RefRW<LifetimeData>, RefRO<LocalTransform>>().WithAll<PlantTag>())
        {
            float factor = 1f;
            float3 pos = lt.ValueRO.Position;
            for (int i = 0; i < preyPositions.Length; i++)
            {
                if (math.distance(preyPositions[i], pos) < touchDist)
                {
                    factor *= 2f;
                    break;
                }
            }
            lifetime.ValueRW.DecreasingFactor = factor;
        }

        // Update prey lifetimes
        int preyIdx = 0;
        foreach (var (lifetime, lt) in SystemAPI.Query<RefRW<LifetimeData>, RefRO<LocalTransform>>().WithAll<PreyTag>())
        {
            float factor = 1f;
            bool reproduced = false;
            float3 pos = lt.ValueRO.Position;

            for (int i = 0; i < plantPositions.Length; i++)
            {
                if (math.distance(plantPositions[i], pos) < touchDist)
                {
                    factor /= 2f;
                    break;
                }
            }

            for (int i = 0; i < predatorPositions.Length; i++)
            {
                if (math.distance(predatorPositions[i], pos) < touchDist)
                {
                    factor *= 2f;
                    break;
                }
            }

            for (int i = 0; i < preyPositions.Length; i++)
            {
                if (i == preyIdx) { preyIdx++; continue; }
                if (math.distance(preyPositions[i], pos) < touchDist)
                {
                    reproduced = true;
                    break;
                }
            }

            lifetime.ValueRW.DecreasingFactor = factor;
            lifetime.ValueRW.Reproduced = reproduced;
            preyIdx++;
        }

        // Update predator lifetimes
        int predIdx = 0;
        foreach (var (lifetime, lt) in SystemAPI.Query<RefRW<LifetimeData>, RefRO<LocalTransform>>().WithAll<PredatorTag>())
        {
            float factor = 1f;
            bool reproduced = false;
            float3 pos = lt.ValueRO.Position;

            for (int i = 0; i < predatorPositions.Length; i++)
            {
                if (i == predIdx) continue;
                if (math.distance(predatorPositions[i], pos) < touchDist)
                {
                    reproduced = true;
                    break;
                }
            }

            for (int i = 0; i < preyPositions.Length; i++)
            {
                if (math.distance(preyPositions[i], pos) < touchDist)
                {
                    factor /= 2f;
                }
            }

            lifetime.ValueRW.DecreasingFactor = factor;
            lifetime.ValueRW.Reproduced = reproduced;
            predIdx++;
        }

        plantPositions.Dispose();
        preyPositions.Dispose();
        predatorPositions.Dispose();
    }
}