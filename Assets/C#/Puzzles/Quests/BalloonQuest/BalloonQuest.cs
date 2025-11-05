using UnityEngine;

public class BalloonQuest : Quest
{
    [SerializeField] int balloonsRequired = 3;
    [SerializeField] GameObject fullBridge;
    [SerializeField] GameObject brokenBridge;
    [SerializeField] GameObject bridgeBarrier;

    int balloonsCollected = 0;

    [Header("External References")]
    [SerializeField] ClownNPC clownNPC;
    public override void StartQuest()
    {
        objectives = balloonsRequired;
        isQuestStarted = true;

        CreateMissionItem();
    }
    public override void CompleteQuest()
    {
        base.CompleteQuest();
        isQuestStarted = false;
        isQuestCompleted = true;
        missionUI.ShowCompletedIcon();

        bridgeBarrier.SetActive(false);
        brokenBridge.SetActive(false);
        fullBridge.SetActive(true);
    }
    public void CollectBalloon(BalloonItem item)
    {
        if (!isQuestStarted || isQuestCompleted)
            return;

        balloonsCollected++;
        Destroy(item.gameObject);
        if(missionUI != null) missionUI.UpdateMissionProgress(balloonsCollected);

        var v = clownNPC.ViewController;
        v.CuteAnimator.SetTrigger(clownNPC.animCelebrationTriggerName);

        clownNPC.cw_balloonsAnimator.SetInteger("Balloons", balloonsCollected);
        clownNPC.dw_ballonsAnimator.SetInteger("Balloons", balloonsCollected);

        if (balloonsCollected >= balloonsRequired)
        {
            v.CuteAnimator.SetBool(clownNPC.animHappyBoolName, true);
            CompleteQuest();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var x = collision.GetComponent<BalloonItem>();
        if(x != null && !clownNPC.isChasing) 
            CollectBalloon(x);  
    }
}
