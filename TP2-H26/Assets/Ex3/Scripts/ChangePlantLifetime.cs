// using Unity.Burst;
// using Unity.Collections;
// using Unity.Jobs;
// using UnityEngine;

// // public class PlantLifetimeManager : MonoBehaviour
// // {
// //     private NativeArray<Vector3> preyPositions;
// //     private NativeArray<Vector3> plantPositions;
// //     private NativeArray<float> decreasingFactors;

// //     void Start()
// //     {
// //         int preyCount = Ex4Spawner.PreyTransforms.Length;
// //         int plantCount = Ex4Spawner.PlantTransforms.Length;

// //         preyPositions = new NativeArray<Vector3>(preyCount, Allocator.Persistent);
// //         plantPositions = new NativeArray<Vector3>(plantCount, Allocator.Persistent);
// //         decreasingFactors = new NativeArray<float>(plantCount, Allocator.Persistent);
// //     }

// //     void Update()
// //     {
// //         if (Ex4Spawner.Instance == null)
// //             return;
// //         if (Ex4Spawner.PlantTransforms == null || Ex4Spawner.PreyTransforms == null)
// //             return;
// //         int preyCount = Ex4Spawner.PreyTransforms.Length;
// //         int plantCount = Ex4Spawner.PlantTransforms.Length;

// //         // Copy prey positions
// //         for (int i = 0; i < preyCount; i++)
// //         {
// //             preyPositions[i] = Ex4Spawner.PreyTransforms[i].position;
// //         }

// //         // Copy plant positions
// //         for (int i = 0; i < plantCount; i++)
// //         {
// //             plantPositions[i] = Ex4Spawner.PlantTransforms[i].position;
// //         }

// //         var job = new PlantLifetimeJob
// //         {
// //             preyPositions = preyPositions,
// //             plantPositions = plantPositions,
// //             decreasingFactors = decreasingFactors,
// //             touchingDistance = Ex3Config.TouchingDistance

// //         };

// //         JobHandle handle = job.Schedule(plantCount, 32);
// //         handle.Complete();

// //         // Apply results back to lifetimes
// //         for (int i = 0; i < plantCount; i++)
// //         {
// //             Ex4Spawner.PlantLifetimes[i].decreasingFactor = decreasingFactors[i];
// //         }
// //     }

// //     void OnDestroy()
// //     {
// //         if (preyPositions.IsCreated) preyPositions.Dispose();
// //         if (plantPositions.IsCreated) plantPositions.Dispose();
// //         if (decreasingFactors.IsCreated) decreasingFactors.Dispose();
// //     }
// // }


// // [BurstCompile]
// // public struct PlantLifetimeJob : IJobParallelFor
// // {
// //     [ReadOnly] public NativeArray<Vector3> preyPositions;
// //     [ReadOnly] public NativeArray<Vector3> plantPositions;

// //     public NativeArray<float> decreasingFactors;

// //     public float touchingDistance;

// //     public void Execute(int index)
// //     {
// //         float factor = 1f;
// //         Vector3 plantPos = plantPositions[index];

// //         for (int i = 0; i < preyPositions.Length; i++)
// //         {
// //             if (Vector3.Distance(preyPositions[i], plantPos) < touchingDistance)
// //             {
// //                 factor *= 2f;
// //                 break;
// //             }
// //         }

// //         decreasingFactors[index] = factor;
// //     }
// // }

// public class ChangePlantLifetime : MonoBehaviour
// {
//     private Lifetime _lifetime;
    
//     public void Start()
//     {
//         _lifetime = GetComponent<Lifetime>();
//     }

//     public void Update()
//     {
//         _lifetime.decreasingFactor = 1.0f;
//         foreach(var prey in Ex4Spawner.PreyTransforms)
//         {
//             if (Vector3.Distance(prey.position, transform.position) < Ex3Config.TouchingDistance)
//             {
//                 _lifetime.decreasingFactor *= 2f;
//                 break;
//             }
//         }
//     }
// }