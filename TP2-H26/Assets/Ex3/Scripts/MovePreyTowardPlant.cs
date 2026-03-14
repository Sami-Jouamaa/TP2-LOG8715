using UnityEngine;

public class MovePreyTowardPlant : MonoBehaviour
{
    private void Update()
    {
        if (Ex4Spawner.PreyTransforms == null || Ex4Spawner.PlantTransforms == null) return;

        for (int i = 0; i < Ex4Spawner.PreyTransforms.Length; i++)
        {
            var preyTransform = Ex4Spawner.PreyTransforms[i];
            var preyVelocity = Ex4Spawner.PreyVelocities[i];

            if (preyTransform == null || preyVelocity == null || !preyTransform.gameObject.activeSelf)
                continue;

            float minDistance = float.MaxValue;
            Vector3 closestPlantPosition = preyTransform.position;
            bool foundPlant = false;

            for (int j = 0; j < Ex4Spawner.PlantTransforms.Length; j++)
            {
                var plantTransform = Ex4Spawner.PlantTransforms[j];
                if (plantTransform == null || !plantTransform.gameObject.activeSelf)
                    continue;

                float distance = Vector3.Distance(preyTransform.position, plantTransform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPlantPosition = plantTransform.position;
                    foundPlant = true;
                }
            }

            if (!foundPlant)
            {
                preyVelocity.velocity = Vector3.zero;
                continue;
            }

            Vector3 direction = (closestPlantPosition - preyTransform.position).normalized;
            preyVelocity.velocity = direction * Ex3Config.PreySpeed;
        }
    }
}
