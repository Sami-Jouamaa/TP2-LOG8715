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

    void Start()
    {
        ResetLifetime();
    }

    public void ResetLifetime()
    {
        reproduced = false;
        decreasingFactor = 1f;
        _startingLifetime = Random.Range(StartingLifetimeLowerBound, StartingLifetimeUpperBound);
        _lifetime = _startingLifetime;
        gameObject.SetActive(true);
    }

    void Update()
    {
        _lifetime -= Time.deltaTime * decreasingFactor;

        if (_lifetime > 0f)
            return;

        if (reproduced || alwaysReproduce)
        {
            ResetLifetime();
            Ex4Spawner.Instance.Respawn(transform);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}