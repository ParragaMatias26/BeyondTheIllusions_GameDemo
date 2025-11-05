using UnityEngine;

public class IddleOscilation : MonoBehaviour
{
    public float amplitude = 0.05f;
    public float speed = 2f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * speed) * amplitude;
        transform.localPosition = initialPosition + new Vector3(0f, offsetY, 0f);
    }
}
