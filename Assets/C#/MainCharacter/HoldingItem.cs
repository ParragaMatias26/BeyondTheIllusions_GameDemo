using UnityEngine;

public class HoldingItem : MonoBehaviour
{
    [Header("PickupValues")]
    [SerializeField] float _pickupRange = 3f;
    [SerializeField] Transform _pickupPos;

    public PickeableObject objOnRange;
    public PickeableObject inventoryItem;

    Vector3 cursorPos = Vector3.zero;
    PlayerModel playerModel;

    private void Start()
    {
        playerModel = GetComponent<PlayerModel>();
    }

    private void Update()
    {
        OverlapSphereCheck();
        GetDirectionToCursor();
    }

    public void ObjectPick()
    {
        var h = playerModel._hallucinationManager.HallucinationAmmount;

        if (objOnRange.OnlyCuteWorldPickeable && h < 10f) return;

        if (inventoryItem == null && objOnRange != null)
        {
            playerModel.OnPickupEvent();

            objOnRange.Pick(_pickupPos);
            inventoryItem = objOnRange;
        }
    }
    public void DropObject()
    {
        if (inventoryItem != null)
        {
            inventoryItem.Drop();
            inventoryItem = null;

            playerModel.OnDropEvent();
        }
    }
    public void ThrowObject(Vector3 dir)
    {
        if(inventoryItem != null)
        {
            inventoryItem.Throw(dir);
            inventoryItem = null;
        }
    }
    void OverlapSphereCheck()
    {
        Collider2D[] posiblePickups;
        posiblePickups = Physics2D.OverlapCircleAll(transform.position, _pickupRange);

        float distance = Mathf.Infinity;

        foreach (Collider2D item in posiblePickups)
        {
            if (posiblePickups[0] != null)
            {
                var x = item.GetComponent<PickeableObject>(); 
                if (x != null && Vector3.Distance(transform.position, item.transform.position) < distance)
                {
                    objOnRange = x;
                    distance = Vector3.Distance(transform.position, item.transform.position);

                    objOnRange.ShowPickFrame();
                }
            }

            else return;
        }

        if (distance > _pickupRange) 
        {
            distance = Mathf.Infinity;
            if(objOnRange != null) objOnRange.HidePickFrame();
            objOnRange = null;
        } 
    }
    public Vector3 GetDirectionToCursor()
    {
        cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPos.z = 0;

        return (cursorPos - transform.position).normalized;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow + Color.blue;
        Gizmos.DrawWireSphere(transform.position, _pickupRange);
    }
}
