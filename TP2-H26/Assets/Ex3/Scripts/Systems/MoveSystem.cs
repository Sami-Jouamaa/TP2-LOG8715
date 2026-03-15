using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(VelocitySystem))]
public partial struct MoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridConfigSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Gather plant positions
        var plantQuery = SystemAPI.QueryBuilder().WithAll<PlantTag, LocalTransform>().Build();
        var preyQuery = SystemAPI.QueryBuilder().WithAll<PreyTag, LocalTransform>().Build();

        var plantPositions = new NativeArray<float3>(plantQuery.CalculateEntityCount(), Allocator.TempJob);
        var preyPositions = new NativeArray<float3>(preyQuery.CalculateEntityCount(), Allocator.TempJob);

        int idx = 0;
        foreach (var lt in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PlantTag>())
            plantPositions[idx++] = lt.ValueRO.Position;

        idx = 0;
        foreach (var lt in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PreyTag>())
            preyPositions[idx++] = lt.ValueRO.Position;

        float preySpeed = Ex3Config.PreySpeed;
        float predatorSpeed = Ex3Config.PredatorSpeed;

        // Prey moves toward closest plant
        foreach (var (vel, lt) in SystemAPI.Query<RefRW<VelocityData>, RefRO<LocalTransform>>().WithAll<PreyTag>())
        {
            float3 pos = lt.ValueRO.Position;
            float minDist = float.MaxValue;
            float3 closest = pos;

            for (int i = 0; i < plantPositions.Length; i++)
            {
                float d = math.distance(pos, plantPositions[i]);
                if (d < minDist)
                {
                    minDist = d;
                    closest = plantPositions[i];
                }
            }

            vel.ValueRW.Value = (closest - pos) * preySpeed;
        }

        // Predator moves toward closest prey
        foreach (var (vel, lt) in SystemAPI.Query<RefRW<VelocityData>, RefRO<LocalTransform>>().WithAll<PredatorTag>())
        {
            float3 pos = lt.ValueRO.Position;
            float minDist = float.MaxValue;
            float3 closest = pos;

            for (int i = 0; i < preyPositions.Length; i++)
            {
                float d = math.distance(pos, preyPositions[i]);
                if (d < minDist)
                {
                    minDist = d;
                    closest = preyPositions[i];
                }
            }

            vel.ValueRW.Value = (closest - pos) * predatorSpeed;
        }

        plantPositions.Dispose();
        preyPositions.Dispose();
    }
}