using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ZombieFSM : FSM
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int AttackAnim = Animator.StringToHash("Attack");

    public enum FSMState
    {
        Idle,
        Wander,
        Chase,
        Attack,
        Staggered,
        Dead,
    }

    public FSMState currentState = FSMState.Idle;
    public float speed = 2.0f;
    public float chaseSpeed = 5.0f;
    public float rotateSpeed = 2.0f;
    private bool isDead = false;
    protected float elapsedTime = 0;
    public float patrolRadius = 100;
    public FSM baseFSM;
    Rigidbody rigidBody;
    protected Transform playerTransform;
    protected Vector3 targetPosition;
    protected GameObject[] pointList;

    public float wanderRadius = 100;
    public float attackRadius = 2;
    public float playerNearRadius = 30;
    Animator animator;
    NavMeshAgent agent;

    protected override void Initialize()
    {
        pointList = GameObject.FindGameObjectsWithTag("WanderPoint");
        baseFSM = GetComponent<FSM>();
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        rigidBody = GetComponent<Rigidbody>();
        playerTransform = objPlayer.transform;
        if (!playerTransform)
        {
            print("Player not found. does your player obj have the tag 'Player'?");
        }

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    protected override void FSMUpdate()
    {
        switch (currentState)
        {
            case FSMState.Idle:
                animator.SetFloat(Speed, 0);
                Idle();
                break;
            case FSMState.Wander:
                animator.SetFloat(Speed, 2);
                Wander();
                break;
            case FSMState.Chase:
                animator.SetFloat(Speed, 5);
                Chase();
                break;
            case FSMState.Attack:
                animator.SetFloat(Speed, 0);
                Attack();
                break;
            case FSMState.Dead:
                animator.SetFloat(Speed, 0);
                Dead();
                break;
        }

        elapsedTime += Time.deltaTime;
    }

    private void Dead()
    {
    }

    private void Attack()
    {
        targetPosition = playerTransform.position;
        Vector3 frontVector = Vector3.forward;

        float dist = Vector3.Distance(transform.position, targetPosition);
        if (dist >= attackRadius && dist < playerNearRadius)
        {
            animator.SetBool(AttackAnim, false);
            currentState = FSMState.Chase;
            return;
        }
        else if (dist >= playerNearRadius)
        {
            animator.SetBool(AttackAnim, false);
            currentState = FSMState.Wander;
            return;
        }

        Quaternion targetRotation = Quaternion.FromToRotation(frontVector, targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        animator.SetBool(AttackAnim, true);
    }

    private void Chase()
    {
        agent.speed = chaseSpeed;
        targetPosition = playerTransform.position;
        Vector3 frontVector = Vector3.forward;

        float dist = Vector3.Distance(transform.position, targetPosition);
        if (dist <= attackRadius)
        {
            currentState = FSMState.Attack;
        }
        else if (dist >= playerNearRadius)
        {
            currentState = FSMState.Wander;
        }

        Quaternion targetRotation = Quaternion.FromToRotation(frontVector, targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    private void Wander()
    {
        agent.speed = speed;
        if (Vector3.Distance(transform.position, targetPosition) <= patrolRadius)
        {
            print("Reached patrol point");
            FindNextPoint();
        }
        else if (Vector3.Distance(transform.position, playerTransform.position) <= playerNearRadius)
        {
            print("switch to chase");
            currentState = FSMState.Chase;
        }

        // rotate to target
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);

        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    private void Idle()
    {
        agent.speed = 0;
    }

    protected override void FSMFixedUpdate()
    {
    }

    void FindNextPoint()
    {
        print("finding next point");
        int randomIndex = Random.Range(0, pointList.Length);
        float randomRadius = 10;
        Vector3 randomPosition = Vector3.zero;
        targetPosition = pointList[randomIndex].transform.position + randomPosition;

        if (IsInCurrentRange(targetPosition))
        {
            randomPosition = new Vector3(Random.Range(-randomRadius, randomRadius), 0,
                Random.Range(-randomRadius, randomRadius));
            targetPosition = pointList[randomIndex].transform.position + randomPosition;
        }
    }

    bool IsInCurrentRange(Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - transform.position.x);
        float zPos = Mathf.Abs(pos.z - transform.position.z);

        if (xPos <= patrolRadius && zPos <= patrolRadius) return true;

        return false;
    }
}