using UnityEngine;

public class PerkPickup : InteractableProp
{
    [Header("Give Perk")]
    [SerializeField] private PerkSO perkToGive;
    private PerkManager playerPerkManager => GameManager.Instance.PlayerPerkManager;
    private PerkInventoryUI perkInventoryUI => GameManager.Instance.PerkInventoryUI;
    public override void Interact()
    {
        base.Interact();

        if (!playerPerkManager.unlockedPerks.Contains(perkToGive)) 
        {
            playerPerkManager.unlockedPerks.Add(perkToGive);
            perkInventoryUI.SetActivePerk(perkToGive);   
            Debug.Log($"Perk Desbloqueada: {perkToGive.perkName}");
        }
        else 
        {
            Debug.Log($"Ya tienes desbloqueada la perk {perkToGive.perkName}");
        }
    }
}
