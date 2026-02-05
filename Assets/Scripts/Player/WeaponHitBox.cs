using System.Collections;
using UnityEngine;

public class WeaponHitBox : MonoBehaviour
{
    public int damage = 10;
    public float impulseForce;
    public GameObject sandSlashFX;
    Animator enemyAnimator;

    private void OnTriggerEnter(Collider other)
    {
        var enemyHealth = other.GetComponent<EnemyHealth>();
        if (other.TryGetComponent<IDamageable>(out var target))
        {
            Vector3 hitDirection = (other.transform.position - transform.position).normalized;
            target.Damage(damage);
            SpawnSlashFX(other);
            enemyHealth.ResetImpulseInfluence();
            enemyHealth.ApplyImpulse(impulseForce, transform.root);
            StartCoroutine(HitStop(0.1f));
        }
    }

    private void SpawnSlashFX(Collider hit)
    {
        Vector3 point = hit.ClosestPoint(transform.position);
        Vector3 dir = transform.forward;
        var fx = Instantiate(sandSlashFX, point, Quaternion.LookRotation(dir));

        Destroy(fx, 1);
    }

    IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}