using UnityEngine;

public class WeaponHitBox : MonoBehaviour
{
    public int damage = 10;
    public GameObject sandSlashFX;    

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var target)) 
        {
            target.Damage(damage);

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
}
