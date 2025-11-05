
public class Boss_DeathState : IState
{
    private FSM _myFSM;
    private Boss _myEntity;
    public Boss_DeathState(FSM myFSM, Boss myEntity)
    {
        _myFSM = myFSM;
        _myEntity = myEntity;
    }
    public void OnEnter()
    {
        //Reproducir animacion de muerte.
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }
}
