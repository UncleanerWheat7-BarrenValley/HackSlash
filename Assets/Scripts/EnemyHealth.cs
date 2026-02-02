using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    private static readonly int LaunchedUp = Animator.StringToHash("LaunchedUp");
    public float startingHealth = 100;
    public float currentHealth;
    public GameObject mesh, sandDissolveOBJ;
    public Rigidbody rb;
    float cooldownDuration = 0.5f;
    bool canBeHit = true;
    bool grounded;
    [SerializeField] private float groundCheckDistance = 0.4f;
    [SerializeField] private LayerMask groundLayer;
    private float impulseInfluence = 1;
    public Vector3 LastHitDirection { get; private set; }

    public UnityEvent OnHighTime;
    public UnityEvent OnHighTimeLand;
    public UnityEvent OnStagger;

    public Vector3 Position
    {
        get { return transform.position; }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = startingHealth;
    }

    public void Damage(float damage)
    {
        if (!canBeHit) return;

        if (currentHealth > 0)
        {
            canBeHit = false;
            Invoke(nameof(ResetCooldown), cooldownDuration);
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void ResetCooldown()
    {
        canBeHit = true;
    }

    public void ApplyImpulse(float force, Transform attacker)
    {
        LastHitDirection = (transform.position - attacker.position).normalized;
        FacePlayer(attacker);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (force <= 0)
        {
            OnStagger.Invoke();
        }
        else if (force > 0)
        {
            OnHighTime.Invoke();
            StartCoroutine(CheckForLanding());
        }

        rb.AddForce(Vector3.up * (force * impulseInfluence), ForceMode.VelocityChange);
    }

    private void FacePlayer(Transform attacker)
    {
        if (TryGetComponent(out ZombieFSM fsm))
        {
            Vector3 toPlayer = attacker.transform.position - transform.position;
            toPlayer.y = 0f;

            if (toPlayer.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(toPlayer);
        }
    }

    public bool CheckGrounded()
    {
        // Cast a ray from the enemy's base downward
        grounded = Physics.Raycast(
            transform.position + Vector3.up * 0.2f,
            Vector3.down,
            out _,
            groundCheckDistance,
            groundLayer
        );

        print("Grounded " + grounded);
        if (grounded)
        {
            impulseInfluence = 1;
        }

        return grounded;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.1f,
            transform.position + Vector3.down * groundCheckDistance);
    }

    public void ReduceImpulseInfluence()
    {
        if (impulseInfluence > 0)
            impulseInfluence -= 0.05f;
    }

    public void ResetImpulseInfluence()
    {
        impulseInfluence = 1;
    }

    private void Die()
    {
        Instantiate(sandDissolveOBJ, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private IEnumerator CheckForLanding()
    {
        GetComponent<Animator>().SetBool(LaunchedUp, true);
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            yield return new WaitForFixedUpdate(); // Check every physics frame

            if (CheckGrounded())
                break;
        }

        // Once landed, reset the LaunchedUp state
        OnHighTimeLand.Invoke();
        GetComponent<Animator>().SetBool(LaunchedUp, false);
        impulseInfluence = 1;
    }
}