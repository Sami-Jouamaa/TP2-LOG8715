using UnityEngine;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Jobs;

public class ChangePredatorLifetime : MonoBehaviour
{
    void Update()
    {
        // intentionally empty
    }
}



// // public class PredatorLifetimeManager : MonoBehaviour
// // {
// //     private NativeArray<Vector3> predatorPositions;
// //     private NativeArray<Vector3> preyPositions;

// //     private NativeArray<float> decreasingFactors;
// //     private NativeArray<bool> reproduced;

// //     void Start()
// //     {
// //         int predatorCount = Ex4Spawner.PredatorTransforms.Length;
// //         int preyCount = Ex4Spawner.PreyTransforms.Length;

// //         predatorPositions = new NativeArray<Vector3>(predatorCount, Allocator.Persistent);
// //         preyPositions = new NativeArray<Vector3>(preyCount, Allocator.Persistent);

// //         decreasingFactors = new NativeArray<float>(predatorCount, Allocator.Persistent);
// //         reproduced = new NativeArray<bool>(predatorCount, Allocator.Persistent);
// //     }

// //     void Update()
// //     {
// //         if (Ex4Spawner.Instance == null)
// //             return;
// //         if (Ex4Spawner.PredatorTransforms == null || Ex4Spawner.PreyTransforms == null)
// //             return;

// //         int predatorCount = Ex4Spawner.PredatorTransforms.Length;
// //         int preyCount = Ex4Spawner.PreyTransforms.Length;

// //         // Copy predator positions
// //         for (int i = 0; i < predatorCount; i++)
// //         {
// //             predatorPositions[i] = Ex4Spawner.PredatorTransforms[i].position;
// //         }

// //         // Copy prey positions
// //         for (int i = 0; i < preyCount; i++)
// //         {
// //             preyPositions[i] = Ex4Spawner.PreyTransforms[i].position;
// //         }

// //         var job = new PredatorLifetimeJob
// //         {
// //             predatorPositions = predatorPositions,
// //             preyPositions = preyPositions,
// //             decreasingFactors = decreasingFactors,
// //             reproduced = reproduced,
// //             touchingDistance = Ex3Config.TouchingDistance
// //         };

// //         JobHandle handle = job.Schedule(predatorCount, 32);
// //         handle.Complete();

// //         // Apply results back to lifetimes
// //         for (int i = 0; i < predatorCount; i++)
// //         {
// //             Ex4Spawner.PredatorLifetimes[i].decreasingFactor = decreasingFactors[i];
// //             Ex4Spawner.PredatorLifetimes[i].reproduced = reproduced[i];
// //         }
// //     }

// //     void OnDestroy()
// //     {
// //         if (predatorPositions.IsCreated) predatorPositions.Dispose();
// //         if (preyPositions.IsCreated) preyPositions.Dispose();
// //         if (decreasingFactors.IsCreated) decreasingFactors.Dispose();
// //         if (reproduced.IsCreated) reproduced.Dispose();
// //     }
// // }


// // [BurstCompile]
// // public struct PredatorLifetimeJob : IJobParallelFor
// // {
// //     [ReadOnly] public NativeArray<Vector3> predatorPositions;
// //     [ReadOnly] public NativeArray<Vector3> preyPositions;

// //     public NativeArray<float> decreasingFactors;
// //     public NativeArray<bool> reproduced;

// //     public float touchingDistance;

// //     public void Execute(int index)
// //     {
// //         float factor = 1f;
// //         bool rep = false;

// //         Vector3 predatorPos = predatorPositions[index];

// //         // predator reproduction check
// //         for (int i = 0; i < predatorPositions.Length; i++)
// //         {
// //             if (i == index) continue;

// //             if (Vector3.Distance(predatorPositions[i], predatorPos) < touchingDistance)
// //             {
// //                 rep = true;
// //                 break;
// //             }
// //         }

// //         // prey interaction
// //         for (int i = 0; i < preyPositions.Length; i++)
// //         {
// //             if (Vector3.Distance(preyPositions[i], predatorPos) < touchingDistance)
// //             {
// //                 factor /= 2f;
// //             }
// //         }

// //         decreasingFactors[index] = factor;
// //         reproduced[index] = rep;
// //     }
// // }


// public class ChangePredatorLifetime : MonoBehaviour
// {
//     private Lifetime _lifetime;
    
//     public void Start()
//     {
//         _lifetime = GetComponent<Lifetime>();
//     }

//     public void Update()
//     {
//         _lifetime.decreasingFactor = 1.0f;
//         foreach(var predator in Ex4Spawner.PredatorTransforms)
//         {
//             if (Vector3.Distance(predator.position, transform.position) < Ex3Config.TouchingDistance)
//             {
//                 _lifetime.reproduced = true;
//                 break;
//             }
//         }
        
//         foreach(var prey in Ex4Spawner.PreyTransforms)
//         {
//             if (Vector3.Distance(prey.position, transform.position) < Ex3Config.TouchingDistance)
//             {
//                 _lifetime.decreasingFactor /= 2;
//             }
//         }
//     }
// }