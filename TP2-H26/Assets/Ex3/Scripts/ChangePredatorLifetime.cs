using UnityEngine;

public class ChangePredatorLifetime : MonoBehaviour
{
    private void Update()
    {
        if (Ex4Spawner.PredatorTransforms == null || Ex4Spawner.PreyTransforms == null) return;

        for (int i = 0; i < Ex4Spawner.PredatorTransforms.Length; i++)
        {
            var predatorTransform = Ex4Spawner.PredatorTransforms[i];
            var predatorLifetime = Ex4Spawner.PredatorLifetimes[i];

            if (predatorTransform == null || predatorLifetime == null || !predatorTransform.gameObject.activeSelf)
                continue;

            float factor = 1f;
            bool reproduced = false;

            for (int j = 0; j < Ex4Spawner.PredatorTransforms.Length; j++)
            {
                if (j == i) continue;

                var otherPredatorTransform = Ex4Spawner.PredatorTransforms[j];
                if (otherPredatorTransform == null || !otherPredatorTransform.gameObject.activeSelf)
                    continue;

                if (Vector3.Distance(predatorTransform.position, otherPredatorTransform.position) < Ex3Config.TouchingDistance)
                {
                    reproduced = true;
                    break;
                }
            }

            for (int j = 0; j < Ex4Spawner.PreyTransforms.Length; j++)
            {
                var preyTransform = Ex4Spawner.PreyTransforms[j];
                if (preyTransform == null || !preyTransform.gameObject.activeSelf)
                    continue;

                if (Vector3.Distance(predatorTransform.position, preyTransform.position) < Ex3Config.TouchingDistance)
                {
                    factor /= 2f;
                }
            }

            predatorLifetime.decreasingFactor = factor;
            predatorLifetime.reproduced = reproduced;

            if (predatorLifetime.Tick(Time.deltaTime))
            {
                predatorLifetime.ResolveEndOfLife(predatorTransform);
            }
        }
    }
}
