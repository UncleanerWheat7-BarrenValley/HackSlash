using UnityEngine;

public class WeaponHitBox : MonoBehaviour
{
    public int damage = 10;
    public float impulseForce = 0f;
    public GameObject sandSlashFX;    

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var target)) 
        {
            target.Damage(damage);
            if (impulseForce > 0 && other.attachedRigidbody != null)
            {
                ApplyImpulse(other.attachedRigidbody);
                impulseForce = 0;
            }
            SpawnSlashFX(other);
        }
    }

    private void SpawnSlashFX(Collider hit)
    { 
        Vector3 point = hit.ClosestPoint(transform.position);
        Vector3 dir = transform.forward;
        var fx = Instantiate(sandSlashFX, point, Quaternion.LookRotation(dir));
        
        Destroy(fx, 1);
    }
    
    private void ApplyImpulse(Rigidbody rb)
    {
        rb.AddForce(Vector3.up * impulseForce, ForceMode.Impulse);
        Debug.Log("Applied impulse to: " + rb.name);
    }
}
