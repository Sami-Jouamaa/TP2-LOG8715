using UnityEngine;

public class MovePredatorTowardPrey : MonoBehaviour
{
    private void Update()
    {
        if (Ex4Spawner.PredatorTransforms == null || Ex4Spawner.PreyTransforms == null) return;

        for (int i = 0; i < Ex4Spawner.PredatorTransforms.Length; i++)
        {
            var predatorTransform = Ex4Spawner.PredatorTransforms[i];
            var predatorVelocity = Ex4Spawner.PredatorVelocities[i];

            if (predatorTransform == null || predatorVelocity == null || !predatorTransform.gameObject.activeSelf)
                continue;

            float minDistance = float.MaxValue;
            Vector3 closestPreyPosition = predatorTransform.position;
            bool foundPrey = false;

            for (int j = 0; j < Ex4Spawner.PreyTransforms.Length; j++)
            {
                var preyTransform = Ex4Spawner.PreyTransforms[j];
                if (preyTransform == null || !preyTransform.gameObject.activeSelf)
                    continue;

                float distance = Vector3.Distance(predatorTransform.position, preyTransform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPreyPosition = preyTransform.position;
                    foundPrey = true;
                }
            }

            if (!foundPrey)
            {
                predatorVelocity.velocity = Vector3.zero;
                continue;
            }

            Vector3 direction = (closestPreyPosition - predatorTransform.position).normalized;
            predatorVelocity.velocity = direction * Ex3Config.PredatorSpeed;
        }
    }
}
