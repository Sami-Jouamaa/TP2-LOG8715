using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class SyncGameObjectSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((GameObjectRef goRef, in LocalTransform lt) =>
        {
            if (goRef.GameObject == null) return;
            goRef.GameObject.transform.position = lt.Position;
            goRef.GameObject.transform.localScale = Vector3.one * lt.Scale;
        }).WithoutBurst().Run();

        // Handle disabled entities — hide their GameObjects
        Entities.WithAll<Disabled>().ForEach((GameObjectRef goRef) =>
        {
            if (goRef.GameObject != null)
                goRef.GameObject.SetActive(false);
        }).WithoutBurst().Run();
    }
}