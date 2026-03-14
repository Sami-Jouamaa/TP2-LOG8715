using UnityEngine;

public class Lifetime : MonoBehaviour
{
    private const float StartingLifetimeLowerBound = 5f;
    private const float StartingLifetimeUpperBound = 15f;

    public float decreasingFactor = 1f;
    public bool alwaysReproduce;
    public bool reproduced;

    private float _startingLifetime;
    private float _lifetime;

    public float GetProgression()
    {
        if (_startingLifetime <= 0f) return 0f;
        return _lifetime / _startingLifetime;
    }

    private void Awake()
    {
        ResetLifetime();
    }

    private void OnEnable()
    {
        ResetLifetime();
    }

    public void ResetLifetime()
    {
        reproduced = false;
        decreasingFactor = 1f;
        _startingLifetime = Random.Range(StartingLifetimeLowerBound, StartingLifetimeUpperBound);
        _lifetime = _startingLifetime;
    }

    public bool Tick(float deltaTime)
    {
        _lifetime -= deltaTime * decreasingFactor;
        return _lifetime <= 0f;
    }

    public void ResolveEndOfLife(Transform cachedTransform)
    {
        if (reproduced || alwaysReproduce)
        {
            ResetLifetime();
            if (Ex4Spawner.Instance != null)
            {
                Ex4Spawner.Instance.Respawn(cachedTransform);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
