using UnityEngine;

public class InteractableProp : MonoBehaviour, IInteractuable
{
    [Header("Interact Mark")]
    [SerializeField] protected GameObject interactMark;
    [SerializeField] public bool onlyInteractOnCuteWorld = true;
    [SerializeField] public bool destroyOnPickup = false;

    [HideInInspector] public bool canShowMark = true;
    private void Start()
    {
        interactMark.SetActive(false);
    }
    public virtual void Interact() 
    {
        if (onlyInteractOnCuteWorld && !RealityChangeManager.Instance.CuteWorld) return;
        if(destroyOnPickup) Destroy(gameObject);
    }
    public virtual void ToggleInteractMark(bool state) 
    {
        if (!canShowMark) 
        {
            interactMark.SetActive(false);
            return;
        }

        interactMark.SetActive(state);
    }
}
