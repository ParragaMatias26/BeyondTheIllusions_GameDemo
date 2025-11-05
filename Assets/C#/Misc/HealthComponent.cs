using System;
using System.Collections;
using UnityEngine;
public class HealthComponent : MonoBehaviour, IDamageable
{
    [Header("Values")]
    [SerializeField] int maxHealth;
    [SerializeField] float iFrames;
    [SerializeField] bool canTakeDamage = true;
    [SerializeField] bool doCameraShake = true;

    int health;
    bool isAlive = true;
    public bool IsAlive { get { return isAlive; } }
    Coroutine dmgEndRoutine;

    float duration;
    float magnitude;
    public int CurrentHealth { get { return health; } set { health = value; } }
    public float MaxHealth { get { return maxHealth; } }
    public bool CanTakeDamage { get { return canTakeDamage; } set { canTakeDamage = value; } }

    public event Action OnHealthUp = delegate { };
    public event Action<Vector3, Animator> OnDamageTake = delegate { };
    public event Action<Animator, float> OnDeath = delegate { };

    private void Start()
    {
        health = maxHealth;

        duration = GameManager.Instance.onHitDuration;
        magnitude = GameManager.Instance.onHitMagnitude;
    }

    public void TakeDamage(int damage, Vector3 instigator, Animator instAnim, float dieAnimTime)
    {
        if (!canTakeDamage) return;

        health -= damage;
        canTakeDamage = false;

        dmgEndRoutine = StartCoroutine(DamageEndRoutine());

        if (health <= 0)
        {
            health = 0;
            Die(instAnim, dieAnimTime);
        }

        if(doCameraShake)
            GameManager.Instance.CameraController.ShakeCamera(duration, magnitude);

        OnDamageTake?.Invoke(instigator, instAnim);
    }

    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        OnHealthUp?.Invoke();
    }

    IEnumerator DamageEndRoutine()
    {
        yield return new WaitForSeconds(iFrames);
        canTakeDamage = true;
        dmgEndRoutine = null;
    }

    public void Die(Animator instAnim, float dieAnimTime)
    {
        StopAllCoroutines();
        OnDeath?.Invoke(instAnim, dieAnimTime);

        canTakeDamage = false;
        isAlive = false;
    }
    public void ResetHealth() 
    {
        health = maxHealth;

        if (dmgEndRoutine != null) StopCoroutine(dmgEndRoutine);
        dmgEndRoutine = null;
        canTakeDamage = true;
    }
}
