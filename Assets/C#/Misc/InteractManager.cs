using UnityEngine;

public class InteractManager : MonoBehaviour
{
    [Header("Interact Values")]
    [SerializeField] private float _interactRange;
    [SerializeField] private InteractableProp _interactuableItem;
    public InteractableProp InteractuableItem { get {  return _interactuableItem; } }

    public void OverlapSphereCheck_Interactuables()
    {
        Collider2D[] _interactuableItems;
        _interactuableItems = Physics2D.OverlapCircleAll(transform.position, _interactRange);

        float distance = Mathf.Infinity;

        foreach (Collider2D item in _interactuableItems)
        {
            if (_interactuableItems[0] != null)
            {
                var x = item.GetComponent<InteractableProp>();
                if (x != null && Vector3.Distance(transform.position, item.transform.position) < distance)
                {
                    if(_interactuableItem != null) _interactuableItem.ToggleInteractMark(false);

                    _interactuableItem = item.gameObject.GetComponent<InteractableProp>();
                    _interactuableItem.ToggleInteractMark(true);

                    distance = Vector3.Distance(transform.position, item.transform.position);
                }
            }

            else return;
        }

        if (distance > _interactRange) 
        {
            if(_interactuableItem != null) _interactuableItem.ToggleInteractMark(false);
            _interactuableItem = null;
        }
    }
    private void OnDrawGizmoSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactRange);
    }
}
