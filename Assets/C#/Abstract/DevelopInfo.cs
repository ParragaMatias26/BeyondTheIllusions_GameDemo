using UnityEngine;

public abstract class DevelopInfo : MonoBehaviour
{
    private void Start()
    {
        if(!GameManager.Instance.AllDevelopInfo.Contains(this))
            GameManager.Instance.AllDevelopInfo.Add(this);
    }
}
