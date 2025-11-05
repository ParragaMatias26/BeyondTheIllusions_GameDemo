using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialogueUI;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image portraitImage;
    public Image memoryImage;

    public bool isDialogueActive = false;

    private Dialogue currentDialogue;
    private NPC currentNPC;
    private MemoryObject currentMemoryObject;
    private int currentLine;

    private Coroutine typingCoroutine;
    private bool isTyping = false;

    [Header("Reality Text Settings")]
    public Sprite cuteTextPanel;
    public Sprite darkTextPanel;
    public Color cwTextColor;
    public Color dwTextColor;

    [Header("Player Portraits")]
    [SerializeField] Sprite[] playerPortraits;

    public event Action CurrentCustomEvent = delegate { };
    private void Awake()
    {
        Instance = this;
        dialogueUI.SetActive(false);
    }

    public void StartDialogue_NPC(Dialogue dialogue, NPC npc)
    {
        if (isDialogueActive)
        {
            DisplayNextLine();
            return;
        }

        GameManager.Instance.Player._hallucinationManager.canDecrease = false;
        GameManager.Instance.ToggleAllCanvas(false);
        GameManager.Instance.StopGlobalMovement();

        currentDialogue = dialogue;
        currentNPC = npc;
        currentLine = 0;

        isDialogueActive = true;
        if (npc != null)
        {
            nameText.text = npc.npcName;

            if (npc.portraits[0] != null)
            {
                portraitImage.gameObject.SetActive(true);
                portraitImage.sprite = npc.portraits[0];
            }
        }
        else
        {
            nameText.text = "Me";
        }

        dialogueUI.SetActive(true);
        DisplayNextLine();

        foreach (var c in GameManager.Instance.AllCanvas)
            c.SetActive(false);
        GameManager.Instance.StopGlobalMovement();
    }
    public void StartDialogue_Memory(Dialogue memoryDialogue, MemoryObject obj)
    {
        if (isDialogueActive)
        {
            DisplayNextLine();
            return;
        }

        GameManager.Instance.Player._hallucinationManager.canDecrease = false;
        GameManager.Instance.ToggleAllCanvas(false);
        GameManager.Instance.StopGlobalMovement();

        if (playerPortraits[0] != null)
        {
            portraitImage.gameObject.SetActive(true);
            portraitImage.sprite = playerPortraits[0];
        }

        currentMemoryObject = obj;
        currentDialogue = memoryDialogue;
        currentLine = 0;

        isDialogueActive = true;
        nameText.text = "Me";

        memoryImage.gameObject.SetActive(true);
        memoryImage.sprite = obj.m_ImageSprite;
        memoryImage.SetNativeSize();

        dialogueUI.SetActive(true);
        DisplayNextLine();

        foreach(var c in GameManager.Instance.AllCanvas)
            c.SetActive(false);

        GameManager.Instance.StopGlobalMovement();
    }
    public void DisplayNextLine()
    {
        if (isTyping)
        {
            CompleteLineInstantly();
            return;
        }

        if (currentDialogue == null) return;

        if (currentLine >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.lines[currentLine];
        StopAllCoroutines();

        if (line.isCuteText)
        {
            dialogueUI.GetComponent<Image>().sprite = cuteTextPanel;
            dialogueText.color = cwTextColor;
            nameText.color = cwTextColor;
        }
        else
        {
            dialogueUI.GetComponent<Image>().sprite = darkTextPanel;
            dialogueText.color = dwTextColor;
            nameText.color = dwTextColor;
        }

        typingCoroutine = StartCoroutine(TypeLine(line.text));

        if (currentNPC != null && line.portraitIndex >= 0 && line.portraitIndex < currentNPC.portraits.Length)
        {
            portraitImage.sprite = currentNPC.portraits[line.portraitIndex];
        }

        HandleDialogueEvent(line);
        currentLine++;
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (var c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }

        isTyping = false;
    }

    private void CompleteLineInstantly()
    {
        if (currentDialogue == null || currentLine == 0) return;

        string fullLine = currentDialogue.lines[currentLine - 1].text;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = fullLine;
        isTyping = false;
    }

    private void HandleDialogueEvent(DialogueLine line)
    {
        var realityManager = RealityChangeManager.Instance;

        switch (line.eventType)
        {
            case DialogueEventType.ChangeReality:
                if (realityManager.CuteWorld)
                    realityManager.DisableCuteWorld();
                else
                    realityManager.EnableCuteWorld();
                break;
            case DialogueEventType.GiveItem:
                Debug.Log("Dar item");
                break;
            case DialogueEventType.PlayAnimation:
                if (line.myAnimator != null) line.myAnimator.TriggerAnimation(line.animEventName);
                Debug.Log("Reproducir animacion");
                break;
            case DialogueEventType.Wait:
                StartCoroutine(WaitAndContinue(line.waitTime));
                break;
            case DialogueEventType.Custom:
                EventManager.Instance.TriggerCustomEvent(line.customEventID);
                break;
        }
    }
    private IEnumerator WaitAndContinue(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        DisplayNextLine();
    }

    private void EndDialogue()
    {
        if (currentNPC != null) currentNPC.StartQuest();
        if(currentMemoryObject != null) 
        {
            Destroy(currentMemoryObject.gameObject);
            currentMemoryObject = null;
        }

        GameManager.Instance.Player._hallucinationManager.canDecrease = true;
        GameManager.Instance.ToggleAllCanvas(true);
        GameManager.Instance.ResumeGlobalMovement();

        dialogueUI.SetActive(false);
        memoryImage.gameObject.SetActive(false);

        currentDialogue = null;
        isDialogueActive = false;

        foreach (var c in GameManager.Instance.AllCanvas)
            c.SetActive(true);
        GameManager.Instance.ResumeGlobalMovement();
    }
}
