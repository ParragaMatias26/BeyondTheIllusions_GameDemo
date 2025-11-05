using System.Collections;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(BoxCollider2D))]
public class EntranceTeleport : InteractableProp
{
    [Header("Transition Settings")]
    [SerializeField] Transform startPos;
    [SerializeField] Transform endPos;
    [SerializeField] float transitionTime = 2f;
    
    
    [SerializeField] Transform tp_End;

    Coroutine transitionRoutine;
    PlayerModel playerModel;

    private void Start()
    {
        BoxCollider2D trigger = GetComponent<BoxCollider2D>();
        trigger.isTrigger = true;

        playerModel = GameManager.Instance.Player;
    }

    public override void Interact()
    {
        if (transitionRoutine != null) return;

        playerModel.transform.position = startPos.position;
        playerModel.ToggleCollider(false);
        transitionRoutine = StartCoroutine(TransitionAnimation());
        GameManager.Instance.StopGlobalMovement();
        TransitionCanvas.Instance.FadeScreenEffect();

        canShowMark = false;
    }

    IEnumerator TransitionAnimation()
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            playerModel.transform.position = Vector3.Lerp(startPos.position, endPos.position, elapsedTime / transitionTime);
            yield return null;
        }

        playerModel.transform.position = tp_End.position;
        GameManager.Instance.MainCam.transform.position = new Vector3(tp_End.position.x, tp_End.position.y, GameManager.Instance.MainCam.transform.position.z);

        transitionRoutine = null;

        GameManager.Instance.ResumeGlobalMovement();
        playerModel.ToggleCollider(true);

        canShowMark = true;
    }
}
