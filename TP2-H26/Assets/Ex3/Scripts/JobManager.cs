using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class JobManager : MonoBehaviour
{
    NativeArray<Vector3> plantPositions;
    NativeArray<Vector3> preyPositions;
    NativeArray<Vector3> predatorPositions;
    NativeArray<Vector3> preyVelocities;
    NativeArray<Vector3> predatorVelocities;

    NativeArray<float> plantFactors;
    NativeArray<float> preyFactors;
    NativeArray<float> predatorFactors;

    NativeArray<bool> preyReproduced;
    NativeArray<bool> predatorReproduced;


    void Start()
    {
        if (Ex4Spawner.PlantTransforms == null)
        {
            Debug.LogError("Spawner not initialized yet.");
            return;
        }
        int plantCount = Ex4Spawner.PlantTransforms.Length;
        int preyCount = Ex4Spawner.PreyTransforms.Length;
        int predatorCount = Ex4Spawner.PredatorTransforms.Length;

        plantPositions = new NativeArray<Vector3>(plantCount, Allocator.Persistent);
        preyPositions = new NativeArray<Vector3>(preyCount, Allocator.Persistent);
        predatorPositions = new NativeArray<Vector3>(predatorCount, Allocator.Persistent);

        preyVelocities = new NativeArray<Vector3>(preyCount, Allocator.Persistent);
        predatorVelocities = new NativeArray<Vector3>(predatorCount, Allocator.Persistent);

        plantFactors = new NativeArray<float>(plantCount, Allocator.Persistent);
        preyFactors = new NativeArray<float>(preyCount, Allocator.Persistent);
        predatorFactors = new NativeArray<float>(predatorCount, Allocator.Persistent);

        preyReproduced = new NativeArray<bool>(preyCount, Allocator.Persistent);
        predatorReproduced = new NativeArray<bool>(predatorCount, Allocator.Persistent);


    }

    void Update()
    {
        if (Ex4Spawner.Instance == null) return;
        if (Ex4Spawner.PlantTransforms == null) return;
        if (Ex4Spawner.PreyTransforms == null) return;
        if (Ex4Spawner.PredatorTransforms == null) return;

        int plantCount = Ex4Spawner.PlantTransforms.Length;
        int preyCount = Ex4Spawner.PreyTransforms.Length;
        int predatorCount = Ex4Spawner.PredatorTransforms.Length;

        for (int i = 0; i < plantCount; i++)
            plantPositions[i] = Ex4Spawner.PlantTransforms[i].position;

        for (int i = 0; i < preyCount; i++)
            preyPositions[i] = Ex4Spawner.PreyTransforms[i].position;

        for (int i = 0; i < predatorCount; i++)
            predatorPositions[i] = Ex4Spawner.PredatorTransforms[i].position;
        
        for (int i = 0; i < preyCount; i++)
            preyVelocities[i] = Ex4Spawner.PreyVelocities[i].velocity;

        for (int i = 0; i < predatorCount; i++)
            predatorVelocities[i] = Ex4Spawner.PredatorVelocities[i].velocity;

        var lifetimeJob = new LifetimeJob
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

        JobHandle lifetimeHandle = lifetimeJob.Schedule(maxCount, 32);
        lifetimeHandle.Complete();

        var moveJob = new MoveJob
        {
            plantPositions = plantPositions,
            preyPositions = preyPositions,
            predatorPositions = predatorPositions,

            preyVelocities = preyVelocities,
            predatorVelocities = predatorVelocities,

            preySpeed = Ex3Config.PreySpeed,
            predatorSpeed = Ex3Config.PredatorSpeed
        };

        JobHandle moveHandle = moveJob.Schedule(maxCount, 32); 
        moveHandle.Complete();

        for (int i = 0; i < plantCount; i++)
            Ex4Spawner.PlantLifetimes[i].decreasingFactor = plantFactors[i];

        for (int i = 0; i < preyCount; i++)
        {
            Ex4Spawner.PreyLifetimes[i].decreasingFactor = preyFactors[i];
            Ex4Spawner.PreyLifetimes[i].reproduced = preyReproduced[i];

            Ex4Spawner.PreyVelocities[i].velocity = preyVelocities[i];
        }

        for (int i = 0; i < predatorCount; i++)
        {
            Ex4Spawner.PredatorLifetimes[i].decreasingFactor = predatorFactors[i];
            Ex4Spawner.PredatorLifetimes[i].reproduced = predatorReproduced[i];

            Ex4Spawner.PredatorVelocities[i].velocity = predatorVelocities[i];
        }
    }

    void OnDestroy()
    {
        if (plantPositions.IsCreated) plantPositions.Dispose();
        if (preyPositions.IsCreated) preyPositions.Dispose();
        if (predatorPositions.IsCreated) predatorPositions.Dispose();

        if (preyVelocities.IsCreated) preyVelocities.Dispose();
        if (predatorVelocities.IsCreated) predatorVelocities.Dispose();

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

[BurstCompile]
struct MoveJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<Vector3> preyPositions;
    [ReadOnly] public NativeArray<Vector3> predatorPositions;
    [ReadOnly] public NativeArray<Vector3> plantPositions;

    public NativeArray<Vector3> preyVelocities;
    public NativeArray<Vector3> predatorVelocities;

    public float preySpeed;
    public float predatorSpeed;

    public void Execute(int index)
    {
        // Move prey toward closest plant
        if (index < preyPositions.Length)
        {
            float minDist = float.MaxValue;
            Vector3 closestPlant = preyPositions[index];
            for (int i = 0; i < plantPositions.Length; i++)
            {
                float d = math.distance(preyPositions[index], plantPositions[i]);
                if (d < minDist)
                {
                    minDist = d;
                    closestPlant = plantPositions[i];
                }
            }
            preyVelocities[index] = (closestPlant - preyPositions[index]) * preySpeed;
        }

        // Move predator toward closest prey
        if (index < predatorPositions.Length)
        {
            float minDist = float.MaxValue;
            Vector3 closestPrey = predatorPositions[index];
            for (int i = 0; i < preyPositions.Length; i++)
            {
                float d = math.distance(predatorPositions[index], preyPositions[i]);
                if (d < minDist)
                {
                    minDist = d;
                    closestPrey = preyPositions[i];
                }
            }
            predatorVelocities[index] = (closestPrey - predatorPositions[index]) * predatorSpeed;
        }
    }
}
