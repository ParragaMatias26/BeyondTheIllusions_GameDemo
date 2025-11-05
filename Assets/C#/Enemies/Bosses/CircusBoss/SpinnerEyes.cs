using UnityEngine;

public class SpinnerEyes : MonoBehaviour
{
    [SerializeField] Boss myModel;

    [Header("Attack Values")]
    [SerializeField] int attackDamage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var x = collision.gameObject.GetComponent<PlayerModel>();
        if(x != null)
            x.PlayerHealth.TakeDamage(attackDamage, transform.position, null, GameManager.Instance.circusBossPlayerKillTime);
    }
}
