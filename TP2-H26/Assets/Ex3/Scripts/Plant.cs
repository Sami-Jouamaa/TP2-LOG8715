using UnityEngine;

[RequireComponent(typeof(Lifetime))]
public class Plant : MonoBehaviour
{
    private Lifetime _lifetime;

    private void Awake()
    {
        _lifetime = GetComponent<Lifetime>();
    }

    private void Update()
    {
        transform.localScale = Vector3.one * Mathf.Max(0f, _lifetime.GetProgression());
    }
}
