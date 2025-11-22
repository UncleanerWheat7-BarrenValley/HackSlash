using UnityEngine;

public class DummyDead : MonoBehaviour
{
    public ParticleSystem ps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ps.Play();
        Destroy(gameObject, 10);
        
    }
   
}
