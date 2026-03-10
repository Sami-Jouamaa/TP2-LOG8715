// using Unity.Burst;
// using Unity.Collections;
// using Unity.Jobs;
// using UnityEngine;

// // public class PreyLifetimeManager : MonoBehaviour
// // {
// //     private NativeArray<Vector3> plantPositions;
// //     private NativeArray<Vector3> predatorPositions;
// //     private NativeArray<Vector3> preyPositions;

// //     private NativeArray<float> decreasingFactors;
// //     private NativeArray<bool> reproduced;

// //     void Start()
// //     {
// //         int plantCount = Ex4Spawner.PlantTransforms.Length;
// //         int predatorCount = Ex4Spawner.PredatorTransforms.Length;
// //         int preyCount = Ex4Spawner.PreyTransforms.Length;

// //         plantPositions = new NativeArray<Vector3>(plantCount, Allocator.Persistent);
// //         predatorPositions = new NativeArray<Vector3>(predatorCount, Allocator.Persistent);
// //         preyPositions = new NativeArray<Vector3>(preyCount, Allocator.Persistent);

// //         decreasingFactors = new NativeArray<float>(preyCount, Allocator.Persistent);
// //         reproduced = new NativeArray<bool>(preyCount, Allocator.Persistent);
// //     }

// //     void Update()
// //     {
// //         if (Ex4Spawner.Instance == null)
// //             return;
// //         if (Ex4Spawner.PreyTransforms == null)
// //             return;

// //         int plantCount = Ex4Spawner.PlantTransforms.Length;
// //         int predatorCount = Ex4Spawner.PredatorTransforms.Length;
// //         int preyCount = Ex4Spawner.PreyTransforms.Length;

// //         // Copy plant positions
// //         for (int i = 0; i < plantCount; i++)
// //         {
// //             plantPositions[i] = Ex4Spawner.PlantTransforms[i].position;
// //         }

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

// //         var job = new PreyLifetimeJob
// //         {
// //             plantPositions = plantPositions,
// //             predatorPositions = predatorPositions,
// //             preyPositions = preyPositions,
// //             decreasingFactors = decreasingFactors,
// //             reproduced = reproduced,
// //             touchingDistance = Ex3Config.TouchingDistance
// //         };

// //         JobHandle handle = job.Schedule(preyCount, 32);
// //         handle.Complete();

// //         // Apply results back
// //         for (int i = 0; i < preyCount; i++)
// //         {
// //             Ex4Spawner.PreyLifetimes[i].decreasingFactor = decreasingFactors[i];
// //             Ex4Spawner.PreyLifetimes[i].reproduced = reproduced[i];
// //         }
// //     }

// //     void OnDestroy()
// //     {
// //         if (plantPositions.IsCreated) plantPositions.Dispose();
// //         if (predatorPositions.IsCreated) predatorPositions.Dispose();
// //         if (preyPositions.IsCreated) preyPositions.Dispose();
// //         if (decreasingFactors.IsCreated) decreasingFactors.Dispose();
// //         if (reproduced.IsCreated) reproduced.Dispose();
// //     }
// // }

// // [BurstCompile]
// // public struct PreyLifetimeJob : IJobParallelFor
// // {
// //     [ReadOnly] public NativeArray<Vector3> plantPositions;
// //     [ReadOnly] public NativeArray<Vector3> predatorPositions;
// //     [ReadOnly] public NativeArray<Vector3> preyPositions;

// //     public NativeArray<float> decreasingFactors;
// //     public NativeArray<bool> reproduced;

// //     public float touchingDistance;

// //     public void Execute(int index)
// //     {
// //         float factor = 1f;
// //         bool rep = false;

// //         Vector3 preyPos = preyPositions[index];

// //         // Check plants
// //         for (int i = 0; i < plantPositions.Length; i++)
// //         {
// //             if (Vector3.Distance(plantPositions[i], preyPos) < touchingDistance)
// //             {
// //                 factor /= 2f;
// //                 break;
// //             }
// //         }

// //         // Check predators
// //         for (int i = 0; i < predatorPositions.Length; i++)
// //         {
// //             if (Vector3.Distance(predatorPositions[i], preyPos) < touchingDistance)
// //             {
// //                 factor *= 2f;
// //                 break;
// //             }
// //         }

// //         // Check other prey
// //         for (int i = 0; i < preyPositions.Length; i++)
// //         {
// //             if (i == index) continue;

// //             if (Vector3.Distance(preyPositions[i], preyPos) < touchingDistance)
// //             {
// //                 rep = true;
// //                 break;
// //             }
// //         }

// //         decreasingFactors[index] = factor;
// //         reproduced[index] = rep;
// //     }
// // }


// public class ChangePreyLifetime : MonoBehaviour
// {
//     private Lifetime _lifetime;
    
//     public void Start()
//     {
//         _lifetime = GetComponent<Lifetime>();
//     }

//     public void Update()
//     {
//         _lifetime.decreasingFactor = 1.0f;
//         foreach(var plant in Ex4Spawner.PlantTransforms)
//         {
//             if (Vector3.Distance(plant.position, transform.position) < Ex3Config.TouchingDistance)
//             {
//                 _lifetime.decreasingFactor /= 2;
//                 break;
//             }
//         }
        
//         foreach(var predator in Ex4Spawner.PredatorTransforms)
//         {
//             if (Vector3.Distance(predator.position, transform.position) < Ex3Config.TouchingDistance)
//             {
//                 _lifetime.decreasingFactor *= 2f;
//                 break;
//             }
//         }
        
//         foreach(var prey in Ex4Spawner.PreyTransforms)
//         {
//             if (Vector3.Distance(prey.position, transform.position) < Ex3Config.TouchingDistance)
//             {
//                 _lifetime.reproduced = true;
//                 break;
//             }
//         }
//     }
// }