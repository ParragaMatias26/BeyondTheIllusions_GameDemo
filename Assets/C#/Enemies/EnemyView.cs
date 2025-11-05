using System.Collections;
using UnityEngine;
public class EnemyView : MonoBehaviour
{
    Enemy myEntity;

    [Header("Components")]
    [SerializeField] private SpriteRenderer _cuteSpriteRenderer;
    [SerializeField] private SpriteRenderer _darkSpriteRenderer;
    [SerializeField] private Animator _cuteWorldAnimator;
    [SerializeField] private Animator _darkWorldAnimator;
    [SerializeField] private string _animIsMovingBoolName = "IsMoving";
    [SerializeField] private string _animAttackTriggerName = "Attack";
    [SerializeField] private string _animHitTriggerName = "Hit";
    [SerializeField] private string _animDeadBoolName = "IsDeath";

    [Header("On Die")]
    [SerializeField] private GameObject bloodParticles;
    [SerializeField] private FloorBlood floorBlood;

    public SpriteRenderer CuteSprite { get { return _cuteSpriteRenderer; } }
    public SpriteRenderer DarkSprite { get { return _darkSpriteRenderer; } }
    public Animator CuteAnimator { get { return _cuteWorldAnimator; } }
    public Animator DarkAnimator { get { return _darkWorldAnimator; } }
    public void GetEntity(Enemy i) => myEntity = i;
    public void AnimatorSetValues(Vector3 velocity, bool isMoving)
    {
        _darkWorldAnimator.SetBool(_animIsMovingBoolName, isMoving);
        _cuteWorldAnimator.SetBool(_animIsMovingBoolName, isMoving);

        if (myEntity.movement.Velocity.x != 0) SetSpriteDirection(velocity.x);

        //var fadePercentaje = RealityChangeManager.Instance.HallucinationAmmount / RealityChangeManager.Instance.MaxHallucination;
        //_cuteSpriteRenderer.color = new Color(1f,1f,1f, fadePercentaje);
        //_darkSpriteRenderer.color = new Color(1f, 1f, 1f, 1f - fadePercentaje);
    }
    public void TriggerAttackAnim()
    {
        _darkWorldAnimator?.SetTrigger(_animAttackTriggerName);
        _cuteWorldAnimator?.SetTrigger(_animAttackTriggerName);
    }
    public void TriggerHitAnim() => _cuteWorldAnimator?.SetTrigger(_animHitTriggerName);
    public void TriggerDieAnim()
    {
        _cuteWorldAnimator.SetBool(_animDeadBoolName, true);
        _darkWorldAnimator.SetBool(_animDeadBoolName, true);
        StartCoroutine(ParticleDelayRoutine());
    }
    IEnumerator ParticleDelayRoutine()
    {
        floorBlood.CreateBlood();

        yield return new WaitForSeconds(.3f);
        Instantiate(bloodParticles, transform.position, Quaternion.identity);
    }

    public void SetSpriteDirection(float x)
    {
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
}
