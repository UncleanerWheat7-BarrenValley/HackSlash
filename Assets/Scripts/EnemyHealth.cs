using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float startingHealth = 100;
    public float currentHealth;    
    public GameObject mesh;
    public ParticleSystem dissolveFX;
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    private void Start()
    {
        currentHealth = startingHealth;        
    }

    public void Damage(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        mesh.gameObject.SetActive(false);
        dissolveFX.Play();
        Destroy(gameObject,10);
    }
}
