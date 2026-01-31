using UnityEngine;

public class WeaponHitBox : MonoBehaviour
{
    public int damage = 10;
    public float impulseForce;
    public GameObject sandSlashFX;

    private void OnTriggerEnter(Collider other)
    {
        var enemyHealth = other.GetComponent<EnemyHealth>();
        if (other.TryGetComponent<IDamageable>(out var target))
        {
            target.Damage(damage);
            SpawnSlashFX(other);
            
            enemyHealth.ResetImpulseInfluence();
            enemyHealth.ApplyImpulse(impulseForce, transform.parent);
        }
    }

    private void SpawnSlashFX(Collider hit)
    {
        Vector3 point = hit.ClosestPoint(transform.position);
        Vector3 dir = transform.forward;
        var fx = Instantiate(sandSlashFX, point, Quaternion.LookRotation(dir));

        Destroy(fx, 1);
    }
}