using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionItem : MonoBehaviour
{
    [Header("MissionItem Values")]
    [SerializeField] TMP_Text missionNameText;
    [SerializeField] TMP_Text missionDescText;
    [SerializeField] TMP_Text missionProgressText;
    [SerializeField] GameObject completedMissionIcon;

    [Header("View Components")]
    [SerializeField] Image BG;
    [SerializeField] Image Object;
    [SerializeField] Image Completed;

    [Header("CuteWorld - Sprites")]
    [SerializeField] Sprite Cute_BG;
    [SerializeField] Sprite Cute_Object;
    [SerializeField] Sprite Cute_Completed;

    [Header("DarkWorld - Sprites")]
    [SerializeField] Sprite Dark_BG;
    [SerializeField] Sprite Dark_Object;
    [SerializeField] Sprite Dark_Completed;

    int totalObjectives;
    private void Update()
    {
        UpdateSprites();
    }
    public void Initialize(string name, string desc, int total)
    {
        missionNameText.text = name;
        missionDescText.text = desc;

        totalObjectives = total;
        missionProgressText.text = $"0 / {total}";

        completedMissionIcon.SetActive(false);
    }

    public void UpdateMissionProgress(int obtained)
    {
        missionProgressText.text = $"{obtained} / {totalObjectives}";
    }

    public void ShowCompletedIcon()
    {
        completedMissionIcon.SetActive(true);
    }

    void UpdateSprites()
    {
        if (RealityChangeManager.Instance.CuteWorld)
        {
            BG.sprite = Cute_BG;
            Object.sprite = Cute_Object;
            Completed.sprite = Cute_Completed;
        }
        else
        {
            BG.sprite = Dark_BG;
            Object.sprite = Dark_Object;
            Completed.sprite = Dark_Completed;
        }
    }
}
