using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform _target;
    [SerializeField] private Vector2 offset;

    [Header("Movement Settings")]
    [SerializeField] private float smoothSpeed = 0.15f;

    [Header("Bounds Settings")]
    private Vector2 minLimits;
    private Vector2 maxLimits;

    [Header("Follow Control")]
    [SerializeField] private bool follow = true;

    private Vector3 velocity = Vector3.zero;
    private bool isRecentering = false;

    private Vector3 shakeOffset = Vector3.zero;
    private Coroutine shakeCoroutine;

    private void LateUpdate()
    {
        if (_target == null)
        {
            transform.position = transform.position + shakeOffset;
            return;
        }

        Vector3 basePosition = transform.position;

        if (follow)
        {
            Vector3 desiredPosition = (Vector2)_target.position + offset;
            desiredPosition.z = transform.position.z;

            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minLimits.x, maxLimits.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minLimits.y, maxLimits.y);

            if (isRecentering)
            {
                basePosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
                if (Vector3.Distance(basePosition, desiredPosition) < 0.05f)
                    isRecentering = false;
            }
            else
            {
                basePosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
            }
        }

        transform.position = basePosition + shakeOffset;
    }

    public void SetBounds(Vector2 min, Vector2 max)
    {
        minLimits = min;
        maxLimits = max;
    }

    public void RecenterCamera()
    {
        if (_target == null) return;
        isRecentering = true;
    }

    public void SetFollowActive(bool active)
    {
        follow = active;
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            shakeOffset = new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        float fadeTime = 0.1f;
        Vector3 startOffset = shakeOffset;
        float t = 0f;

        while (t < 1f)
        {
            shakeOffset = Vector3.Lerp(startOffset, Vector3.zero, t);
            t += Time.deltaTime / fadeTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        shakeCoroutine = null;
    }
}

