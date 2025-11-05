using UnityEngine;
[RequireComponent (typeof(BoxCollider2D))]
public class AbilityUnlocker : MonoBehaviour
{
    public string abilityToUnlock;
    private void Start()
    {
        var trigger = GetComponent<BoxCollider2D>();
        if(trigger != null)
        {
            trigger.isTrigger = true;
            trigger.size = new Vector2(2f, 2f);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        var x = other.gameObject.GetComponent<PlayerModel>();
        if (x != null)
        {
            var manager = other.GetComponent<AbilityManager>();
            manager.UnlockAbility(abilityToUnlock);
            Destroy(gameObject);
        }
    }
}
