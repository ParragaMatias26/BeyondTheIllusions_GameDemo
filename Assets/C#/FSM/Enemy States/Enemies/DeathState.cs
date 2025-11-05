using UnityEngine;
public class DeathState : IState
{
    public DeathState(FSM myFSM, Enemy entity)
    {

    }

    public void OnEnter()
    {

    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        Debug.Log($"A");
    }
}
