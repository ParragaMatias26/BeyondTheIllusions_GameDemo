using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(MovementComponent))]
[RequireComponent(typeof(DamageBurst))]
[RequireComponent(typeof(Flash))]
[RequireComponent(typeof(View))]
public abstract class Boss : MonoBehaviour
{
    [HideInInspector] public HealthComponent health;
    [HideInInspector] public MovementComponent movement;
    [HideInInspector] public View view;

    private Rigidbody2D rb;

    [HideInInspector] public FSM myFSM;
    protected Flash spriteFlash;
    protected DamageBurst damageBurst;

    [HideInInspector] public bool canAttack = true;
    [HideInInspector] public bool isBossActive = false;
    [HideInInspector] public bool hasBossStarted = false;
    [HideInInspector] public Coroutine currentAttack;
    [HideInInspector] public Coroutine currentAttackCD;

    protected bool hasIntroPlayed = false;
    protected bool attackInterrupted = false;
    protected int currentBossPhase = 1;

    private Vector3 startPos;
    protected Transform target;

    protected List<BossAttack> myAttacks = new List<BossAttack>();
    protected BossAttack lastUsedAttack;

    public event Action OnBossStart = delegate { };
    public event Action OnBossStop = delegate { };
    public event Action OnBossAttack = delegate { };
    public event Action OnBossCancelAttack = delegate { };
    public event Action OnBossDamage = delegate { };
    public event Action OnBossDeath = delegate { };
    public event Action OnBossReset = delegate { };
    public event Action OnAttackCDStart = delegate { };
    public event Action OnAttackCDEnd = delegate { };

    [SerializeField] protected int contactDamage = 5;
    [SerializeField][Tooltip("Tiempo que se toma el jefe entre ataques")] protected float attackCooldown = 1.5f;
    [SerializeField] protected float introAnimTime;
    [SerializeField] protected float playerKillAnimTime;
    [SerializeField] protected bool enableAttackCDEvents = false;
    public float KillAnimTime { get { return playerKillAnimTime; } }
    protected void Initialize() 
    {
        startPos = transform.position;
        target = GameManager.Instance.Player.transform;

        myFSM = new FSM();
        myFSM.AddState(FSM.AgentStates.BossIddle, new Boss_IddleState(myFSM, this));
        myFSM.AddState(FSM.AgentStates.BossAttack, new Boss_AttackState(myFSM, this));
        myFSM.AddState(FSM.AgentStates.BossDeath, new Boss_DeathState(myFSM, this));

        myFSM.ChangeState(FSM.AgentStates.BossIddle);

        view = GetComponent<View>();
        health = GetComponent<HealthComponent>();
        movement = GetComponent<MovementComponent>();
        spriteFlash = GetComponent<Flash>();
        damageBurst = GetComponent<DamageBurst>();

        SetBossPhase(1);

        if(spriteFlash != null) {
            spriteFlash.AddSprites(view.CuteSprite, view.DarkSprite);
        }

        rb = GetComponent<Rigidbody2D>();
        if(rb != null) 
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 0f;
        }
    }
    public void PlayBossIntro()
    {
        if (!hasIntroPlayed)
        {
            StartCoroutine(BossIntro_Execute());
            hasIntroPlayed = true;
        }
        else 
        {
            StartBoss();
        }   
    }
    public abstract IEnumerator BossIntro_Execute();
    public virtual void StartBoss() 
    {
        if (!hasBossStarted)
            StartCoroutine(StartBoss_Execute(introAnimTime));
        else
            StartCoroutine(StartBoss_Execute(1f));
    }
    public virtual void StopBoss() 
    {
        StopAttack();
        StopAllCoroutines();

        isBossActive = false;
        OnBossStop?.Invoke();
    }
    public virtual IEnumerator StartBoss_Execute(float delay) 
    {
        yield return new WaitForSeconds(delay);
        hasBossStarted = true;
        isBossActive = true;

        OnBossStart?.Invoke();
        SetBossPhase(1);
    }
    public virtual void SelectRandomAttack()
    {
        var randomIndex = UnityEngine.Random.Range(0, myAttacks.Count);
        if (myAttacks[randomIndex] == lastUsedAttack && myAttacks.Count > 1)
        {
            SelectRandomAttack();
            return;
        }

        canAttack = false;
        lastUsedAttack = myAttacks[randomIndex];
        currentAttack = StartCoroutine(myAttacks[randomIndex].Execute());
    }
    protected IEnumerator AttackCD(float cooldown) 
    {
        if (enableAttackCDEvents) OnAttackCDStart.Invoke();
        yield return new WaitForSeconds(cooldown);

        if (enableAttackCDEvents) OnAttackCDEnd.Invoke();
        canAttack = true;
    }
    public abstract void SetBossPhase(int index);
    public virtual void ResetBoss() 
    {
        myAttacks.Clear();
        SetBossPhase(1);

        transform.position = startPos;
        health.ResetHealth();

        OnBossReset?.Invoke();
        canAttack = true;
    }
    public virtual void Attack()
    {
        OnBossAttack?.Invoke();
    }
    public virtual void StopAttack() 
    {
        if(currentAttack != null) 
        {
            StopCoroutine(currentAttack);
            OnBossCancelAttack?.Invoke();
        }
        if(currentAttackCD != null) 
        {
            StopCoroutine(currentAttackCD);
            currentAttackCD = null;
        }

        currentAttack = null;
    }
    public virtual void Death() 
    {
        StopBoss();
        view.SetAnimatorsBool(view._animDeadBoolName, true);

        myFSM.ChangeState(FSM.AgentStates.BossDeath);
        OnBossDeath?.Invoke();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var x = collision.gameObject.GetComponent<PlayerModel>();
        if (x != null)
            x.PlayerHealth.TakeDamage(contactDamage, transform.position, null, playerKillAnimTime);
    }
}
