using UnityEngine;

public class WanderAI : MonoBehaviour
{
    Enemy _entity;

    [Header("Movimiento")]
    [SerializeField] GameObject waypointPrefab;
    public Vector2 areaSize = new Vector2(5f, 5f);
    public float moveSpeed = 5f;
    public float smoothTime = 0.3f;
    public float stoppingDistance = 0.1f;

    GameObject waypoint;

    [Header("Delay aleatorio")]
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    private float waitTimer;
    private bool moving = false;
    private Vector3 basePosition;

    void Start()
    {
        basePosition = transform.position;
        _entity = GetComponent<Enemy>();
        waypoint = Instantiate(waypointPrefab, transform.position, Quaternion.identity);
    }

    public void WanderArroundPoint()
    {
        if (moving)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPosition);

            var obs = LayerMask.GetMask("Obstacles", GameManager.Instance.propsLayerMask, GameManager.Instance.obstaclesLayerMask);
            
            if (Physics2D.Raycast(transform.position, direction, distance, obs))
            {
                moving = false;
                waitTimer = Random.Range(minWaitTime, maxWaitTime);
                basePosition = transform.position;
                return;
            }

            _entity.movement.MoveTo(targetPosition);

            if (distance < stoppingDistance)
            {
                moving = false;
                _entity.movement.CancelVelocity();
                waitTimer = Random.Range(minWaitTime, maxWaitTime);
                basePosition = transform.position;
            }
        }
        else
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                PickNewTarget();
            }
        }
    }

    void PickNewTarget()
    {
        if (waypoint == null)
        {
            Debug.LogWarning("Waypoint no asignado.");
            return;
        }

        float minX = waypoint.transform.position.x - areaSize.x / 2f;
        float maxX = waypoint.transform.position.x + areaSize.x / 2f;
        float minY = waypoint.transform.position.y - areaSize.y / 2f;
        float maxY = waypoint.transform.position.y + areaSize.y / 2f;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        targetPosition = new Vector3(randomX, randomY, transform.position.z);
        moving = true;
    }

    void OnDrawGizmos()
    {
        if (waypoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(waypoint.transform.position, new Vector3(areaSize.x, areaSize.y, 0f));
        }
    }
}
