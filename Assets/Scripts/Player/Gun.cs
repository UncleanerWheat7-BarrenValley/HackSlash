using UnityEngine;
using UnityEngine.Audio;

public class Gun : MonoBehaviour
{
    public Transform firePoint;
    public AudioSource audioSource;
    public AudioClip gunFireSound;

    public void Fire(Vector3 direction) 
    {
        //play muzzle flash
        //play sound
        audioSource.clip = gunFireSound;
        audioSource.Play();

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, direction, out hit, 100f)) 
        {
            var dmg = hit.collider.GetComponent<IDamageable>();
            if (dmg != null) 
            {
                dmg.Damage(1);
            }
        }
    }
}
