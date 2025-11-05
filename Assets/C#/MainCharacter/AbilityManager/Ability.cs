using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public string abilityName;
    public bool isUnlocked;

    public abstract void Use();
}
