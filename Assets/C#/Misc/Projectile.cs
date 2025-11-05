using UnityEngine;

[RequireComponent (typeof(CircleCollider2D))]
public class Projectile : MonoBehaviour
{
    public bool doDamageToPlayer;

    private Vector2 direction;
    private Vector2 targetPos;
    private float speed;
    private int proyectileDamage;
    private bool initialized = false;
    private bool isMushroom = false;
    GameObject mushPrefab;
    GameObject damageImpact;
    public void Initialize(Vector2 dir, Vector2 target, float spd, bool isMush, GameObject mush, int dmg, GameObject damageImpact) 
    {
        direction = dir;
        speed = spd;
        targetPos = target;
        initialized = true;
        isMushroom = isMush;
        mushPrefab = mush;
        proyectileDamage = dmg;
        this.damageImpact = damageImpact;

        var c = GetComponent<CircleCollider2D>();
        c.isTrigger = true;
    }
    private void Update()
    {
        if (!initialized) return;

        direction.Normalize();
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) <= 0.5f && isMushroom)
            OnHitGround();
        else if (Vector2.Distance(transform.position, targetPos + direction * 10) <= 0.5f)
            OnHitGround();
    }
    void OnHitGround()
    {
        if (isMushroom)
            Instantiate(mushPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        var x = other.GetComponent<PlayerModel>();
        if(x != null && proyectileDamage > 0 && doDamageToPlayer)
        {
            x.PlayerHealth.TakeDamage(proyectileDamage, transform.position, null, GameManager.Instance.circusBossPlayerKillTime);
            
            if(isMushroom)
                Instantiate(mushPrefab, transform.position, Quaternion.identity);

            Instantiate(damageImpact, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
