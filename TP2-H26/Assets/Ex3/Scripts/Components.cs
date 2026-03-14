using System;
using Unity.Entities;
using Unity.Mathematics;

public struct Position : IComponentData
{
    public float3 value;
}

public struct LifetimeRange : IComponentData
{
    // Progress and max value for lifetime
    // could maybe use a dictionary instead
    public Tuple<float, float> lifetime;
}

public struct Velocities : IComponentData
{
    public float3 velocity;
}

public struct Rotation : IComponentData
{
    public float3 rotation;
}

public struct Scale : IComponentData
{
    public float3 scale;
}

public enum EntityType : byte
{
    Plant,
    Prey,
    Predator
}

public struct BlockType : IComponentData
{
    public EntityType blockType;
}

public struct Reproduced : IComponentData
{
    public bool hasReproduced;
}

public struct LifetimeDecreasingFactor : IComponentData
{
    public float decreasingFactor;
}