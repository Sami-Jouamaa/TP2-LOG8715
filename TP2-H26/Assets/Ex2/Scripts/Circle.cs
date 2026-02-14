using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Circle : MonoBehaviour
{
    [FormerlySerializedAs("I")] [HideInInspector]
    public int i;

    [FormerlySerializedAs("J")] [HideInInspector]
    public int j;

    public float Health { get; private set; }

    private const float BaseHealth = 1000;

    private const float HealingPerSecond = 1;
    private const float HealingRange = 3;
    private GridShape grid;
    private Circle[] nearbyCircles; // array of unchanging nearby circles, instead of searching them every frame.
    // Start is called before the first frame update
    private void Start()
    {
        Health = BaseHealth;
        grid = GameObject.FindFirstObjectByType<GridShape>(); // find grid once
        nearbyCircles = Physics2D.OverlapCircleAll(transform.position, HealingRange).Select(collider => collider.GetComponent<Circle>()).ToArray(); // find nearby circles once
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateColor();
        HealNearbyShapes();
    }

    private void UpdateColor()
    {
        // var grid = GameObject.FindFirstObjectByType<GridShape>();
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = grid.Colors[i, j] * Health / BaseHealth;
    }

    private void HealNearbyShapes()
    {
        foreach (Circle circle in nearbyCircles)
        {
            circle.ReceiveHp(HealingPerSecond * Time.deltaTime);
        }
    }

    public void ReceiveHp(float hpReceived)
    {
        Health += hpReceived;
        Health = Mathf.Clamp(Health, 0, BaseHealth);
    }
}
