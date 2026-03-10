using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class LifetimeManager : MonoBehaviour
{
    NativeArray<Vector3> plantPositions;
    NativeArray<Vector3> preyPositions;
    NativeArray<Vector3> predatorPositions;

    NativeArray<float> plantFactors;
    NativeArray<float> preyFactors;
    NativeArray<float> predatorFactors;

    NativeArray<bool> preyReproduced;
    NativeArray<bool> predatorReproduced;

    void Start()
    {
        int plantCount = Ex4Spawner.PlantTransforms.Length;
        int preyCount = Ex4Spawner.PreyTransforms.Length;
        int predatorCount = Ex4Spawner.PredatorTransforms.Length;

        plantPositions = new NativeArray<Vector3>(plantCount, Allocator.Persistent);
        preyPositions = new NativeArray<Vector3>(preyCount, Allocator.Persistent);
        predatorPositions = new NativeArray<Vector3>(predatorCount, Allocator.Persistent);

        plantFactors = new NativeArray<float>(plantCount, Allocator.Persistent);
        preyFactors = new NativeArray<float>(preyCount, Allocator.Persistent);
        predatorFactors = new NativeArray<float>(predatorCount, Allocator.Persistent);

        preyReproduced = new NativeArray<bool>(preyCount, Allocator.Persistent);
        predatorReproduced = new NativeArray<bool>(predatorCount, Allocator.Persistent);
    }

    void Update()
    {
        if (Ex4Spawner.Instance == null) return;

        int plantCount = Ex4Spawner.PlantTransforms.Length;
        int preyCount = Ex4Spawner.PreyTransforms.Length;
        int predatorCount = Ex4Spawner.PredatorTransforms.Length;

        for (int i = 0; i < plantCount; i++)
            plantPositions[i] = Ex4Spawner.PlantTransforms[i].position;

        for (int i = 0; i < preyCount; i++)
            preyPositions[i] = Ex4Spawner.PreyTransforms[i].position;

        for (int i = 0; i < predatorCount; i++)
            predatorPositions[i] = Ex4Spawner.PredatorTransforms[i].position;

        var job = new LifetimeJob
        {
            plantPositions = plantPositions,
            preyPositions = preyPositions,
            predatorPositions = predatorPositions,

            plantFactors = plantFactors,
            preyFactors = preyFactors,
            predatorFactors = predatorFactors,

            preyReproduced = preyReproduced,
            predatorReproduced = predatorReproduced,

            touchingDistance = Ex3Config.TouchingDistance
        };

        int maxCount = Mathf.Max(plantCount, preyCount, predatorCount);

        JobHandle handle = job.Schedule(maxCount, 32);
        handle.Complete();

        for (int i = 0; i < plantCount; i++)
            Ex4Spawner.PlantLifetimes[i].decreasingFactor = plantFactors[i];

        for (int i = 0; i < preyCount; i++)
        {
            var life = Ex4Spawner.PreyLifetimes[i];
            life.decreasingFactor = preyFactors[i];
            life.reproduced = preyReproduced[i];
        }

        for (int i = 0; i < predatorCount; i++)
        {
            var life = Ex4Spawner.PredatorLifetimes[i];
            life.decreasingFactor = predatorFactors[i];
            life.reproduced = predatorReproduced[i];
        }
    }

    void OnDestroy()
    {
        if (plantPositions.IsCreated) plantPositions.Dispose();
        if (preyPositions.IsCreated) preyPositions.Dispose();
        if (predatorPositions.IsCreated) predatorPositions.Dispose();

        if (plantFactors.IsCreated) plantFactors.Dispose();
        if (preyFactors.IsCreated) preyFactors.Dispose();
        if (predatorFactors.IsCreated) predatorFactors.Dispose();

        if (preyReproduced.IsCreated) preyReproduced.Dispose();
        if (predatorReproduced.IsCreated) predatorReproduced.Dispose();
    }
}

[BurstCompile]
public struct LifetimeJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<Vector3> plantPositions;
    [ReadOnly] public NativeArray<Vector3> preyPositions;
    [ReadOnly] public NativeArray<Vector3> predatorPositions;

    public NativeArray<float> plantFactors;
    public NativeArray<float> preyFactors;
    public NativeArray<float> predatorFactors;

    public NativeArray<bool> preyReproduced;
    public NativeArray<bool> predatorReproduced;

    public float touchingDistance;

    public void Execute(int index)
    {
        float dist;

        if (index < plantPositions.Length)
        {
            float factor = 1f;
            Vector3 plant = plantPositions[index];

            for (int i = 0; i < preyPositions.Length; i++)
            {
                dist = Vector3.Distance(preyPositions[i], plant);
                if (dist < touchingDistance)
                {
                    factor *= 2f;
                    break;
                }
            }

            plantFactors[index] = factor;
        }

        if (index < preyPositions.Length)
        {
            float factor = 1f;
            bool reproduced = false;
            Vector3 prey = preyPositions[index];

            for (int i = 0; i < plantPositions.Length; i++)
            {
                if (Vector3.Distance(plantPositions[i], prey) < touchingDistance)
                {
                    factor /= 2f;
                    break;
                }
            }

            for (int i = 0; i < predatorPositions.Length; i++)
            {
                if (Vector3.Distance(predatorPositions[i], prey) < touchingDistance)
                {
                    factor *= 2f;
                    break;
                }
            }

            for (int i = 0; i < preyPositions.Length; i++)
            {
                if (i == index) continue;

                if (Vector3.Distance(preyPositions[i], prey) < touchingDistance)
                {
                    reproduced = true;
                    break;
                }
            }

            preyFactors[index] = factor;
            preyReproduced[index] = reproduced;
        }

        if (index < predatorPositions.Length)
        {
            float factor = 1f;
            bool reproduced = false;
            Vector3 predator = predatorPositions[index];

            for (int i = 0; i < predatorPositions.Length; i++)
            {
                if (i == index) continue;

                if (Vector3.Distance(predatorPositions[i], predator) < touchingDistance)
                {
                    reproduced = true;
                    break;
                }
            }

            for (int i = 0; i < preyPositions.Length; i++)
            {
                if (Vector3.Distance(preyPositions[i], predator) < touchingDistance)
                {
                    factor /= 2f;
                }
            }

            predatorFactors[index] = factor;
            predatorReproduced[index] = reproduced;
        }
    }
}