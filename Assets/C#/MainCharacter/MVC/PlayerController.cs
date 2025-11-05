using UnityEngine;

public class PlayerController
{
    PlayerModel _model;
    AbilityManager _abilityManager;
    public PlayerController(PlayerModel model, AbilityManager abilityManager) 
    {
        _model = model; _abilityManager = abilityManager;
    }

    public void UpdateInputs(HoldingItem item, IInteractuable interactuable)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var dash = _abilityManager.GetAbilityByName("Dash");
            if (dash != null && dash.isUnlocked) dash.Use();
        }

        if(Input.GetKeyDown(KeyCode.Tab)) 
            CanvasManager.Instance.ToggleMissionPanel();

        if (Input.GetKeyDown(KeyCode.E)) 
        {
            if (interactuable != null)
            {
                interactuable.Interact();
                return;
            }

            if (_model._canPickup)
            {
                if (item.objOnRange != null && item.inventoryItem == null)
                {
                    item.ObjectPick();
                    return;
                }

                if (item.inventoryItem != null && item.inventoryItem.Throweable)
                {
                    item.ThrowObject(item.GetDirectionToCursor());
                    return;
                }

                if (item.inventoryItem != null && !item.inventoryItem.Throweable)
                {
                    item.DropObject();
                    return;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            _model.Heal();
        }
    }

    public Vector3 GetInputs()
    {
        if (!_model.Movement.CanMove) return Vector3.zero;

        var x = Input.GetAxisRaw("Horizontal"); 
        var y = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(x, y);
        dir.Normalize();

        return dir;
    }
}
