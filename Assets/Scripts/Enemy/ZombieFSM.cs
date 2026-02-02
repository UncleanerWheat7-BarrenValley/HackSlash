using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ZombieFSM : FSM
{
    private static readonly int SpeedAnim = Animator.StringToHash("Speed");
    private static readonly int AttackAnim = Animator.StringToHash("Attack");
    private static readonly int LaunchedUpAnim = Animator.StringToHash("LaunchedUp");
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int HitX = Animator.StringToHash("HitX");
    private static readonly int HitY = Animator.StringToHash("HitY");
    int StandUpStateHash = Animator.StringToHash("Base Layer.StandUp");

    public enum FSMState
    {
        Idle,
        Wander,
        Chase,
        Attack,
        Stagger,
        LaunchedUp,
        Recovery,
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
    public EnemyHealth enemyHealth;
    private bool standUpStarted;
    [SerializeField] private float staggerDuration = 1.6f;
    private float staggerTimer;
    private float hitX, hitZ;
    public float staggerForce;

    private FSMState previousState;

    private void StateEnter(FSMState newState)
    {
        // Reset all animation bools
        animator.SetBool(AttackAnim, false);
        animator.SetBool(LaunchedUpAnim, false);

        // Set the appropriate animation bool for the new state
        switch (newState)
        {
            case FSMState.Idle:
                speed = 0;
                break;
            case FSMState.Wander:
                speed = 1;
                break;
            case FSMState.Chase:
                speed = 3;
                break;
            case FSMState.Attack:
                speed = 0;
                animator.SetBool(AttackAnim, true);
                break;
            case FSMState.Stagger:
                speed = 0;
                staggerTimer = 0f;
                GetHitDirection(out hitX, out hitZ);
                Vector3 vect =  new Vector3(hitX, 0, hitZ);
                ApplyKnockback(vect);
                animator.SetBool(Hit, true);
                animator.SetFloat(HitY, hitX);
                animator.SetFloat(HitY, hitZ);
                break;
            case FSMState.LaunchedUp:
                speed = 0;
                animator.SetBool(LaunchedUpAnim, true);
                break;
            case FSMState.Recovery:
                standUpStarted = false;
                speed = 0;
                break;
            case FSMState.Dead:
                speed = 0;
                break;
        }

        animator.SetFloat(SpeedAnim, speed);
    }

    private void GetHitDirection(out float hitX, out float hitY)
    {
        Vector3 localHitDir =
            transform.InverseTransformDirection(enemyHealth.LastHitDirection);

        localHitDir.y = 0f;
        localHitDir.Normalize();

        float forwardDot = Vector3.Dot(Vector3.forward, localHitDir);
        float rightDot = Vector3.Dot(Vector3.right, localHitDir);

        // Decide dominant axis
        if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
        {
            hitX = 0f;
            hitY = forwardDot > 0f ? 1f : -1f;
        }
        else
        {
            hitX = rightDot > 0f ? 1f : -1f;
            hitY = 0f;
        }
    }

    public void ApplyKnockback(Vector3 direction)
    {
        rigidBody.linearVelocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;

        rigidBody.AddForce(direction * staggerForce, ForceMode.Impulse);
    }

    private void StateExit(FSMState oldState)
    {
        switch (oldState)
        {
            case FSMState.Stagger:
                animator.SetBool(Hit, false);
                animator.SetFloat(HitX, 0f);
                animator.SetFloat(HitY, 0f);
                break;

            case FSMState.Attack:
                animator.SetBool(AttackAnim, false);
                break;

            case FSMState.LaunchedUp:
                animator.SetBool(LaunchedUpAnim, false);
                break;
        }
    }


    protected override void Initialize()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        pointList = GameObject.FindGameObjectsWithTag("WanderPoint");
        baseFSM = GetComponent<FSM>();
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        rigidBody = GetComponent<Rigidbody>();
        playerTransform = objPlayer.transform;
        if (!playerTransform)
        {
            print("Player not found. does your player obj have the tag 'Player'?");
        }

        enemyHealth.OnHighTime.AddListener(() => currentState = FSMState.LaunchedUp);
        enemyHealth.OnHighTimeLand.AddListener(() => currentState = FSMState.Recovery);
        enemyHealth.OnStagger.AddListener(HandleStagger);

        animator = GetComponent<Animator>();
        previousState = currentState;
    }

    protected override void FSMUpdate()
    {
        if (previousState != currentState)
        {
            StateExit(previousState);
            StateEnter(currentState); // Call the StateEnter function
            previousState = currentState;
        }

        switch (currentState)
        {
            case FSMState.Idle:
                Idle();
                break;
            case FSMState.Wander:
                Wander();
                break;
            case FSMState.Chase:
                Chase();
                break;
            case FSMState.Attack:
                Attack();
                break;
            case FSMState.Stagger:
                Stagger();
                break;
            case FSMState.LaunchedUp:
                LaunchedUp();
                break;
            case FSMState.Recovery:
                Recovery();
                break;
            case FSMState.Dead:
                Dead();
                break;
        }

        elapsedTime += Time.deltaTime;
    }

    private void Recovery()
    {
        if (animator.IsInTransition(0))
            return;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        // Phase 1: wait for StandUp to begin
        if (!standUpStarted)
        {
            if (info.fullPathHash == StandUpStateHash)
            {
                standUpStarted = true;
            }

            return;
        }

        // Phase 2: wait for StandUp to finish
        if (info.fullPathHash == StandUpStateHash &&
            info.normalizedTime < 1f)
        {
            return;
        }

        // Phase 3: safe to exit
        currentState = FSMState.Wander;
    }

    private void Dead()
    {
        //change to dead animation and turn off collision with player etc
    }

    private void Attack()
    {
        targetPosition = playerTransform.position;
        Vector3 frontVector = Vector3.forward;
        float dist = Vector3.Distance(transform.position, targetPosition);
        if (dist >= attackRadius && dist < playerNearRadius)
        {
            currentState = FSMState.Chase;
            return;
        }
        else if (dist >= playerNearRadius)
        {
            currentState = FSMState.Wander;
            return;
        }

        SetRotation();
    }

    private void Chase()
    {
        targetPosition = playerTransform.position;

        float dist = Vector3.Distance(transform.position, targetPosition);
        if (dist <= attackRadius)
        {
            currentState = FSMState.Attack;
        }
        else if (dist >= playerNearRadius)
        {
            currentState = FSMState.Wander;
        }

        SetRotation();
    }

    private void HandleStagger()
    {
        if (currentState == FSMState.Dead ||
            currentState == FSMState.LaunchedUp ||
            currentState == FSMState.Recovery)
            return;

        currentState = FSMState.Stagger;
    }

    private void Stagger()
    {
        staggerTimer += Time.deltaTime;

        if (staggerTimer >= staggerDuration)
        {
            staggerTimer = 0f;
            currentState = FSMState.Wander;
        }
    }

    private void SetRotation()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, targetPosition - transform.position);
        Vector3 eular = targetRotation.eulerAngles;
        eular.x = 0;
        eular.z = 0;
        Quaternion flattenedRot = Quaternion.Euler(eular);
        transform.rotation = Quaternion.Slerp(transform.rotation, flattenedRot, Time.deltaTime * rotateSpeed);
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    private void Wander()
    {
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
        SetRotation();
    }

    private void Idle()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) <= playerNearRadius)
        {
            print("switch to chase");
            currentState = FSMState.Chase;
        }
    }

    private void LaunchedUp()
    {
        //turn off movement and enter the LaunchedUp animation or whatever
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