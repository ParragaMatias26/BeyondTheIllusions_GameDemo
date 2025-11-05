using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject dialogueMark;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image textBoxImage;
    [SerializeField] private Image uiImage;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text nameText;

    [Header("Interact Values")]
    [SerializeField] bool onlyInteractOnce = false;
    [SerializeField] bool onlyInteractOnCuteWorld = false;
    bool hasInteract = false;

    [Header("Quest Options")]
    [SerializeField] bool haveAnyQuest;
    [SerializeField] Quest myQuest;

    private bool isQuestDeliver;

    [Header("Text Values")]
    [SerializeField] string _npcName;
    [SerializeField] float dialogueTime;
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;

    [Header("Index with Reality Change")]
    [SerializeField] bool startInDarkWorld;
    [SerializeField] int[] index;

    [Header("Images")]
    [SerializeField] Sprite _cuteRealityImageToShow;
    [SerializeField] Sprite _darkRealityImageToShow;
    [SerializeField] Sprite _cuteRealityTextBox;
    [SerializeField] Sprite _darkRealityTextBox;

    [Header("Colors")]
    [SerializeField] Color _cuteWorldTextColor;
    [SerializeField] Color _darkWorldTextColor;

    private bool isPlayerOnRange;
    private bool didDialogueStart;
    private int lineIndex;

    bool actualReality;

    private void Start()
    {
        actualReality = RealityChangeManager.Instance.CuteWorld;
    }

    private void Update()
    {
        if (isPlayerOnRange && Input.GetKeyDown(KeyCode.E) && !hasInteract) if (!didDialogueStart) StartDialogue();
        else if (dialogueText.text == dialogueLines[lineIndex]) NextDialogueLine();

        else
        {
            StopAllCoroutines();
            dialogueText.text = dialogueLines[lineIndex];
        }

        if (!_cuteRealityImageToShow || !_darkRealityImageToShow) return;

        if (!actualReality)
        {
            uiImage.sprite = _cuteRealityImageToShow;
            textBoxImage.sprite = _cuteRealityTextBox;
            dialogueText.color = _cuteWorldTextColor;
            nameText.color = _cuteWorldTextColor;
        }
        else 
        {
            uiImage.sprite = _darkRealityImageToShow;
            textBoxImage.sprite = _darkRealityTextBox;
            dialogueText.color = _darkWorldTextColor;
            nameText.color = _darkWorldTextColor;
        } 
    }

    public void StartDialogue()
    {
        if (onlyInteractOnCuteWorld) 
        {
            if(!RealityChangeManager.Instance.CuteWorld) return;
        }

        nameText.text = _npcName;
        uiImage.sprite = _cuteRealityImageToShow;

        didDialogueStart = true;
        dialoguePanel.SetActive(true);
        dialogueMark.SetActive(false);

        if (_cuteRealityImageToShow != null) uiImage.gameObject.SetActive(true);

        lineIndex = 0;

        StartCoroutine(ShowLine());

        foreach(GameObject canvas in GameManager.Instance.AllCanvas)
        {
            canvas.SetActive(false);
        }

        GameManager.Instance.Player.Movement.CanMove = false;
    }

    void NextDialogueLine()
    {
        lineIndex++;

        foreach(int i in index)
        {
            if(i == lineIndex)
            {
                actualReality = !actualReality;

                if (actualReality)
                {
                    RealityChangeManager.Instance.ToggleCuteWorld(false);
                }
                else 
                {
                    RealityChangeManager.Instance.ToggleCuteWorld(true);
                } 
            }
        }

        if(lineIndex < dialogueLines.Length) StartCoroutine(ShowLine());

        else
        {
            didDialogueStart = false;
            dialoguePanel.SetActive(false);
            dialogueMark.SetActive(true);

            if(uiImage != null) uiImage.gameObject.SetActive(false);

            foreach (GameObject canvas in GameManager.Instance.AllCanvas)
            {
                canvas.SetActive(true);
            }

            RealityChangeManager.Instance.ToggleCuteWorld(false);

            if (haveAnyQuest && !isQuestDeliver)
            {
                myQuest.StartQuest();
                isQuestDeliver = true;
            }

            GameManager.Instance.Player.Movement.CanMove = true;
            if (onlyInteractOnce) hasInteract = true;
        }
    }

    IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;

        foreach(char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(dialogueTime);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        var x = col.GetComponent<PlayerModel>();
        if(x != null)
        {
            isPlayerOnRange = true;
            dialogueMark.SetActive(true);
        }
    }
    
    void OnTriggerExit2D(Collider2D col)
    {
        var x = col.GetComponent<PlayerModel>();
        if (x != null)
        {
            isPlayerOnRange = false;
            dialogueMark.SetActive(false);
        }
    }
}
