using System.Collections;
using UnityEngine;

[SelectionBase]
public class Checkpoint : InteractableProp
{
    [Header("View Values")]
    [SerializeField] Animator animator;
    [SerializeField] GameObject[] lights;

    [Header("Animator Parameters")]
    [SerializeField] string animSaveBoolName;
    [SerializeField] string animPressKeyTriggerName;

    [SerializeField] Transform endPos;
    public SpriteRenderer mySprite;

    public Vector3 EndPos { get { return endPos.position; } }

    bool isSave = false;
    Coroutine mRoutine;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public override void Interact()
    {
        if (mRoutine != null) return;

        canShowMark = false;

        PlaySaveAnimation();
        CheckpointSystem.Instance.GetNewCheckpoint(this);

        mRoutine = StartCoroutine(StopMovementRoutine());
        GameManager.Instance.Player.transform.position = endPos.position;
    }
    IEnumerator StopMovementRoutine() 
    {
        GameManager.Instance.StopGlobalMovement();
        yield return new WaitForSeconds(2.5f);
        GameManager.Instance.ResumeGlobalMovement();
        mRoutine = null;
        canShowMark = true;
    }

    public void SetCheckSprite(bool state) 
    {
        isSave = state;
        foreach (var light in lights) light.gameObject.SetActive(state);
    } 

    void PlaySaveAnimation()
    {
        animator.SetBool(animSaveBoolName, true);
        if (isSave) animator.SetTrigger(animPressKeyTriggerName);
    }
}
