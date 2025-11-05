using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class PlayerModel : MonoBehaviour
{
    public PlayerStats stats;

    PlayerView _myView;
    PlayerController _myController;
    Flash _spriteFlash;
    DamageBurst _burst;
    Knockback _knockback;
    HoldingItem _holdingItem;
    HealthComponent _health;
    InteractManager _interactManager;
    PlayerColliderSystem _playerColliderSystem;
    PlayerMovement _playerMovement;
    public PlayerController PlayerController { get { return _myController; } }
    public PlayerMovement Movement { get { return _playerMovement; } }
    public HoldingItem PlayerInventory { get { return _holdingItem; } }
    public HealthComponent PlayerHealth { get { return _health; } }

    [Header("External Reference")]
    public HallucinationManager _hallucinationManager;
    public AbilityManager _abilityManager;

    [Header("PlayerComponents")]
    [SerializeField] Transform _swordTransform;
    [SerializeField] SwordDMGCollider _swordCollider;
    [SerializeField] CapsuleCollider2D m_Collider;

    int attackCount = 0;
    bool _canAttack = true;

    public bool _canPickup = false;
    public event Action OnAttack = delegate { };
    public event Action OnPlayerHit = delegate { };
    public static event Action OnPlayerDie = delegate { };

    Coroutine AttackComboRoutine;
    Coroutine Routine_AttackLounge;
    Coroutine Routine_AttackKnockback;
    Coroutine Routine_PlayerDieRoutine;
    SpriteRenderer lastInstigator = null;

    public event Action onPickup = delegate { };
    public event Action onDrop = delegate { };

    private void Awake()
    {
        _health = GetComponent<HealthComponent>();
        _holdingItem = GetComponent<HoldingItem>();
        _knockback = GetComponent<Knockback>();
        _spriteFlash = GetComponent<Flash>();
        _interactManager = GetComponent<InteractManager>();
        _playerColliderSystem = GetComponent<PlayerColliderSystem>();
        _playerMovement = GetComponent<PlayerMovement>();
        _burst = GetComponent<DamageBurst>();
        
        _myController = new PlayerController(this, _abilityManager);
        if (_myView == null) _myView = GetComponent<PlayerView>();

        if(_spriteFlash != null)
        {
            _spriteFlash.AddSprites(_myView.PlayerSprite, null);
        }

        if(_health != null) 
        {
            _health.OnDamageTake += (Vector3 inst, Animator instAnim) =>
            {
                _knockback.ApplyKnokckBack(inst);
                if (BloodCanvas.Instance != null) BloodCanvas.Instance.CreateBlood();

                _myView.TriggerBloodParticles();
                _hallucinationManager.DecraseHallucination(stats.lostHallucinationOnDamage);

                _spriteFlash.StartFlash();
                _burst.TriggerEffect();

                if(CanvasManager.Instance != null) 
                {
                    if (RealityChangeManager.Instance.CuteWorld)
                    {
                        CanvasManager.Instance.PlayerUIAnimator.Play("CuteTakeDamage");
                        CanvasManager.Instance.PlayerUIAnimator.Play("CuteIddle");
                    }
                    else if (!RealityChangeManager.Instance.CuteWorld)
                    {
                        CanvasManager.Instance.PlayerUIAnimator.Play("DarkTakeDamage");
                        CanvasManager.Instance.PlayerUIAnimator.Play("DarkIddle");
                    }

                    CanvasManager.Instance.UpdateHealthBar(_health.CurrentHealth);
                }
            };

            _health.OnDeath += (Animator instigatorAnim, float dieAnimDuration) => 
            {
                _knockback.CancelKnockback();
                GameManager.Instance.StopGlobalMovement();

                _myView.HidePlayerSprite();
                if (instigatorAnim != null) instigatorAnim.SetBool(_myView.PlayerDieAnimName, true);

                if (Routine_PlayerDieRoutine != null)
                    StopCoroutine(Routine_PlayerDieRoutine);

                 PlayerDie(instigatorAnim, dieAnimDuration);
            };
        }

        if(GameManager.Instance != null) 
        {
            GameManager.Instance.StopMovementEvent += () => _playerMovement.CanMove = false;
            GameManager.Instance.ResumeMovementEvent += () =>  _playerMovement.CanMove = true;
        }
    }
    private void Start()
    {
        GameManager.Instance.Player = this;
        _swordCollider.Initialize(stats.swordDamage, this.transform, this);

        _myView = GetComponent<PlayerView>();
        _hallucinationManager = GetComponent<HallucinationManager>();

        RealityChangeManager.OnCuteWorldEnabled += () => {
            stats.currentMoveSpeed = stats.cw_MovementSpeed;
        };
        RealityChangeManager.OnCuteWorldDisabled += () => {
            stats.currentMoveSpeed = stats.dw_MovementSpeed;
        }; 
    }
    private void Update()
    {
        _playerMovement.UpdateSpeed();
        _playerMovement.MoveTo(_myController.GetInputs(), stats.currentMoveSpeed);
        SetAttackDirection();

        _interactManager?.OverlapSphereCheck_Interactuables();
        _myController.UpdateInputs(_holdingItem, _interactManager.InteractuableItem);
        _myView?.AnimatorUpdateValues(_playerMovement.CurrentDir);

        if (_holdingItem.inventoryItem != null && !RealityChangeManager.Instance.CuteWorld && _holdingItem.inventoryItem.DropOnDarkWorld) _holdingItem.DropObject();

        if(_playerColliderSystem != null) 
        {
            _playerColliderSystem.DetectObjects(_myController.GetInputs());
            _playerColliderSystem.UpdateColliders();
        }
    }
    void SetAttackDirection()
    {
        var playerScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (playerScreenPos.z <= 0f)
            return;

        var cursorScreenPos = Input.mousePosition;

        var directionToCursor = (cursorScreenPos - playerScreenPos).normalized;

        float angle = Mathf.Atan2(directionToCursor.y, directionToCursor.x) * Mathf.Rad2Deg;
        angle -= 90f;

        if (angle < 0) angle -= 360;

        int directionIndex = Mathf.RoundToInt(-angle / 90f) % 4;

        float fixDirIndex = Mathf.Repeat(directionIndex, 4);

        _myView.SetAttackAnimationDir((int)fixDirIndex);

        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;

        // 0 - Arriba
        // 1 - Derecha
        // 2 - Abajo
        // 3 - Izquierda

        switch (fixDirIndex)
        {
            case (0):
                _swordTransform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case (1):
                _swordTransform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case (2):
                _swordTransform.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case (3):
                _swordTransform.rotation = Quaternion.Euler(0, 0, 180);
                break;
        }

        if (_canAttack) Attack(directionToCursor);
    }
    public void Heal() 
    {
        if(_hallucinationManager.HallucinationAmmount >= stats.hallucinationToHeal) 
        {
            _hallucinationManager.HallucinationAmmount = stats.hallucinationToHeal;
            _health.Heal(stats.healAmmount);

            CanvasManager.Instance.UpdateHealthBar(_health.CurrentHealth);
        }
        else 
        {
            Debug.Log("Alucinacion insuficiente para curarse");
        }
    }
    public void Attack(Vector2 attackDir)
    {
        if (!RealityChangeManager.Instance.CuteWorld || !Movement.CanMove) return;
        _swordCollider.gameObject.SetActive(true);

        _playerMovement.CanMove = false;

        OnAttack();
        AttackLounge(attackDir);

        attackCount++;
        if (attackCount >= 3f) attackCount = 0;
        _myView.SetAttackCounter(attackCount);

        _canAttack = false;

        if(AttackComboRoutine == null)
        {
            AttackComboRoutine = StartCoroutine(AttackComboTimer());
        }
        else
        {
            StopCoroutine(AttackComboRoutine);
            AttackComboRoutine = StartCoroutine(AttackComboTimer());
        }

        StartCoroutine(EndAttackCoroutine());
    }
    IEnumerator AttackComboTimer()
    {
        yield return new WaitForSeconds(3f);

        attackCount = 0;
        _myView.SetAttackCounter(attackCount);
        AttackComboRoutine = null;
    }
    private void AttackLounge(Vector2 attackDir) 
    {
        if (!_playerMovement.CanMoveRay(attackDir)) return;

        if (Routine_AttackLounge != null)
        {
            StopCoroutine(Routine_AttackLounge);
            Routine_AttackLounge = null;
        }

        Routine_AttackLounge = StartCoroutine(AttackLunge_Execute(attackDir));
    }
    private IEnumerator AttackLunge_Execute(Vector2 attackDir)
    {
        Vector2 start = transform.position;
        Vector2 dashDir = attackDir.normalized;

        RaycastHit2D hit = Physics2D.Raycast(start, dashDir, stats.attackLungeDistance * 1.2f, LayerMask.GetMask("Enemies"));
        bool hitEnemy = hit.collider != null;

        if (hitEnemy) 
        {
            Routine_AttackLounge = null;
            yield break;
        }

        Vector2 end = start + dashDir * stats.attackLungeDistance;

        float elapsed = 0f;
        while (elapsed < stats.attackLungeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / stats.attackLungeDuration;
            transform.position = Vector2.Lerp(start, end, t);
            yield return null;
        }

        Routine_AttackLounge = null;
    }
    public void ApplyAttackKnockback(Vector2 attackDir, bool hitEnemy) 
    {
        Vector2 knockback = -attackDir.normalized * stats.attackKnockbackDistance;

        if(Routine_AttackKnockback != null) StopCoroutine(Routine_AttackKnockback);
        Routine_AttackKnockback = StartCoroutine(KnockbackCoroutine(knockback, stats.attackKnockbackDuration));
    }
    private IEnumerator KnockbackCoroutine(Vector2 knockback, float duration)
    {
        Vector2 start = transform.position;
        Vector2 end = start + knockback;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.position = Vector2.Lerp(start, end, t);
            yield return null;
        }
    }
    IEnumerator EndAttackCoroutine()
    {
        yield return new WaitForSeconds(stats.attackCD / 2f);
        _swordCollider.gameObject.SetActive(false);
        _canAttack = true;

        _playerMovement.CanMove = true;
    }
    public void PlayerDie(Animator instigatorAnim, float deadAnimTime) => Routine_PlayerDieRoutine = StartCoroutine(DieRoutine(instigatorAnim, deadAnimTime));
    IEnumerator DieRoutine(Animator instigatorAnim, float deadAnimTime)
    {
        OnPlayerDie?.Invoke();

        if(instigatorAnim != null)
            Debug.Log($"Instigator: {instigatorAnim.gameObject.name}, Tiempo de Animacion: {deadAnimTime}");
        else
            Debug.Log($"Instigator: Null, Tiempo de Animacion: {deadAnimTime}");

        yield return new WaitForSeconds(deadAnimTime);
        Debug.Log("Respawneando.");
        Respawn(instigatorAnim);
    }
    void Respawn(Animator instigatorAnim)
    {
        if (lastInstigator != null) lastInstigator.enabled = true;

        _myView.ShowPlayerSprite();
        if(instigatorAnim != null) instigatorAnim.SetBool(_myView.PlayerDieAnimName, false);
        GameManager.Instance.ResumeGlobalMovement();

        _holdingItem.DropObject();

        _playerMovement.CanMove = true;
        _myView.SetDarkWorldController();

        CheckpointSystem.Instance.RespawnInCheckpoint(transform);

        _health.ResetHealth();
        CanvasManager.Instance.UpdateHealthBar(_health.CurrentHealth);
    }
    public void ToggleCollider(bool state) => m_Collider.enabled = state;
    public void OnPickupEvent() => onPickup();
    public void OnDropEvent() => onDrop();
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, transform.position + _myController.GetInputs() * 5f);
    }
}

[System.Serializable]
public class PlayerStats
{
    [Header("Movement Stats")]
    public float cw_MovementSpeed = 10f;
    public float dw_MovementSpeed = 4f;
    public float currentMoveSpeed = 4f;

    [Header("Health Values")]
    public int healAmmount = 5;
    public float hallucinationToHeal = 15f;

    [Header("Combat Values")]
    public int swordDamage = 5;
    public float attackCD = .35f;
    public float lostHallucinationOnDamage = 15f;
    public float attackLungeDistance = 0.7f;
    public float attackLungeDuration = 0.1f;
    public float attackKnockbackDistance = 1f;
    public float attackKnockbackDuration = .2f;
}
