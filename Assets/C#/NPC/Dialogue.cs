using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    public DialogueLine[] lines;
}
[Serializable]
public class DialogueLine 
{
    [TextArea(4,6)]
    public string text;

    public int portraitIndex = -1;
    public DialogueEventType eventType;

    [Header("Event Settings")]
    public float waitTime;
    public View myAnimator;
    public string animEventName;
    public string customEventID;

    public bool isCuteText = true;
}
public enum DialogueEventType 
{
    None,
    ChangeReality,
    GiveItem,
    PlayAnimation,
    Wait,
    Custom
}
