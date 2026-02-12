using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    
    [SerializeField] private float groundCheckDistance = 0.4f;
    [SerializeField] private LayerMask groundLayer;
    bool grounded;
    
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

        print("Player Grounded " + grounded);
        return grounded;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.1f,
            transform.position + Vector3.down * groundCheckDistance);
    }
}
