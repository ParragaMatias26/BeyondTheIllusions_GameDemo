using UnityEngine;

public class IddleState : IState
{
    private FSM _myFSM;
    private Enemy _myEntity;

    public IddleState(FSM myFSM, Enemy entity)
    {
        _myFSM = myFSM; _myEntity = entity;
    }

    public void OnEnter()
    {
        var dist = Vector2.Distance(_myEntity.transform.position, _myEntity.startPos);
        if(dist > 1f) _myEntity.movement.GoToPosition(_myEntity.startPos);
    }

    public void OnExit()
    {
        _myEntity.movement.CancelMovement();
    }

    public void OnUpdate()
    {
        if(_myEntity.DetectPlayer())
            _myFSM.ChangeState(FSM.AgentStates.Chase);
    }
}
