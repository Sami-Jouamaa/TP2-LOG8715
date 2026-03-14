using UnityEngine;

public class ChangePlantLifetime : MonoBehaviour
{
    private void Update()
    {
        if (Ex4Spawner.PlantTransforms == null || Ex4Spawner.PreyTransforms == null) return;

        for (int i = 0; i < Ex4Spawner.PlantTransforms.Length; i++)
        {
            var plantTransform = Ex4Spawner.PlantTransforms[i];
            var plantLifetime = Ex4Spawner.PlantLifetimes[i];

            if (plantTransform == null || plantLifetime == null || !plantTransform.gameObject.activeSelf)
                continue;

            float factor = 1f;

            for (int j = 0; j < Ex4Spawner.PreyTransforms.Length; j++)
            {
                var preyTransform = Ex4Spawner.PreyTransforms[j];
                if (preyTransform == null || !preyTransform.gameObject.activeSelf)
                    continue;

                if (Vector3.Distance(plantTransform.position, preyTransform.position) < Ex3Config.TouchingDistance)
                {
                    factor *= 2f;
                    break;
                }
            }

            plantLifetime.decreasingFactor = factor;

            if (plantLifetime.Tick(Time.deltaTime))
            {
                plantLifetime.ResolveEndOfLife(plantTransform);
            }
        }
    }
}
