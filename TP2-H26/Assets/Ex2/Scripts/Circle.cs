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
    private SpriteRenderer spriteRenderer;
    private Circle[] nearbyCircles; // array of unchanging nearby circles, instead of searching them every frame.
    // Start is called before the first frame update
    private void Start()
    {
        Health = BaseHealth;
        grid = FindFirstObjectByType<GridShape>(); // find grid once
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>(); // find sprite renderer once
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
        // grid and spriteRenderer set on start
        spriteRenderer.color = grid.Colors[i, j] * Health / BaseHealth;
    }

    private void HealNearbyShapes()
    {
        ReceiveHp(nearbyCircles.Length * HealingPerSecond * Time.deltaTime);
    }

    public void ReceiveHp(float hpReceived)
    {
        float newHealth = Health + hpReceived;
        Health = Mathf.Clamp(newHealth, 0, BaseHealth);
    }
}
