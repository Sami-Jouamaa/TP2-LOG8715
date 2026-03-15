using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class ECSSpawner : MonoBehaviour
{
    public Ex3Config config;
    public GameObject predatorPrefab;
    public GameObject preyPrefab;
    public GameObject plantPrefab;

    private int _height;
    private int _width;

    void Awake()
    {
        float size = config.gridSize;
        float ratio = Camera.main.aspect;
        _height = (int)Math.Round(Math.Sqrt(size / ratio));
        _width = (int)Math.Round(size / _height);
    }

    void Start()
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        var random = new Random((uint)UnityEngine.Random.Range(1, int.MaxValue));

        // Grid config singleton
        var configEntity = em.CreateEntity();
        em.AddComponentData(configEntity, new GridConfigSingleton
        {
            HalfWidth = _width / 2,
            HalfHeight = _height / 2,
            RandomSeed = (uint)UnityEngine.Random.Range(1, int.MaxValue)
        });

        for (int i = 0; i < config.plantCount; i++)
            CreateEntity(em, ref random, plantPrefab, new PlantTag(), true);

        for (int i = 0; i < config.preyCount; i++)
            CreateEntity(em, ref random, preyPrefab, new PreyTag(), false);

        for (int i = 0; i < config.predatorCount; i++)
            CreateEntity(em, ref random, predatorPrefab, new PredatorTag(), false);
    }

    private void CreateEntity<T>(EntityManager em, ref Random random, GameObject prefab, T tag, bool alwaysReproduce) where T : unmanaged, IComponentData
    {
        var entity = em.CreateEntity();

        float startLife = random.NextFloat(5f, 15f);
        float3 pos = new float3(
            random.NextInt(-_width / 2, _width / 2),
            random.NextInt(-_height / 2, _height / 2),
            0f
        );

        em.AddComponentData(entity, new LifetimeData
        {
            StartingLifetime = startLife,
            CurrentLifetime = startLife,
            DecreasingFactor = 1f,
            AlwaysReproduce = alwaysReproduce,
            Reproduced = false
        });

        em.AddComponentData(entity, new VelocityData { Value = float3.zero });

        em.AddComponentData(entity, new LocalTransform
        {
            Position = pos,
            Rotation = quaternion.identity,
            Scale = 1f
        });

        em.AddComponentData(entity, tag);

        var go = Instantiate(prefab, pos, Quaternion.identity);

        em.AddComponentObject(entity, new GameObjectRef { GameObject = go });
    }
}