using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
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
        var gridConfig = SystemAPI.GetSingleton<GridConfigSingleton>();
        int width = gridConfig.HalfWidth * 2;
        int height = gridConfig.HalfHeight * 2;
        float cellSize = Ex3Config.TouchingDistance;

        var plantGrid = new NativeHashMap<int, NativeList<Entity>>(1000, Allocator.Temp);
        var preyGrid = new NativeHashMap<int, NativeList<Entity>>(1000, Allocator.Temp);

        var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);

        foreach (var (lt, entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PlantTag>().WithEntityAccess())
        {
            int2 cell = (int2)math.floor(lt.ValueRO.Position.xy / cellSize);
            if (cell.x < 0 || cell.x >= width || cell.y < 0 || cell.y >= height) continue;
            int flatIndex = cell.x + cell.y * width;

            if (!plantGrid.ContainsKey(flatIndex))
                plantGrid[flatIndex] = new NativeList<Entity>(Allocator.Temp);
            plantGrid[flatIndex].Add(entity);
        }

        foreach (var (lt, entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PreyTag>().WithEntityAccess())
        {
            int2 cell = (int2)math.floor(lt.ValueRO.Position.xy / cellSize);
            if (cell.x < 0 || cell.x >= width || cell.y < 0 || cell.y >= height) continue;
            int flatIndex = cell.x + cell.y * width;

            if (!preyGrid.ContainsKey(flatIndex))
                preyGrid[flatIndex] = new NativeList<Entity>(Allocator.Temp);
            preyGrid[flatIndex].Add(entity);
        }

        float preySpeed = Ex3Config.PreySpeed;
        float predatorSpeed = Ex3Config.PredatorSpeed;

        foreach (var (vel, lt, entity) in SystemAPI.Query<RefRW<VelocityData>, RefRO<LocalTransform>>().WithAll<PreyTag>().WithEntityAccess())
        {
            float3 pos = lt.ValueRO.Position;
            float minDistSq = float.MaxValue;
            float3 target = pos;

            int2 cell = (int2)math.floor(pos.xy / cellSize);

            for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                int2 neighbor = cell + new int2(dx, dy);
                if (neighbor.x < 0 || neighbor.x >= width || neighbor.y < 0 || neighbor.y >= height) continue;
                int flatIndex = neighbor.x + neighbor.y * width;

                if (plantGrid.TryGetValue(flatIndex, out var list))
                {
                    for (int i = 0; i < list.Length; i++)
                    {
                        var other = list[i];
                        float3 otherPos = transforms[other].Position;
                        float distSq = math.distancesq(pos, otherPos);
                        if (distSq < minDistSq)
                        {
                            minDistSq = distSq;
                            target = otherPos;
                        }
                    }
                }
            }

            vel.ValueRW.Value = (target - pos) * preySpeed;
        }

        foreach (var (vel, lt, entity) in SystemAPI.Query<RefRW<VelocityData>, RefRO<LocalTransform>>().WithAll<PredatorTag>().WithEntityAccess())
        {
            float3 pos = lt.ValueRO.Position;
            float minDistSq = float.MaxValue;
            float3 target = pos;

            int2 cell = (int2)math.floor(pos.xy / cellSize);

            for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                int2 neighbor = cell + new int2(dx, dy);
                if (neighbor.x < 0 || neighbor.x >= width || neighbor.y < 0 || neighbor.y >= height) continue;
                int flatIndex = neighbor.x + neighbor.y * width;

                if (preyGrid.TryGetValue(flatIndex, out var list))
                {
                    for (int i = 0; i < list.Length; i++)
                    {
                        var other = list[i];
                        float3 otherPos = transforms[other].Position;
                        float distSq = math.distancesq(pos, otherPos);
                        if (distSq < minDistSq)
                        {
                            minDistSq = distSq;
                            target = otherPos;
                        }
                    }
                }
            }

            vel.ValueRW.Value = (target - pos) * predatorSpeed;
        }

        foreach (var kvp in plantGrid) kvp.Value.Dispose();
        foreach (var kvp in preyGrid) kvp.Value.Dispose();
        plantGrid.Dispose();
        preyGrid.Dispose();
    }
}