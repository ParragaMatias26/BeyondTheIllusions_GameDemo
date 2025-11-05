using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkInventoryUI : MonoBehaviour
{
    [SerializeField] GameObject[] perksSlots;
    [SerializeField] PerkSlotUI perkButtonPrefab;

    [Header("Equiped Perk UI Settings")]
    [SerializeField] PerkManager m_PerkManager;
    [SerializeField] Transform perksContainer;
    [SerializeField] GameObject perkSlotPrefab;
    [SerializeField] TextMeshProUGUI costText;

    private void Start()
    {
        UpdateUI();
    }
    public void SetActivePerk(PerkSO perk)
    {
        perksSlots[perk.uiIndex].SetActive(true);

        var p = perksSlots[perk.uiIndex].GetComponentInChildren<PerkSlotUI>();
        p.Initialize(perk);

    }
    public void UpdateUI()
    {
        foreach (Transform child in perksContainer)
            Destroy(child.gameObject);

        foreach (var perk in m_PerkManager.equippedPerks)
        {
            var slot = Instantiate(perkSlotPrefab, perksContainer);
            slot.GetComponentInChildren<Image>().sprite = perk.icon;
        }

        costText.text = $"Coste Total: {m_PerkManager.currentCost} / {m_PerkManager.maxCost}";
    }
}
