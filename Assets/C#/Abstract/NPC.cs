using Unity.VisualScripting;
using UnityEngine;
[SelectionBase]
[RequireComponent(typeof(View))]
public abstract class NPC : InteractableProp, IInteractuable
{
    protected View myView;
    public View ViewController { get { return myView; } }

    [Header("NPC Settings")]
    public string npcName;
    public bool onlyInteractOnce = false;
    public Quest myQuest;

    [Header("Dialogues")]
    public Dialogue startDialogue;
    public Dialogue currentQuestDialogue;
    public Dialogue completeQuestDialogue;

    [Header("Expressions")]
    public Sprite[] portraits;

    protected bool canInteractHallucination => GameManager.Instance.Player._hallucinationManager.HallucinationAmmount > 15f;
    protected bool interactedOnce = false;
    protected bool playerInRange = false;
    protected void Initialize()
    {
        myView = GetComponent<View>();

        if (myQuest != null)
            myQuest.OnQuestCompleted += TriggerCompletedDialogue;
    }
    public override void Interact()
    {
        base.Interact();

        if (onlyInteractOnce && interactedOnce && !DialogueManager.Instance.isDialogueActive) return;
        if (onlyInteractOnCuteWorld && !canInteractHallucination && !DialogueManager.Instance.isDialogueActive) return;
        if (myQuest.isQuestCompleted && !DialogueManager.Instance.isDialogueActive) return;

        if (DialogueManager.Instance.isDialogueActive)
        {
            DialogueManager.Instance.DisplayNextLine();
            return;
        }

        if (!DialogueManager.Instance.isDialogueActive && !interactedOnce)
            DialogueManager.Instance.StartDialogue_NPC(startDialogue, this);
        else if (!DialogueManager.Instance.isDialogueActive && interactedOnce)
            DialogueManager.Instance.StartDialogue_NPC(currentQuestDialogue, this);

        interactedOnce = true;
    }
    public void StartQuest() 
    {
        if (myQuest != null && !myQuest.isQuestStarted && !myQuest.isQuestCompleted) myQuest.StartQuest();
    }
    void TriggerCompletedDialogue()
    {
        if (completeQuestDialogue != null)
            DialogueManager.Instance.StartDialogue_NPC(completeQuestDialogue, this);

        canShowMark = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var x = collision.GetComponent<PlayerModel>();
        playerInRange = x != null;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var x = collision.GetComponent<PlayerModel>();
        playerInRange = false;
    }

}
