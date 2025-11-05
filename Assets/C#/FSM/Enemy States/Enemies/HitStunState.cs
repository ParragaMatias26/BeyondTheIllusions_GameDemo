using System.Collections;
using UnityEngine;

public class HitStunState : IState
{
    private FSM _myFSM;
    private Enemy _myEntity;
    public HitStunState(FSM myFSM, Enemy entity)
    {
        _myFSM = myFSM; _myEntity = entity;
    }
    public void OnEnter()
    {
        _myEntity.StopAllCoroutines();

        if (_myEntity.Health.CurrentHealth <= 0)
        {
            _myFSM.ChangeState(FSM.AgentStates.Death);
            return;
        }

        if(_myEntity.stunRoutine != null) _myEntity.StopCoroutine(_myEntity.stunRoutine);
        _myEntity.stunRoutine = _myEntity.StartCoroutine(HitStunDuration());
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }
    IEnumerator HitStunDuration() 
    {
        yield return new WaitForSeconds(_myEntity.hitStunDuration);
        _myEntity.stunRoutine = null;

        _myEntity.movement.ToggleMovement(true);
        _myEntity.StopAttack();

        _myFSM.ChangeState(FSM.AgentStates.Iddle);
    }
}
