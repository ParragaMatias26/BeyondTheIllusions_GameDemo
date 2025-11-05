using UnityEngine;
public class DashWall : MonoBehaviour
{
    public float openDuration = 0.2f;
    private bool isPassable = false;

    [SerializeField] DashAbility ability;
    [SerializeField] BoxCollider2D myCollider;

    Coroutine openRoutine;

    private void Update()
    {
        if(!ability) return;

        if (ability.IsDashing && openRoutine == null) Destroy(gameObject);
        BlockPlayer();
    }
    private void BlockPlayer() => myCollider.isTrigger = isPassable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var x = other.GetComponent<DashAbility>();
        if (x != null) ability = x;
    }
}
