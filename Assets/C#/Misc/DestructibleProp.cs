using System.Collections;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent (typeof(DamageBurst))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(ColliderFixObject))]
public class DestructibleProp : MonoBehaviour
{
    [Header("Hit Shake Settings")]
    [SerializeField] private float duration = 0.08f;
    [SerializeField] private float magnitude = 0.05f;

    private HealthComponent healthComp;
    private DamageBurst burstVFX;
    private Vector3 originalPos;

    Coroutine currentShake;
    private void Awake()
    {
        healthComp = GetComponent<HealthComponent>();
        burstVFX = GetComponent<DamageBurst>();

        originalPos = transform.localPosition;

        if (healthComp != null) 
        {
            healthComp.OnDamageTake += (_,_) => 
            {
                if (burstVFX != null)
                    burstVFX.TriggerEffect();

                if(currentShake != null)
                    StopCoroutine(currentShake);

                currentShake = StartCoroutine(ShakeRoutine(duration, magnitude));
            };
            healthComp.OnDeath += (_, _) => 
            {
                //Instanciar particulas
                Destroy(gameObject);
            };
        }
    }
    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            Vector2 offset = Random.insideUnitCircle * magnitude;
            transform.localPosition = originalPos + (Vector3)offset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        currentShake = null;
    }
}
