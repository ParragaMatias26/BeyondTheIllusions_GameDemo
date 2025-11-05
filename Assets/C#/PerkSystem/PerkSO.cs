using UnityEngine;

public abstract class PerkSO : ScriptableObject
{
    [Header("General Info")]
    public string perkName;
    public string description;
    public Sprite icon;
    public int uiIndex = 0;

    [Header("Gameplay Settings")]
    public int cost = 1;
    public abstract void Apply(PlayerStats stats, GameObject player);
    public virtual void Remove(PlayerStats stats, GameObject player) { }
}
