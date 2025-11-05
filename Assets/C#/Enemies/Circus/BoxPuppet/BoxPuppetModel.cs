using System.Collections;
using UnityEngine;

public class BoxPuppetModel : Enemy
{
    [Header("References")]
    [SerializeField] Transform head;
    [SerializeField] Transform headAnchor;
    [SerializeField] LineRenderer springLine;
    [SerializeField] GameObject headObject;

    [Header("Head Components")]
    [SerializeField] Animator cwAnimator_Head;
    [SerializeField] Animator dwAnimator_Head;
    [SerializeField] Material cwMat;
    [SerializeField] Material dwMat;

    [Header("Attack Settings")]
    [SerializeField] float launchSpeed = 15f;
    [SerializeField] float maxDistance = 6f;
    [SerializeField] float returnSpeed = 12f;
    [SerializeField] float springDamping = 6f;
    [SerializeField] float stopThreshold = 0.05f;

    [Header("View Settings")]
    [SerializeField] string endAttackTriggerName = "EndAttack";

    private void Start()
    {
        Initialize();
        myHealth.OnDamageTake += (_,_) =>
        {
            if(attackRoutine != null)
                StopCoroutine(attackRoutine);

            headObject.SetActive(false);
            head.position = headAnchor.position;

            springLine.SetPosition(0, new Vector3(headAnchor.position.x, headAnchor.position.y, 0f));
            springLine.SetPosition(1, new Vector3(headAnchor.position.x, headAnchor.position.y, 0f));

            view.ResetTriggers(view._animAttackTriggerName);
            view.TriggerAnimation(endAttackTriggerName);
            myFSM.ChangeState(FSM.AgentStates.HitStun);
        };
        myHealth.OnDeath += (_, _) => 
        {
            Destroy(headObject);

            view.ResetTriggers(endAttackTriggerName);
            view.ResetTriggers(view._animAttackTriggerName);
        };
        RealityChangeManager.OnCuteWorldEnabled += () => {
            springLine.material = cwMat;
        };
        RealityChangeManager.OnCuteWorldDisabled += () => {
            springLine.material = dwMat;
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
        base.Attack();
        attackRoutine = StartCoroutine(Attack_Execute());
    }
    public override IEnumerator Attack_Execute() 
    {
        Debug.Log($"{this.name}: Attack Execute");

        Vector3 m_target = target.position;
        movement.ToggleMovement(false);

        view.TriggerAnimation(view._animAttackTriggerName);
        yield return new WaitForSeconds(attackAnimTime);

        Vector3 start = headAnchor.position;
        Vector3 dir = (m_target - start).normalized;
        Vector3 end = start + dir * maxDistance;

        headObject.SetActive(true);

        cwAnimator_Head.SetTrigger(view._animAttackTriggerName);
        dwAnimator_Head.SetTrigger(view._animAttackTriggerName);

        float distance = Vector3.Distance(start, end);
        float t = 0f;

        while(t < 1f) 
        {
            t += Time.deltaTime * (launchSpeed / distance);
            head.position = Vector3.Lerp(start, end, t);

            if(springLine != null) 
            {
                springLine.SetPosition(0, new Vector3(headAnchor.position.x, headAnchor.position.y, 0f));
                springLine.SetPosition(1, new Vector3(head.position.x, head.position.y, 0f));
            }

            yield return null;
        }

        Vector3 velocity = Vector3.zero;
        
        while(Vector3.Distance(head.position, headAnchor.position) > stopThreshold) 
        {
            head.position = Vector3.SmoothDamp(head.position, headAnchor.position, ref velocity, 1f / springDamping, returnSpeed);

            if(springLine != null) 
            {
                springLine.SetPosition(0, new Vector3(headAnchor.position.x, headAnchor.position.y, 0f));
                springLine.SetPosition(1, new Vector3(head.position.x, head.position.y, 0f));
            }

            yield return null;
        }

        view.ResetTriggers(view._animAttackTriggerName);
        view.TriggerAnimation(endAttackTriggerName);

        head.position = headAnchor.position;
        headObject.SetActive(false);

        if (springLine != null)
        {
            springLine.SetPosition(0, headAnchor.position);
            springLine.SetPosition(1, head.position);
        }

        movement.ToggleMovement(true);
        attackRoutine = null;
    }
}
