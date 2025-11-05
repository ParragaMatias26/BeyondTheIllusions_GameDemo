using System.Collections;
using UnityEngine;

public class MimicEnemy : MonoBehaviour, IDamageable, IInteractuable
{
    [Header("EnemyValues")]
    [SerializeField] float _maxHealth = 25f;
    [SerializeField] float _interactionRange = 5f;
    [SerializeField] float _movSpeed = 3f;

    [Header("Attack Values")]
    [SerializeField] float _attackRange = 1.5f;
    [SerializeField] float duration = 1f;
    [SerializeField] private float heightY = 3f;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private CoinProyectile coinPrefab;
    [SerializeField] private GameObject throwProjectileShadow;


    [Header("ViewValues")]
    [SerializeField] SpriteRenderer _sprite;
    [SerializeField] GameObject _buttonMark;
    [SerializeField] Animator _cuteWorldAnimator;
    [SerializeField] Animator _darkWorldAnimator;

    [Header("Animator Parameter Names")]
    [SerializeField] string _animIsActiveName;
    [SerializeField] string _animIsAttackingName;
    [SerializeField] string _animVelocityName;

    float currentHeal;
    Coroutine attackRoutine;
    Coroutine attackCD;

    bool _active = false;
    PlayerModel _target;

    bool _canMove = true;
    bool _isKnockedBack = false;
    bool isAttacking = false;

    Flash _spriteFlash;

    private void Start()
    {
        _target = GameManager.Instance.Player;
        currentHeal = _maxHealth;

        _spriteFlash = GetComponent<Flash>();
    }

    private void Update()
    {
        if (_active) MoveToTarget();

        ShowButtonMark();
        AnimatorSetValues();
        CheckDistance();
    }

    void MoveToTarget()
    {
        if (!_canMove) return;

        var dir = _target.transform.position - transform.position;
        dir.Normalize();

        _cuteWorldAnimator.SetInteger(_animVelocityName, (int)dir.magnitude);
        _darkWorldAnimator.SetInteger(_animVelocityName, (int)dir.magnitude);

        transform.position += dir * _movSpeed * Time.deltaTime;
    }

    void CheckDistance()
    {
        if (Vector2.Distance(transform.position, _target.transform.position) <= _attackRange && attackRoutine == null && _active && attackCD == null)
        {
            StopMovement();
            attackRoutine = StartCoroutine(AttackRoutine());
        }
    }

    void StopMovement() => _canMove = false;
    void ResumeMovement() => _canMove = true;

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        var pos = _target.transform.position;

        for (int i = 0; i < 3; i++)
        {
            if(i == 0) pos = _target.transform.position;
            else
            {
                var x = Random.Range(-2f, 2f);
                var y = Random.Range(-2f, 2f);
                pos = new Vector3(_target.transform.position.x + x, _target.transform.position.y + y);
            }

            var dir = pos - transform.position;

            ThrowCoin(dir);
            yield return new WaitForSeconds(1f);
        }

        attackRoutine = null;
        isAttacking = false;
        attackCD = StartCoroutine(AttackCDRoutine());
    }
    IEnumerator AttackCDRoutine()
    {
        yield return new WaitForSeconds(2.5f);
        attackCD = null;
        ResumeMovement();
    }

    void ThrowCoin(Vector3 dir)
    {
        CoinProyectile coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
        GameObject itemShadow = Instantiate(throwProjectileShadow, transform.position + new Vector3(0, -0.3f, 0), Quaternion.identity);

        coin.Initialize(duration, animCurve, heightY, itemShadow, dir);
    }

    void ApplyKnockBack(Vector3 knockbackTarget, float knockbackSpeed)
    {
        if (_isKnockedBack)
        {
            transform.position = Vector3.MoveTowards(transform.position, knockbackTarget, knockbackSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, knockbackTarget) < .1f)
            {
                _isKnockedBack = false;
            }
        }
    }

    public void TakeDamage(int damage, Vector3 instigator, Animator instAnim, float dieAnimTime)
    {
        _spriteFlash.StartFlash();

        currentHeal -= damage;
        if (currentHeal <= 0) Die(instAnim, 0);
    }

    public void Die(Animator instAnim, float dieAnimTime)
    {
        Destroy(gameObject);
    }

    void ShowButtonMark()
    {
        if(Vector3.Distance(transform.position, _target.transform.position) <= _interactionRange && RealityChangeManager.Instance.CuteWorld && !_active) _buttonMark.SetActive(true);
        else _buttonMark.SetActive(false);
    }

    void AnimatorSetValues()
    {
        _cuteWorldAnimator.SetBool(_animIsActiveName, _active);
        _darkWorldAnimator.SetBool(_animIsActiveName, _active);

        _darkWorldAnimator.SetBool(_animIsAttackingName, isAttacking);
        _cuteWorldAnimator.SetBool(_animIsAttackingName, isAttacking);
    }

    public void Interact()
    {
        if(RealityChangeManager.Instance.CuteWorld && !_active)
        {
            _target.PlayerHealth.TakeDamage(5, transform.position, null, 0f);
            _active = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
