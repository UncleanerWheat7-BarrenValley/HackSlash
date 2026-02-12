using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float health = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 Position { get; }
    public void Damage(float damage)
    {
        health -= damage;
        Debug.Log($"Player hit! health: {health}");
    }
}
