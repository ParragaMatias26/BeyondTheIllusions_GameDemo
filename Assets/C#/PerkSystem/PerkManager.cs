using System.Collections.Generic;
using UnityEngine;

public class PerkManager : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] PlayerModel myPlayer;
    [SerializeField] PerkInventoryUI PerksUI;

    [Header("Perks Data")]
    public List<PerkSO> unlockedPerks = new List<PerkSO>();
    public List<PerkSO> equippedPerks = new List<PerkSO>();

    [Header("Perk Capacity")]
    public int maxCost = 4;
    public int currentCost = 0;
    public bool CanEquip(PerkSO perk) 
    {
        return currentCost + perk.cost <= maxCost && !equippedPerks.Contains(perk);
    }
    public void EquipPerk(PerkSO perk) 
    {
        if (CanEquip(perk)) 
        {
            equippedPerks.Add(perk);
            perk.Apply(myPlayer.stats, gameObject);

            currentCost += perk.cost;
            PerksUI.UpdateUI();
        }
        else 
        {
            Debug.LogWarning($"No se puede equipar: {perk.perkName}");
        }
    }
    public void UnequipPerk(PerkSO perk) 
    {
        if (equippedPerks.Contains(perk)) 
        {
            currentCost -= perk.cost;
            perk.Remove(myPlayer.stats, gameObject);

            equippedPerks.Remove(perk);
            PerksUI.UpdateUI();
        }
    }
}
