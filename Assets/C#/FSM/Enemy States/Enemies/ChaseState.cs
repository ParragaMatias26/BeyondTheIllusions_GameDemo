using UnityEngine;

public class ChaseState : IState
{
    private FSM _myFSM;
    private Enemy _myEntity;
    public ChaseState(FSM myFSM, Enemy entity)
    {
        _myFSM = myFSM; _myEntity = entity;
    }

    public void OnEnter()
    {
        
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        if (Vector3.Distance(_myEntity.transform.position, _myEntity.target.position) < _myEntity._stopDistance) _myFSM.ChangeState(FSM.AgentStates.Attack);
        if (!_myEntity.DetectPlayer()) _myFSM.ChangeState(FSM.AgentStates.Pathfinding);

        _myEntity.movement.MoveTo(_myEntity.target.position);
    }
}
