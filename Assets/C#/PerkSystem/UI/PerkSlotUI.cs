using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class PerkSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;

    private PerkSO perk;
    private PerkManager perkManager => GameManager.Instance.PlayerPerkManager;
    public bool HasPerk => perk != null;
    public void Initialize(PerkSO perk)
    {
        ClearSlot();
        SetPerk(perk);
    }
    public void SetPerk(PerkSO newPerk) => perk = newPerk;
    public void ClearSlot()
    {
        perk = null;
        infoPanel.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (perk == null) return;

        infoPanel.SetActive(true);
        infoPanel.transform.SetAsLastSibling();

        nameText.text = perk.perkName;
        descriptionText.text = perk.description;
        costText.text = "Costo: " + perk.cost;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        infoPanel.SetActive(false);
    }
    public void BTN_Click()
    {
        if (perk == null) return;

        if (perkManager.equippedPerks.Contains(perk))
            perkManager.UnequipPerk(perk);
        else
            perkManager.EquipPerk(perk);
    }
}
