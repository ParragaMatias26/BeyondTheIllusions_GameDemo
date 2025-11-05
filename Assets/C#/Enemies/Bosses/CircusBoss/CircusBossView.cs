using System.Collections;
using UnityEngine;
using System;

public class CircusBossView : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Animator cwAnimator;
    [SerializeField] Animator dwAnimator;
    //[SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] PlatformManager platformManager;

    [Header("Animation Parameters Names")]
    [SerializeField] string introPhaseIntName;
    [SerializeField] string hurtTriggerName;
    [SerializeField] string jumpTriggerName;
    [SerializeField] string screamBoolName;
    [SerializeField] string playerKillBoolName;

    [Header("Boss Intro Values")]
    [SerializeField] Transform startPos;
    [SerializeField] Transform endPos;
    [SerializeField] float descentSpeed;

    [Header("Juggle Attack Values")]
    [SerializeField] string juggleAttackTriggerName;
    [SerializeField] string juggleEndTriggerName;
    [SerializeField] float juggleAttackTime;

    [Header("Spiner Attack Values")]
    [SerializeField] string spinAttackBoolName;
    [SerializeField] float spinAttackTime;
    [SerializeField] float playerKillAnimTime;

    public float PlayerKillAnimTime { get { return playerKillAnimTime; } }

    public bool hasIntroPlayed = false;
    public event Action OnIntroEnd = delegate { };

    private void Start()
    {
        if (!hasIntroPlayed) 
        {
            transform.position = startPos.position;
            hasIntroPlayed = true;
        }

        CheckpointSystem.Instance.OnRespawn += () => {
            dwAnimator.SetBool(playerKillBoolName, false);
        };
    }

    public void SetIntroPhaseIndex(int index) => cwAnimator.SetInteger(introPhaseIntName, index);
    public void AnimSetBool(string boolName, bool state) 
    {
        if (cwAnimator != null) cwAnimator.SetBool(boolName, state);
        if (dwAnimator != null) dwAnimator.SetBool(boolName, state);
    }
    public void TriggerAnimation(string animName) 
    {
        if(cwAnimator != null) cwAnimator.SetTrigger(animName);
        if(dwAnimator != null) dwAnimator.SetTrigger(animName);
    }
    public void StartBossIntro() 
    {
        Debug.Log("Iniciar intro jefe");
        StartCoroutine(BossIntroCutscene());
    }
    IEnumerator BossIntroCutscene() 
    {
        Debug.Log("INICIANDO");
        SetIntroPhaseIndex(1);
        yield return new WaitForSeconds(.2f);

        while(Vector2.Distance(transform.position, endPos.position) > 0.05f) 
        {
            var dir = (endPos.position - transform.position).normalized;
            transform.position += dir * (descentSpeed * Time.deltaTime);

            yield return null;
        }

        Debug.Log("Bajada completada");

        transform.position = endPos.position;
        SetIntroPhaseIndex(2);

        yield return new WaitForSeconds(.5f);
        OnIntroEnd?.Invoke();
    }
    public void StartJuggleAttack() => StartCoroutine(JuggleAttackSequence());
    IEnumerator JuggleAttackSequence() 
    {
        TriggerAnimation(juggleAttackTriggerName);
        yield return new WaitForSeconds(juggleAttackTime);
        TriggerAnimation(juggleEndTriggerName);
    }
    public void StartSpinAttack() => StartCoroutine(SpinAttackSequence());
    IEnumerator SpinAttackSequence() 
    {
        AnimSetBool(spinAttackBoolName, true);
        yield return new WaitForSeconds(spinAttackTime);
        AnimSetBool(spinAttackBoolName, false);
    }
    public void TriggerHurtAnimation() => TriggerAnimation(hurtTriggerName);
    public void TriggerJumpAnimation() => TriggerAnimation(jumpTriggerName);
    public void ToggleSceamAnimation(bool state) => dwAnimator.SetBool(screamBoolName, state);
    public void TogglePlayerKillName(bool state) => dwAnimator.SetBool(playerKillBoolName, state);
}
