using Unity.Entities;

public struct GridConfigSingleton : IComponentData
{
    public int HalfWidth;
    public int HalfHeight;
    public uint RandomSeed;
}