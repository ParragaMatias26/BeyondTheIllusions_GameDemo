using UnityEngine;

public class AttackState : IState
{
    private FSM _myFSM;
    private Enemy _myEntity;
    private Transform _myTransform;
    public AttackState(FSM myFSM, Transform position, Enemy entity)
    {
        _myFSM = myFSM; _myTransform = position; _myEntity = entity;
    }

    public void OnEnter()
    {
        
    }
    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        if(Vector3.Distance(_myTransform.position, _myEntity.target.position) <= _myEntity._stopDistance && _myEntity.attackRoutine == null) 
        {
            _myEntity.Attack();
        }
        if (Vector3.Distance(_myTransform.position, _myEntity.target.position) > 3f && _myEntity.attackRoutine == null) _myFSM.ChangeState(FSM.AgentStates.Chase);
    }
}
