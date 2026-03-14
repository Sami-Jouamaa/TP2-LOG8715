using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class JobManager : MonoBehaviour
{
    private NativeArray<Vector3> plantPositions;
    private NativeArray<Vector3> preyPositions;
    private NativeArray<Vector3> predatorPositions;

    private NativeArray<Vector3> preyVelocities;
    private NativeArray<Vector3> predatorVelocities;

    private NativeArray<float> plantFactors;
    private NativeArray<float> preyFactors;
    private NativeArray<float> predatorFactors;

    private NativeArray<bool> preyReproduced;
    private NativeArray<bool> predatorReproduced;

    private NativeArray<bool> plantActive;
    private NativeArray<bool> preyActive;
    private NativeArray<bool> predatorActive;

    private bool _initialized;

    void Update()
    {
        if (!_initialized)
        {
            if (!TryInitialize())
                return;
        }

        int plantCount = Ex4Spawner.PlantTransforms.Length;
        int preyCount = Ex4Spawner.PreyTransforms.Length;
        int predatorCount = Ex4Spawner.PredatorTransforms.Length;

        for (int i = 0; i < plantCount; i++)
        {
            plantPositions[i] = Ex4Spawner.PlantTransforms[i].position;
            plantActive[i] = Ex4Spawner.PlantTransforms[i].gameObject.activeSelf;
        }

        for (int i = 0; i < preyCount; i++)
        {
            preyPositions[i] = Ex4Spawner.PreyTransforms[i].position;
            preyVelocities[i] = Ex4Spawner.PreyVelocities[i].velocity;
            preyActive[i] = Ex4Spawner.PreyTransforms[i].gameObject.activeSelf;
        }

        for (int i = 0; i < predatorCount; i++)
        {
            predatorPositions[i] = Ex4Spawner.PredatorTransforms[i].position;
            predatorVelocities[i] = Ex4Spawner.PredatorVelocities[i].velocity;
            predatorActive[i] = Ex4Spawner.PredatorTransforms[i].gameObject.activeSelf;
        }

        int maxCount = Mathf.Max(plantCount, preyCount, predatorCount);

        var lifetimeJob = new LifetimeJob
        {
            plantPositions = plantPositions,
            preyPositions = preyPositions,
            predatorPositions = predatorPositions,

            plantActive = plantActive,
            preyActive = preyActive,
            predatorActive = predatorActive,

            plantFactors = plantFactors,
            preyFactors = preyFactors,
            predatorFactors = predatorFactors,

            preyReproduced = preyReproduced,
            predatorReproduced = predatorReproduced,

            touchingDistance = Ex3Config.TouchingDistance
        };

        JobHandle lifetimeHandle = lifetimeJob.Schedule(maxCount, 32);
        lifetimeHandle.Complete();

        var moveJob = new MoveJob
        {
            plantPositions = plantPositions,
            preyPositions = preyPositions,
            predatorPositions = predatorPositions,

            plantActive = plantActive,
            preyActive = preyActive,
            predatorActive = predatorActive,

            preyVelocities = preyVelocities,
            predatorVelocities = predatorVelocities,

            preySpeed = Ex3Config.PreySpeed,
            predatorSpeed = Ex3Config.PredatorSpeed
        };

        JobHandle moveHandle = moveJob.Schedule(maxCount, 32);
        moveHandle.Complete();

        for (int i = 0; i < plantCount; i++)
        {
            if (!plantActive[i]) continue;
            Ex4Spawner.PlantLifetimes[i].decreasingFactor = plantFactors[i];
        }

        for (int i = 0; i < preyCount; i++)
        {
            if (!preyActive[i]) continue;

            Ex4Spawner.PreyLifetimes[i].decreasingFactor = preyFactors[i];
            Ex4Spawner.PreyLifetimes[i].reproduced = preyReproduced[i];
            Ex4Spawner.PreyVelocities[i].velocity = preyVelocities[i];
        }

        for (int i = 0; i < predatorCount; i++)
        {
            if (!predatorActive[i]) continue;

            Ex4Spawner.PredatorLifetimes[i].decreasingFactor = predatorFactors[i];
            Ex4Spawner.PredatorLifetimes[i].reproduced = predatorReproduced[i];
            Ex4Spawner.PredatorVelocities[i].velocity = predatorVelocities[i];
        }
    }

    private bool TryInitialize()
    {
        if (Ex4Spawner.Instance == null) return false;
        if (Ex4Spawner.PlantTransforms == null) return false;
        if (Ex4Spawner.PreyTransforms == null) return false;
        if (Ex4Spawner.PredatorTransforms == null) return false;

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

        plantActive = new NativeArray<bool>(plantCount, Allocator.Persistent);
        preyActive = new NativeArray<bool>(preyCount, Allocator.Persistent);
        predatorActive = new NativeArray<bool>(predatorCount, Allocator.Persistent);

        _initialized = true;
        return true;
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

        if (plantActive.IsCreated) plantActive.Dispose();
        if (preyActive.IsCreated) preyActive.Dispose();
        if (predatorActive.IsCreated) predatorActive.Dispose();
    }
}

