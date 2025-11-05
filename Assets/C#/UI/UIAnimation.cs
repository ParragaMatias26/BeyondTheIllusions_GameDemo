using UnityEngine;

[CreateAssetMenu(fileName = "NewUIAnimation", menuName = "UI/UIAnimation")]
public class UIAnimation : ScriptableObject
{
    public string animationName;
    public Sprite[] frames;
    public float frameRate = 0.1f;

    public bool loop = false;
    public bool hasExitTime = false;
}