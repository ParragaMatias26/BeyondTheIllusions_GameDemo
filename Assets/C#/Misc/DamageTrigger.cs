using UnityEngine;

[RequireComponent (typeof(CapsuleCollider2D))]
public class DamageTrigger : MonoBehaviour
{
    [SerializeField] int damage;
    private void Awake()
    {
        var col = GetComponent<CapsuleCollider2D>();
        col.isTrigger = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var x = collision.GetComponent<PlayerModel>();
        if(x != null)
            x.PlayerHealth.TakeDamage(damage, transform.position, null, 0f);
    }
}
