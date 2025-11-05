
public class Boss_IddleState : IState
{
    private FSM _myFSM;
    private Boss _myEntity;
    public Boss_IddleState(FSM myFSM, Boss myEntity) 
    {
        _myFSM = myFSM;
        _myEntity = myEntity;
    }
    public void OnEnter()
    {
        
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        if (!_myEntity.isBossActive || !_myEntity.canAttack) return;

        if (_myEntity.currentAttack == null)
            _myFSM.ChangeState(FSM.AgentStates.BossAttack);
    }
}
