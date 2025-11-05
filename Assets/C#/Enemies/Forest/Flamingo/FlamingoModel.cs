using System.Collections;
using UnityEngine;

[SelectionBase]
public class FlamingoModel : Enemy
{
    [Header("Move Change Values")]
    [SerializeField] float cuteWorldMovSpeed;

    [Header("Dash Values")]
    [SerializeField] float dashSpeed = 13f;
    [SerializeField] float overshootDistance = 1.2f;
    [SerializeField] float friction = 5f;
    [SerializeField] float dashDuration = .3f;

    private Vector3 dashTarget;

    private void Start()
    {
        Initialize();

        RealityChangeManager.OnCuteWorldEnabled += () => {
            movement.ModifySpeed(cuteWorldMovSpeed);
        };
        RealityChangeManager.OnCuteWorldDisabled += () => {
            movement.ResetSpeed();
        };
    }
    private void Update()
    {
        if (!myHealth.IsAlive) return;

        view.AnimatorUpdateValues(movement.Velocity, movement.IsMoving());
        myFSM.ArtificialUpdate();

        UpdateDevelopInfo();
    }
    public override void Attack()
    {
        if (attackRoutine != null) return;
        attackRoutine = StartCoroutine(Attack_Execute());
    }
    public override IEnumerator Attack_Execute()
    {
        movement.ToggleMovement(false);
        movement.CancelVelocity();

        yield return new WaitForSeconds(.5f);

        view.TriggerAnimation(view._animAttackTriggerName);
        StartCoroutine(AttackDashRoutine(target.position));

        yield return new WaitForSeconds(Random.Range(timeInterAttacks.x, timeInterAttacks.y));
        attackRoutine = null;
    }
    IEnumerator AttackDashRoutine(Vector3 finalPos)
    {
        Vector3 velocity = movement.Velocity;

        Vector3 directionToPlayer = (target.position - finalPos).normalized;
        dashTarget = finalPos + directionToPlayer * overshootDistance;

        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        velocity = (dashTarget - transform.position).normalized * dashSpeed;
        while (velocity.magnitude > 0.1f)
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, friction * Time.deltaTime);
            transform.position += velocity * Time.deltaTime;
            yield return null;
        }

        movement.ToggleMovement(true);

        yield return new WaitForSeconds(Random.Range(.7f, .9f));
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
