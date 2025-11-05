using System.Collections;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("ViewValues")]
    [SerializeField] SpriteRenderer _playerSprite;
    [SerializeField] GameObject _shadowSprite;
    [SerializeField] Animator _animator;

    [Header("Anim Timers")]
    public float deadAnimTime = 1.5f;
    public float checkpointAnimTime = 1.5f;

    public SpriteRenderer PlayerSprite { get { return _playerSprite; } }

    [Header("ParametersNames")]
    [SerializeField] string _xFloatName;
    [SerializeField] string _yFloatName;
    [SerializeField] string _animAttackTriggerName;
    [SerializeField] string _animAttackDirName;
    [SerializeField] string _animAttackCountName;
    [SerializeField] string _animPickupTriggerName;
    [SerializeField] string _animDropTriggerName;
    [SerializeField] string _animDeadAnimationBoolName;
    [SerializeField] string _animRespawnTriggerName;
    [SerializeField] string _animPlayerDieName;
    [SerializeField] string _animIsMovingBoolName;
    public string PlayerDieAnimName { get { return _animPlayerDieName; } }
    [SerializeField] PlayerModel _myPlayer;

    int attackCounter = 0;
    Color originalColor;

    private void Start()
    {
        originalColor = _playerSprite.color;

        _myPlayer.OnAttack += PlayAttackAnimations;

        RealityChangeManager.OnCuteWorldEnabled += SetCuteWorldController;
        RealityChangeManager.OnCuteWorldDisabled += SetDarkWorldController;

        RealityChangeManager.OnCuteWorldEnabled += LightPlayerSprite;
        RealityChangeManager.OnCuteWorldDisabled += DarkenPlayerSprite;

        _myPlayer.onPickup += TriggerPickupAnimation;
        _myPlayer.onDrop += TriggerDropAnimation;

        if (CheckpointSystem.Instance != null) CheckpointSystem.Instance.OnSaveGame += () => {
            TriggerCheckpointAnimation();
        };
        
    }
    public void AnimatorUpdateValues(Vector2 velocity) 
    {
        _animator.SetBool(_animIsMovingBoolName, velocity.sqrMagnitude > 0.01f);

        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
        {
            _animator.SetFloat(_xFloatName, Mathf.RoundToInt(Mathf.Sign(velocity.x)));
            _animator.SetFloat(_yFloatName, 0);
        }
        else
        {
            _animator.SetFloat(_xFloatName, 0);
            _animator.SetFloat(_yFloatName, Mathf.RoundToInt(Mathf.Sign(velocity.y)));
        }
    }
    public void SetAttackAnimationDir(float dirIndex)
    {
        _animator.SetFloat(_animAttackDirName, (int)dirIndex);
    }
    public void SetAttackCounter(int count) => attackCounter = count;
    private void PlayAttackAnimations()
    {
        _animator.SetTrigger(_animAttackTriggerName);
        _animator.SetInteger(_animAttackCountName, attackCounter);
    }
    public void TriggerBloodParticles()
    {
        //var b = Instantiate(_bloodParticles, transform.position, Quaternion.identity);
        //b.Play();
    }
    public void TriggerPickupAnimation() => _animator.SetTrigger(_animPickupTriggerName); 
    public void TriggerDropAnimation() => _animator.SetTrigger(_animDropTriggerName);
    public void TriggerDeadAnimation(bool state)
    {
        RealityChangeManager.Instance.ToggleCuteWorld(false);
        _animator.SetBool(_animDeadAnimationBoolName, state);
    }
    public void TriggerCheckpointAnimation()
    {
        _myPlayer.Movement.CanMove = false;
        _playerSprite.enabled = false;
        _shadowSprite?.SetActive(false);

        StartCoroutine(CheckpointAnimRoutine());
    }
    IEnumerator CheckpointAnimRoutine()
    {
        yield return new WaitForSeconds(checkpointAnimTime);

        _playerSprite.enabled = true;
        _shadowSprite?.SetActive(true);
        _myPlayer.Movement.CanMove = true;
    }
    void SetCuteWorldController() => _animator.SetLayerWeight(1, 1f);
    public void SetDarkWorldController() => _animator.SetLayerWeight(1, 0f);
    void LightPlayerSprite() => _playerSprite.color = Color.white;
    void DarkenPlayerSprite() => _playerSprite.color = originalColor;
    public void HidePlayerSprite() => PlayerSprite.enabled = false;
    public void ShowPlayerSprite() => PlayerSprite.enabled = true;
}
