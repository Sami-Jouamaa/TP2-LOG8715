using UnityEngine;

public class ChangePreyLifetime : MonoBehaviour
{
    private void Update()
    {
        if (Ex4Spawner.PreyTransforms == null || Ex4Spawner.PlantTransforms == null || Ex4Spawner.PredatorTransforms == null)
            return;

        for (int i = 0; i < Ex4Spawner.PreyTransforms.Length; i++)
        {
            var preyTransform = Ex4Spawner.PreyTransforms[i];
            var preyLifetime = Ex4Spawner.PreyLifetimes[i];

            if (preyTransform == null || preyLifetime == null || !preyTransform.gameObject.activeSelf)
                continue;

            float factor = 1f;
            bool reproduced = false;

            for (int j = 0; j < Ex4Spawner.PlantTransforms.Length; j++)
            {
                var plantTransform = Ex4Spawner.PlantTransforms[j];
                if (plantTransform == null || !plantTransform.gameObject.activeSelf)
                    continue;

                if (Vector3.Distance(preyTransform.position, plantTransform.position) < Ex3Config.TouchingDistance)
                {
                    factor /= 2f;
                    break;
                }
            }

            for (int j = 0; j < Ex4Spawner.PredatorTransforms.Length; j++)
            {
                var predatorTransform = Ex4Spawner.PredatorTransforms[j];
                if (predatorTransform == null || !predatorTransform.gameObject.activeSelf)
                    continue;

                if (Vector3.Distance(preyTransform.position, predatorTransform.position) < Ex3Config.TouchingDistance)
                {
                    factor *= 2f;
                    break;
                }
            }

            for (int j = 0; j < Ex4Spawner.PreyTransforms.Length; j++)
            {
                if (j == i) continue;

                var otherPreyTransform = Ex4Spawner.PreyTransforms[j];
                if (otherPreyTransform == null || !otherPreyTransform.gameObject.activeSelf)
                    continue;

                if (Vector3.Distance(preyTransform.position, otherPreyTransform.position) < Ex3Config.TouchingDistance)
                {
                    reproduced = true;
                    break;
                }
            }

            preyLifetime.decreasingFactor = factor;
            preyLifetime.reproduced = reproduced;

            if (preyLifetime.Tick(Time.deltaTime))
            {
                preyLifetime.ResolveEndOfLife(preyTransform);
            }
        }
    }
}
