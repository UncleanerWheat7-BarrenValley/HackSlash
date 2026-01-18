using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float startingHealth = 100;
    public float currentHealth;
    public GameObject mesh, sandDissolveOBJ;
    public Rigidbody rb;
    float cooldownDuration = 0.5f;
    bool canBeHit = true;
    bool grounded;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    private float impulseInfluence = 1;

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

    public void ApplyImpulse(float force)
    {
        Vector3 newVelocity = rb.linearVelocity;
        rb.linearVelocity = newVelocity;
        rb.linearVelocity += Vector3.up * (force * impulseInfluence);
    }

    public bool CheckGrounded()
    {
        // Cast a ray from the enemy's base downward
        grounded = Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
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
            impulseInfluence -= 0.15f;
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
}