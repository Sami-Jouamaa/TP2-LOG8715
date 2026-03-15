using Unity.Entities;

public struct LifetimeData : IComponentData
{
    public float StartingLifetime;
    public float CurrentLifetime;
    public float DecreasingFactor;
    public bool AlwaysReproduce;
    public bool Reproduced;
}