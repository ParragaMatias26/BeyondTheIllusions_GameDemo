using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance;

    [SerializeField] Image _inventorySlot;
    [SerializeField] Image _inventoryUI;

    [SerializeField] Sprite _cuteRealitySprite;
    [SerializeField] Sprite _darkRealitySprite;

    UISpriteAnimator _animator;
    bool wasOnCuteWorld = false;

    private void Awake()
    {
        Instance = this;
        _animator = GetComponent<UISpriteAnimator>();
    }
    private void Update()
    {
        if (!wasOnCuteWorld && RealityChangeManager.Instance.CuteWorld)
        {
            wasOnCuteWorld = true;
            _animator.Play("ToCute");
        }

        if (wasOnCuteWorld && !RealityChangeManager.Instance.CuteWorld)
        {
            wasOnCuteWorld = false;
            _animator.Play("ToDark");
        }
    }
    void InventorySlotSpriteChange()
    {
        var cute = RealityChangeManager.Instance.CuteWorld;

        if(cute) _inventoryUI.sprite = _cuteRealitySprite;
        else _inventoryUI.sprite = _darkRealitySprite;
    }

    public void UpdateInventorySlot(Sprite item)
    {
        _inventorySlot.enabled = true;
        _inventorySlot.sprite = item;
    }

    public void HideInventorySlot()
    {
        if (_inventorySlot == null) return;
        _inventorySlot.enabled = false;
    }
}
