using UnityEngine;

public class JuggleOrbit : MonoBehaviour
{
    Vector3 centerPoint;
    float radius;
    float speed;
    float angle;

    public void Initialize(Vector3 center, float radius, float speed, float angleOffset)
    {
        this.centerPoint = center;
        this.radius = radius;
        this.speed = speed;
        this.angle = angleOffset;
    }
    void Update()
    {
        angle += speed * Time.deltaTime;
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        transform.position = centerPoint + new Vector3(x, y, 0);
    }
}
