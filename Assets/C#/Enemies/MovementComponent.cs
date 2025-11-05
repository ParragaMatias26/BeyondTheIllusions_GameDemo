using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MovementComponent : MonoBehaviour
{
    Enemy _myEntity;
    HealthComponent m_HealthComponent;

    [Header("Movement Values")]
    [SerializeField] float _detectionRadius = 5f;
    [SerializeField] float _movSpeed = 3f;
    [SerializeField] float _maxForce = 1f;
    [SerializeField] float _smoothTime = 0.1f;
    [SerializeField] bool canMove;

    Coroutine movementRoutine;
    float ogSpeed;

    [SerializeField] Vector3 _velocity;
    public Vector3 Velocity { get { return _velocity; } set { _velocity = value; } }
    public bool CanMove { get { return canMove; } }

    private void Start()
    {
        ogSpeed = _movSpeed;

        GameManager.Instance.StopMovementEvent += () => 
        {
            ToggleMovement(false);
            CancelVelocity();
        }; 
        GameManager.Instance.ResumeMovementEvent += () => ToggleMovement(true);

        m_HealthComponent = GetComponent<HealthComponent>();
        _myEntity = GetComponent<Enemy>();
    }
    public void ToggleMovement(bool state) => canMove = state;
    public void ModifySpeed(float ammount) => _movSpeed = ammount;
    public void ResetSpeed() => _movSpeed = ogSpeed;
    public void MoveTo(Vector3 finalPos) => Flocking(finalPos);
    public void GoToPosition(Vector3 finalPos)
    {
        CancelMovement();
        if(movementRoutine != null) 
        {
            StopCoroutine(movementRoutine);
            movementRoutine = null;
        }
        movementRoutine = StartCoroutine(GoToPositionRoutine(finalPos));
    }
    IEnumerator GoToPositionRoutine(Vector3 finalPos) 
    {
        if(GameManager.Instance.InLineOfSight(transform.position, finalPos)) 
        {
            float dist = Vector3.Distance(transform.position, finalPos);
            while (dist > .5f)
            {
                MoveTo(finalPos);
                dist = Vector3.Distance(transform.position, finalPos);

                yield return null;
            }

            CancelMovement();
        }
        else ApplyPathfinding(finalPos);
    }
    public void ApplyPathfinding(Vector3 finalPos) 
    {
        CancelMovement();
        _myEntity.Path = _myEntity.pathfinder.GetPath(transform.position, finalPos);

        if (_myEntity.Path != null)
        {
            _myEntity.FollowPath(_myEntity.Path);
        }
        else
        {
            var node = _myEntity.pathfinder.CalculateNearNode(transform.position);
            transform.position = node.transform.position;
            _myEntity.myFSM.ChangeState(FSM.AgentStates.Iddle);

            Debug.LogWarning("Enemigo Stuckeado");
        }
    }
    public void CancelMovement() 
    {
        if(movementRoutine != null) 
        {
            StopCoroutine(movementRoutine);
            CancelVelocity();

            movementRoutine = null;
        }
    }
    public void CancelVelocity() => _velocity = Vector3.zero;
    void Flocking(Vector3 target)
    {
        Vector3 steer = Vector3.zero;

        steer += Pursuit(target);

        steer += Separation(GameManager.Instance.Enemies, GameManager.Instance.radiusSeparation) * GameManager.Instance.weightSeparation;
        steer += Aligment(GameManager.Instance.Enemies, _detectionRadius) * GameManager.Instance.weightAligment;
        steer += Cohesion(GameManager.Instance.Enemies, _detectionRadius) * GameManager.Instance.weightCohesion;

        AddForce(steer);
    }

    public bool IsMoving()
    {
        if(!canMove) return false;
        return Velocity.x != 0 || Velocity.y != 0;
    }

    void AddForce(Vector3 dir)
    {
        if (!canMove)
        {
            _velocity = Vector3.zero;
            return;
        }

        _velocity = Vector3.ClampMagnitude(Velocity + dir, _movSpeed);
        Vector3 targetPosition = transform.position + _velocity * Time.deltaTime;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);
    }

    #region Flocking

    Vector3 Pursuit(Vector3 position)
    {
        Vector3 desired = position - transform.position;
        desired.Normalize();
        desired *= _movSpeed;

        var steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, 1);

        return steering;
    }
    Vector3 Separation(List<Enemy> boids, float radius)
    {
        Vector3 desired = Vector3.zero;

        foreach (var boid in boids)
        {
            var dir = boid.transform.position - transform.position;

            if (dir.magnitude > radius || boid == this)
                continue;

            desired -= dir;
        }

        if (desired == Vector3.zero)
            return desired;

        desired.Normalize();
        desired *= _movSpeed;

        var steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce);

        return steering;
    }
    Vector3 Aligment(List<Enemy> boids, float radius)
    {
        Vector3 desired = Vector3.zero;
        int count = 0;

        foreach (Enemy item in boids)
        {
            if (item == this)
                continue;

            if (Vector3.Distance(transform.position, item.transform.position) <= radius)
            {
                desired += item.movement.Velocity;
                count++;
            }
        }

        if (count <= 0)
            return Vector3.zero;

        desired /= count;

        desired.Normalize();
        desired *= _movSpeed;

        var steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce);

        return steering;
    }
    Vector3 Cohesion(List<Enemy> boids, float radius)
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        foreach (var item in boids)
        {
            if (item == this)
                continue;

            float dist = Vector3.Distance(transform.position, item.transform.position);
            if (dist > radius)
                continue;

            centerOfMass += item.transform.position;
            count++;
        }

        if (count <= 0)
            return Vector3.zero;

        centerOfMass /= count;

        Vector3 direction = centerOfMass - transform.position;


        if (direction.magnitude > radius * 1.5f)
            return Vector3.zero;

        direction.Normalize();
        direction *= _movSpeed;

        Vector3 steering = direction - Velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce);

        return steering;
    }
    Vector3 Flee(Vector3 position)
    {
        return -Pursuit(position);
    }

    #endregion
}
