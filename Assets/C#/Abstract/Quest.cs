using System;
using UnityEngine;
public abstract class Quest : MonoBehaviour
{
    [Header("Quest Values")]
    public string QuestName;
    [TextArea(4,6)] public string Description;
    [HideInInspector] public bool isQuestCompleted = false;
    [HideInInspector] public bool isQuestStarted = false;

    protected MissionItem missionUI;
    protected int objectives = 0;

    [Header("Inspector Values")]
    [SerializeField] protected MissionItem questItemPrefab;

    public event Action OnQuestCompleted = delegate { };
    protected virtual void CreateMissionItem()
    {
        var contentRectTransform = CanvasManager.Instance.QuestUIContentRectTransform;

        missionUI = Instantiate(questItemPrefab, contentRectTransform);
        missionUI.Initialize(QuestName, Description, objectives);
    }
    public abstract void StartQuest();
    public virtual void CompleteQuest() 
    {
        OnQuestCompleted?.Invoke();
    }
}
