using System.Collections;

[System.Serializable]
public class BossAttack
{
    public string attackName;
    public System.Func<IEnumerator> Execute;
}
