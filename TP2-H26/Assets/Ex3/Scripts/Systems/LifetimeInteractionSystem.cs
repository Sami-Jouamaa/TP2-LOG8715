using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(LifetimeSystem))]
[UpdateBefore(typeof(MoveSystem))]
public partial struct LifetimeInteractionSystem : ISystem
{
    private EntityQuery allQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridConfigSingleton>();
        allQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform>().Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gridConfig = SystemAPI.GetSingleton<GridConfigSingleton>();
        int width = gridConfig.HalfWidth * 2;
        int height = gridConfig.HalfHeight * 2;
        float cellSize = Ex3Config.TouchingDistance;

        float touchDistSq = Ex3Config.TouchingDistance * Ex3Config.TouchingDistance;

        var grid = new NativeHashMap<int, NativeList<Entity>>(allQuery.CalculateEntityCount(), Allocator.Temp);

        var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);

        foreach (var (lt, entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithEntityAccess())
        {
            int2 cell = (int2)math.floor(lt.ValueRO.Position.xy / cellSize);
            if (cell.x < 0 || cell.x >= width || cell.y < 0 || cell.y >= height) continue;

            int flatIndex = cell.x + cell.y * width;

            if (!grid.ContainsKey(flatIndex))
                grid[flatIndex] = new NativeList<Entity>(Allocator.Temp);

            grid[flatIndex].Add(entity);
        }
        foreach (var (lt, lifetime, entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<LifetimeData>>().WithEntityAccess())
        {
            float3 pos = lt.ValueRO.Position;
            float factor = 1f;
            bool reproduced = false;

            int2 cell = (int2)math.floor(pos.xy / cellSize);

            for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                int2 neighbor = cell + new int2(x, y);
                if (neighbor.x < 0 || neighbor.x >= width || neighbor.y < 0 || neighbor.y >= height) continue;

                int flatIndex = neighbor.x + neighbor.y * width;

                if (grid.TryGetValue(flatIndex, out var list))
                {
                    for (int i = 0; i < list.Length; i++)
                    {
                        var other = list[i];
                        if (other == entity) continue;

                        float3 otherPos = transforms[other].Position;
                        if (math.distancesq(pos, otherPos) < touchDistSq)
                        {
                            bool isPlant = SystemAPI.HasComponent<PlantTag>(other);
                            bool isPrey = SystemAPI.HasComponent<PreyTag>(other);
                            bool isPredator = SystemAPI.HasComponent<PredatorTag>(other);

                            if (SystemAPI.HasComponent<PlantTag>(entity) && isPrey) factor *= 2f;
                            if (SystemAPI.HasComponent<PreyTag>(entity))
                            {
                                if (isPlant) factor /= 2f;
                                if (isPredator) factor *= 2f;
                                if (isPrey) reproduced = true;
                            }
                            if (SystemAPI.HasComponent<PredatorTag>(entity))
                            {
                                if (isPrey) factor /= 2f;
                                if (isPredator) reproduced = true;
                            }
                        }
                    }
                }
            }

            lifetime.ValueRW.DecreasingFactor = factor;
            lifetime.ValueRW.Reproduced = reproduced;
        }
        foreach (var kvp in grid)
            kvp.Value.Dispose();

        grid.Dispose();
    }
}