[BurstCompile]
public struct LifetimeJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<Vector3> plantPositions;
    [ReadOnly] public NativeArray<Vector3> preyPositions;
    [ReadOnly] public NativeArray<Vector3> predatorPositions;

    [ReadOnly] public NativeArray<bool> plantActive;
    [ReadOnly] public NativeArray<bool> preyActive;
    [ReadOnly] public NativeArray<bool> predatorActive;

    public NativeArray<float> plantFactors;
    public NativeArray<float> preyFactors;
    public NativeArray<float> predatorFactors;

    public NativeArray<bool> preyReproduced;
    public NativeArray<bool> predatorReproduced;

    public float touchingDistance;

    public void Execute(int index)
    {
        if (index < plantPositions.Length)
        {
            if (!plantActive[index])
            {
                plantFactors[index] = 1f;
            }
            else
            {
                float factor = 1f;
                Vector3 plant = plantPositions[index];

                for (int i = 0; i < preyPositions.Length; i++)
                {
                    if (!preyActive[i]) continue;

                    if (Vector3.Distance(preyPositions[i], plant) < touchingDistance)
                    {
                        factor *= 2f;
                        break;
                    }
                }

                plantFactors[index] = factor;
            }
        }

        if (index < preyPositions.Length)
        {
            if (!preyActive[index])
            {
                preyFactors[index] = 1f;
                preyReproduced[index] = false;
            }
            else
            {
                float factor = 1f;
                bool reproduced = false;
                Vector3 prey = preyPositions[index];

                for (int i = 0; i < plantPositions.Length; i++)
                {
                    if (!plantActive[i]) continue;

                    if (Vector3.Distance(plantPositions[i], prey) < touchingDistance)
                    {
                        factor /= 2f;
                        break;
                    }
                }

                for (int i = 0; i < predatorPositions.Length; i++)
                {
                    if (!predatorActive[i]) continue;

                    if (Vector3.Distance(predatorPositions[i], prey) < touchingDistance)
                    {
                        factor *= 2f;
                        break;
                    }
                }

                for (int i = 0; i < preyPositions.Length; i++)
                {
                    if (i == index) continue;
                    if (!preyActive[i]) continue;

                    if (Vector3.Distance(preyPositions[i], prey) < touchingDistance)
                    {
                        reproduced = true;
                        break;
                    }
                }

                preyFactors[index] = factor;
                preyReproduced[index] = reproduced;
            }
        }

        if (index < predatorPositions.Length)
        {
            if (!predatorActive[index])
            {
                predatorFactors[index] = 1f;
                predatorReproduced[index] = false;
            }
            else
            {
                float factor = 1f;
                bool reproduced = false;
                Vector3 predator = predatorPositions[index];

                for (int i = 0; i < predatorPositions.Length; i++)
                {
                    if (i == index) continue;
                    if (!predatorActive[i]) continue;

                    if (Vector3.Distance(predatorPositions[i], predator) < touchingDistance)
                    {
                        reproduced = true;
                        break;
                    }
                }

                for (int i = 0; i < preyPositions.Length; i++)
                {
                    if (!preyActive[i]) continue;

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
}

[BurstCompile]
public struct MoveJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<Vector3> plantPositions;
    [ReadOnly] public NativeArray<Vector3> preyPositions;
    [ReadOnly] public NativeArray<Vector3> predatorPositions;

    [ReadOnly] public NativeArray<bool> plantActive;
    [ReadOnly] public NativeArray<bool> preyActive;
    [ReadOnly] public NativeArray<bool> predatorActive;

    public NativeArray<Vector3> preyVelocities;
    public NativeArray<Vector3> predatorVelocities;

    public float preySpeed;
    public float predatorSpeed;

    public void Execute(int index)
    {
        if (index < preyPositions.Length)
        {
            if (!preyActive[index])
            {
                preyVelocities[index] = Vector3.zero;
            }
            else
            {
                float minDist = float.MaxValue;
                Vector3 closestPlant = preyPositions[index];

                for (int i = 0; i < plantPositions.Length; i++)
                {
                    if (!plantActive[i]) continue;

                    float d = math.distance(preyPositions[index], plantPositions[i]);
                    if (d < minDist)
                    {
                        minDist = d;
                        closestPlant = plantPositions[i];
                    }
                }

                preyVelocities[index] = (closestPlant - preyPositions[index]) * preySpeed;
            }
        }

        if (index < predatorPositions.Length)
        {
            if (!predatorActive[index])
            {
                predatorVelocities[index] = Vector3.zero;
            }
            else
            {
                float minDist = float.MaxValue;
                Vector3 closestPrey = predatorPositions[index];

                for (int i = 0; i < preyPositions.Length; i++)
                {
                    if (!preyActive[i]) continue;

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
}