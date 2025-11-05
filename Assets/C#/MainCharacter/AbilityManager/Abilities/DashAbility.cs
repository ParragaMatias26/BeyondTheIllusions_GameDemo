using System.Collections;
using UnityEngine;

public class DashAbility : Ability
{
    PlayerModel _model;

    [SerializeField] TrailRenderer _dashTrail;
    [SerializeField] float dashTime = .2f;
    [SerializeField] float dashCD = .25f;

    Coroutine dashRoutine;
    bool _isDashing = false;
    public bool IsDashing { get { return _isDashing; } }

    private void Awake()
    {
        _model = GetComponent<PlayerModel>();
    }
    public override void Use()
    {
        if (!isUnlocked) return;
        Dash();
    }
    public void Dash() 
    {
        if (_isDashing || dashRoutine != null || !RealityChangeManager.Instance.CuteWorld)
            return;

        Debug.Log("Dash");
        dashRoutine = StartCoroutine(Dash_Execute());
    }
    private IEnumerator Dash_Execute()
    {
        Debug.Log("Dash_Execute");

        _isDashing = true;
        _model.PlayerHealth.CanTakeDamage = false;
        _dashTrail.emitting = true;

        float dashDistance = 5f;
        float dashDuration = 0.2f;
        float elapsed = 0f;

        Vector3 startPos = transform.position;
        Vector3 dashDir = _model.Movement.CurrentDir.normalized;
        Vector3 targetPos = startPos + dashDir * dashDistance;

        // Movimiento suave con Lerp
        while (elapsed < dashDuration)
        {
            float t = elapsed / dashDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Forzar posición final exacta
        transform.position = targetPos;

        // Fin del dash
        _dashTrail.emitting = false;
        _isDashing = false;
        _model.PlayerHealth.CanTakeDamage = true;
        dashRoutine = null;


        /*var ogSpeed = _model.stats.currentMoveSpeed;
        if (!_isDashing && dashRoutine == null && RealityChangeManager.Instance.CuteWorld)
        {
            _isDashing = true;
            _model.PlayerHealth.CanTakeDamage = false;
            _dashTrail.emitting = true;
            _model.stats.currentMoveSpeed *= _dashForce;
            dashRoutine = StartCoroutine(EndDashCoroutine(ogSpeed));
        }*/
    }
    IEnumerator EndDashCoroutine(float ogSpeed)
    {
        yield return new WaitForSeconds(dashTime);
        _model.Movement.MoveSpeed = ogSpeed;
        _dashTrail.emitting = false;

        yield return new WaitForSeconds(dashCD);
        _isDashing = false;
        _model.PlayerHealth.CanTakeDamage = true;

        dashRoutine = null;
    }
}
