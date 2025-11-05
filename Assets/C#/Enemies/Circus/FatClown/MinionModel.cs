using System.Collections;
using UnityEngine;

public class MinionModel : Enemy
{
    private void Start()
    {
        Initialize();
        StartCoroutine(SpawnMovementRoutine());
    }
    private void Update()
    {
        view.AnimatorUpdateValues(movement.Velocity, movement.IsMoving());
        myFSM.ArtificialUpdate();
    }
    public override void Attack()
    {
        
    }
    public override IEnumerator Attack_Execute()
    {
        yield return null;        
    }
    IEnumerator SpawnMovementRoutine() 
    {
        var c = GetComponent<CapsuleCollider2D>();
        c.enabled = false;
        movement.ToggleMovement(false);
        yield return new WaitForSeconds(1f);

        c.enabled = true;
        movement.ToggleMovement(true);
    }

}
