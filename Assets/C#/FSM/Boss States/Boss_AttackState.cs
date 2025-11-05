
public class Boss_AttackState : IState
{
    private FSM _myFSM;
    private Boss _myEntity;
    public Boss_AttackState(FSM myFSM, Boss myEntity)
    {
        _myFSM = myFSM;
        _myEntity = myEntity;
    }
    public void OnEnter()
    {
        _myEntity.SelectRandomAttack();
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        if (_myEntity.currentAttack == null && _myEntity.canAttack)
            _myFSM.ChangeState(FSM.AgentStates.BossIddle);
            
    }
}
