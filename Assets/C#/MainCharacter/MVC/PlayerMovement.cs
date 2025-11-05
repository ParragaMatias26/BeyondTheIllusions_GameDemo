using System.Collections;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float currentMovSpeed = 0f;
    [SerializeField] private bool canMove = true;

    [Header("Raycast Settings")]
    [SerializeField] float moveRayDist = .5f;
    [SerializeField] Vector2 raycastOffset;

    private PlayerModel model => GetComponent<PlayerModel>();

    private Vector3 lastDir;
    private Vector3 currentDir = Vector3.zero;
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    public float MoveSpeed {  get { return currentMovSpeed; } set { currentMovSpeed = value; } }
    public Vector3 LastDir { get { return lastDir; } }
    public Vector3 CurrentDir { get { return currentDir; } }

    private Coroutine currentMovementRoutine;
    private Coroutine stopMovementRoutine;

    public event Action OnMove = delegate { };
    public event Action OnMoveStop = delegate { };
    public void MoveTo(Vector3 dir, float speed) 
    {
        if (!canMove || !CanMoveRay(model.PlayerController.GetInputs())) 
        {
            currentDir = Vector3.zero;
            return;
        }

        dir.Normalize();
        transform.position += dir * (speed * Time.deltaTime);
        currentDir = dir;

        if (dir.x != 0) lastDir = dir;
        if (dir.x == 0 && dir.y == 0) currentDir = Vector3.zero;
    }
    public void MoveToPosition(Vector3 endPos, float speed) 
    {
        if (currentMovementRoutine != null)
            StopCoroutine(currentMovementRoutine);

        currentMovementRoutine = StartCoroutine(MoveToPosition_Execute(endPos, speed));
    }
    private IEnumerator MoveToPosition_Execute(Vector3 endPos, float speed) 
    {
        CanMove = false;
        while(Vector3.Distance(transform.position, endPos) > .05f) 
        {
            var dir = endPos - transform.position;
            dir.Normalize();

            transform.position += dir * (speed * Time.deltaTime);
            currentDir = dir;

            if (dir.x != 0) lastDir = dir;
            yield return null;
        }

        CanMove = true;
        currentDir = Vector3.zero;
        transform.position = endPos;
    }
    public void StopMovementInTime(float time) 
    {
        if(stopMovementRoutine != null)
            StopCoroutine(stopMovementRoutine);

        stopMovementRoutine = StartCoroutine(StopMovementInTime_Execute(time));
    }
    private IEnumerator StopMovementInTime_Execute(float time) 
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
    public bool CanMoveRay(Vector3 dir) 
    {
        int obstacleLayerMask = LayerMask.GetMask("Obstacles");
        Vector3 startPos = transform.position + (Vector3)raycastOffset;

        Debug.DrawRay(startPos, dir * moveRayDist, Color.green);
        return !Physics2D.Raycast(startPos, dir, moveRayDist, obstacleLayerMask);
    }
    public bool IsMoving()
    {
        if (!canMove) return false;
        return currentDir.x != 0 || currentDir.y != 0;
    }
    public void UpdateSpeed()
    {
        if (RealityChangeManager.Instance.CuteWorld)
            model.stats.currentMoveSpeed = model.stats.cw_MovementSpeed;
        else
            model.stats.currentMoveSpeed = model.stats.dw_MovementSpeed;
    }
}
