using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ClownNPC : NPC
{
    [Header("Componentes")]
    [SerializeField] AudioPlayer audioPlayer;
    [SerializeField] CustomPathfinder pathfinder;
    [SerializeField] BoxCollider2D boxCollider;

    [Header("View Values")]
    [SerializeField] public Animator cw_balloonsAnimator;
    [SerializeField] public Animator dw_ballonsAnimator;
    [SerializeField] public string animChaseBoolName;
    [SerializeField] public string animCelebrationTriggerName;
    [SerializeField] public string animHappyBoolName;

    [Header("Chase Settings")]
    [SerializeField] float chaseSpeed = 3f;
    [SerializeField] string chaseSoundName;
    [SerializeField] HoldingItem playerInventory;

    public bool isChasing;
    Coroutine chaseRoutine;
    Coroutine followPathRoutine;
    Transform player;
    Vector3 startPos;

    private void Start()
    {
        Initialize();

        player = GameManager.Instance.Player.transform;
        startPos = transform.position;

        if (myQuest != null)
            myQuest.OnQuestCompleted += OnQuestCompleted;
    }
    private void Update()
    {
        CheckPlayerLineOfSight();

        if(isChasing && RealityChangeManager.Instance.CuteWorld)
            EndChase();

        if(playerInventory.inventoryItem != null) 
        {
            var x = playerInventory.inventoryItem.GetComponent<BalloonItem>();
            if (x != null && !RealityChangeManager.Instance.CuteWorld && !isChasing) StartChase();
        }
        else 
        {
            EndChase();
        }
    }
    public void StartChase()
    {
        if (isChasing) return;

        isChasing = true;
        myView.DarkAnimator.SetBool(animChaseBoolName, true);
        boxCollider.enabled = false;

        var path = pathfinder.FindPath(transform.position, player.position);
        followPathRoutine = StartCoroutine(FollowPath(path));

        audioPlayer.PlaySound(chaseSoundName);
        audioPlayer.Loop(true);
    }
    public void EndChase()
    {
        if (chaseRoutine != null) StopCoroutine(chaseRoutine);
        if (followPathRoutine != null) StopCoroutine(followPathRoutine);

        audioPlayer.StopSound();
        transform.position = startPos;
        myView.DarkAnimator.SetBool(animChaseBoolName, false);
        isChasing = false;
        boxCollider.enabled = true;
    }
    IEnumerator ChasePlayer()
    {
        var p = GameManager.Instance.Player;

        while (Vector3.Distance(transform.position, player.position) > .5f)
        {
            var dir = player.position - transform.position;
            dir.Normalize();
            transform.position += dir * (chaseSpeed * Time.deltaTime);
            yield return null;
        }

        if (isChasing)
            p.PlayerHealth.TakeDamage(999, transform.position, myView.DarkAnimator, 3f);
    }
    IEnumerator FollowPath(List<CustomNode> path)
    {
        if (path == null || path.Count == 0) yield break;
        int index = 0;

        while (index < path.Count)
        {
            Vector3 goalPos = path[index].transform.position;
            while (Vector3.Distance(transform.position, goalPos) > 0.4f)
            {
                transform.position = Vector3.MoveTowards(transform.position, goalPos, chaseSpeed * Time.deltaTime);
                yield return null;
            }
            index++;
            yield return null;
        }

        if (!GameManager.Instance.InLineOfSight(transform.position, player.position))
        {
            var newPath = pathfinder.FindPath(transform.position, player.position);
            followPathRoutine = StartCoroutine(FollowPath(newPath));
        }
        else
        {
            chaseRoutine = StartCoroutine(ChasePlayer());
        }
    }
    void CheckPlayerLineOfSight()
    {
        if (!isChasing) return;
        if (GameManager.Instance.InLineOfSight(transform.position, player.position) && followPathRoutine != null)
        {
            StopCoroutine(followPathRoutine);
            followPathRoutine = null;
            chaseRoutine = StartCoroutine(ChasePlayer());
        }
    }
    private void OnQuestCompleted()
    {
        EndChase();
    }
    private void OnDestroy()
    {
        if (myQuest != null)
            myQuest.OnQuestCompleted -= OnQuestCompleted;
    }
}
