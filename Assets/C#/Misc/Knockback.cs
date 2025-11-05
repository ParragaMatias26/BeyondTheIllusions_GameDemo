using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float knockbackDuration = .5f;
    [SerializeField] private float knockbackSpeed = 17f;
    [SerializeField][Range(0f,1f)] private float knockbackResistance = 0f;

    [Header("Raycast Values")]
    [SerializeField] private Vector2 raycastOffset;
    [SerializeField] private float raycastDistance = 1f;

    Coroutine actualKnockback;

    public void ApplyKnokckBack(Vector2 instigator)
    {
        Vector3 diff = (Vector2)transform.position - instigator;

        if (diff.sqrMagnitude < 0.0001f)
            return;

        Vector3 direction = diff.normalized;

        if (actualKnockback != null) return;
        actualKnockback = StartCoroutine(KnockbackRoutine(direction, knockbackSpeed));
    }
    public void CancelKnockback() 
    {
        if(actualKnockback != null) 
        {
            StopCoroutine(actualKnockback);
            actualKnockback = null;
        }
    }
    private bool CanKnockback(Vector2 dir)
    {
        int obstacleLayerMask = LayerMask.GetMask("Obstacles", "Props");
        Vector3 startPos = transform.position + (Vector3)raycastOffset;

        return !Physics2D.Raycast(startPos, dir, raycastDistance, obstacleLayerMask);
    }
    IEnumerator KnockbackRoutine(Vector2 direction, float knockbackSpeed)
    {
        float elapsedTime = 0f;
        float fixedSpeed = knockbackSpeed * (1f - knockbackResistance);
        
        while (elapsedTime < knockbackDuration)
        {
            if (!CanKnockback(direction)) 
            {
                CancelKnockback();
                yield break;
            }

            float t = elapsedTime / knockbackDuration;
            float currentSpeed = Mathf.Lerp(fixedSpeed, 0, t);

            transform.position += (Vector3)direction * currentSpeed * Time.deltaTime;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        actualKnockback = null;
    }
}
