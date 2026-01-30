using UnityEngine;
using UnityEngine.Audio;

public class Gun : MonoBehaviour
{
    public Transform firePoint;
    public AudioSource audioSource;
    public AudioClip gunFireSound;
    public GameObject muzzleFlash;
    public EnemyHealth enemyHealth;


    public void Fire(Vector3 direction)
    {
        //play muzzle flash
        GameObject flash = Instantiate(muzzleFlash, firePoint.position, firePoint.rotation);
        Destroy(flash, 0.15f);
        //play sound
        audioSource.clip = gunFireSound;
        audioSource.Play();

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, direction, out hit, 100f))
        {
            var dmg = hit.collider.GetComponent<IDamageable>();
            enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            if (dmg != null)
            {
                dmg.Damage(1);

                if (enemyHealth.CheckGrounded()) return;

                enemyHealth.ApplyImpulse(1.5f);
                enemyHealth.ReduceImpulseInfluence();
            }
        }
    }
}