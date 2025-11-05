using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(MovementComponent))]
[RequireComponent (typeof(HealthComponent))]
[RequireComponent(typeof(Pathfinder))]
[RequireComponent(typeof(Knockback))]
[RequireComponent(typeof(Flash))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(View))]
[RequireComponent(typeof(DamageBurst))]
[RequireComponent(typeof(FloorBlood))]
public abstract class Enemy : MonoBehaviour
{
    public MovementComponent movement;
    public View view;

    public FSM myFSM;
    public Pathfinder pathfinder;
    protected HealthComponent myHealth;
    protected Knockback myKnockback;
    protected Flash spriteFlash;
    protected DamageBurst damageVFX;
    public FloorBlood floorBlood;
    public Transform target;

    public HealthComponent Health { get { return myHealth; } }

    [Header("EnemyValues")]
    public float _detectionRadius = 5f;
    public float _stopDistance = 5f;
    public int _atackDmg = 5;
    public float hitStunDuration = .2f;
    public Vector2 timeInterAttacks = new Vector2(.5f, 1f);

    [Header("Develop Values")]
    [SerializeField] protected Develop_TextBox fsmStateInfo;
    [SerializeField] protected Develop_TextBox healthInfo;
    [SerializeField] protected Develop_TextBox isAttackingInfo;
    [SerializeField] protected Develop_TextBox attackRoutineInfo;

    [SerializeField] protected float attackAnimTime;
    public Coroutine attackRoutine;

    public Coroutine pathRoutine;
    public Coroutine stunRoutine;
    public List<Node> Path;
    public Vector3 startPos;

    public event Action OnAttack = delegate { };
    private void Awake()
    {
        startPos = transform.position;
    }
    protected void UpdateDevelopInfo()
    {
        if (fsmStateInfo != null) fsmStateInfo.UpdateText($"{myFSM._currentState}");
        if (isAttackingInfo != null) isAttackingInfo.UpdateText($"HP: {myHealth.CurrentHealth} IsAlive: {myHealth.CurrentHealth > 0}");
        if (healthInfo != null) healthInfo.UpdateText($"CanMove: {movement.CanMove}");
        if (attackRoutineInfo != null) attackRoutineInfo.UpdateText($"Current Attack: {attackRoutine != null}");
    }
    protected virtual void Initialize()
    {
        myFSM = new FSM();
        myFSM.AddState(FSM.AgentStates.Iddle, new IddleState(myFSM, this));
        myFSM.AddState(FSM.AgentStates.Chase, new ChaseState(myFSM, this));
        myFSM.AddState(FSM.AgentStates.HitStun, new HitStunState(myFSM, this));
        myFSM.AddState(FSM.AgentStates.Pathfinding, new PFState(myFSM, this.transform, this));
        myFSM.AddState(FSM.AgentStates.Attack, new AttackState(myFSM, this.transform, this));
        myFSM.AddState(FSM.AgentStates.Death, new DeathState(myFSM, this));

        myFSM.ChangeState(FSM.AgentStates.Iddle);
        GameManager.Instance.Enemies.Add(this);

        movement = GetComponent<MovementComponent>();
        view = GetComponent<View>();
        pathfinder = GetComponent<Pathfinder>();
        spriteFlash = GetComponentInChildren<Flash>();
        myHealth = GetComponent<HealthComponent>();
        myKnockback = GetComponent<Knockback>();
        damageVFX = GetComponent<DamageBurst>();
        floorBlood = GetComponent<FloorBlood>();

        movement.ToggleMovement(true);

        var rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if(view != null) view.GetEntity(this);
        target = GameManager.Instance.Player.transform;

        if(CheckpointSystem.Instance != null) 
        {
            CheckpointSystem.Instance.OnRespawn += () =>
            {
                CancelPath();
                transform.position = (Vector2)startPos;
            };
        }
        GameManager.Instance.StopMovementEvent += () => {
            movement.ToggleMovement(false);
        };
        GameManager.Instance.ResumeMovementEvent += () => {
            movement.ToggleMovement(true);
        };

        if(spriteFlash != null) {
            spriteFlash.AddSprites(view.CuteSprite, view.DarkSprite);
        }
        if (myHealth != null)
        {
            myHealth.OnDamageTake += (Vector3 instigator, Animator _) =>
            {
                spriteFlash.StartFlash();
                myKnockback.ApplyKnokckBack(instigator);
                if(damageVFX != null) damageVFX.TriggerEffect();

                myFSM.ChangeState(FSM.AgentStates.HitStun);
            };

            myHealth.OnDeath += (Animator, dieAnimTime) =>
            {
                myFSM.ChangeState(FSM.AgentStates.Death);

                CapsuleCollider2D col = GetComponent<CapsuleCollider2D>();
                col.enabled = false;

                movement.CancelMovement();
                GameManager.Instance.Enemies.Remove(this);
                StopAllCoroutines();
                attackRoutine = null;


                view.SetAnimatorsBool(view._animDeadBoolName, true);
                view.TriggerBloodParticles();
            };
        }
    }
    public virtual void Attack() 
    {
        OnAttack?.Invoke();

        if (attackRoutine != null) return;
        attackRoutine = StartCoroutine(Attack_Execute());
    }
    public virtual void StopAttack()
    {
        if (attackRoutine != null)
            StopCoroutine(attackRoutine);

        attackRoutine = null;
        movement.ToggleMovement(true);
    }
    public abstract IEnumerator Attack_Execute();
    public IEnumerator AttackCD_Execute() 
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(timeInterAttacks.x, timeInterAttacks.y));
        attackRoutine = null;
    }
    public virtual bool DetectPlayer()
    {
        if (GameManager.Instance.InLineOfSight(transform.position, target.position))
        {
            var dir = target.position - transform.position;
            return dir.magnitude < _detectionRadius;
        }
        return false;
    }
    public void FollowPath(List<Node> path) 
    {
        if (pathRoutine != null)
            StopCoroutine(pathRoutine);

        pathRoutine = StartCoroutine(FollowPathCoroutine(path));
    }
    public void CancelPath()
    {
        if (pathRoutine != null)
        {
            StopCoroutine(pathRoutine);
            pathRoutine = null;

            if (Path != null && Path.Count > 0) Path.Clear();
        }
    }
    IEnumerator FollowPathCoroutine(List<Node> path)
    {
        foreach (Node node in path)
        {
            while (Vector2.Distance(transform.position, node.worldPosition) > 0.05f)
            {
                movement.MoveTo(node.worldPosition);
                yield return null;
            }
        }

        myFSM.ChangeState(FSM.AgentStates.Iddle);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var x = collision.gameObject.GetComponent<PlayerModel>();
        if (x != null && myHealth.IsAlive) x.PlayerHealth.TakeDamage(_atackDmg, transform.position, view.DarkAnimator, view.playerKillAnimTime);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
