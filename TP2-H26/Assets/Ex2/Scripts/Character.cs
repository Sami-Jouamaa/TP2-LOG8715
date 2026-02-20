using UnityEngine;

public class Character : MonoBehaviour
{
    private Vector3 _velocity = Vector3.zero;

    private Vector3 _acceleration = Vector3.zero;

    private const float AccelerationMagnitude = 2;

    private const float MaxVelocityMagnitude = 5;

    private const float DamagePerSecond = 50;

    private const float DamageRange = 10;
    private const int MaxCircles = (int)(DamageRange*DamageRange*4 + 0.5);
    private ContactFilter2D collisionFilter;
    private Collider2D[] colliders = new Collider2D[MaxCircles];
    private int colliderCount;
    private Circle[] nearbyCircles = new Circle[MaxCircles];
    private GridShape grid;
    private void Start()
    {
        grid = FindFirstObjectByType<GridShape>();
    }

    private void Update()
    {
        Move();

        findNearbyCircle();
        DamageNearbyShapes();
        UpdateAcceleration();
    }

    private void Move()
    {
        _velocity += _acceleration * Time.deltaTime;
        if (_velocity.magnitude > MaxVelocityMagnitude)
        {
            _velocity = _velocity.normalized * MaxVelocityMagnitude;
        }
        transform.position += _velocity * Time.deltaTime;
    }

    private void UpdateAcceleration()
    {
        float directionX = 0;
        float directionY = 0;
        Vector3 tPos = transform.position;
        float transformDiffX = tPos.x - grid._width / 2f;
        float transformDiffY = tPos.y - grid._height / 2f;

        for (int i = 0; i < colliderCount; i++)
        {
            Circle circle = nearbyCircles[i];
            directionX += (circle.i - transformDiffX) * circle.Health;
            directionY += (circle.j - transformDiffY) * circle.Health;
            
        }
        _acceleration.x = directionX;
        _acceleration.y = directionY;
        _acceleration = _acceleration.normalized * AccelerationMagnitude;
    }

    private void DamageNearbyShapes()
    {
        // Si aucun cercle proche, on retourne a (0,0,0)
        if (colliderCount == 0)
        {
            transform.position = Vector3.zero;
            return;
        }
        float healthChange = -DamagePerSecond * Time.deltaTime;
        for (int i = 0; i < colliderCount; i++)
        {
            nearbyCircles[i].ReceiveHp(healthChange);
        }

    }

    private void findNearbyCircle()
    {
        // find nearby circles once every frame
        colliderCount = Physics2D.OverlapCircle(transform.position, DamageRange, collisionFilter.NoFilter(), colliders);
        for (int i = 0; i < colliderCount; i++)
        {
            nearbyCircles[i] = colliders[i].GetComponent<Circle>();
        }
    }
}
