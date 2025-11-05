using System.Collections;
using UnityEngine;
public class View : MonoBehaviour
{
    Enemy myEntity;

    [Header("Components")]
    [SerializeField] protected SpriteRenderer _cuteSpriteRenderer;
    [SerializeField] protected SpriteRenderer _darkSpriteRenderer;
    [SerializeField] protected Animator _cuteWorldAnimator;
    [SerializeField] protected Animator _darkWorldAnimator;

    [Header("Basic Animation Parameter Names")]
    [SerializeField] public string _animIsMovingBoolName = "IsMoving";
    [SerializeField] public string _animAttackTriggerName = "Attack";
    [SerializeField] public string _animDeadBoolName = "IsDeath";
    [SerializeField] public string _onPlayerKillTriggerName = "PlayerDead";

    [Header("Animation Times")]
    [SerializeField] public float playerKillAnimTime = 2f;

    [Header("On Die")]
    private FloorBlood floorBlood => myEntity.floorBlood;
    public SpriteRenderer CuteSprite { get { return _cuteSpriteRenderer; } }
    public SpriteRenderer DarkSprite { get { return _darkSpriteRenderer; } }
    public Animator CuteAnimator { get { return _cuteWorldAnimator; } }
    public Animator DarkAnimator { get { return _darkWorldAnimator; } }
    public void GetEntity(Enemy i) => myEntity = i;
    public void AnimatorUpdateValues(Vector3 velocity, bool isMoving)
    {
        if(_darkWorldAnimator != null) _darkWorldAnimator.SetBool(_animIsMovingBoolName, isMoving);
        if(_cuteWorldAnimator != null) _cuteWorldAnimator.SetBool(_animIsMovingBoolName, isMoving);

        if (myEntity.movement.Velocity.x != 0) SetSpriteDirection(velocity.x);
    }
    public void SetAnimatorsBool(string name, bool state) 
    {
        if(_darkWorldAnimator != null) _darkWorldAnimator.SetBool(name, state);
        if(_cuteWorldAnimator != null) _cuteWorldAnimator.SetBool(name, state);
    }
    public void TriggerAnimation(string animName)
    {
        if(_darkWorldAnimator != null) _darkWorldAnimator.SetTrigger(animName);
        if(_cuteWorldAnimator != null) _cuteWorldAnimator.SetTrigger(animName);
    }
    public void ResetTriggers(string animName) 
    {
        if (_darkWorldAnimator == null || _cuteWorldAnimator == null) return;

        _darkWorldAnimator.ResetTrigger(animName);
        _cuteWorldAnimator.ResetTrigger(animName);
    }
    public void SetAnimatorsInt(string name, int value) 
    {
        if (_darkWorldAnimator != null) _darkWorldAnimator.SetInteger(name, value);
        if (_cuteWorldAnimator != null) _cuteWorldAnimator.SetInteger(name, value);
    }
    public void TriggerBloodParticles() => StartCoroutine(ParticleDelayRoutine());
    public void SetSpriteDirection(float x)
    {
        if (_cuteSpriteRenderer == null || _darkSpriteRenderer == null) return;

        if (x <= 0)
        {
            _cuteSpriteRenderer.flipX = false;
            _darkSpriteRenderer.flipX = false;
        }
        else
        {
            _cuteSpriteRenderer.flipX = true;
            _darkSpriteRenderer.flipX = true;
        }
    }
    IEnumerator ParticleDelayRoutine()
    {
        floorBlood.CreateBlood();
        yield break;
    }
}
