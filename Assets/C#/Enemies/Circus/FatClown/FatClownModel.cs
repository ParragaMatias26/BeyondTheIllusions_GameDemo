using System.Collections;
using UnityEngine;
public class FatClownModel : Enemy
{
    [Header("Combat Values")]
    [SerializeField] float attackRange;

    [Header("Minions Values")]
    [SerializeField] int minionAmmount;
    [SerializeField] float maxSpawnRange;
    [SerializeField] GameObject minionPrefab;
    void Start()
    {
        Initialize();

        myHealth.OnDeath += (_,_) => {
            SpawnMinions();
        };
    }
    void Update()
    {
        if (!myHealth.IsAlive) return;

        myFSM.ArtificialUpdate();
        view.AnimatorUpdateValues(movement.Velocity, movement.IsMoving());

        UpdateDevelopInfo();
    }
    public override void Attack()
    {
        if (attackRoutine != null || !myHealth.IsAlive) return;
        attackRoutine = StartCoroutine(Attack_Execute());
    }
    public override IEnumerator Attack_Execute() 
    {
        view.TriggerAnimation(view._animAttackTriggerName);
        movement.ToggleMovement(false);
        yield return new WaitForSeconds(attackAnimTime);

        var dist = Vector3.Distance(transform.position, target.position);
        Debug.Log($"HIT: {dist <= attackRange}");

        if (dist <= attackRange)
            target.GetComponent<PlayerModel>().PlayerHealth.TakeDamage(_atackDmg, transform.position, null, view.playerKillAnimTime);

        yield return new WaitForSeconds(.5f);

        movement.ToggleMovement(true);
        attackRoutine = null;
    }
    void SpawnMinions() 
    {
        for (int i = 0; i < minionAmmount; i++) 
        {
            var x = Random.Range(-maxSpawnRange, maxSpawnRange);
            var y = Random.Range(-maxSpawnRange, maxSpawnRange);
            var finalPos = transform.position + new Vector3(x, y, 0);

            Instantiate(minionPrefab, finalPos, Quaternion.identity);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxSpawnRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _stopDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
