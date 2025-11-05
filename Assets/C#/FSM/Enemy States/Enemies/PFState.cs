using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFState : IState
{
    private FSM _myFSM;
    private Enemy _myEntity;
    private Transform _myTransform;

    Vector3 _lastSeenPos;

    public PFState(FSM myFSM, Transform position, Enemy entity)
    {
        _myFSM = myFSM; _myTransform = position; _myEntity = entity;
    }
    public void OnEnter()
    {
        _lastSeenPos = _myEntity.target.position;
        if (_lastSeenPos == null) 
        {
            Debug.LogWarning($"Posicion Invalida.");
            return;
        }

        _myEntity.movement.ApplyPathfinding(_lastSeenPos);
    }

    public void OnExit()
    {
        _myEntity.Path = null;
        _myEntity.CancelPath();
    }

    public void OnUpdate()
    {
        if (_myEntity.DetectPlayer())
            _myFSM.ChangeState(FSM.AgentStates.Chase);
            
    }
}
