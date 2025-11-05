using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [Header("Balloon Event Settings")]
    [SerializeField] private BalloonItem[] balloons;
    [SerializeField] Animator cw_ballonsAnimator;
    [SerializeField] Animator dw_ballonsAnimator;
    [SerializeField] Animator clownAnimator;

    private void Awake()
    {
        if(Instance == null) 
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void TriggerCustomEvent(string eventID)
    {
        switch (eventID)
        {
            case "RunningBallons":
                BalloonsRunStart(balloons);
                break;
        }
    }
    private void BalloonsRunStart(BalloonItem[] balloons)
    {
        cw_ballonsAnimator.SetInteger("Balloons", 0);
        dw_ballonsAnimator.SetInteger("Balloons", 0);

        clownAnimator.SetBool("HappyIddle", false);

        foreach (var b in balloons)
        {
            if (b == null) continue;
            b.gameObject.SetActive(true);
            b.GoToEndPos();
        }
    }
}
