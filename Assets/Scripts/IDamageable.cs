using UnityEngine;

public interface IDamageable
{
    Vector3 Position { get; }
    void Damage(float damage);
}
