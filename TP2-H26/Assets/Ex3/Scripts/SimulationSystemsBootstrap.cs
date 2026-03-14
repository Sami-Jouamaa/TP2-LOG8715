using UnityEngine;

public class SimulationSystemsBootstrap : MonoBehaviour
{
    [Header("Optional auto-add of systems on this GameObject")]
    [SerializeField] private bool autoAddMissingSystems = true;

    private void Awake()
    {
        if (!autoAddMissingSystems) return;

        AddIfMissing<MovePreyTowardPlant>();
        AddIfMissing<MovePredatorTowardPrey>();
        AddIfMissing<ApplyVelocitySystem>();
        AddIfMissing<ChangePlantLifetime>();
        AddIfMissing<ChangePreyLifetime>();
        AddIfMissing<ChangePredatorLifetime>();
    }

    private void AddIfMissing<T>() where T : Component
    {
        if (GetComponent<T>() == null)
        {
            gameObject.AddComponent<T>();
        }
    }
}
