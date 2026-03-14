using UnityEngine;

public class ApplyVelocitySystem : MonoBehaviour
{
    private void Update()
    {
        Apply(Ex4Spawner.PlantTransforms, Ex4Spawner.PlantVelocities);
        Apply(Ex4Spawner.PreyTransforms, Ex4Spawner.PreyVelocities);
        Apply(Ex4Spawner.PredatorTransforms, Ex4Spawner.PredatorVelocities);
    }

    private void Apply(Transform[] transforms, Velocity[] velocities)
    {
        if (transforms == null || velocities == null) return;

        for (int i = 0; i < transforms.Length; i++)
        {
            var currentTransform = transforms[i];
            var currentVelocity = velocities[i];

            if (currentTransform == null || currentVelocity == null || !currentTransform.gameObject.activeSelf)
                continue;

            currentTransform.localPosition += currentVelocity.velocity * Time.deltaTime;
        }
    }
}
