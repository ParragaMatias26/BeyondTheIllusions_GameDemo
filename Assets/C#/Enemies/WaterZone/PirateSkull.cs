using System.Collections;
using UnityEngine;

public class PirateSkull : Enemy
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float timeBeforeAttack = .5f;
    [SerializeField] private float secondAttackWaitTime = .6f;
    [SerializeField] private Vector2 attackDistanceOffset;

    [Header("AttackLunge Settings")]
    [SerializeField] private float attackLungeDistance = 1f;
    [SerializeField] private float attackLungeDuration = .1f;

    private bool canDoDamage = false;
    private int attackCount = 0;

    Coroutine attackLoungeRoutine;
    private void Start()
    {
        Initialize();

        myHealth.OnDamageTake += (_,_) => 
        {
            attackCount = 0;
        };
    }
    private void Update()
    {
        UpdateDevelopInfo();

        if (!myHealth.IsAlive) return;

        myFSM.ArtificialUpdate();
        view.AnimatorUpdateValues(movement.Velocity, movement.IsMoving());
    }
    public override IEnumerator Attack_Execute()
    {
        for (int i = 0; i < 2; i++)
        {
            var direction = target.position - transform.position;
            movement.ToggleMovement(false);

            yield return new WaitForSeconds(timeBeforeAttack);

            if(attackLoungeRoutine != null)
                StopCoroutine(attackLoungeRoutine);

            attackLoungeRoutine = StartCoroutine(AttackLunge_Execute(direction));

            view.TriggerAnimation(view._animAttackTriggerName);
            view.SetAnimatorsInt("AttackCount", attackCount);

            attackCount++;
            yield return new WaitForSeconds(secondAttackWaitTime);
        }

        attackCount = 0;
        StartCoroutine(AttackCD_Execute());

        yield return new WaitForSeconds(.65f);
        movement.ToggleMovement(true);
    }
    IEnumerator AttackLunge_Execute(Vector3 direction) 
    {
        Vector2 start = transform.position;
        Vector2 dashDir = direction.normalized;
        Vector2 end = start + dashDir * attackLungeDistance;

        movement.Velocity = dashDir;

        float elapsed = 0f;
        while (elapsed < attackLungeDuration)
        {
            canDoDamage = true;

            elapsed += Time.deltaTime;
            float t = elapsed / attackLungeDuration;
            transform.position = Vector2.Lerp(start, end, t);

            var attackDistanceStartPos = transform.position + (Vector3)attackDistanceOffset;
            if (Vector3.Distance(attackDistanceStartPos, target.position) <= attackRange && canDoDamage)
                target.GetComponent<PlayerModel>().PlayerHealth.TakeDamage(_atackDmg, transform.position, null, view.playerKillAnimTime);
            

            yield return null;
        }

        canDoDamage = false;
        attackLoungeRoutine = null;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(transform.position + (Vector3)attackDistanceOffset, attackRange);
    }
}
